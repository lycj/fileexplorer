using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
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

    public class MultiSelectEventProcessor : UIEventProcessorBase
    {
        public MultiSelectEventProcessor()
        {
            _processEvents.AddRange(
               new[] {
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

            Print.PrintConsoleAction = t => Console.WriteLine(t);
        }

        private static dynamic _cmdDic = new DynamicDictionary<IScriptCommand>(false);
       

        protected override FileExplorer.Script.IScriptCommand onEvent(RoutedEvent eventId)
        {

            switch (eventId.Name)
            {
                case "PreviewTouchDown":
                case "PreviewMouseDown":
                    return _cmdDic.PreviewMouseDown ?? (_cmdDic.PreviewMouseDown = 
                        HubScriptCommands.ObtainPointerPosition(
                        HubScriptCommands.SetHandledIfNotFocused()));
                case "MouseDrag":
                case "TouchDrag":
                    if (EnableMultiSelect)
                        return _cmdDic.MouseDrag ?? (_cmdDic.MouseDrag = 
                            //Ignore if Handled
                            HubScriptCommands.IfNotRoutedEventHandled(
                            //Ignore If initiator by an element over AdornerLayer
                            HubScriptCommands.IfExistsVisualParent("{EventArgs.OriginalSource}", FindMethodType.Type,  "AdornerLayer", 
                                ResultCommand.NoError, 
                                ScriptCommands.RunICommand(UnselectAllCommand, null, false, 
                                HubScriptCommands.CaptureMouse(CaptureMouseMode.ScrollContentPresenter,
                                    HubScriptCommands.SetRoutedEventHandled(
                                    HubScriptCommands.SetDependencyPropertyIfDifferentValue("{Sender}",
                                        UIEventHubProperties.IsSelectingProperty, true,
                                        HubScriptCommands.ObtainPointerPosition(
                                            HubScriptCommands.AttachSelectionAdorner("{SelectionAdorner}",                                              
                                                HubScriptCommands.FindSelectedItems( 
                                                    HubScriptCommands.HighlightItems()))),
                                    ResultCommand.NoError)))))));
                    break;
                case "TouchMove":
                case "MouseMove":
                    return _cmdDic.MouseMove ?? (_cmdDic.MouseMove = 
                         HubScriptCommands.ThrottleTouchDrag(5, 
                        HubScriptCommands.IfDependencyPropertyEquals("{Sender}",
                        UIEventHubProperties.IsSelectingProperty, true,
                        HubScriptCommands.ObtainPointerPosition(
                            HubScriptCommands.UpdateSelectionAdorner("{SelectionAdorner}",
                                        HubScriptCommands.FindSelectedItems( 
                                            HubScriptCommands.HighlightItems(
                                                HubScriptCommands.AutoScroll())))))));
                case "PreviewTouchUp":
                case "PreviewMouseUp":
                    return _cmdDic.PreviewMouseUp ?? (_cmdDic.PreviewMouseUp = 
                        //Ignore if Handled
                        HubScriptCommands.IfNotRoutedEventHandled(
                        //Ignore If initiator by an element over AdornerLayer
                        HubScriptCommands.IfExistsVisualParent("{EventArgs.OriginalSource}", FindMethodType.Type,  "AdornerLayer", 
                            ResultCommand.NoError, 
                            HubScriptCommands.ReleaseMouse(
                                //HubScriptCommands.SetRoutedEventHandled(
                                    HubScriptCommands.ObtainPointerPosition(
                                        HubScriptCommands.DettachSelectionAdorner("{SelectionAdorner}",
                                            //Assign IsSelecting attached property to {WasSelecting}
                                            HubScriptCommands.GetDependencyProperty("{Sender}", UIEventHubProperties.IsSelectingProperty, "{WasSelecting}",
                                            //Reset IsSelecting attached property.
                                            HubScriptCommands.SetDependencyPropertyValue("{Sender}", UIEventHubProperties.IsSelectingProperty, false,
                                            ScriptCommands.IfTrue("{WasSelecting}",
                                                //WasSelecting
                                                HubScriptCommands.FindSelectedItems(HubScriptCommands.SelectItems()),
                                                //WasNotSelecting
                                                HubScriptCommands.AssignItemUnderMouse("{ItemUnderMouse}", false,
                                                    ScriptCommands.IfAssigned("{ItemUnderMouse}",
                                                        //If an Item Under Mouse
                                                        ScriptCommands.IfTrue("{ItemUnderMouse.IsSelected}", 
                                                          ResultCommand.NoError, 
                                                          ScriptCommands.RunICommand(UnselectAllCommand, null, false,
                                                              ScriptCommands.SetPropertyValue("{ItemUnderMouse}", "IsSelected", true))),
                                                        //If click on empty area
                                                        ScriptCommands.RunICommand(UnselectAllCommand, null, false))
                        ))))))))));
            }

            return base.onEvent(eventId);
        }

        public static DependencyProperty UnselectAllCommandProperty =
            DependencyProperty.Register("UnselectAllCommand", typeof(ICommand),
            typeof(MultiSelectEventProcessor));

        public ICommand UnselectAllCommand
        {
            get { return (ICommand)GetValue(UnselectAllCommandProperty); }
            set { SetValue(UnselectAllCommandProperty, value); }
        }

        public static DependencyProperty EnableMultiSelectProperty =
        DependencyProperty.Register("EnableMultiSelect", typeof(bool),
        typeof(MultiSelectEventProcessor), new PropertyMetadata(true));

        public bool EnableMultiSelect
        {
            get { return (bool)GetValue(EnableMultiSelectProperty); }
            set { SetValue(EnableMultiSelectProperty, value); }
        }

        public static DependencyProperty IsCheckboxEnabledProperty =
        DependencyProperty.Register("IsCheckboxEnabled", typeof(bool),
        typeof(MultiSelectEventProcessor), new PropertyMetadata(true));

        public bool IsCheckboxEnabled
        {
            get { return (bool)GetValue(IsCheckboxEnabledProperty); }
            set { SetValue(IsCheckboxEnabledProperty, value); }
        }

    }
}
