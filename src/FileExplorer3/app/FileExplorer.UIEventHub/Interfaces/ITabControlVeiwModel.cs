using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{
    public interface ITabControlViewModel<T>
       where T : IDraggable
    {
        int GetTabIndex(T evm);
        void MoveTab(int srcIdx, int targetIdx);

        int SelectedIndex { get; set; }
        T SelectedItem { get; set; }
    }

}
