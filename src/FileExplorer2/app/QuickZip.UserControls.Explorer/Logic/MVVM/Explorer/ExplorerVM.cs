using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM;
using System.Windows.Data;
using System.ComponentModel;
//using QuickZip.IO.COFE.UserControls.ViewModel;
using QuickZip.UserControls.Input;
using Cinch;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;
using QuickZip.UserControls.MVVM.Notification.ViewModel;
using QuickZip.UserControls.Logic.Tools.DragnDrop;

namespace QuickZip.UserControls.MVVM
{
    public enum ViewerMode
    {
        None, Directory, Media, W3
    }

    public partial class ExplorerViewModel<FI, DI, FSI> : RootModelBase
        where DI : FSI
        where FI : FSI
    {


        public EventHandler<DirectoryChangedEventArgs> DirectoryChanged = (o, e) => { };

        #region Constructor

        private void propertyChangeHandler(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "ViewMode" :
                    _viewMode = (CurrentBrowserViewModel as DirectoryViewerViewModel<FI, DI, FSI>).ViewMode;
                    break;
                case "ViewSize":                    
                    _viewSize = (CurrentBrowserViewModel as DirectoryViewerViewModel<FI, DI, FSI>).ViewSize;
                    break;
            }
        }

        private void dirChangedHandler(object sender, DirectoryChangedEventArgs args)
        {
            if (args is DirectoryChangedEventArgs<FI, DI, FSI>)
            {
                DirectoryChangedEventArgs<FI, DI, FSI> genericArgs =
                    args as DirectoryChangedEventArgs<FI, DI, FSI>;

                ChangeCurrentBrowserViewModel(genericArgs.Directory);
                args.ChangeAllowed = true;
            }
            else
            {
                if (args.DirectoryString.StartsWith("http://"))
                {
                    ChangeCurrentBrowserViewModel(new Uri(args.DirectoryString));
                    args.ChangeAllowed = true;

                }
                else
                    args.ChangeAllowed = false;
            }

            if (args.ChangeAllowed)
                DirectoryChanged(sender, args);
        }

        public ExplorerViewModel(Profile<FI, DI, FSI> profile)
        {
            _profile = profile;

            var notificationSources = from m in _profile.GetNotificationSources() select m.ToViewModel();
            _notificationViewModel = new NotificationBarViewModel(notificationSources.ToArray());

            _navigationViewModel = new NavigationRootViewModel<FI, DI, FSI>(_profile);
            _currentBrowserViewModel =
                    new DirectoryViewerViewModel<FI, DI, FSI>(_profile, _viewSize);
            _breadcrumbViewModel = new BreadcrumbViewModel<FI, DI, FSI>(_profile, this);
            _searchViewModel = new SearchViewModel<FI, DI, FSI>(_profile);

            _navigationViewModel.DirectoryChanged += dirChangedHandler;

            if (CurrentEntryBrowserViewModel != null)
                CurrentEntryBrowserViewModel.DirectoryChanged += dirChangedHandler;
            _breadcrumbViewModel.DirectoryChanged += dirChangedHandler;
            _searchViewModel.DirectoryChanged += dirChangedHandler;

            ChangeCurrentBrowserViewModel(_profile.ConstructDirectoryModel(profile.RootDirectories[0]));

            
            setupCommands();            
        }

        #endregion

        #region Methods

       

        public virtual ICommand GetViewerCommand(string commandName)
        {            
            
            switch (commandName)
            {
                case "NewFolderCommand": return CurrentBrowserViewModel.NewFolderCommand;
                case "CopyCommand": return CurrentBrowserViewModel.CopyCommand;
                case "PasteCommand": return CurrentBrowserViewModel.PasteCommand;
                case "SelectAllCommand": return CurrentBrowserViewModel.SelectAllCommand;
                case "DeleteCommand": return CurrentBrowserViewModel.DeleteCommand;
                case "PropertiesCommand": return CurrentBrowserViewModel.PropertiesCommand;
                case "RefreshCommand": return CurrentBrowserViewModel.RefreshCommand;
                case "ToggleBookmarkCommand": return CurrentBrowserViewModel.ToggleBookmarkCommand;
            }
            return null;
        }

