using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using QuickZip.IO.PIDL.UserControls.Model;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Data;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class CurrentDirectoryViewModel : DirectoryViewModel
    {
        #region Constructor
        public CurrentDirectoryViewModel(FileListViewModel rootModel, Model.DirectoryModel model)
            : base(rootModel, model)
        {
            IsLoaded = false;
            _rootModel = rootModel;

            _subEntries = new CollectionViewSource();            
            _subEntries.Source = SubEntriesInternal;
            _subEntries.SortDescriptions.Add(new SortDescription("IsDirectory", ListSortDirection.Descending));
            _subEntries.SortDescriptions.Add(new SortDescription("FullName", ListSortDirection.Ascending));
            
            _refreshCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => Refresh()
            };


            #region FileSystemWatcher
            _watcher = new FileSystemWatcherEx(model.EmbeddedDirectoryEntry);

            var handler = (FileSystemEventHandlerEx)delegate(object sender, FileSystemEventArgsEx args)
            {
                if (args.FullPath.Equals(model.FullName))
                    Refresh();
            };
            var renameHandler = (RenameEventHandlerEx)delegate(object sender, RenameEventArgsEx args)
            {
                if (args.OldFullPath.Equals(model.FullName))
                    Refresh();
            };

            _watcher.OnChanged += handler;
            _watcher.OnCreated += handler;
            _watcher.OnDeleted += handler;
            _watcher.OnRenamed += renameHandler;
            #endregion


        }
        #endregion

        #region Methods
        protected override void OnDispose()
        {
            base.OnDispose();
            if (_watcher != null)
                _watcher.Dispose();
        }

        protected override void FetchData(bool refresh)
        {
            base.FetchData(refresh);

            if (bgWorker_LoadSubEntries == null)
                setUpBackgroundWorker();     

            if (!IsLoaded)
            {
                refresh = true; //Not loaded before, force refresh.
                SubEntriesInternal.Clear();
            }

            if (refresh)
            {
                Refresh();
            }
        }

        internal void NotifySelectionChanged(FileListViewItemViewModel model, bool isSelected)
        {
            lock (_selectedSubEntries)
                if (isSelected)
                    if (!_selectedSubEntries.Contains(model))
                        _selectedSubEntries.Add(model);
                    else
                        if (_selectedSubEntries.Contains(model))
                            _selectedSubEntries.Remove(model);
            SelectedCount = (uint)_selectedSubEntries.Count;
        }

        internal void NotifyIsEditingChanged(FileListViewItemViewModel model)
        {
            NotifyPropertyChanged(isEditingChangeArgs);
        }

        public void Refresh()
        {
            try //Make sure entry still exists.
            {
                if (!IsLoading)
                {
                    FileSystemInfoEx entry = EmbeddedDirectoryModel.EmbeddedEntry;
                    if (entry != null)
                    {
                        bgWorker_LoadSubEntries.RunBackgroundTask();
                    }
                }
            }
            catch (Exception) {  }
        }

        public void Select(FileSystemInfoEx[] itemsToSelect)
        {
            foreach (FileListViewItemViewModel item in SubEntries.View)            
                item.IsSelected = itemsToSelect.Contains(item.EmbeddedModel.EmbeddedEntry);            
        }

        public void Select(FileSystemInfoEx itemToSelect)
        {
            Select(new FileSystemInfoEx[] { itemToSelect });
        }


        public void UnselectAll()
        {
            foreach (FileListViewItemViewModel item in SubEntries.View)
                item.IsSelected = false;
        }

        public void SelectAll()
        {
            foreach (FileListViewItemViewModel item in SubEntries.View)
                item.IsSelected = true;
        }

        public FileListViewItemViewModel this[FileSystemInfoEx itemToSelect]
        {
            get
            {
                foreach (FileListViewItemViewModel item in SubEntries.View)
                    if (item.EmbeddedModel.EmbeddedEntry.Equals(itemToSelect))                                            
                        return item;
                    
                return null;
            }
        }

        public DirectoryInfoEx NewFolder()
        {
            string template = "New Folder{0}";            
            string folderName = String.Format(template, "").Trim(' ');
            int idx = 1;
            while (EmbeddedDirectoryModel.EmbeddedDirectoryEntry[folderName] != null)
                folderName = String.Format(template, "(" + idx++.ToString() + ")");

            DirectoryInfoEx retDir = this.EmbeddedDirectoryModel.EmbeddedDirectoryEntry.CreateDirectory(folderName);

            //Hook select the new item after refreshed.
            EventHandler<EventArgs> handler = null;
            handler = (EventHandler<EventArgs>)delegate(object sender, EventArgs args)
            {
                Select(retDir);
                bgWorker_LoadSubEntries.BackgroundTaskCompleted -= handler;
            };
            bgWorker_LoadSubEntries.BackgroundTaskCompleted += handler;
            //Refresh
            Refresh();

            return retDir;
        }

        private List<FileListViewItemViewModel> getEntries()
        {
            var retVal = from entry in EmbeddedDirectoryModel.EmbeddedDirectoryEntry.EnumerateFileSystemInfos()
                         where (entry is DirectoryInfoEx && ListDirectories) || (entry is FileInfoEx && ListFiles)
                         select new FileListViewItemViewModel(_rootModel, ExModel.FromExEntry(entry)); ;
            _cachedSubEntries = retVal.ToArray();

            return new List<FileListViewItemViewModel>(_cachedSubEntries);
        }

        private List<FileListViewItemViewModel> filterEntries()
        {
            if (_cachedSubEntries == null)
                _cachedSubEntries = getEntries().ToArray();
            var retVal = from entry in _cachedSubEntries
                         where (String.IsNullOrEmpty(Filter) || IOTools.MatchFileMask(entry.Name, Filter + "*"))
                         select entry;
            return new List<FileListViewItemViewModel>(retVal);
        }

        //0.2
        public void ChangeSortMethod(ExComparer.SortCriteria sortBy, ListSortDirection sortDirection)
        {                        
            ListCollectionView dataView = (ListCollectionView)(CollectionViewSource.GetDefaultView(_subEntries.View));

            dataView.SortDescriptions.Clear();
            dataView.CustomSort = null;

            ExComparer.SortDirectionType direction = sortDirection == ListSortDirection.Ascending ? 
                ExComparer.SortDirectionType.sortAssending : ExComparer.SortDirectionType.sortDescending;
            
            dataView.CustomSort = new ExModelComparer(sortBy, direction);             
           
        }

        private void setUpBackgroundWorker()
        {
            Action<List<FileListViewItemViewModel>> updateSubEntries = (result) =>
                {
                    List<FileListViewItemViewModel> delList = new List<FileListViewItemViewModel>(SubEntriesInternal.ToArray());
                    List<FileListViewItemViewModel> addList = new List<FileListViewItemViewModel>();

                    foreach (FileListViewItemViewModel model in result)
                        if (delList.Contains(model))
                            delList.Remove(model);
                        else addList.Add(model);

                    foreach (FileListViewItemViewModel model in delList)
                        SubEntriesInternal.Remove(model);

                    foreach (FileListViewItemViewModel model in addList)
                        SubEntriesInternal.Add(model);

                    DirectoryCount = (uint)(from model in SubEntriesInternal where model.EmbeddedModel is DirectoryModel select model).Count();
                    FileCount = (uint)(SubEntriesInternal.Count - DirectoryCount);

                    HasSubEntries = SubEntriesInternal.Count > 0;
                    
                };


            bgWorker_LoadSubEntries = new BackgroundTaskManager<List<FileListViewItemViewModel>>(
                () =>
                {
                    IsLoading = true;                   
                    return getEntries();
                },
                (result) =>
                {
                    updateSubEntries(result);                   
                    IsLoading = false;
                });            

            bgWorker_FilterSubEntries = new BackgroundTaskManager<List<FileListViewItemViewModel>>(
                () =>
                {
                    IsLoading = true;
                    return filterEntries();
                },
                (result) =>
                {
                    updateSubEntries(result);
                    IsLoading = false;
                });
            
        }

        #endregion

        #region Data

        private FileSystemWatcherEx _watcher = null;
        private string _filter = null;
        private bool _listFile = true;
        private bool _listDir = true;
        private bool _hasSubEntries = false;
        private uint _fileCount = 0;
        private uint _dirCount = 0;
        private uint _selCount = 0;
        private FileListViewModel _rootModel;
        private bool _isLoading = false;
        private SimpleCommand _refreshCommand;
        private List<FileListViewItemViewModel> _selectedSubEntries = new List<FileListViewItemViewModel>();
        private FileListViewItemViewModel[] _cachedSubEntries;
        

        private ObservableCollection<FileListViewItemViewModel> _subEntriesInternal =
            new ObservableCollection<FileListViewItemViewModel>();
        private CollectionViewSource _subEntries;
        private BackgroundTaskManager<List<FileListViewItemViewModel>>
           bgWorker_LoadSubEntries = null;

        private BackgroundTaskManager<List<FileListViewItemViewModel>>
           bgWorker_FilterSubEntries = null;

        #endregion

        #region Public Properties

        public bool IsLoaded { get; private set; }

        public SimpleCommand RefreshCommand { get { return _refreshCommand; } }

        public Model.DirectoryModel EmbeddedDirectoryModel
        {
            get { return EmbeddedModel as Model.DirectoryModel; }
        }

        static PropertyChangedEventArgs listFilesChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.ListFiles);

        public bool ListFiles
        {
            get { return _listFile; }
            set { _listFile = value; NotifyPropertyChanged(listFilesChangeArgs); }
        }

        static PropertyChangedEventArgs listDirsChangeArgs =
          ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.ListDirectories);


        public bool ListDirectories
        {
            get { return _listDir; }
            set { _listDir = value; NotifyPropertyChanged(listDirsChangeArgs); }
        }

        static PropertyChangedEventArgs isLoadingChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.IsLoading);

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(isLoadingChangeArgs);
            }
        }

        static PropertyChangedEventArgs isEditingChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.IsEditing);

        public bool IsEditing
        {
            get { return SelectedCount == 1 && SelectedViewModels[0].IsEditing; }
            set { if (SelectedCount == 1) SelectedViewModels[0].IsEditing = value; }
        }


        static PropertyChangedEventArgs filterChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.Filter);

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                NotifyPropertyChanged(filterChangeArgs);                
                bgWorker_FilterSubEntries.RunBackgroundTask();
            }
        }


        static PropertyChangedEventArgs bgWorkerChangeArgs =
            ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.BgWorker);

        public BackgroundTaskManager<List<FileListViewItemViewModel>> BgWorker
        {
            get { return bgWorker_LoadSubEntries; }
            set
            {
                bgWorker_LoadSubEntries = value;
                NotifyPropertyChanged(bgWorkerChangeArgs);
            }
        }

        static PropertyChangedEventArgs fileCountChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.FileCount);

        public uint FileCount
        {
            get { return _fileCount; }
            private set
            {
                _fileCount = value;
                NotifyPropertyChanged(fileCountChangeArgs);
            }
        }

        static PropertyChangedEventArgs directoryCountChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.DirectoryCount);

        public uint DirectoryCount
        {
            get { return _dirCount; }
            private set
            {
                _dirCount = value;
                NotifyPropertyChanged(directoryCountChangeArgs);
            }
        }

        static PropertyChangedEventArgs selectedCountChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.SelectedCount);

        public uint SelectedCount
        {
            get { return _selCount; }
            private set
            {
                _selCount = value;                
                NotifyPropertyChanged(selectedCountChangeArgs);
                NotifyPropertyChanged(selectedViewModelsChangeArgs);
                NotifyPropertyChanged(selectedEntriesChangeArgs);
            }
        }

        static PropertyChangedEventArgs selectedViewModelsChangeArgs =
          ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.SelectedViewModels);

        public FileListViewItemViewModel[] SelectedViewModels
        {
            get { return (from FileListViewItemViewModel vm in this.SubEntries.View where vm.IsSelected select vm).ToArray(); }
        }

        static PropertyChangedEventArgs selectedEntriesChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.SelectedEntries);

        public FileSystemInfoEx[] SelectedEntries
        {
            get { return (from vm in SelectedViewModels select vm.EmbeddedModel.EmbeddedEntry).ToArray(); }
        }


        static PropertyChangedEventArgs hasSubEntriessChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.HasSubEntries);

        public bool HasSubEntries
        {
            get { return _hasSubEntries; }
            set
            {
                _hasSubEntries = value;
                NotifyPropertyChanged(hasSubEntriessChangeArgs);
            }
        }


        static PropertyChangedEventArgs subEntriesInternalChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.SubEntriesInternal);

        public ObservableCollection<FileListViewItemViewModel> SubEntriesInternal
        {
            get { return _subEntriesInternal; }
            set
            {
                _subEntriesInternal = value;
                NotifyPropertyChanged(subEntriesChangeArgs);
            }
        }

        static PropertyChangedEventArgs subEntriesChangeArgs =
           ObservableHelper.CreateArgs<CurrentDirectoryViewModel>(x => x.SubEntries);

        public CollectionViewSource SubEntries
        {
            get { return _subEntries; }
            set
            {
                _subEntries = value;
                NotifyPropertyChanged(subEntriesChangeArgs);
            }
        }


        #endregion


        
    }
}
