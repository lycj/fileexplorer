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
    public class ResizeAdornerEventProcessor : UIEventProcessorBase
    {
        public ResizeAdornerEventProcessor()
        {
            _processEvents.AddRange(
               new[] {                    
                   FrameworkElement.PreviewMouseDownEvent,
                    FrameworkElement.PreviewTouchDownEvent,
                   FrameworkElement.MouseMoveEvent,
                    FrameworkElement.TouchMoveEvent,                    
                    FrameworkElement.PreviewMouseUpEvent,
                    FrameworkElement.PreviewTouchUpEvent
                }
            );

            Print.PrintConsoleAction = msg => Console.WriteLine(msg);
        }

        private static IScriptCommand resizeNorthCommand = ScriptCommands.Subtract("{CanvasResize.ResizeItem.Height}", "{DiffY}", "{CanvasResize.ResizeItem.Height}",
                                         ScriptCommands.Add("{CanvasResize.ResizeItem.Top}", "{DiffY}", "{CanvasResize.ResizeItem.Top}")) ;
        private static IScriptCommand resizeSouthCommand = ScriptCommands.Add("{CanvasResize.ResizeItem.Height}", "{DiffY}", "{CanvasResize.ResizeItem.Height}");
        private static IScriptCommand resizeWestCommand = ScriptCommands.Subtract("{CanvasResize.ResizeItem.Width}", "{DiffX}", "{CanvasResize.ResizeItem.Width}",
                                                        ScriptCommands.Add("{CanvasResize.ResizeItem.Left}", "{DiffX}", "{CanvasResize.ResizeItem.Left}"));
        private static IScriptCommand resizeEastCommand = ScriptCommands.Add("{CanvasResize.ResizeItem.Width}", "{DiffX}", "{CanvasResize.ResizeItem.Width}");
        
        protected override Script.IScriptCommand onEvent(RoutedEvent eventId)
        {
            switch (eventId.Name)
            {

     
                case "MouseMove":
                case "TouchMove":
                    return
                      ScriptCommands.AssignGlobalParameterDic("{CanvasResize}", false,
                        ScriptCommands.IfTrue("{CanvasResize.IsResizing}",
                            HubScriptCommands.AssignCursorPosition(PositionRelativeToType.Panel, "{CanvasResize.CurrentPosition}", false,
                               HubScriptCommands.UpdateResizeItemAdorner("{CanvasResize.ResizeItemAdorner}",
                                 "{CanvasResize.ResizeMode}", "{CanvasResize.StartPosition}", "{CanvasResize.CurrentPosition}"))));
                        
                //case "MouseDrag":
                //case "TouchDrag":
                case "PreviewTouchDown":
                case "PreviewMouseDown":
                    return
                       ScriptCommands.AssignGlobalParameterDic("{CanvasResize}", false,
                           HubScriptCommands.SetRoutedEventHandled(
                                ScriptCommands.Assign("{CanvasResize.IsResizing}", true, false,
                                   HubScriptCommands.AssignCursorPosition(PositionRelativeToType.Panel, "{CanvasResize.StartPosition}", false,                                        
                                        //Assign Source's Name (e.g. N, NW) to CanvasResize.ResizeMode.
                                        ScriptCommands.Assign("{CanvasResize.ResizeMode}", "{EventArgs.Source.Name}", false, 
                                            HubScriptCommands.CaptureMouse(CaptureMouseMode.UIElement))))));


                case "PreviewTouchUp":
                case "PreviewMouseUp":
                    return
                        ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetX}", 0, false, 
                        ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetY}", 0, false, 
                        ScriptCommands.AssignGlobalParameterDic("{CanvasResize}", false,
                            ScriptCommands.Assign("{CanvasResize.IsResizing}", false, false,
                                HubScriptCommands.SetRoutedEventHandled(
                                    HubScriptCommands.CaptureMouse(CaptureMouseMode.Release,
                                      ScriptCommands.Subtract("{CanvasResize.CurrentPosition.X}", "{CanvasResize.StartPosition.X}", "{DiffX}", 
                                      ScriptCommands.Subtract("{CanvasResize.CurrentPosition.Y}", "{CanvasResize.StartPosition.Y}", "{DiffY}",                                        
                                      ScriptCommands.Switch<string>("{CanvasResize.ResizeMode}", 
                                        new Dictionary<string,IScriptCommand>()
                                        {
                                           { "N" , resizeNorthCommand },
                                           { "NE", ScriptCommands.RunSequence(resizeNorthCommand, resizeEastCommand) },
                                           { "E" , resizeEastCommand }, 
                                           { "SE", ScriptCommands.RunSequence(resizeSouthCommand, resizeEastCommand) },
                                           { "S" , resizeSouthCommand },
                                           { "SW", ScriptCommands.RunSequence(resizeSouthCommand, resizeWestCommand) },
                                           { "W" , resizeWestCommand },                                           
                                           { "NW", ScriptCommands.RunSequence(resizeNorthCommand, resizeWestCommand) },
                                        }, 
                                        ScriptCommands.PrintConsole("Not supported : {CanvasResize.ResizeMode}, {DiffX},{DiffY}"))))))))));         
            }
            return base.onEvent(eventId);
        }
    }
}
