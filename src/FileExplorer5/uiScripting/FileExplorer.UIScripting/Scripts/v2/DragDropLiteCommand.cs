using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FileExplorer.UIEventHub
{
    public static partial class DragDropScriptCommands
    {
        public static IScriptCommand StartCanvasDrag(string dragSourceVariable = "{ISupportDrag}",
            IScriptCommand nextCommand = null, IScriptCommand failCommand = null)
        {
            return new DragDropLiteCommand()
            {
                State = DragDropLiteState.StartCanvas,
                DragSourceKey = dragSourceVariable,
                NextCommand = (ScriptCommandBase)nextCommand,
                FailCommand = (ScriptCommandBase)failCommand
            };
        }


        public static IScriptCommand StartLiteDrag(string dragSourceVariable = "{ISupportDrag}",
            IScriptCommand nextCommand = null, IScriptCommand failCommand = null)
        {
            return
                HubScriptCommands.AssignDragMethod(QueryDrag.DragMethodKey,
                new DragDropLiteCommand()
            {
                State = DragDropLiteState.StartLite,
                DragSourceKey = dragSourceVariable,
                NextCommand = (ScriptCommandBase)nextCommand,
                FailCommand = (ScriptCommandBase)failCommand
            });
        }

        public static IScriptCommand EndLiteDrag(string dragSourceVariable = "{ISupportDrag}",
           IScriptCommand nextCommand = null, IScriptCommand failCommand = null)
        {
            return
                HubScriptCommands.AssignDragMethod(QueryDrag.DragMethodKey,
                new DragDropLiteCommand()
                {
                    State = DragDropLiteState.EndLite,
                    DragSourceKey = dragSourceVariable,
                    NextCommand = (ScriptCommandBase)nextCommand,
                    FailCommand = (ScriptCommandBase)failCommand
                });
        }


        public static IScriptCommand EndCanvasDrag(IScriptCommand nextCommand = null)
        {
            return new DragDropLiteCommand()
            {
                State = DragDropLiteState.EndCanvas,
                NextCommand = (ScriptCommandBase)nextCommand,
            };
        }

        public static IScriptCommand CancelCanvasDrag(IScriptCommand nextCommand = null)
        {
            return new DragDropLiteCommand()
            {
                State = DragDropLiteState.CancelCanvas,
                NextCommand = (ScriptCommandBase)nextCommand,
            };
        }
    }

    public enum DragDropLiteState { StartCanvas, EndCanvas, CancelCanvas, StartLite, EndLite }

    public class DragDropLiteCommand : UIScriptCommandBase<Control, RoutedEventArgs>
    {
        public DragDropLiteState State { get; set; }

        /// <summary>
        /// Point to DataContext (ISupportDrag) to initialize the drag, default = "{ISupportDrag}".
        /// </summary>
        public string DragSourceKey { get; set; }

        /// <summary>
        /// Point to DataContext (ISupportDrop) to initialize the drag, default = "{ISupportDrop}".
        /// </summary>
        public string DropTargetKey { get; set; }


        /// <summary>
        /// Current position relative to sender, adjusted with scrollbar position.
        /// </summary>
        public string CurrentPositionAdjustedKey { get; set; }

        public static string DragDropModeKey { get; set; }
        public static string DragDropDeviceKey { get; set; }
        public static string DragDropDraggingItemsKey { get; set; }
        public static string DragDropEffectsKey { get; set; }
        public static string DragDropDragSourceKey { get; set; }
        
        /// <summary>
        /// Destination of drop Default={DragDrop.DropTarget}, assigned by DragDropLiteEventProcessor during PointerMove and Up.
        /// </summary>
        public static string DragDropDropTargetKey { get; set; }
        public static string DragDropStartPositionKey { get; set; }

        public ScriptCommandBase FailCommand { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DragDropLiteCommand>();

        static DragDropLiteCommand()
        {
            DragDropModeKey = "{DragDrop.Mode}";
            DragDropDeviceKey = "{DragDrop.Device}";
            DragDropDraggingItemsKey = "{DragDrop.Draggables}";
            DragDropEffectsKey = "{DragDrop.Effects}";
            DragDropDragSourceKey = "{DragDrop.DragSource}";
            DragDropDropTargetKey = "{DragDrop.DropTarget}";
            DragDropStartPositionKey = "{DragDrop.StartPosition}";
        }

        public DragDropLiteCommand()
            : base("DragDropLiteCommand")
        {
            DragSourceKey = "{ISupportDrag}";
            DropTargetKey = "{ISupportDrop}";
            CurrentPositionAdjustedKey = "{CurrentPositionAdjusted}";
        }


        private bool dragStart(ParameterDic pm, IUIInput input, string mode)
        {
            ISupportDrag isd = pm.GetValue<ISupportDrag>(DragSourceKey);
            if (pm.GetValue<string>(DragDropModeKey) == null &&  isd != null)
            {
                var draggables = isd.GetDraggables();
                IDataObject dataObj = isd is ISupportShellDrag ?
                    (isd as ISupportShellDrag).GetDataObject(draggables) : null;
                DragDropEffectsEx effect = isd.QueryDrag(draggables);
                
                pm.SetValue(DragDropModeKey, mode);
                pm.SetValue(DragDropDeviceKey, input.InputType);
                pm.SetValue(DragDropDraggingItemsKey, draggables);
                pm.SetValue(DragDropEffectsKey, effect);
                pm.SetValue(DragDropDragSourceKey, isd);
                pm.SetValue(ParameterDic.CombineVariable(DragDropDragSourceKey, ".IsDraggingFrom", false), true);
                pm.SetValue(DragDropStartPositionKey, pm.GetValue<Point>(CurrentPositionAdjustedKey));
                pm.SetValue(InputKey, new DragInput(input, dataObj, DragDropEffectsEx.Copy, (eff) => { }));

                return true;
            }

            return false;
        }

        private void dragEnd(ParameterDic pm)
        {
            pm.SetValue<object>(DragDropModeKey, null);
            pm.SetValue<object>(DragDropDeviceKey, null);
            pm.SetValue<object>(DragDropDraggingItemsKey, null);
            pm.SetValue<object>(DragDropEffectsKey, null);
            pm.SetValue(ParameterDic.CombineVariable(DragDropDragSourceKey, ".IsDraggingFrom", false), false);
            pm.SetValue<object>(DragDropDragSourceKey, null);
            pm.SetValue<object>(DragDropDropTargetKey, null);
            pm.SetValue<object>(DragDropStartPositionKey, null);
        }

        protected override IScriptCommand executeInner(ParameterDic pm, Control sender,
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            if (!pm.HasValue<ParameterDic>("{DragDrop}"))
                ScriptCommands.AssignGlobalParameterDic("{DragDrop}", false).Execute(pm);
            if (!pm.HasValue<Point>(CurrentPositionAdjustedKey))
                HubScriptCommands.ObtainPointerPosition().Execute(pm);

            logger.Debug(State.ToString());
            switch (State)
            {
                case DragDropLiteState.StartLite:
                case DragDropLiteState.StartCanvas:
                    string mode = State == DragDropLiteState.StartLite ? "Lite" : "Canvas";
                    if (dragStart(pm, input, mode))
                    {
                        foreach (var item in
                             pm.GetValue<IEnumerable<IDraggable>>(DragDropDraggingItemsKey)
                             .Where(i => (State == DragDropLiteState.StartLite) || i is IPositionAware))
                            item.IsDragging = true;
                        return NextCommand;
                    }
                    else return FailCommand;

                case DragDropLiteState.EndLite:
                case DragDropLiteState.EndCanvas:
                case DragDropLiteState.CancelCanvas:
                    //foreach (var item in pm.GetValue<IEnumerable<IDraggable>>(DragDropDraggingItemsKey))
                    //    item.IsDragging = true;

                    if (pm.HasValue<Point>(DragDropStartPositionKey))
                    {
                        Point currentPosition = pm.GetValue<Point>(CurrentPositionAdjustedKey);
                        Point startPosition = pm.GetValue<Point>(DragDropStartPositionKey);
                        Vector movePt = currentPosition - startPosition;

                        var items = pm.GetValue<IEnumerable<IDraggable>>(DragDropDraggingItemsKey);                            
                        foreach (var item in items)
                            item.IsDragging = false;

                        if (State == DragDropLiteState.EndCanvas)
                            foreach (var posAwareItem in items.Cast<IPositionAware>())
                                posAwareItem.OffsetPosition(movePt);

                    }

                    dragEnd(pm);
                    return NextCommand;

                default: return ResultCommand.Error(new NotSupportedException(State.ToString()));
            }


        }
    }
}
