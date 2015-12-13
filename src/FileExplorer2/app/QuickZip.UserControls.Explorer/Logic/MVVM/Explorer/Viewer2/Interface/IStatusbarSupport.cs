using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.MVVM
{
    public interface IStatusbarSupport
    {
         #region Methods


        void UpdateStatusbar();

        #endregion

        #region Public Properties
        /// <summary>
        /// Contains a list of status items.
        /// </summary>
        ObservableCollection<MetadataViewModel> StatusItemList { get; }

        /// <summary>
        /// Whether to use one text simple statusbar, or use StatusItemList.
        /// </summary>
        bool IsSimpleStatusbar { get; }

        /// <summary>
        /// If IsSimpleStatusbar, specify the text.
        /// </summary>
        string StatusText { get; }

        /// <summary>
        /// Whether if SelectedViewModel = null?
        /// </summary>
        bool IsSelectedViewModelsEmpty { get; }

        #endregion
    }
}
