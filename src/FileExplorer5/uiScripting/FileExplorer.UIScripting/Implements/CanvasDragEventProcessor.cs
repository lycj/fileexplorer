using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.BaseControls
{
    public class CanvasDragEventProcessor : UIEventProcessorBase
    {
        public CanvasDragEventProcessor()
        {
            _processEvents.AddRange(
               new[] {
                    FrameworkElement.KeyDownEvent,
                    FrameworkElement.PreviewMouseDownEvent,
                    UIEventHub.MouseDragEvent,
                    UIEventHub.TouchDragEvent,
                    FrameworkElement.PreviewMouseUpEvent,
                    FrameworkElement.MouseMoveEvent,

                    FrameworkElement.PreviewTouchDownEvent,
                    FrameworkElement.TouchMoveEvent,
                    FrameworkElement.PreviewTouchUpEvent
                }
            );
        }

        protected override FileExplorer.Script.IScriptCommand onEvent(RoutedEvent eventId)
        {            

            switch (eventId.Name)
            {
                case "KeyDown":
                    return 
                        ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false, 
                            //If current drag drop mode is Canvas,
                            ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Canvas",
                                //And Escape is pressed.
                                HubScriptCommands.IfKeyGesture("Esc",
                                   //Cancel Canvas drop (by clear {DragDrop} parameters)
                                    DragDropScriptCommands.CancelCanvasDrag(
                                        //Set {EventArgs.Handled} to true.
                                        HubScriptCommands.SetRoutedEventHandled(
                                           //and Detach adorner.
                                           HubScriptCommands.DettachSelectedItemsAdorner("SelectedItemsAdorner}"))))));

                case "PreviewTouchDown":
                case "PreviewMouseDown":
                    return 
                        //Find a DataContext that implement SupportDrag.
                        HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", 
                            DataContextType.SupportDrag, "{ISupportDrag}", null, false,
                            //And If there's one.
                            ScriptCommands.IfAssigned("{ISupportDrag}",                              
                                //Calculate a number of positions.
                                HubScriptCommands.ObtainPointerPosition(
                                    //Assign the datacontext item to {ItemUnderMouse}
                                    HubScriptCommands.AssignItemUnderMouse("{ItemUnderMouse}", false,
                                        //And set Sender's StartDraggingItem to {ItemUnderMouse}        
                                        ScriptCommands.IfAssigned("{ItemUnderMouse}", 
                                          ScriptCommands.IfTrue("{ItemUnderMouse.IsSelected}", 
                                        HubScriptCommands.SetDependencyProperty("{Sender}",
                                            UIEventHubProperties.StartDraggingItemProperty, "{ItemUnderMouse}")))))));
                case "TouchDrag":
                case "MouseDrag":
                    return 
                        //If event not marked handled.
                        HubScriptCommands.IfNotRoutedEventHandled(
                        HubScriptCommands.IfDependencyPropertyEqualDefaultValue<object>("{Sender}", 
                                UIEventHubProperties.StartDraggingItemProperty, 
                            //If StartDraggingProperty = null, return.
                            ResultCommand.NoError,
                            //If StartDraggingProperty != null, Check and set IsDraggingProperty to true.
                            HubScriptCommands.SetDependencyPropertyIfDifferentValue("{Sender}",
                                UIEventHubProperties.IsDraggingProperty, true,
                                //If changed IsDraggingProperty, Find DataContext that support ISupportDrag to {ISupportDrag} variable.
                                HubScriptCommands.AssignDataContext("{EventArgs.OriginalSource}", DataContextType.SupportDrag, "{ISupportDrag}", null, false,
                                    //If ISupportDrag is assigned to a non-null value,                                     
                                    //Initialize DragLiteParameters (in {DragDrop} global parameterDic).
                                        DragDropScriptCommands.StartCanvasDrag("{ISupportDrag}",
                                        //then set {EventArgs.Handled} to true.
                                        HubScriptCommands.SetRoutedEventHandled( 
                                            //And attach/update adorner.
                                            HubScriptCommands.AttachSelectedItemsAdorner("{SelectedItemsAdorner}", 
                                              HubScriptCommands.UpdateSelectedItemsAdorner("{SelectedItemsAdorner}")))))
                           , null)));
                case "MouseMove" :
                case "TouchMove":
                    return  
                        //Only 1 of 5 TouchEvent will be processed
                        HubScriptCommands.ThrottleTouchDrag(5, 
                            ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false, 
                                ScriptCommands.IfEquals(DragDropLiteCommand.DragDropModeKey, "Canvas",
                                    HubScriptCommands.ObtainPointerPosition(
                                        //Update adorner CentrePosition.
                                        HubScriptCommands.UpdateSelectedItemsAdorner("{SelectedItemsAdorner}")))));
                case "PreviewTouchUp":
                case "PreviewMouseUp":
                    return 
                         HubScriptCommands.SetDependencyPropertyIfDifferentValue("{Sender}", UIEventHubProperties.IsDraggingProperty, false,
                            HubScriptCommands.SetDependencyPropertyValue<object>("{Sender}", UIEventHubProperties.StartDraggingItemProperty, null,
                                //Update position of each IPostionAware and clear {DragDrop} parameters)
                                DragDropScriptCommands.EndCanvasDrag( 
                                    //Set {EventArgs.Handled} to true.
                                    HubScriptCommands.SetRoutedEventHandled(
                                        //Detach adorner.
                                        HubScriptCommands.DettachSelectedItemsAdorner("SelectedItemsAdorner}")))));
            
            }

            return base.onEvent(eventId);
        }
    }
}
