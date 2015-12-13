using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.ComponentModel;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Collections;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM
{
    public interface IDirectoryViewerVM<FI, DI, FSI> : IEntryViewerVM<FI, DI, FSI>
        where FI : FSI
        where DI : FSI
    {

        #region Methods

       

        #endregion


        #region Public Properties


        /// <summary>
        /// Specify the DirectoryViewModel it respresenting.
        /// </summary>
        DirectoryViewModel<FI, DI, FSI> EmbeddedDirectoryViewModel { get; }

        

        #region SubEntries, Sort

        /// <summary>
        /// Subentries of the current model.
        /// </summary>
        CollectionViewSource SubEntries { get; }

        /// <summary>
        /// Specify how subentries sort it's items.
        /// </summary>
        SortCriteria SortBy { get; }

        /// <summary>
        /// Specify the direction of sorting (ascending of descending).
        /// </summary>
        ListSortDirection SortDirection { get; }

        #endregion

        #region Selection

        IList UISelectedItems { set; }
        List<EntryModel<FI, DI, FSI>> SelectedModels { get; }

        List<EntryViewModel<FI, DI, FSI>> SelectedViewModels { get; }

        #endregion



        #region Expand related.

        /// <summary>
        /// Whether Expand command is enabled.
        /// </summary>
        bool IsExpandEnabled { get; }

        /// <summary>
        /// How to react when a directory is d-clicked.
        /// </summary>
        OpenMode DirectoryExpandMode { get; set;}

        /// <summary>
        /// How to react when a file is d-clicked.
        /// </summary>
        OpenMode FileExpandMode { get; set; }

        #endregion

        #endregion
    }
}
