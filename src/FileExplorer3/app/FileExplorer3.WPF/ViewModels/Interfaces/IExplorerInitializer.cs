using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public interface IExplorerInitializer
    {
        IEventAggregator Events { get; set; }
        IWindowManager WindowManager { get; set; }
        IEntryModel[] RootModels { get; set; }

        IExplorerInitializer Clone();

        [Obsolete]
        List<IViewModelInitializer<IExplorerViewModel>> Initializers { get; }

        Task InitializeModelCreatedAsync(IExplorerViewModel evm);
        Task InitializeViewAttachedAsync(IExplorerViewModel evm);
    }

    
}
