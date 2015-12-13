using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickZip.UserControls.MVVM
{
    public interface IViewerVM : IViewerModeSupport, IStatusbarSupport, IToolbarSupport
    {
        #region Methods
        /// <summary>
        /// Invoked when the viewmodel unloaded (e.g. changed to another view model)
        /// </summary>
        void OnUnload();
        /// <summary>
        /// Execute Viewer refresh code.
        /// </summary>
        void Refresh();
        /// <summary>
        /// Expand the selected item.
        /// </summary>
        void Expand();
        /// <summary>
        /// Show Context menu at the specified position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        string ContextMenu(Point pos);

        

        #endregion

        #region Public Properties

        bool IsContextMenuEnabled { get; }
        bool IsDirectoryTreeEnabled { get; }
        string Label { get; }
        string ToolTip { get; }
        ImageSource SmallIcon { get; }
        bool IsBreadcrumbVisible { get; }
        string MediaFile { get; }

        #endregion
    }
}
