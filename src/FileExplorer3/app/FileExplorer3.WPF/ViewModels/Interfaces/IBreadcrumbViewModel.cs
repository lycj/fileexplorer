using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public interface IBreadcrumbViewModel : ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>, ISupportCommandManager
    {
        IEntryModel[] RootModels { set; }
        IProfile[] Profiles { set; }

        bool EnableBreadcrumb { get; set; }
        bool EnableBookmark { get; set; }

        Task SelectAsync(IEntryModel value);
    }
}
