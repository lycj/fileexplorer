using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.ViewModels
{
    public interface IDirectoryTreeViewModel : ISupportTreeSelector<IDirectoryNodeViewModel, IEntryModel>,  ISupportCommandManager
    {
        IEntryModel[] RootModels { get;  set; }
        IProfile[] Profiles { set; }

        bool EnableDrag { get; set; }
        bool EnableDrop { get; set; }
        bool EnableContextMenu { get; set; }

        Task SelectAsync(IEntryModel value);
        void ExpandRootEntryModels();
    }

    public interface IDirectoryNodeViewModel : IEntryViewModel, ISupportTreeSelector<IDirectoryNodeViewModel, IEntryModel>, IDraggable
    {
        bool ShowCaption { get; set; }
        bool IsBringIntoView { get; set; }
    }

}