        public void ChangeSortMethod(SortCriteria sortBy, SortDirectionType sortDirection)
        {
            if (CurrentBrowserViewModel is BaseDirectoryViewerViewModel<FI, DI, FSI>)
            {
                var dirVM = (CurrentBrowserViewModel as BaseDirectoryViewerViewModel<FI, DI, FSI>).EmbeddedDirectoryViewModel;
                dirVM.SortBy = sortBy;
                dirVM.SortDirection = sortDirection == SortDirectionType.sortAssending ?
                    ListSortDirection.Ascending : ListSortDirection.Descending;
            }
        }

        /// <summary>
        /// Called when CurrentBrowserViewModel updated, used to update other UI VMs.
        /// </summary>
        /// <param name="newViewModel"></param>
        protected virtual void UpdateOtherViewModels(ViewerBaseVM newViewModel)
        {
            _breadcrumbViewModel.IsBreadcrumbEnabled = newViewModel.IsBreadcrumbVisible;

            if (newViewModel is BaseW3ViewerViewModel)
            {
                BaseW3ViewerViewModel w3VM = newViewModel as BaseW3ViewerViewModel;
                _searchViewModel.ConfirmedParseName = w3VM.WebAddress.AbsoluteUri;               
                return;
            }
            
            if (newViewModel is BaseDirectoryViewerViewModel<FI, DI, FSI>)
            {
                BaseDirectoryViewerViewModel<FI, DI, FSI> directoryViewerVM = newViewModel as BaseDirectoryViewerViewModel<FI, DI, FSI>;
                DirectoryModel<FI, DI, FSI> directoryModel = directoryViewerVM.EmbeddedDirectoryViewModel.EmbeddedDirectoryModel;

                _navigationViewModel.SelectedModel = directoryModel;
                _breadcrumbViewModel.SelectedModel = directoryModel;
                _searchViewModel.SelectedModel = directoryModel;
                

                return;
            }

            if (newViewModel is MediaViewerViewModel<FI, DI, FSI>)
            {
                MediaViewerViewModel<FI, DI, FSI> fileViewerVM = newViewModel as MediaViewerViewModel<FI, DI, FSI>;
                EntryModel<FI, DI, FSI> fileModel = fileViewerVM.EmbeddedEntryViewModel.EmbeddedModel;
                DirectoryModel<FI, DI, FSI> parentModel = _profile.ConstructDirectoryModel(fileModel.Parent);

                _navigationViewModel.SelectedModel = parentModel;
                _breadcrumbViewModel.SelectedModel = fileModel;
                _searchViewModel.SelectedModel = fileModel;

                return;
            }

            

#if DEBUG
            throw new NotImplementedException();
#endif
        }


        #region Change Current BrowserViewModel, and other related VMs
        /// <summary>
        /// Change web adress
        /// </summary>
        /// <param name="uri"></param>
        protected virtual void ChangeCurrentBrowserViewModel(Uri uri)
        {
            //if (!(CurrentEntryViewModel is BaseW3ViewerViewModel) ||
            //    !uri.Equals((CurrentEntryViewModel as BaseW3ViewerViewModel).WebAddress))
            //{
                ChangeCurrentBrowserViewModel(new StatusW3ViewerViewModel(uri));              
            //}
        }        

