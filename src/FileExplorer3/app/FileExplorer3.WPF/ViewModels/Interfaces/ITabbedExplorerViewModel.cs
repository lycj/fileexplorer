using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{

    public interface ITabbedExplorerViewModel : ITabControlViewModel<IExplorerViewModel>,
        IConductor, IConductActiveItem, IParent<IScreen>
    {
        IExplorerViewModel OpenTab(IEntryModel model = null);
        void CloseTab(IExplorerViewModel evm);
    }

}
