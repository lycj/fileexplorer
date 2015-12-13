using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Cinch;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using QuickZip.UserControls.MVVM.Model;
using System.ComponentModel;
using System.IO;
using QuickZip.UserControls.Logic.Tools.DragnDrop;

namespace QuickZip.UserControls.MVVM
{
    public class DirectoryChangedEventArgs : EventArgs
    {
        public string DirectoryString { get; private set; }
        public bool ChangeAllowed { get; set; }

        public DirectoryChangedEventArgs(string directoryString)
        {
            DirectoryString = directoryString;
            ChangeAllowed = true;
        }
    }



    public class DirectoryChangedEventArgs<FI, DI, FSI> : DirectoryChangedEventArgs
        where FI : FSI
        where DI : FSI
    {
        public EntryModel<FI, DI, FSI> Directory { get; set; }
        public DirectoryChangedEventArgs(EntryModel<FI, DI, FSI> entry)
            : base(entry.ParseName)
        {
            Directory = entry;
        }

        public DirectoryChangedEventArgs(string directoryString)
            : base(directoryString)
        {
        }
    }

    public class NavigationRootViewModel<FI, DI, FSI> : ExplorerRootModelBase, 
        ISupportDrag<EntryViewModel<FI, DI, FSI>>,
        ISupportDrop<EntryViewModel<FI, DI, FSI>>
        where FI : FSI
        where DI : FSI
    {        

        #region Constructor

        public NavigationRootViewModel(Profile<FI, DI, FSI> profile)
        {
            _profile = profile;
            _subDirectories = new ObservableCollection<NavigationItemViewModel<FI, DI, FSI>>();
            foreach (DI rootDirectory in profile.RootDirectories)
            {                
                _subDirectories.Add(
                    new NavigationItemViewModel<FI, DI, FSI>
                        (this, null, profile.ConstructDirectoryModel(rootDirectory)) 
                        { IsExpanded = rootDirectory.Equals(profile.DefaultRootDirectory) });
            }

            setupCommands();
            //_hierarchy = new ObservableCollection<NavigationItemViewModel<FI, DI, FSI>>();
            //setupBackgroundWorker();
        }

        #endregion

        #region Methods


        #region Bounty related

        internal void RequestBountyReward(NavigationItemViewModel<FI, DI, FSI> foundModel)
        {
            Bounty = default(DirectoryModel<FI, DI, FSI>);
            foundModel.IsSelected = true;
        }


        internal void ReportBeingSelected(NavigationItemViewModel<FI, DI, FSI> selectedModel)
        {
            if (!selectedModel.Equals(SelectedNavigationViewModel))
            {
                if (SelectedNavigationViewModel != null)
                    SelectedNavigationViewModel.IsSelected = false;
                SelectedNavigationViewModel = selectedModel;
                DirectoryChanged(this, new DirectoryChangedEventArgs<FI, DI, FSI>(selectedModel.EmbeddedEntryModel));
            }
            //ProcessHierarchy(selectedModel);
        }

        internal void ReportDraggedOver(NavigationItemViewModel<FI, DI, FSI> highlightedModel)
        {
            DraggingOverNavigationViewModel = highlightedModel;
        }

        internal void ReportDraggedLeave(NavigationItemViewModel<FI, DI, FSI> highlightedModel)
        {
            //DraggingOverNavigationViewModel = null;
        }


        public virtual void PlaceBounty(EntryModel<FI, DI, FSI> bountyModel)
        {
            if (bountyModel is DirectoryModel<FI, DI, FSI>)
            {

                if (SelectedNavigationViewModel != null &&
                    SelectedNavigationViewModel.EmbeddedDirectoryModel.EmbeddedDirectory.Equals(bountyModel))
                {
                    //False alarm, already Selected
                    SelectedNavigationViewModel.IsSelected = true;
                    return;
                }
                else
                    if (SelectedNavigationViewModel != null &&
                    SelectedNavigationViewModel.EmbeddedDirectoryModel.HasChild(bountyModel.EmbeddedEntry))
                    {
                        //Fast mode, item is subentry of current selected
                        Bounty = (DirectoryModel<FI, DI, FSI>)bountyModel;
                        SelectedNavigationViewModel.IsSelected = false;
                        SelectedNavigationViewModel.PlaceBounty();

                    }
                    else
                    {
                        //Slow mode, iterate from root
                        if (SelectedNavigationViewModel != null)
                            SelectedNavigationViewModel.IsSelected = false;

                        Bounty = (DirectoryModel<FI, DI, FSI>)bountyModel;

                        foreach (NavigationItemViewModel<FI, DI, FSI> subDir in
                            _subDirectories.OrderBy((nivm) => { return -nivm.EmbeddedEntryModel.CustomPosition; }))
                            if (subDir.EmbeddedDirectoryModel.EqualsOrHasChild(bountyModel))
                            {
                                subDir.PlaceBounty();
                                break;
                            }
                            else subDir.CollapseAll();
                    }
            }
        }
        #endregion

        #region Broadcast FileSystem Changes

        public void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            foreach (NavigationItemViewModel<FI, DI, FSI> subDir in SubDirectories)
                subDir.BroadcastChange(parseName, changeType);
        }

