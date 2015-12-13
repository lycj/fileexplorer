using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using QuickZip.IO.PIDL.UserControls.Model;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class DirectoryTreeItemViewModel : HierarchyViewModel
    {
        private static DirectoryTreeItemViewModel dummyNode = new DirectoryTreeItemViewModel();

        #region Constructor

        private DirectoryTreeItemViewModel() : base() { } //For DummyNode        

        public DirectoryTreeItemViewModel(RootModelBase rootModel, HierarchyViewModel parentModel, Model.DirectoryModel model) :
            base(rootModel, parentModel, model)
        {
            HasSubDirectories = EmbeddedDirectoryModel.EmbeddedDirectoryEntry.HasSubFolder;
            if (HasSubDirectories)
                _subDirs.Add(dummyNode);
            setUpBackgroundWorker();

            _refreshCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => Refresh()
            };
        }

        #endregion

        #region Methods

        internal void BroascastCurrentDirectoryChanging(DirectoryInfoEx newDirectory)
        {
            if (IsExpanded)
                if (this.EmbeddedDirectoryModel.EmbeddedDirectoryEntry.Equals(newDirectory) ||
                    IOTools.HasParent(newDirectory, this.EmbeddedDirectoryModel.EmbeddedDirectoryEntry) ||
                    IOTools.HasParent(this.EmbeddedDirectoryModel.EmbeddedDirectoryEntry, newDirectory))
                {
                    if (IsLoaded)
                        foreach (DirectoryTreeItemViewModel subItem in SubDirectories)
                            subItem.BroascastCurrentDirectoryChanging(newDirectory);
                }
                else IsExpanded = false;
        }


        internal void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            if (IsLoaded)
                foreach (DirectoryTreeItemViewModel subItem in SubDirectories)
                    subItem.BroadcastChange(parseName, changeType);

            switch (changeType)
            {
                case WatcherChangeTypesEx.Created:
                case WatcherChangeTypesEx.Deleted:
                    if (EmbeddedDirectoryModel.FullName.Equals(PathEx.GetDirectoryName(parseName)))
                        Refresh();
                    break;
                default:
                    if (EmbeddedDirectoryModel.FullName.Equals(parseName))
                        Refresh();
                    break;
            }
        }

        public DirectoryTreeItemViewModel LookupChild(DirectoryInfoEx directory, Func<bool> cancelCheck)
        {
            if (cancelCheck != null && cancelCheck())
                return null;

            if (Parent != null)
                Parent.IsExpanded = true;
            if (directory == null)
                return null;

            if (!IsLoaded)
            {
                IsLoading = true;
                SubDirectories = getDirectories();
                HasSubDirectories = SubDirectories.Count > 0;
                IsLoading = false;
            }

            foreach (DirectoryTreeItemViewModel subDirModel in SubDirectories)
            {
                if (!subDirModel.Equals(dummyNode))
                {
                    DirectoryInfoEx subDir = subDirModel.EmbeddedDirectoryModel.EmbeddedDirectoryEntry;
                    if (directory.Equals(subDir))
                        return subDirModel;
                    else if (IOTools.HasParent(directory, subDir))
                        return subDirModel.LookupChild(directory, cancelCheck);
                }
            }
            return null;
        }

        public void Refresh()
        {
            try //Make sure entry still exists.
            {
                FileSystemInfoEx entry = EmbeddedDirectoryModel.EmbeddedEntry;
                if (entry == null)
                    if (Parent is DirectoryTreeItemViewModel) (Parent as DirectoryTreeItemViewModel).Refresh();
                if (entry.FullName == "C:\\Temp\\Archives")
                    return;
                bgWorker_loadSub.RunBackgroundTask();
            }
            catch (Exception) { if (Parent is DirectoryTreeItemViewModel) (Parent as DirectoryTreeItemViewModel).Refresh(); }
        }



        private DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel> getDirectories()
        {
            return new DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>(
                        from dir in EmbeddedDirectoryModel.EmbeddedDirectoryEntry.GetDirectories()
                        select new DirectoryTreeItemViewModel(RootModel, this, ExModel.FromExEntry(dir)));
        }

        private void setUpBackgroundWorker()
        {
            bgWorker_loadSub = new BackgroundTaskManager<DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>>(
                () =>
                {
                    IsLoading = true;
                    return getDirectories();
                },
                (result) =>
                {
                    List<DirectoryTreeItemViewModel> delList = new List<DirectoryTreeItemViewModel>(SubDirectories.ToArray());
                    List<DirectoryTreeItemViewModel> addList = new List<DirectoryTreeItemViewModel>();

                    foreach (DirectoryTreeItemViewModel model in result)
                        if (delList.Contains(model))
                            delList.Remove(model);
                        else addList.Add(model);

                    foreach (DirectoryTreeItemViewModel model in delList)
                        SubDirectories.Remove(model);

                    foreach (DirectoryTreeItemViewModel model in addList)
                        SubDirectories.Add(model);

                    HasSubDirectories = SubDirectories.Count > 0;


                    IsLoading = false;
                });
        }



        protected override void FetchData(bool refresh)
        {
            base.FetchData(refresh);

            if (bgWorker_loadSub == null)
                return;

            if (!IsLoaded)
            {
                refresh = true; //Not loaded before, force refresh.
                SubDirectories.Clear();
            }

            if (refresh)
            {
                bgWorker_loadSub.RunBackgroundTask();
            }
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();
            FetchData(false);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (HasSubDirectories && SubDirectories.Count == 1 && SubDirectories[0].Equals(dummyNode))
            {
                HasSubDirectories = EmbeddedDirectoryModel.EmbeddedDirectoryEntry.HasSubFolder;
                if (!HasSubDirectories)
                    SubDirectories.Clear();
            }

            //Fixed DirectoryTree Collapse unrelated directory (1)
            //SelectedDirectory now changed from here instead of DirectoryTree.xaml.cs
            (RootModel as DirectoryTreeViewModel).SelectedDirectoryModel = this;
        }

        #endregion

        #region Data
        private bool _hasSubDirectories = false;
        private bool _isLoading = false;
        private SimpleCommand _refreshCommand;

        private Cinch.DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel> _subDirs =
            new Cinch.DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>();

        private BackgroundTaskManager<DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>>
           bgWorker_loadSub = null;

        #endregion

        #region Public Properties

        public bool IsLoaded { get { return !(SubDirectories.Count == 1 && SubDirectories[0].Equals(dummyNode)); } }
        public SimpleCommand RefreshCommand { get { return _refreshCommand; } }

        public Model.DirectoryModel EmbeddedDirectoryModel
        {
            get { return EmbeddedModel as Model.DirectoryModel; }
        }

        static PropertyChangedEventArgs subDirsChangeArgs =
           ObservableHelper.CreateArgs<DirectoryTreeItemViewModel>(x => x.SubDirectories);

        public DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel> SubDirectories
        {
            get { return _subDirs; }
            set
            {
                _subDirs = value;
                NotifyPropertyChanged(subDirsChangeArgs);
            }
        }

        static PropertyChangedEventArgs hasSubDirsChangeArgs =
           ObservableHelper.CreateArgs<DirectoryTreeItemViewModel>(x => x.HasSubDirectories);

        public bool HasSubDirectories
        {
            get { return _hasSubDirectories; }
            set
            {
                _hasSubDirectories = value;
                NotifyPropertyChanged(hasSubDirsChangeArgs);
            }
        }

        static PropertyChangedEventArgs isLoadingChangeArgs =
           ObservableHelper.CreateArgs<DirectoryTreeItemViewModel>(x => x.IsLoading);

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(isLoadingChangeArgs);
            }
        }

        static PropertyChangedEventArgs bgWorkerChangeArgs =
            ObservableHelper.CreateArgs<DirectoryTreeItemViewModel>(x => x.BgWorker);

        public BackgroundTaskManager<DispatcherNotifiedObservableCollection<DirectoryTreeItemViewModel>> BgWorker
        {
            get { return bgWorker_loadSub; }
            set
            {
                bgWorker_loadSub = value;
                NotifyPropertyChanged(bgWorkerChangeArgs);
            }
        }


        #endregion

    }
}
