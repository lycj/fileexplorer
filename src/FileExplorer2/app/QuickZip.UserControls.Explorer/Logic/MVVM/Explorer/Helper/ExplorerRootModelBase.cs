using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.IO.Tools;
using System.IO;
//using QuickZip.IO.COFE.UserControls;
using QuickZip.IO.COFE;
using Cinch;

namespace QuickZip.UserControls.MVVM
{
    public class ExplorerRootModelBase : RootModelBase
    {
        #region Data

        private SimpleCommand _renameCommand = null, _refreshCommand = null, _openCommand = null,
            _expandCommand = null, _unselectAllCommand = null,
             _pasteCommand = null, _copyCommand = null, _newFolderCommand = null, 
            _selectAllCommand = null, _deleteCommand = null, _propertiesCommand = null,
            _contextMenuCommand = null, _dropCommand = null, _toggleBookmarkCommand = null;

        #endregion

        #region Public Properties

        public SimpleCommand OpenCommand { get { return _openCommand; } protected set { _openCommand = value; NotifyPropertyChanged("OpenCommand"); } }
        public SimpleCommand ExpandCommand { get { return _expandCommand; } protected set { _expandCommand = value; NotifyPropertyChanged("ExpandCommand"); } }
        public SimpleCommand NewFolderCommand { get { return _newFolderCommand; } protected set { _newFolderCommand = value; NotifyPropertyChanged("NewFolderCommand"); } }
        public SimpleCommand RenameCommand { get { return _renameCommand; } protected set { _renameCommand = value; NotifyPropertyChanged("RenameCommand"); } }
        public SimpleCommand RefreshCommand { get { return _refreshCommand; } protected set { _refreshCommand = value; NotifyPropertyChanged("RefreshCommand"); } }
        public SimpleCommand ContextMenuCommand { get { return _contextMenuCommand; } protected set { _contextMenuCommand = value; NotifyPropertyChanged("ContextMenuCommand"); } }
        public SimpleCommand UnselectAllCommand { get { return _unselectAllCommand; } protected set { _unselectAllCommand = value; NotifyPropertyChanged("UnselectAllCommand"); } }
        public SimpleCommand SelectAllCommand { get { return _selectAllCommand; } protected set { _selectAllCommand = value; NotifyPropertyChanged("SelectAllCommand"); } }
        public SimpleCommand CopyCommand { get { return _copyCommand; } protected set { _copyCommand = value; NotifyPropertyChanged("CopyCommand"); } }
        public SimpleCommand PasteCommand { get { return _pasteCommand; } protected set { _pasteCommand = value; NotifyPropertyChanged("PasteCommand"); } }
        public SimpleCommand DeleteCommand { get { return _deleteCommand; } protected set { _deleteCommand = value; NotifyPropertyChanged("DeleteCommand"); } }
        public SimpleCommand PropertiesCommand { get { return _propertiesCommand; } protected set { _propertiesCommand = value; NotifyPropertyChanged("PropertiesCommand"); } }
        public SimpleCommand ToggleBookmarkCommand { get { return _toggleBookmarkCommand; } protected set { _toggleBookmarkCommand = value; NotifyPropertyChanged("ToggleBookmarkCommand"); } }

        /// <summary>
        /// Occur when item dropped on the rootmodel
        /// </summary>
        public SimpleCommand DropCommand { get { return _dropCommand; } protected set { _dropCommand = value; NotifyPropertyChanged("DropCommand"); } }

        #endregion
    }
}