        #endregion

        #region Commands

        private void setupCommands()
        {
            ContextMenuCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                    {
                        return SelectedNavigationViewModel != null &&
                            SelectedNavigationViewModel.ContextMenuCommand.CanExecute(x);
                    },
                ExecuteDelegate = (x) =>
                    {
                        SelectedNavigationViewModel.ContextMenuCommand.Execute(x);
                    }
            };
        }

        #endregion

        #region Drag n Drop

        public DragDropEffects SupportedDragDropEffects
        {
            get { return DragDropEffects.Copy | DragDropEffects.Link; }
        }

        public EntryViewModel<FI, DI, FSI>[] SelectedItems
        {
            get { return new EntryViewModel<FI, DI, FSI>[] { SelectedNavigationViewModel.EmbeddedDirectoryViewModel }; }
        }

        public DragDropItemInfo<EntryViewModel<FI, DI, FSI>> GetItemInfo(EntryViewModel<FI, DI, FSI> item)
        {
            bool isDir;
            string path = _profile.GetDiskPath(item.EmbeddedModel.EmbeddedEntry, out isDir, false);
            return new DragDropItemInfo<EntryViewModel<FI, DI, FSI>>()
            {
                EmbeddedItem = item,
                FileSystemPath = _profile.GetDiskPath(item.EmbeddedModel.EmbeddedEntry, false),
                IsTemp = item.EmbeddedModel.IsVirtual,
                IsFolder = isDir
            };
        }

        public bool BeforeDrag(DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            return true;
        }

        public void PrepareDrop(DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            this.SelectedNavigationViewModel.EmbeddedDirectoryModel.PrepareDrop(_profile, dropInfo);
        }

        public IDropTarget<EntryViewModel<FI, DI, FSI>> CurrentDropTarget
        {
            get
            {                
                NavigationItemViewModel<FI, DI, FSI> dragOverVM = DraggingOverNavigationViewModel;
                if (dragOverVM is IDropTarget<EntryViewModel<FI, DI, FSI>>)
                    //if (dragOverVM.IsDraggingOver)
                        return dragOverVM;

                return null;
            }
        }

        #endregion

        #endregion

        #region Data
        private Profile<FI, DI, FSI> _profile;
        private ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> _subDirectories;

        private NavigationItemViewModel<FI, DI, FSI> _selectedNavigationViewModel = null;
        private NavigationItemViewModel<FI, DI, FSI> _draggingOverNavigationViewModel = null;


        #endregion

        #region Public Properties
        internal Profile<FI, DI, FSI> Profile { get { return _profile; } }

        public EventHandler<DirectoryChangedEventArgs<FI, DI, FSI>> DirectoryChanged = (o, args) => { };
        internal DirectoryModel<FI, DI, FSI> Bounty { get; private set; }

        public NavigationItemViewModel<FI, DI, FSI> SelectedNavigationViewModel
        {
            get { return _selectedNavigationViewModel; }
            protected set
            {
                _selectedNavigationViewModel = value;
                NotifyPropertyChanged("SelectedNavigationViewModel");
                NotifyPropertyChanged("SelectedModel");
            }
        }

        public NavigationItemViewModel<FI, DI, FSI> UISelectedNavigationViewModel
        {
            get { return _selectedNavigationViewModel; }
            set
            {
                if (value != null)
                    SelectedModel = value.EmbeddedEntryModel;
            }
        }


        public EntryModel<FI, DI, FSI> SelectedModel
        {
            get { return SelectedNavigationViewModel == null ? null : SelectedNavigationViewModel.EmbeddedEntryModel; }
            set { PlaceBounty(value); }
        }

        public NavigationItemViewModel<FI, DI, FSI> DraggingOverNavigationViewModel
        {
            get { return _draggingOverNavigationViewModel; }
            protected set
            {
                _draggingOverNavigationViewModel = value;                
                NotifyPropertyChanged("DraggingOverNavigationViewModel");
            }
        }


        //public ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> Hierarchy
        //{ get { return _hierarchy; } }

        public ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> SubDirectories
        { get { return _subDirectories; } }

        #endregion



       
    }
}
