using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.UserControls.MVVM
{
    public interface IViewerModeSupport
    {

        #region Public Properties

        ViewerMode CurrentViewerMode { get; }

        /// <summary>
        /// Used by View to decide to show which control
        /// </summary>
        bool IsDirectoryViewModel { get; }
        
        /// <summary>
        /// Used by View to decide to show which control
        /// </summary>
        bool IsWWWViewModel { get; }
        
        /// <summary>
        /// Used by View to decide to show which control
        /// </summary>
        bool IsMediaViewModel { get; }

        #endregion
    }
}
