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
    public interface IBreadcrumbItemViewModel : IEntryViewModel, ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>
    {
        #region Constructor
        
        #endregion

        #region Methods
        
        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        bool ShowCaption { get; set; }        
        IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }
        
        #endregion
    }
}
