using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Cinch;
using QuickZip.IO.PIDL.UserControls.Model;
using System.IO;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class FileListViewModel : RootModelBase
    {

        #region Constructor

        public FileListViewModel()
        {
            _refreshCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => CurrentDirectoryModel.Refresh()
            };

    }

        #endregion


        #region Data

        private SimpleCommand _refreshCommand;
        private DirectoryInfoEx _currentDirectory;
        private CurrentDirectoryViewModel _currentDirectoryModel;
        private bool _isLoading = false;
        private ExComparer.SortCriteria _sortBy = ExComparer.SortCriteria.sortByName; //0.2
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        #endregion


        #region Methods
        public void OnCurrentDirectoryPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsLoading")
                IsLoading = CurrentDirectoryModel.IsLoading;
        }
        #endregion       

        #region Public Properties


        public SimpleCommand RefreshCommand
        {
            get
            {
                return _refreshCommand;
            }
        }
 

        static PropertyChangedEventArgs currentDirectoryChangeArgs =
           ObservableHelper.CreateArgs<FileListViewModel>(x => x.CurrentDirectory);

        public DirectoryInfoEx CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {                
                _currentDirectory = value;
                NotifyPropertyChanged(currentDirectoryChangeArgs);

                if (value != null)
                    if (_currentDirectoryModel == null || _currentDirectoryModel.EmbeddedModel.FullName != value.FullName)
                    {
                        if (_currentDirectoryModel != null)
                            _currentDirectoryModel.PropertyChanged -= new PropertyChangedEventHandler(OnCurrentDirectoryPropertyChanged);
                        _currentDirectoryModel = new CurrentDirectoryViewModel(this, ExModel.FromExEntry(value));
                        _currentDirectoryModel.ChangeSortMethod(this.SortBy, this.SortDirection);
                        _currentDirectoryModel.PropertyChanged += new PropertyChangedEventHandler(OnCurrentDirectoryPropertyChanged);

                        NotifyPropertyChanged(currentDirectoryModelChangeArgs);
                    }
            }
        }

        static PropertyChangedEventArgs currentDirectoryModelChangeArgs =
          ObservableHelper.CreateArgs<FileListViewModel>(x => x.CurrentDirectoryModel);

        public CurrentDirectoryViewModel CurrentDirectoryModel
        {
            get { return _currentDirectoryModel; }
            set
            {
                _currentDirectoryModel = value;
                _currentDirectoryModel.ChangeSortMethod(this.SortBy, this.SortDirection);
                NotifyPropertyChanged(currentDirectoryModelChangeArgs);

                if (value != null)
                    if (_currentDirectory == null || _currentDirectory.FullName != value.EmbeddedModel.FullName)
                    {
                        _currentDirectory = value.EmbeddedDirectoryModel.EmbeddedDirectoryEntry;
                        NotifyPropertyChanged(currentDirectoryChangeArgs);
                    }
            }
        }

        static PropertyChangedEventArgs isLoadingChangeArgs =
          ObservableHelper.CreateArgs<FileListViewModel>(x => x.IsLoading);

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(isLoadingChangeArgs);
            }
        }

        static PropertyChangedEventArgs sortByChangeArgs =
          ObservableHelper.CreateArgs<FileListViewModel>(x => x.SortBy);

        public ExComparer.SortCriteria SortBy
        {
            get { return _sortBy; }
            set
            {
                if (_currentDirectoryModel != null)
                    _currentDirectoryModel.ChangeSortMethod(value, _sortDirection);
                _sortBy = value;
                NotifyPropertyChanged(sortByChangeArgs);
            }
        }

        static PropertyChangedEventArgs sortDirectionChangeArgs =
          ObservableHelper.CreateArgs<FileListViewModel>(x => x.SortDirection);

        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                if (_currentDirectoryModel != null)
                    _currentDirectoryModel.ChangeSortMethod(_sortBy, value);
                _sortDirection = value;
                NotifyPropertyChanged(sortDirectionChangeArgs);
            }
        }

        #endregion

    }
}
