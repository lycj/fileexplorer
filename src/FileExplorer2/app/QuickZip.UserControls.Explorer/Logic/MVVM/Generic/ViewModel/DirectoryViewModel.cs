using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.MVVM;
using System.ComponentModel;
using Cinch;
using System.Windows.Data;
using QuickZip.IO.COFE;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace QuickZip.UserControls.MVVM.ViewModel
{
    public class DirectoryViewModel<FI, DI, FSI> : EntryViewModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {
        public enum ListMode { Empty, Listing, Completed, Error }

        #region Constructor

        public DirectoryViewModel(Profile<FI, DI, FSI> profile, DirectoryModel<FI, DI, FSI> embeddedDirectoryModel)
            : base(profile, embeddedDirectoryModel)
        {
            setupSubEntries();
            HasSubDirectories = embeddedDirectoryModel.HasSubDirectories;
        }

        #endregion

        #region Methods

        internal override bool BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            bool retVal = base.BroadcastChange(parseName, changeType);

            if (PathEx.GetDirectoryName(parseName).Equals(EmbeddedDirectoryModel.ParseName))
                if (_subEntries != null)
                {
                    retVal = true;
                    if (_subEntries.Count != 0)
                    _subEntries.Load();
                }

            foreach (var vm in SubEntries)
                vm.BroadcastChange(parseName, changeType);

            return retVal;           
        }

        public override void Refresh(bool full = true)
        {
            base.Refresh(full);

            if (full && _subEntries != null && !_subEntries.IsWorking)
                _subEntries.Load(true);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _subEntries.Clear();
            _subEntries = null;
            _subFiles.Clear();
            _subEntries = null;
            _subDirectories.Clear();
            _subDirectories = null;

            //_subEntries.Source = null;
            //_subDirectories.Source = null;
            //_subFiles.Source = null;

            NotifyPropertyChanged("SubEntries");
            NotifyPropertyChanged("SubDirectories");
            NotifyPropertyChanged("SubFiles");
        }

        internal CollectionViewSource createColViewSource(bool showFile, bool showDir)
        {
            CollectionViewSource retVal = new CollectionViewSource();

            retVal.Source = _subEntries;

            if (!showDir || !showFile)
            {
                if (showDir)
                    retVal.Filter += (FilterEventHandler)((o, e) =>
                    {
                        e.Accepted = e.Item is DirectoryViewModel<FI, DI, FSI>;
                    });
                if (showFile)
                    retVal.Filter += (FilterEventHandler)((o, e) =>
                    {
                        e.Accepted = e.Item is FileViewModel<FI, DI, FSI>;
                    });
            }


            return retVal;
        }

        //private void changeSortMethod(SortCriteria sortBy, ListSortDirection sortDirection)
        //{
        //    ListCollectionView dataView = (ListCollectionView)(CollectionViewSource.GetDefaultView(_subEntries));

        //    dataView.SortDescriptions.Clear();
        //    dataView.CustomSort = null;

        //    SortDirectionType direction = sortDirection == ListSortDirection.Ascending ?
        //       SortDirectionType.sortAssending : SortDirectionType.sortDescending;

        //    dataView.CustomSort = new EntryComparer<FI, DI, FSI>(sortBy, direction) 
        //    { IsFolderFirst = true };

        //}

        private void setupSubEntries()
        {
            //Func<EntryModel<FI, DI, FSI>, EntryViewModel<FI, DI, FSI>> toViewModel =
            //    (m) =>
            //    {
            //        EntryViewModel<FI, DI, FSI> retVal = m.ToViewModel();
            //        retVal.ParentViewModel = this;
            //        return retVal;
            //    };
            _subEntries = new AsyncObservableCollection<EntryViewModel<FI, DI, FSI>>
                (
                   from entryModel in EmbeddedDirectoryModel.GetSubEntries(true)
                   select entryModel.ToViewModel(Profile),
                   (list, ex) =>
                   {
                       ListStatus = (ex != null) ? ListMode.Error : ListMode.Completed;
                       _subDirectories.Clear();
                       _subFiles.Clear();
                       foreach (EntryViewModel<FI, DI, FSI> entry in _subEntries)
                           if (entry is FileViewModel<FI, DI, FSI>)
                               _subFiles.Add(entry);
                       
                       foreach (EntryViewModel<FI, DI, FSI> entry in _subEntries)
                           if (entry is DirectoryViewModel<FI, DI, FSI>)
                               _subDirectories.Add(entry);

                       lock (taskCompletedNotifierList)
                       {
                           foreach (Action taskAfterCompleted in taskCompletedNotifierList)
                               taskAfterCompleted();
                           taskCompletedNotifierList.Clear();
                       }

                       HasSubDirectories = _subDirectories.Count > 0;
                   });
            _subDirectories = new ObservableCollection<EntryViewModel<FI, DI, FSI>>();
            _subFiles = new ObservableCollection<EntryViewModel<FI, DI, FSI>>();

            _subEntries.ChangeSortMethod(_sortBy, _sortDirection);
        }


        public void List(bool forceReload = false, Action taskAfterCompleted = null)
        {
            if (taskAfterCompleted != null)
            {
                lock (taskCompletedNotifierList)
                    taskCompletedNotifierList.Add(taskAfterCompleted);
            }


            if (!_subEntries.IsWorking && ListStatus != ListMode.Listing)
            {
                if (ListStatus != ListMode.Empty || ListStatus != ListMode.Error || forceReload)
                {
                    ListStatus = ListMode.Listing;
                    _subEntries.Load(forceReload);
                }
            }
        }


        public override string ToString()
        {
            return "D" + base.ToString();
        }

        public override void OnUnload()
        {
            foreach (EntryViewModel<FI, DI, FSI> subEntry in _subEntries)
                subEntry.OnUnload();
            base.OnUnload();
        }

        #endregion

        #region Data

        private AsyncObservableCollection<EntryViewModel<FI, DI, FSI>> _subEntries;
        private SortCriteria _sortBy = SortCriteria.sortByFullName;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private ObservableCollection<EntryViewModel<FI, DI, FSI>> _subDirectories, _subFiles;
        //private CollectionViewSource _subFiles, _subEntries, _subDirectories;
        private ListMode _listStatus = ListMode.Empty;
        private bool _hasSubDirectories = true;
        private List<Action> taskCompletedNotifierList = new List<Action>();
        private object _selectedItems;

        //private int _viewSize = 80;

        #endregion

        #region Public Properties

        public DirectoryModel<FI, DI, FSI> EmbeddedDirectoryModel { get { return (DirectoryModel<FI, DI, FSI>)EmbeddedModel; } }
        public DI EmbeddedDirectory { get { return EmbeddedDirectoryModel.EmbeddedDirectory; } }
        public AsyncObservableCollection<EntryViewModel<FI, DI, FSI>> SubEntries { get { return _subEntries; } }
        public ObservableCollection<EntryViewModel<FI, DI, FSI>> SubFiles { get { return _subFiles; } }
        public ObservableCollection<EntryViewModel<FI, DI, FSI>> SubDirectories { get { return _subDirectories; } }

        #region ListStatus, IsLoading

        public bool IsLoading { get { return _listStatus == ListMode.Listing; } }

        static PropertyChangedEventArgs listStatusChangeArgs =
            ObservableHelper.CreateArgs<DirectoryViewModel<FI, DI, FSI>>(x => x.ListStatus);

        /// <summary>
        /// Status of subentries listing
        /// </summary>
        public ListMode ListStatus
        {
            get { return _listStatus; }
            set
            {
                _listStatus = value;
                NotifyPropertyChanged(listStatusChangeArgs);
                NotifyPropertyChanged("IsLoading");
            }
        }


        #endregion

        #region SortBy, SortDirection
        static PropertyChangedEventArgs sortByChangeArgs =
         ObservableHelper.CreateArgs<DirectoryViewModel<FI, DI, FSI>>(x => x.SortBy);

        public SortCriteria SortBy
        {
            get { return _sortBy; }
            set
            {
                _subEntries.ChangeSortMethod(value, _sortDirection);
                _sortBy = value;
                NotifyPropertyChanged(sortByChangeArgs);
            }
        }

        static PropertyChangedEventArgs sortDirectionChangeArgs =
          ObservableHelper.CreateArgs<DirectoryViewModel<FI, DI, FSI>>(x => x.SortDirection);

        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                _subEntries.ChangeSortMethod(_sortBy, value);
                _sortDirection = value;
                NotifyPropertyChanged(sortDirectionChangeArgs);
            }
        }
        #endregion

        #region HasSubDirectories
        static PropertyChangedEventArgs hasSubDirectoriesChangeArgs =
            ObservableHelper.CreateArgs<DirectoryViewModel<FI, DI, FSI>>(x => x.HasSubDirectories);
        /// <summary>
        /// If the current directory has sub directories. (default true)
        /// </summary>
        public bool HasSubDirectories
        {
            get { return _hasSubDirectories; }
            set { _hasSubDirectories = value; NotifyPropertyChanged(hasSubDirectoriesChangeArgs); }
        }
        #endregion

        #endregion
    }
}
