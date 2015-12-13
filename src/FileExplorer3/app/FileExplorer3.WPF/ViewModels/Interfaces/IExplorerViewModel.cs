using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Models;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.ViewModels
{
    public interface IExplorerViewModel : ISupportCommandManager, 
        IScreen, IDraggable, ISelectable
    {
        IExplorerInitializer Initializer { get; set; }

        IEntryModel[] RootModels { get; set; }
       
        IDirectoryTreeViewModel DirectoryTree { get; }
        IFileListViewModel FileList { get; }
        IStatusbarViewModel Statusbar { get; }
        ISidebarViewModel Sidebar { get; }
        INavigationViewModel Navigation { get; }
        IBreadcrumbViewModel Breadcrumb { get; }

        IExplorerParameters Parameters { get; set; }
        string FilterStr { get; set; }
        IEntryViewModel CurrentDirectory { get; }        
        Task GoAsync(string gotoPath);
        Task GoAsync(IEntryModel entryModel);

        IEventAggregator Events { get; set; }
        IEventAggregator InternalEvents { get; set; }
        IWindowManager WindowManager { get; set;  }
    }
}
