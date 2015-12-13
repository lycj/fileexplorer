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

   
    public class DragDropEventProcessor : UIEventProcessorBase
    {
        #region Constructor
        public DragDropEventProcessor()
        {
            _processEvents.AddRange(
                new[] {
                    FrameworkElement.PreviewMouseDownEvent,
                    UIEventHub.MouseDragEvent,
                    FrameworkElement.PreviewMouseUpEvent,
                    FrameworkElement.MouseMoveEvent,

                    FrameworkElement.DragEnterEvent,
                    FrameworkElement.DragOverEvent,
                    FrameworkElement.DragLeaveEvent,
                    FrameworkElement.DropEvent,
                    FrameworkElement.GiveFeedbackEvent
                }
             );

            Print.PrintConsoleAction = c => Console.WriteLine(c);
        }
        #endregion

        private static dynamic _cmdDic = new DynamicDictionary<IScriptCommand>(false);        


        protected override FileExplorer.Script.IScriptCommand onEvent(RoutedEvent eventId)
        {            
            if (EnableDrag)
                switch (eventId.Name)
                {
                    case "PreviewMouseDown":
                        return _cmdDic.PreviewMouseDown ?? (_cmdDic.PreviewMouseDown = 
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,                                
                                //Set Default value for CanDrag first.
                                ScriptCommands.Assign("{DragDrop.CanDrag}", false, false,                                        
                                    //Find a DataContext that implement SupportDrag, and if assigned
                                    HubScriptCommands.IfHasDataContext("{EventArgs.OriginalSource}", DataContextType.SupportShellDrag,                                     
                                           //and item under mouse (ISelectable) IsSelected.
                                           DragDropScriptCommands.IfItemUnderMouseIsSelected(
                                                //Notify MouseDrag can drag.
                                                ScriptCommands.Assign("{DragDrop.CanDrag}", true, false, 
                                                    //Prevent MultiSelect from starting.
                                                    HubScriptCommands.SetRoutedEventHandled())
                                                )))));
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
                                                HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportShellDrag, "{DragDrop.SupportDrag}", null, false,                                                     
                                                    //Determine DragMethod and call QueryDrag().
                                                    DragDropScriptCommands.StartShellDrag("{DragDrop.SupportDrag}")
                                                   )))))));                    

                    case "PreviewMouseUp":
                        return _cmdDic.PreviewMouseUp ?? (_cmdDic.PreviewMouseUp =     
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,                                
                                    //HubScriptCommands.SetDependencyPropertyValue("{Sender}", AttachedProperties.IsDraggingProperty, false,
                                        ScriptCommands.IfEquals(QueryDrag.DragMethodKey, DragMethod.Menu,
                                            ResultCommand.NoError,
                                            //This is defined in drop, detach if there's an adorner.
                                            DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}"))));
                    case "MouseMove":
                        return _cmdDic.MouseMove ?? (_cmdDic.MouseMove =     
                            HubScriptCommands.IfDependencyProperty("{Sender}", UIEventHubProperties.IsDraggingProperty,
                                    ComparsionOperator.Equals, true,
                                    HubScriptCommands.SetRoutedEventHandled()));
                }

            if (EnableDrop)
            {                               
                switch (eventId.Name)
                {                   
                    case "DragEnter": return _cmdDic.DragEnter ?? (_cmdDic.DragEnter =     
                    ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                        //Find DataContext that support IShellDrop
                        HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportShellDrop, "{ISupportDrop}", "{ElementSupportDrop}", false,
                            //And if there's one,
                            ScriptCommands.IfAssigned("{ISupportDrop}",                                
                                HubScriptCommands.QueryShellDragInfo("{ISupportDrop}", "{DataObj}", "{DragDrop.Draggables}", "{DragDrop.QueryDropResult}", false,
                                            //Otherwise, 
                                            ScriptCommands.Assign(new Dictionary<string, object>()
                                            {
                                                { "{DragDrop.SupportDrop}", "{ISupportDrop}" }, //Store SupportDrop property to global for future use.
                                                { "{DragDrop.ElementSupportDrop}", "{ElementSupportDrop}" }, //Store SupportDrop element to global for future use.
                                                { "{ISupportDrop.IsDraggingOver}", true }, //Set DataContext.IsDraggingOver to true.                                                
                                                { "{EventArgs.Handled}", true }  //Mark RoutedEvent handled.                                                                                           
                                            }, false,   
                                                //Attach DragAdorner and update it.                               
                                                DragDropScriptCommands.AttachAdorner(                                                    
                                                    "{DragDrop.AdornerLayer}", "{DragDrop.Adorner}", 
                                                    "{ISupportDrop}", "{DragDrop.DragMethod}", "{DragDrop.Draggables}"))), 
                                            DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}")))));

                    case "GiveFeedback": return _cmdDic.GiveFeedback ?? (_cmdDic.GiveFeedback =     
                        ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                            //If QueryDropResult returns none, set cursor to not droppable.
                            ScriptCommands.IfEquals("{DragDrop.QueryDropResult}", QueryDropEffects.None,
                                HubScriptCommands.SetCustomCursor(Cursors.No))));

                    case "DragOver": return _cmdDic.DragOver ?? (_cmdDic.DragOver =     
                        ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                                        DragDropScriptCommands.UpdateAdornerPointerPosition("{DragDrop.Adorner}")));

                    case "DragLeave": return _cmdDic.DragLeave ?? (_cmdDic.DragLeave =     
                         ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                         ScriptCommands.SetPropertyValue("{DragDrop.SupportDrop}", "IsDraggingOver", false, 
                            //Detach adorner if DragLeave current element.
                            ScriptCommands.IfAssigned("{DragDrop.SupportDrop}",                                
                                    DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}")))));

                    case "Drop": 
                        IScriptCommand detachAdornerAndResetDragDrop =
                            ScriptCommands.SetPropertyValue("{DragDrop.SupportDrop}", "IsDraggingOver", false, 
                            DragDropScriptCommands.DetachAdorner("{DragDrop.AdornerLayer}", "{DragDrop.Adorner}",
                                ScriptCommands.Reset(null, "{DragDrop.Adorner}", "{DragDrop.AdornerLayer}",
                                "{DragDrop.SupportDrop}", "{DragDrop.Draggables}")));

                        return _cmdDic.Drop ?? (_cmdDic.Drop =     
                        ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false,
                            //Find DataContext that support ISupportShellDrop
                            HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportShellDrop, "{ISupportDrop}", "{ElementSupportDrop}", false,
                                //If ISupportDrop found.
                                ScriptCommands.IfAssigned("{ISupportDrop}",                                                                
                                    ////Obtain DataObject from event and Call ISupportDrop.QueryDrop() to get IDraggables[] and QueryDropResult.
                                    HubScriptCommands.SetRoutedEventHandled(
                                    HubScriptCommands.QueryShellDragInfo("{ISupportDrop}", "{DataObj}", "{DragDrop.Draggables}", "{DragDrop.QueryDropResult}", false,                                                   
                                        //If QueryShellDragInfo Success, if DragMethod...
                                        ScriptCommands.IfEquals(QueryDrag.DragMethodKey, DragMethod.Menu,                                                     
                                            //Backup because ISupportDrag parameter is reset after this command is completed.
                                            ScriptCommands.Assign("{DragDrop.SupportDragBackup}", "{DragDrop.SupportDrag}", false,
                                                //is Menu, then Show Menu.
                                                HubScriptCommands.ShowDragAdornerContextMenu("{DragDrop.Adorner}", 
                                                            "{DragDrop.QueryDropResult.SupportedEffects}", 
                                                            "{DragDrop.QueryDropResult.PreferredEffect}", 
                                                            "{ResultEffect}", 
                                                //After menu closed...
                                                ScriptCommands.IfEquals("{ResultEffect}", DragDropEffectsEx.None, 
                                                    //If User choose None (click on empty area), detach and reset.
                                                    detachAdornerAndResetDragDrop, 
                                                    //Otherwise, call ISupportDrop.Drop()
                                                    HubScriptCommands.QueryDrop("{DragDrop.SupportDragBackup}", "{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", 
                                                                    "{ResultEffect}", "{DragDrop.DropResult}", false,                                                                    
                                                        //And detach adorner.
                                                        detachAdornerAndResetDragDrop)))),
                                            //If DragMethod is not Menu, Drop immediately.
                                            HubScriptCommands.QueryDrop("{DragDrop.SupportDrag}", "{ISupportDrop}", "{DragDrop.Draggables}", "{DataObj}", 
                                              "{DragDrop.QueryDropResult.PreferredEffect}", "{DragDrop.DropResult}", false, 
                                                detachAdornerAndResetDragDrop)), 
                                                    
                                        //If QueryDropEffects is None, drag failed, detach adorner.
                                        detachAdornerAndResetDragDrop))))));

                }
            }

            return base.onEvent(eventId);
        }

        #region Public Properties

        public static DependencyProperty EnableDragProperty =
            DependencyProperty.Register("EnableDrag", typeof(bool),
            typeof(DragDropEventProcessor), new PropertyMetadata(false));

        public bool EnableDrag
        {
            get { return (bool)GetValue(EnableDragProperty); }
            set { SetValue(EnableDragProperty, value); }
        }

        public static DependencyProperty EnableDropProperty =
            DependencyProperty.Register("EnableDrop", typeof(bool),
            typeof(DragDropEventProcessor), new PropertyMetadata(false));

        public bool EnableDrop
        {
            get { return (bool)GetValue(EnableDropProperty); }
            set { SetValue(EnableDropProperty, value); }
        }

        #endregion
    }
}