        /// <summary>
        /// Change directory (implemented) or file (not implemented)
        /// </summary>
        /// <param name="newEntryModel"></param>
        protected virtual void ChangeCurrentBrowserViewModel(EntryModel<FI, DI, FSI> newEntryModel)
        {
            if (CurrentEntryViewModel == null ||
                !newEntryModel.Equals(CurrentEntryViewModel.EmbeddedModel))
            {
                _breadcrumbViewModel.IsBreadcrumbEnabled = true;

                if (newEntryModel is DirectoryModel<FI, DI, FSI>)
                {
                    DirectoryModel<FI, DI, FSI> directoryModel = 
                        (DirectoryModel<FI, DI, FSI>)newEntryModel;

                    ChangeCurrentBrowserViewModel(
                        new DirectoryViewerViewModel<FI, DI, FSI>(_profile, directoryModel, _viewSize) 
                            { ViewMode = _viewMode, ViewSize = _viewSize }
                        );
                }
                else
                {
                    FileModel<FI, DI, FSI> fileModel =
                        (FileModel<FI, DI, FSI>)newEntryModel;
                    DirectoryModel<FI, DI, FSI> parentModel =
                         _profile.ConstructDirectoryModel(fileModel.Parent);

                    //_breadcrumbViewModel.SelectedModel = fileModel;
                    //_searchViewModel.SelectedModel = parentModel;   

                    ChangeCurrentBrowserViewModel(new MediaViewerViewModel<FI, DI, FSI>(_profile, fileModel));

                    //TO-DO: Lookup parentModel from navigator, use that one instead.

                    //if (newEntryModel is FileModel<FI, DI, FSI>)
                    //    CurrentBrowserViewModel =
                    //    new FileViewerViewModel<FI, DI, FSI>(_profile, fileModel);

                    //_navigationViewModel.SelectedModel = parentModel;
                    //_breadcrumbViewModel.SelectedModel = fileModel;
                    //_searchViewModel.SelectedModel = parentModel;                       
                }
            }
        }

        protected virtual void ChangeCurrentBrowserViewModel(ViewerBaseVM newCurrentBrowserViewModel)
        {
            //if (_CurrentBrowserViewModel != null)
            //    _CurrentBrowserViewModel.Dispose();
            //Unload previous handler
            if (CurrentBrowserViewModel != null)
            {
                CurrentBrowserViewModel.PropertyChanged -= propertyChangeHandler;
                CurrentBrowserViewModel.DirectoryChanged -= dirChangedHandler;
               
                CurrentBrowserViewModel.OnUnload();
            }

            bool showPreview = CurrentBrowserViewModel != null ? CurrentBrowserViewModel.IsPreviewerVisible : false;
            newCurrentBrowserViewModel.IsPreviewerVisible = showPreview;

            _currentBrowserViewModel = newCurrentBrowserViewModel;
            if (CurrentBrowserViewModel != null)
            {                
                CurrentBrowserViewModel.PropertyChanged += propertyChangeHandler;
                CurrentBrowserViewModel.DirectoryChanged += dirChangedHandler;                
            }

            IsDirectoryTreeVisible = CurrentBrowserViewModel.IsDirectoryTreeEnabled;

            UpdateOtherViewModels(newCurrentBrowserViewModel);

            NotifyPropertyChanged("CurrentBrowserViewModel");
            
            NotifyPropertyChanged("CurrentEntryViewModel");
            NotifyPropertyChanged("CurrentEntryModel");
            NotifyPropertyChanged("CurrentEntry");
        }
        #endregion

        public void ChangeCurrentEntry(FSI currentEntry)
        {
            ChangeCurrentBrowserViewModel(_profile.ConstructEntryModel(currentEntry));
        }

        private void setupCommands()
        {
            _refreshCommand = new SimpleCommand()
            {
                ExecuteDelegate =
                (x) =>
                {
                    if (CurrentBrowserViewModel != null)
                        CurrentBrowserViewModel.Refresh();
                }
            };

            //SimpleRoutedCommand.RegisterClass(typeof(Explorer2), _refreshCommand);
        }


        #region Broadcast FileSystem Changes

