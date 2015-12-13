using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Command.ViewModel;
using QuickZip.UserControls.MVVM.Command.Model;

namespace QuickZip.UserControls.MVVM
{
    public interface IToolbarSupport
    {

        #region Methods

        void UpdateToolbar();                

        #endregion

        #region Public Properties
        /// <summary>
        /// Contains a list of toolbar items.
        /// </summary>
        ObservableCollection<CommandViewModel> ToolbarItemList { get; }

        /// <summary>
        /// Whether if Previewer is visible
        /// </summary>
        bool IsPreviewerVisible { get; }

        /// <summary>
        /// Whether if current item is bookmarked
        /// </summary>
        bool IsBookmarked { get; set; }

        /// <summary>
        /// If IsSimpleStatusbar, specify the text.
        /// </summary>
        string PreviewerSource { get; }

        /// <summary>
        /// ViewSize for filelist.
        /// </summary>
        int ViewSize { get; set; }

        /// <summary>
        /// ViewMode for filelist.
        /// </summary>
        ViewMode ViewMode { get; set; }


        
        #endregion
    }
}
