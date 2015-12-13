using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Call droptarget (ISupportDrop).Drop and assign result to destination variable.
        /// </summary>
        /// <param name="dragSourceVariable"></param>
        /// <param name="dropTargetVariable"></param>
        /// <param name="draggablesVariable"></param>
        /// <param name="dataObjVariable"></param>
        /// <param name="dropEffectVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryDrop(string dragSourceVariable = null, string dropTargetVariable = "{ISupportDrop}", string draggablesVariable = "{Draggables}",
            string dataObjVariable = "{DataObj}", string dropEffectVariable = "{Effect}",
            string destinationVariable = "{ResultEffect}",
            bool skipIfExists = false,
            IScriptCommand nextCommand = null)
        {
            return new QueryDropCommand()
            {
                DragSourceKey = dragSourceVariable,
                DropTargetKey = dropTargetVariable,
                DraggablesKey = draggablesVariable,
                DataObjectKey = dataObjVariable,
                DropEffectKey = dropEffectVariable,
                DestinationKey = destinationVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Use DropTarget's QueryDrop to obtain DragDropEffectsEx (QueryDropResult), which contains
    /// PreferredEffect and SupportedEffects
    /// </summary>
    public class QueryDropCommand : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {
        /// <summary>
        /// Point to DataContext (ISupportDrag) to initialize the drag, for notify the drag completed with effect, optional, default = null.
        /// </summary>
        public string DragSourceKey { get; set; }
        /// <summary>
        /// Point to DataContext (ISupportDrop) to initialize the drag, default = "{ISupportDrop}".
        /// </summary>
        public string DropTargetKey { get; set; }

        public string DraggablesKey { get; set; }

        public string DataObjectKey { get; set; }

        /// <summary>
        /// Point to desired DragDropEffect to pass to ISupportDrop.Drop() method, if not set the effect 
        /// will be obtained from the EventArgs (DragEventArgs), Default={DropEffect}.
        /// </summary>
        public string DropEffectKey { get; set; }

        /// <summary>
        /// Point to returned value (DragDropEffects) from DropTarget.Drop() method, Default={DropEffect}.
        /// </summary>
        public string DestinationKey { get; set; }

        public bool SkipIfExists { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<QueryDrag>();



        public QueryDropCommand()
            : base("QueryDropCommand")
        {
            DragSourceKey = null;
            DropTargetKey = "{ISupportDrop}";
            DraggablesKey = "{Draggables}";
            DataObjectKey = "{DataObj}";
            DropEffectKey = "{DropEffect}";
            SkipIfExists = false;
        }



        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            DragEventArgs devnt = evnt as DragEventArgs;
            ISupportDrag dragSource = pm.GetValue<ISupportDrag>(DragSourceKey);
            ISupportDrop dropTarget = pm.GetValue<ISupportDrop>(DropTargetKey);
            IEnumerable<IDraggable> draggables = pm.GetValue<IEnumerable<IDraggable>>(DraggablesKey).ToList();
            if (dropTarget != null && draggables != null)
            {
                DragDropEffectsEx effect =
                    pm.HasValue<DragDropEffectsEx>(DropEffectKey) ? pm.GetValue<DragDropEffectsEx>(DropEffectKey) :
                    devnt != null ? (DragDropEffectsEx)devnt.AllowedEffects : DragDropEffectsEx.All;
                DragDropEffectsEx resultEffect = DragDropEffectsEx.None;

                var dataObj = pm.GetValue<IDataObject>(DataObjectKey);

                if (effect != DragDropEffectsEx.None)
                    if (dropTarget is ISupportShellDrop)
                        resultEffect = (dropTarget as ISupportShellDrop).Drop(draggables, dataObj, effect);
                    else resultEffect = dropTarget.Drop(draggables, effect);

                if (devnt != null)
                    devnt.Effects = (DragDropEffects)resultEffect;

                pm.SetValue(DestinationKey, resultEffect, SkipIfExists);
                if (dragSource != null)
                    dragSource.OnDragCompleted(draggables, resultEffect);
            }

            return NextCommand;
        }

    }
}
