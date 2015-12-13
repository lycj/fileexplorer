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
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand SelectItems(IScriptCommand nextCommand = null)
        {
            return new SelectItems(ItemSelectProcessor.SelectItemInSelectedList)
            {
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand SelectItemsByUpdate(IScriptCommand nextCommand = null)
        {
            return new SelectItems(ItemSelectProcessor.AppendItemInSelectedList)
            {
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }        
    }

    public class SelectItems : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        private IItemSelectProcessor _processor;
        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SelectItems>();

        internal SelectItems(IItemSelectProcessor processor)
            : base("SelectItems")
        {
            _processor = processor;
        }


        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(ic);
            var selectedIdList = pm.ContainsKey("SelectedIdList") ? pm["SelectedIdList"] as List<int> : null;
            var selectedList = pm.ContainsKey("SelectedList") ? pm["SelectedList"] as List<object> : null;

            if (pm.ContainsKey("UnselectCommand"))
            {
                ICommand unselectCommand = pm["UnselectCommand"] as ICommand;
                if (unselectCommand != null && unselectCommand.CanExecute(ic))
                    unselectCommand.Execute(ic);
            }


            bool isISelectable = ic.Items.Count > 0 && ic.Items[0] is ISelectable;


            //Action<int, object, ListBoxItem> updateSelected = null;
            Func<int, FrameworkElement, bool> returnSelected = (idx, item) => false;
            if (selectedIdList != null)
                returnSelected = (idx, item) => selectedIdList.Contains(idx);
            else if (selectedList != null)
                returnSelected = (idx, item) => selectedList.Contains(item);

            logger.Debug(String.Format("Selecting {0} items", selectedIdList != null ? selectedIdList.Count() :
                selectedList.Count()));

            //updateSelected = (idx, vm, item) => _processor.Select(
            //    ExtensionMethods.ToISelectable(vm, item), returnSelected(idx, item));

            for (int i = 0; i < ic.Items.Count; i++)
            {
                FrameworkElement ele = ic.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                
                ISelectable item = ic.Items[i] as ISelectable;
                item.IsSelected = returnSelected(i, ele);
                item.IsSelecting = false;
                //if (ele != null)
                //    AttachedProperties.SetIsSelecting(ele, false);

            }
            return NextCommand;
        }
    }
}
