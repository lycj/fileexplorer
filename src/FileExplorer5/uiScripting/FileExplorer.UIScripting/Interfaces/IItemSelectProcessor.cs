using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{
    public interface IItemSelectProcessor
    {
        void Select(ISelectable item, bool inSelectedList);
    }
    public class ItemSelectProcessor : IItemSelectProcessor
    {
        public static IItemSelectProcessor SelectItemInSelectedList =
            new ItemSelectProcessor((vm, inList) => { vm.IsSelected = inList; });
        public static IItemSelectProcessor AppendItemInSelectedList =
            new ItemSelectProcessor((vm, inList) => { vm.IsSelected = vm.IsSelected || inList; });
        public static IItemSelectProcessor ToggleItemInSelectedList =
            new ItemSelectProcessor((vm, inList) =>
            {
                if (inList)
                    vm.IsSelected = !vm.IsSelected;
            });

        private Action<ISelectable, bool> _selectFunc;
        protected ItemSelectProcessor(Action<ISelectable, bool> selectFunc)
        {
            _selectFunc = selectFunc;
        }
        public void Select(ISelectable item, bool inSelectedList)
        {
            _selectFunc(item, inSelectedList);
        }
    }
}
