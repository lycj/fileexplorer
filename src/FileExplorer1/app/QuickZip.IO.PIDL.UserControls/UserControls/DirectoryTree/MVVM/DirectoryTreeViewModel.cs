using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cinch;
using QuickZip.IO.PIDL.UserControls.Model;
using System.IO;
using System.Diagnostics;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class DirectoryTreeViewModel : RootModelBase
    {

        #region Constructor

        public DirectoryTreeViewModel()
        {
            setUpBackgroundWorker();

            #region FileSystemWatcher
            _watcher = new FileSystemWatcherEx(DirectoryInfoEx.DesktopDirectory);
            var handler = (FileSystemEventHandlerEx)delegate(object sender, FileSystemEventArgsEx args)
            {
                BroadcastChange(args.FullPath, args.ChangeType);
            };
            var renameHandler = (RenameEventHandlerEx)delegate(object sender, RenameEventArgsEx args)
            {
                BroadcastChange(args.OldFullPath, args.ChangeType);
            };

            _watcher.OnChanged += handler;
            _watcher.OnCreated += handler;
            _watcher.OnDeleted += handler;
            _watcher.OnRenamed += renameHandler;
            #endregion

           
        }

        #endregion

        #region Data

        private bool _isLoading = false;
        private bool _autoCollapse = true;
        private FileSystemWatcherEx _watcher = null;
        private DirectoryInfoEx _rootDirectory = null;
        private DirectoryInfoEx _selectedDirectory = null;
        private DirectoryInfoEx _prevSelectedDirectory = null;
        private DirectoryTreeItemViewModel _selectedDirectoryModel = null;
        //private DirectoryTreeItemViewModel _prevSelectedDirectoryModel = null;
        public Cinch.DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel> _rootDirectoryModelList
            = new DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>();
        private BackgroundTaskManager<DirectoryTreeItemViewModel>
           bgWorker_findChild = null;

        #endregion

        #region Methods

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_watcher != null)
                _watcher.Dispose();
        }

        public void BroadcastCurrentDirectoryChanging(DirectoryInfoEx newDirectory)
        {
            foreach (DirectoryTreeItemViewModel rootmodel in _rootDirectoryModelList)
            {
                rootmodel.BroascastCurrentDirectoryChanging(newDirectory);
            }
        }


        public void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            foreach (DirectoryTreeItemViewModel rootmodel in _rootDirectoryModelList)
            {
                rootmodel.BroadcastChange(parseName, changeType);
            }
        }

        private void setUpBackgroundWorker()
        {
            bgWorker_findChild = new BackgroundTaskManager<DirectoryTreeItemViewModel>(
                () =>
                {
                    IsLoading = true;

                    DirectoryTreeItemViewModel lookingUpModel = _selectedDirectoryModel;
                    Func<bool> cancelNow = () =>
                    {
                        bool cont = lookingUpModel != null && lookingUpModel.Equals(_selectedDirectoryModel);
                        return !cont;
                    };

                    DirectoryTreeItemViewModel newSelectedModel = null;
                    
                    {
                        DirectoryInfoEx newSelectedDir = _selectedDirectory;
                        if (newSelectedDir != null)
                        {
                            foreach (DirectoryTreeItemViewModel rootModel in _rootDirectoryModelList)
                            {
                                newSelectedModel = rootModel.LookupChild(newSelectedDir, cancelNow);
                                if (newSelectedModel != null)
                                    return newSelectedModel;
                            }
                        }
                    }

                    return _rootDirectoryModelList.Count == 0 ? null : _rootDirectoryModelList[0];
                },
                (result) =>
                {
                    if (result != null)
                    {                        
                        if (result.Equals(_selectedDirectoryModel))
                            result.IsSelected = true;                        
                    }
                    IsLoading = false;
                });

        }

        #endregion

        #region Public Properties

        static PropertyChangedEventArgs bgWorkerChangeArgs =
            ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.BgWorker_FindChild);

        public BackgroundTaskManager<DirectoryTreeItemViewModel> BgWorker_FindChild
        {
            get { return bgWorker_findChild; }
            set
            {
                bgWorker_findChild = value;
                NotifyPropertyChanged(bgWorkerChangeArgs);
            }
        }

        static PropertyChangedEventArgs rootDirectoryModelListChangeArgs =
          ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.RootDirectoryModelList);

        public DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel> RootDirectoryModelList
        {
            get { return _rootDirectoryModelList; }
            set
            {
                _rootDirectoryModelList = value;
                NotifyPropertyChanged(rootDirectoryModelListChangeArgs);
            }
        }

        static PropertyChangedEventArgs rootDirectoryChangeArgs =
          ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.RootDirectory);

        public DirectoryInfoEx RootDirectory
        {
            get { return _rootDirectory; }
            set
            {
                _rootDirectory = value;
                NotifyPropertyChanged(rootDirectoryChangeArgs);
                RootDirectoryModelList.Clear();
                RootDirectoryModelList.Add(
                    new DirectoryTreeItemViewModel(this, null, ExModel.FromExEntry(_rootDirectory)));
            }
        }

        static PropertyChangedEventArgs selectedDirectoryModelChangeArgs =
          ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.SelectedDirectoryModel);

        public DirectoryTreeItemViewModel SelectedDirectoryModel
        {
            get { return _selectedDirectoryModel; }
            set
            {
                _selectedDirectoryModel = value;
                NotifyPropertyChanged(selectedDirectoryModelChangeArgs);
                bgWorker_findChild.RunBackgroundTask();
            }
        }

        static PropertyChangedEventArgs selectedDirectoryChangeArgs =
          ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.SelectedDirectory);

        public DirectoryInfoEx SelectedDirectory
        {
            get { return _selectedDirectory; }
            set
            {
                if (AutoCollapse && SelectedDirectoryModel != null
                    && !SelectedDirectoryModel.EmbeddedDirectoryModel.EmbeddedDirectoryEntry.Equals(value))
                    BroadcastCurrentDirectoryChanging(value);

                if (_selectedDirectory == null || !_selectedDirectory.Equals(value))
                {
                    _prevSelectedDirectory = _selectedDirectory;
                    _selectedDirectory = value;
                    NotifyPropertyChanged(selectedDirectoryChangeArgs);
                    SelectedDirectoryModel = new DirectoryTreeItemViewModel(this,
                        null, ExModel.FromExEntry(_selectedDirectory));
                }
            }
        }

        static PropertyChangedEventArgs isLoadingChangeArgs =
          ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.IsLoading);

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(isLoadingChangeArgs);
            }
        }

        static PropertyChangedEventArgs autoCollapseChangeArgs =
         ObservableHelper.CreateArgs<DirectoryTreeViewModel>(x => x.AutoCollapse);

        public bool AutoCollapse
        {
            get { return _autoCollapse; }
            set
            {
                _autoCollapse = value;
                NotifyPropertyChanged(isLoadingChangeArgs);
            }
        }

        #endregion

    }
}
