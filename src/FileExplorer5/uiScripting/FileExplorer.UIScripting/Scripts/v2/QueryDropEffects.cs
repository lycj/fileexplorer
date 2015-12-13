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
        /// Get DropEffects by calling ISupportDrop.QueryDrop(Draggables,[DataObject]), and store it to destinationVariable.
        /// </summary>
        /// <param name="dropTargetVariable"></param>
        /// <param name="draggablesVariable"></param>
        /// <param name="dataObjVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryDropEffects(string dropTargetVariable = "{ISupportDrop}", string draggablesVariable = "{Draggables}", 
            string dataObjVariable = "{DataObj}", string allowedEffectVariable = null, string destinationVariable = "{Effects}", bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new QueryDropEffectsCommand()
            {
                DropTargetKey = dropTargetVariable,
                DraggablesKey = draggablesVariable,
                DataObjectKey = dataObjVariable,
                AllowedEffectsKey = allowedEffectVariable,
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
    public class QueryDropEffectsCommand : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {        
        /// <summary>
        /// Point to DataContext (ISupportDrop) to initialize the drag, default = "{ISupportDrop}".
        /// </summary>
        public string DropTargetKey { get; set; }

        public string DraggablesKey { get; set; }

        public string DataObjectKey { get; set; }

        /// <summary>
        /// If specified, use as AllowedEffect when query.
        /// </summary>
        public string AllowedEffectsKey { get; set; }

        /// <summary>
        /// Point to returned value (QueryDropResult) from DropTarget.QueryDrop() method, Default={Effects}.
        /// </summary>
        public string DestinationKey { get; set; }

        public bool SkipIfExists { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<QueryDrag>();



        public QueryDropEffectsCommand()
            : base("QueryDropEffectsCommand")
        {
            DropTargetKey = "{ISupportDrop}";
            DraggablesKey = "{Draggables}";
            DataObjectKey = "{DataObj}";
            AllowedEffectsKey = null;
            DestinationKey = "{Effects}";
            SkipIfExists = false;
        }



        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            DragEventArgs devnt = evnt as DragEventArgs;
            ISupportDrop dropTarget = pm.GetValue<ISupportDrop>(DropTargetKey);
            IEnumerable<IDraggable> draggables = pm.GetValue<IEnumerable<IDraggable>>(DraggablesKey);
            DragDropEffectsEx allowedEffects = pm.HasValue(AllowedEffectsKey) ? pm.GetValue<DragDropEffectsEx>(AllowedEffectsKey)
                : devnt != null ? (DragDropEffectsEx)devnt.AllowedEffects : DragDropEffectsEx.All;

            if (dropTarget != null && draggables != null)
            {
                QueryDropEffects queryDropEffect = QueryDropEffects.None;
                if (devnt != null)
                {
                    queryDropEffect = dropTarget.QueryDrop(draggables, allowedEffects);
                    devnt.Effects = (DragDropEffects)queryDropEffect.SupportedEffects;
                }
                else queryDropEffect = dropTarget.QueryDrop(draggables, allowedEffects);

                pm.SetValue(DestinationKey, queryDropEffect, SkipIfExists);
            }
            return NextCommand;
        }

    }
}
