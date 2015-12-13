using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand HighlightItems(IScriptCommand nextCommand = null)
        {
            return new HighlightItems(false)
                {
                    NextCommand = (ScriptCommandBase)nextCommand
                };
        }
        //public static IScriptCommand HighlightItemsByUpdate(IScriptCommand nextCommand = null)
        //{
        //    return new HighlightItems(true)
        //    {
        //        NextCommand = (ScriptCommandBase)nextCommand
        //    };
        //}
    }

    public class HighlightItems : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        private bool _update;

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<HighlightItems>();

        internal HighlightItems(bool update) : base("HighlightItems")
        {
            _update = update;
        }


        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {            
            if (!_update)
            {                
                var selectedIdList = pm.ContainsKey("SelectedIdList") ? pm["SelectedIdList"] as List<int>
                    : new List<int>();

                logger.Debug(String.Format("Highlighting {0} items", selectedIdList.Count()));

                for (int i = 0; i < ic.Items.Count; i++)
                {
                    DependencyObject item = ic.ItemContainerGenerator.ContainerFromIndex(i);
                    if (item != null)
                    {
                        bool isSelecting = selectedIdList.Contains(i);
                        //UIEventHubProperties.SetIsSelecting(item, isSelecting);
                        var selectable = (item as FrameworkElement).DataContext as ISelectable;
                        if (selectable != null)
                            selectable.IsSelecting = isSelecting;
                    }
                    
                }
            }
            else
            {
                List<object> unselectedList = pm["UnselectedList"] as List<object>;
                List<int> unselectedIdList = pm["UnselectedIdList"] as List<int>;

                List<object> selectedList = pm["SelectedList"] as List<object>;
                List<int> selectedIdList = pm["SelectedIdList"] as List<int>;

                logger.Debug(String.Format("Highlighting {0} items", selectedIdList.Count()));
                logger.Debug(String.Format("De-highlighting {0} items", unselectedIdList.Count()));

                for (int i = 0; i < ic.Items.Count; i++)
                {
                    DependencyObject ele = ic.ItemContainerGenerator.ContainerFromIndex(i);
                     ISelectable item = ic.Items[i] as ISelectable;


                    if (ele != null)
                    {

                        bool isSelecting = UIEventHubProperties.GetIsSelecting(ele);
                        if (isSelecting && unselectedList.Contains(ele))
                            item.IsSelecting = false;
                        //AttachedProperties.SetIsSelecting(item, false);
                        else if (!isSelecting && selectedList.Contains(ele))
                            item.IsSelecting = true;
                            //AttachedProperties.SetIsSelecting(ele, true);
                    }
                }

            }

            return NextCommand;
        }
    }
}
