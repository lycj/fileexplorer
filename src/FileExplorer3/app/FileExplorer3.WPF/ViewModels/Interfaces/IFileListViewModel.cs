using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using System.ComponentModel;

namespace FileExplorer.WPF.ViewModels
{
    public interface IFileListViewModel : ISupportCommandManager, INotifyPropertyChanged
    {        
        /// <summary>
        /// Load entries and apply filters.
        /// </summary>
        IEntriesProcessor<IEntryViewModel> ProcessedEntries { get; }
                
        /// <summary>
        /// Allow customize columns and filters.
        /// </summary>
        IColumnsHelper Columns { get; }

        /// <summary>
        /// Responsible for item selection of the file list.
        /// </summary>
        IListSelector<IEntryViewModel, IEntryModel> Selection { get; }

        /// <summary>
        /// Setting the current directory will start the load of entries.
        /// </summary>
        IEntryModel CurrentDirectory { get; set; }

        bool EnableDrag { get; set; }
        bool EnableDrop { get; set; }
        bool EnableContextMenu { get; set; }
        bool EnableMultiSelect { get; set; }
        bool ShowToolbar { get; set; }
        bool ShowSidebar { get; set; }
        bool ShowGridHeader { get; set; }

        bool IsCheckBoxVisible { get; set; }

        IFileListParameters Parameters { get; set; }


        //bool IsContextMenuVisible { get; set; }

        void SignalChangeDirectory(IEntryModel newDirectory);
        Task SetCurrentDirectoryAsync(IEntryModel em);

        ISidebarViewModel Sidebar { get; }

        IProfile[] Profiles { set; }    
    

    }
}
