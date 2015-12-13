using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public interface IToggleableVisibility : INotifyPropertyChanged
    {
        bool IsVisible { get; set; }
    }

    public interface ISidebarViewModel :  
        IToggleableVisibility, ISupportCommandManager
    {
        IMetadataHelperViewModel Metadata { get; }
    }
}
