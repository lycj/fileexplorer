using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.WPF.BaseControls
{


    public class DragDropLiteEventProcessor : UIEventProcessorBase
    {
        public DragDropLiteEventProcessor()
        {
            _processEvents.AddRange(
                new[] {
                    FrameworkElement.PreviewKeyDownEvent,
                    FrameworkElement.PreviewMouseDownEvent,
                    UIEventHub.MouseDragEvent,
                    FrameworkElement.PreviewMouseUpEvent,
                    FrameworkElement.MouseMoveEvent,
                    FrameworkElement.MouseLeaveEvent,

                    FrameworkElement.TouchLeaveEvent,
                    UIEventHub.TouchDragEvent,
                    FrameworkElement.PreviewTouchDownEvent,
                    FrameworkElement.TouchMoveEvent,
                    FrameworkElement.TouchUpEvent, //Not Preview or it would trigger parent's PreviewTouchUp first.
                    FrameworkElement.QueryCursorEvent
                }
             );

            Print.PrintConsoleAction = c => Console.WriteLine(c);
        }

        #region Methods
        private static dynamic _cmdDic = new DynamicDictionary<IScriptCommand>(false);        


        protected override IScriptCommand onEvent(RoutedEvent eventId)
        {            
            if (eventId.Name.Contains("Mouse") && !EnableMouse)
                return base.onEvent(eventId);
            if (eventId.Name.Contains("Touch") && !EnableTouch)
                return base.onEvent(eventId);
           

            IScriptCommand detachAdornerAndResetDragDrop =
                          ScriptCommands.SetPropertyValue(DragDropLiteCommand.DragDropDropTargetKey, "IsDraggingOver", false,
                          DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}",
                          DragDropScriptCommands.EndLiteDrag("{DragDrop.SupportDrag}")));

          

            if (EnableDrag)
            switch (eventId.Name)
            {
                case "KeyDown":
                    return ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite",
                                HubScriptCommands.IfKeyGesture(new KeyGesture(Key.Escape),
                                    detachAdornerAndResetDragDrop));


                case "PreviewMouseDown":
                case "PreviewTouchDown":                                    
                    return  _cmdDic.PreviewMouseDown ?? (_cmdDic.PreviewMouseDown = 
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,                                
                                //Set Default value for CanDrag first.
                                ScriptCommands.Assign("{DragDrop.CanDrag}", false, false,                                        
                                    //Find a DataContext that implement SupportDrag, and if assigned
                                    HubScriptCommands.IfHasDataContext("{EventArgs.OriginalSource}", DataContextType.SupportDrag,                                     
                                           //and item under mouse (ISelectable) IsSelected.
                                           DragDropScriptCommands.IfItemUnderMouseIsSelected(
                                                //Notify MouseDrag can drag.
                                                ScriptCommands.Assign("{DragDrop.CanDrag}", true, false, 
                                                    //Prevent MultiSelect from starting.
                                                    HubScriptCommands.SetRoutedEventHandled())
                                                )))));
                case "TouchDrag": 
                case "MouseDrag":
                    return _cmdDic.MouseDrag ?? (_cmdDic.MouseDrag =
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                                //If not handled.
                                HubScriptCommands.IfNotRoutedEventHandled(                                    
                                    //IfCanDrag (assigned from PreviewMouseDown)                                    
                                    ScriptCommands.IfTrue("{DragDrop.CanDrag}",                                        
                                        //Reset CanDrag
                                        ScriptCommands.Assign("{DragDrop.CanDrag}", false, false,
                                            //Mark handled, prevent MultiSelect from processing.
                                            HubScriptCommands.SetRoutedEventHandled(
                                                //If changed IsDraggingProperty, Find DataContext that support ISupportDrag to {ISupportDrag} variable.                                                
                                                HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportDrag, "{DragDrop.SupportDrag}", null, false,
                                                    //Determine DragMethod and call QueryDrag().                                                       
                                                    DragDropScriptCommands.StartLiteDrag("{DragDrop.SupportDrag}")
                                                   )))))));
            }

            if (EnableDrop)
                switch (eventId.Name)
                {

                    case "QueryCursor": return _cmdDic.GiveFeedback ?? (_cmdDic.GiveFeedback =
                           ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                            //If QueryDropResult returns none, set cursor to not droppable.
                             ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite",
                                HubScriptCommands.QueryDropEffects("{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", DragDropLiteCommand.DragDropEffectsKey, "{DragDrop.QueryDropResult}", false,
                                    ScriptCommands.IfEquals("{DragDrop.QueryDropResult}", QueryDropEffects.None,
                                        HubScriptCommands.SetCustomCursor(Cursors.No),
                                        HubScriptCommands.SetCustomCursor(Cursors.Arrow))))));

                    case "TouchMove":
                    case "MouseMove":
                        return _cmdDic.MouseMove ?? (_cmdDic.MouseMove =
                            HubScriptCommands.ThrottleTouchDrag(5,
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                            //If not handled.
                                    HubScriptCommands.IfNotRoutedEventHandled(
                                        ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite",
                                         HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportDrop, "{ISupportDrop}", "{ElementSupportDrop}", false,
                            //And if there's one,             
                            //If Moved to different support drop.                                                                                
                                    ScriptCommands.IfEquals("{ISupportDrop}", DragDropLiteCommand.DragDropDropTargetKey,
                                         DragDropScriptCommands.UpdateAdornerPointerPosition("{DragDrop.Adorner}"),
                                         HubScriptCommands.QueryDropEffects("{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", DragDropLiteCommand.DragDropEffectsKey, "{DragDrop.QueryDropResult}", false,
                                                ScriptCommands.SetPropertyValue(DragDropLiteCommand.DragDropDropTargetKey, (ISupportDrop p) => p.IsDraggingOver, false,
                                                    ScriptCommands.Assign(new Dictionary<string, object>()
                                                    {
                                                    { DragDropLiteCommand.DragDropDropTargetKey, "{ISupportDrop}" }, //Store SupportDrop property to global for future use.
                                                    { "{DragDrop.ElementSupportDrop}", "{ElementSupportDrop}" }, //Store SupportDrop element to global for future use.                                                    
                                                    { "{EventArgs.Handled}", true }  //Mark RoutedEvent handled.                                                                                           
                                                }, false,
                            //ScriptCommands.IfNotEquals("{DragDrop.QueryDropResult}", QueryDropEffects.None,                                        
                            //Attach DragAdorner and update it.      
                                                        ScriptCommands.SetPropertyValue(DragDropLiteCommand.DragDropDropTargetKey, (ISupportDrop p) => p.IsDraggingOver, true,
                                                        DragDropScriptCommands.AttachAdorner(
                                                            "{DragDrop.AdornerLayer}", "{DragDrop.Adorner}",
                                                            "{ISupportDrop}", "{DragDrop.DragMethod}", "{DragDrop.Draggables}"))
                            /*DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}")*/))))))))));
                case "TouchUp":
                case "PreviewMouseUp":                     
                    return _cmdDic.MouseUp ?? (_cmdDic.MouseUp =                            
                        ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,                        
                        ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite", 
                            //Find DataContext that support ISupportDrop                             
                            HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportDrop, "{ISupportDrop}", "{ElementSupportDrop}", false,
                                //If ISupportDrop found.
                                ScriptCommands.IfAssigned("{ISupportDrop}",                                                                
                                    ////Obtain DataObject from event and Call ISupportDrop.QueryDrop() to get IDraggables[] and QueryDropResult.
                                    HubScriptCommands.QueryDropEffects("{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", DragDropLiteCommand.DragDropEffectsKey, "{DragDrop.QueryDropResult}", false,                                        
                                        //If QueryShellDragInfo Success, if DragMethod...
                                        ScriptCommands.IfEquals(QueryDrag.DragMethodKey, DragMethod.Menu,          
                                            HubScriptCommands.SetRoutedEventHandled(                                               
                                                HubScriptCommands.ShowDragAdornerContextMenu("{DragDrop.Adorner}", 
                                                            "{DragDrop.QueryDropResult.SupportedEffects}", 
                                                            "{DragDrop.QueryDropResult.PreferredEffect}", 
                                                            "{ResultEffect}", 
                                                //After menu closed...
                                                ScriptCommands.IfEquals("{ResultEffect}", DragDropEffectsEx.None, 
                                                    //If User choose None (click on empty area), detach and reset.
                                                    detachAdornerAndResetDragDrop, 
                                                    //Otherwise, call ISupportDrop.Drop()
                                                    HubScriptCommands.QueryDrop("{DragDrop.SupportDrag}", "{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", 
                                                                    "{ResultEffect}", "{DragDrop.DropResult}", false,                                                                    
                                                        //And detach adorner.
                                                        detachAdornerAndResetDragDrop)))),
                                            //If DragMethod is not Menu, Drop immediately.
                                            HubScriptCommands.QueryDrop("{DragDrop.SupportDrag}", "{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", 
                                              "{DragDrop.QueryDropResult.PreferredEffect}", "{DragDrop.DropResult}", false, 
                                                detachAdornerAndResetDragDrop))), 
                                                    
                                        //If QueryDropEffects is None, drag failed, detach adorner.
                                        detachAdornerAndResetDragDrop)))));
                }


            switch (eventId.Name)
            {
                case "PreviewKeyDown":
                    return 
                         ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                        ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite",
                                HubScriptCommands.IfKeyGesture(new KeyGesture(Key.Escape),
                                    detachAdornerAndResetDragDrop)));
                case "PreviewMouseUp":
                    return 
                         ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                        ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Lite",
                            DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}")));
            }
            return base.onEvent(eventId);
        }

        #endregion

        #region Public Properties

        public static DependencyProperty EnableMouseProperty =
          DependencyProperty.Register("EnableMouse", typeof(bool), typeof(DragDropLiteEventProcessor),
          new PropertyMetadata(true));

        public bool EnableMouse
        {
            get { return (bool)GetValue(EnableMouseProperty); }
            set { SetValue(EnableMouseProperty, value); }
        }


        public static DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(DragDropLiteEventProcessor),
            new PropertyMetadata(true));

        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        public static DependencyProperty EnableDragProperty =
           DragDropEventProcessor.EnableDragProperty.AddOwner(typeof(DragDropLiteEventProcessor));

        public bool EnableDrag
        {
            get { return (bool)GetValue(EnableDragProperty); }
            set { SetValue(EnableDragProperty, value); }
        }

        public static DependencyProperty EnableDropProperty =
            DragDropEventProcessor.EnableDropProperty.AddOwner(typeof(DragDropLiteEventProcessor));

        public bool EnableDrop
        {
            get { return (bool)GetValue(EnableDropProperty); }
            set { SetValue(EnableDropProperty, value); }
        }

        #endregion
    }
}
