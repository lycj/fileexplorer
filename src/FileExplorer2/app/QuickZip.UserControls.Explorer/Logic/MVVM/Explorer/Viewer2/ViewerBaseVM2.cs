using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace QuickZip.UserControls.MVVM
{
    public abstract class ViewerBaseVM2 : IViewerVM
    {
        #region Methods

        /// <summary>
        /// Invoked when the viewmodel unloaded (e.g. changed to another view model)
        /// </summary>
        public abstract void OnUnload();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Invoked when ExplorerViewModel.ExpandCommand triggered
        /// </summary>
        public abstract void Expand();

        /// <summary>
        /// Invoked when ExplorerViewModel.ContextMenuCommand triggered, null means no further action required.
        /// </summary>
        public virtual string ContextMenu(Point pos) { return null; }


        public virtual void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {

        }

        protected abstract string getToolTip();
        protected abstract string getLabel();
        protected abstract ImageSource getSmallIcon();

        #endregion


        public bool IsBookmarked
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int ViewSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ViewMode ViewMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        

        public bool IsContextMenuEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDirectoryTreeEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public string Label
        {
            get { throw new NotImplementedException(); }
        }

        public string ToolTip
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Media.ImageSource SmallIcon
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBreadcrumbVisible
        {
            get { throw new NotImplementedException(); }
        }

        public string MediaFile
        {
            get { throw new NotImplementedException(); }
        }

        public ViewerMode CurrentViewerMode
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDirectoryViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsWWWViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMediaViewModel
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateStatusbar()
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ObservableCollection<ViewModel.MetadataViewModel> StatusItemList
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSimpleStatusbar
        {
            get { throw new NotImplementedException(); }
        }

        public string StatusText
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSelectedViewModelsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateToolbar()
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ObservableCollection<Command.ViewModel.CommandViewModel> ToolbarItemList
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPreviewerVisible
        {
            get { throw new NotImplementedException(); }
        }

        public string PreviewerSource
        {
            get { throw new NotImplementedException(); }
        }
    }
}
