using FileExplorer.Script;
using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FileExplorer.WPF.BaseControls
{
    public class CanvasResizeEventProcessor : UIEventProcessorBase
    {
        public CanvasResizeEventProcessor()
        {
            _processEvents.AddRange(
               new[] {                    
                   FrameworkElement.MouseDownEvent,
                    FrameworkElement.TouchDownEvent,
                    FrameworkElement.MouseMoveEvent,
                    FrameworkElement.TouchMoveEvent,
                    UIEventHub.MouseDragEvent,
                    UIEventHub.TouchDragEvent,
                    FrameworkElement.PreviewMouseUpEvent,
                    FrameworkElement.PreviewTouchUpEvent
                }
            );
        }

        protected override Script.IScriptCommand onEvent(RoutedEvent eventId)
        {
            switch (eventId.Name)
            {
                //case "MouseDrag":
                //case "TouchDrag":
                //    return ScriptCommands.AssignGlobalParameterDic("{CanvasResize}", false,
                //        ScriptCommands.IfAssigned("{CanvasResize.ResizeItemAdorner}",
                //          HubScriptCommands.DettachResizeItemAdorner("{CanvasResize.AdornerLayer}","{CanvasResize.ResizeItemAdorner}")));
                //case "MouseDrag":
                //case "TouchDrag":
                //    return HubScriptCommands.DettachResizeItemAdorner("{ResizeItemAdorner}");

                case "PreviewTouchUp":
                case "PreviewMouseUp":
                    return ScriptCommands.FilterArray("{Sender.Items}", "IsSelected", ComparsionOperator.Equals, true, "{SelectedItems}",                        
                        ScriptCommands.AssignGlobalParameterDic("{CanvasResize}", false,
                        HubScriptCommands.DettachResizeItemAdorner("{CanvasResize.AdornerLayer}","{CanvasResize.ResizeItemAdorner}", 
                            ScriptCommands.IfArrayLength(ComparsionOperator.Equals, "{SelectedItems}", 1,
                                ScriptCommands.Assign("{CanvasResize.ResizeItem}", "{SelectedItems[0]}", false, 
                                HubScriptCommands.AssignAdornerLayer(AdornerType.SelectedItem, "{CanvasResize.AdornerLayer}",false, 
                                    HubScriptCommands.AttachResizeItemAdorner("{CanvasResize.AdornerLayer}","{CanvasResize.ResizeItemAdorner}")))                                                           
                        ))));
            }
            return base.onEvent(eventId);
        }
    }
}