        protected void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            try
            {
                if (CurrentBrowserViewModel != null)
                    CurrentBrowserViewModel.BroadcastChange(parseName, changeType);

                if (NavigationViewModel != null)
                    NavigationViewModel.BroadcastChange(parseName, changeType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
           
        }

        #endregion

        #endregion


        #region Data

        private Profile<FI, DI, FSI> _profile;
        private ViewerBaseVM _currentBrowserViewModel;
        private NavigationRootViewModel<FI, DI, FSI> _navigationViewModel;
        private NotificationBarViewModel _notificationViewModel;
        private BreadcrumbViewModel<FI, DI, FSI> _breadcrumbViewModel;
        private SearchViewModel<FI, DI, FSI> _searchViewModel;
        private SimpleCommand _refreshCommand = null, _copyCommand;
        private bool _isDirectoryTreeVisible = true;
        private ViewMode _viewMode = ViewMode.vmGrid;
        private int _viewSize = (int)ViewMode.vmGrid;
        #endregion

        #region Public Properties

        public SimpleCommand RefreshCommand { get { return _refreshCommand; } }

        public bool IsDirectoryTreeVisible
        {
            get { return _isDirectoryTreeVisible; }
            set { _isDirectoryTreeVisible = value; NotifyPropertyChanged("IsDirectoryTreeVisible"); }
        }

        public ViewerBaseViewModel<FI, DI, FSI> CurrentEntryBrowserViewModel
        {
            get { return _currentBrowserViewModel as ViewerBaseViewModel<FI, DI, FSI>; }
        }


        //#region UI, Current Browser (Directory, W3 or File)
        //public BrowserMode CurrentBrowserMode
        //{
        //    get
        //    {
        //        if (CurrentBrowserViewModel == null)
        //            return BrowserMode.None;
        //        if (CurrentBrowserViewModel is DirectoryViewerViewModel<FI, DI, FSI>)
        //            return BrowserMode.Directory;
        //        if (CurrentBrowserViewModel is FileViewerViewModel<FI, DI, FSI>)
        //            return BrowserMode.File;
        //        if (CurrentBrowserViewModel is W3ViewerViewModel)
        //            return BrowserMode.W3;

        //        throw new NotImplementedException();
        //    }
        //}

        ///// <summary>
        ///// Used by View to decide to show which control
        ///// </summary>
        //public bool IsDirectoryViewModel
        //{
        //    get { return CurrentBrowserMode == BrowserMode.Directory; }
        //}

        ///// <summary>
        ///// Used by View to decide to show which control
        ///// </summary>
        //public bool IsWWWViewModel
        //{
        //    get { return CurrentBrowserMode == BrowserMode.W3; }
        //}

        ///// <summary>
        ///// Used by View to decide to show which control
        ///// </summary>
        //public bool IsMediaViewModel
        //{
        //    get { return CurrentBrowserMode == BrowserMode.File; }
        //}
        //#endregion

        public ViewerBaseVM CurrentBrowserViewModel
        {
            get { return _currentBrowserViewModel; }           
        }
        

        #region Other UI (Search, Breadcrumb, Navigation(DirTree), NotificationBar)
        public Profile<FI, DI, FSI> Profile { get { return _profile; } }
        public SearchViewModel<FI, DI, FSI> SearchViewModel { get { return _searchViewModel; } }
        public BreadcrumbViewModel<FI, DI, FSI> BreadcrumbViewModel { get { return _breadcrumbViewModel; } }
        public NavigationRootViewModel<FI, DI, FSI> NavigationViewModel { get { return _navigationViewModel; } }
        public NotificationBarViewModel NotificationViewModel { get { return _notificationViewModel; } }

        #endregion

        #region Obtain data related to current "directory" (CurrentEntryViewModel, CurrentEntryModel, CurrentEntry)
        public EntryViewModel<FI, DI, FSI> CurrentEntryViewModel
        {
            get
            {
                return CurrentEntryBrowserViewModel == null ? null :
                CurrentEntryBrowserViewModel.EmbeddedEntryViewModel;
            }
        }

        public EntryModel<FI, DI, FSI> CurrentEntryModel
        {
            get { return CurrentEntryViewModel == null ? null : CurrentEntryViewModel.EmbeddedModel; }
        }

        public FSI CurrentEntry
        {
            get { return CurrentEntryModel == null ? default(FSI) : CurrentEntryModel.EmbeddedEntry; }
            set { ChangeCurrentEntry(value); }
        }
        #endregion

        #endregion

        //Statusbar items
    }
}
