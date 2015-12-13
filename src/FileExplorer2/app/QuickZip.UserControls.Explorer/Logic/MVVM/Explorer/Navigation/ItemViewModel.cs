using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Cinch;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.MVVM;
using System.Diagnostics;
using QuickZip.IO.COFE;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using QuickZip.Translation;
using QuickZip.UserControls.Logic.Tools.DragnDrop;
using System.Timers;

namespace QuickZip.UserControls.MVVM
{


    public class NavigationItemViewModel<FI, DI, FSI> : ExplorerRootModelBase,
        IComparable<NavigationItemViewModel<FI, DI, FSI>>,
        IDropTarget<EntryViewModel<FI, DI, FSI>>
        where DI : FSI
        where FI : FSI
    {
        private static NavigationItemViewModel<FI, DI, FSI> dummyNode
            = new NavigationItemViewModel<FI, DI, FSI>();

        protected class ListParameters
        {
            public DirectoryModel<FI, DI, FSI> CurrentDirectoryModel { get; set; }
            public DI LookupDirectory { get; set; }
        }

        #region Constructor
        private NavigationItemViewModel()
        {
            //for dummyNode / separator only.
            _isSeparator = true;
            CustomPosition = -1;            
        }



        public NavigationItemViewModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
            NavigationItemViewModel<FI, DI, FSI> parentModel,
            DirectoryModel<FI, DI, FSI> embedDirectoryModel)
        {
            _isDirectory = true;
            Debug.Assert(embedDirectoryModel != null);
            
            _embeddedEntryModel = _embeddedDirectoryModel = embedDirectoryModel;
            _embeddedEntryViewModel = _embeddedDirectoryViewModel =
                (DirectoryViewModel<FI, DI, FSI>)embedDirectoryModel.ToViewModel(rootModel.Profile);
            _rootModel = rootModel;
            _parentModel = (parentModel != null && rootModel.Profile.IsInsideScope(parentModel.EmbeddedDirectoryModel)) ? parentModel : null;
            CustomPosition = embedDirectoryModel.CustomPosition;
            setupCommands();
        }

        public NavigationItemViewModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
            DirectoryModel<FI, DI, FSI> embedDirectoryModel)
            : this(rootModel,
                   embedDirectoryModel.Parent == null ? null :
                        new NavigationItemViewModel<FI, DI, FSI>(rootModel,
                            rootModel.Profile.ConstructDirectoryModel(embedDirectoryModel.Parent)),
                        embedDirectoryModel)
        {

        }


        public NavigationItemViewModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
            NavigationItemViewModel<FI, DI, FSI> parentModel,
            FileModel<FI, DI, FSI> embedFileModel)
        {
            _isDirectory = false;
            _embeddedEntryModel = embedFileModel;
            _embeddedEntryViewModel = embedFileModel.ToViewModel(rootModel.Profile);
            _rootModel = rootModel;
            _parentModel = (parentModel != null && rootModel.Profile.IsInsideScope(parentModel.EmbeddedDirectoryModel)) ? parentModel : null;
            CustomPosition = embedFileModel.CustomPosition;
            setupCommands();
        }

        public NavigationItemViewModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
          FileModel<FI, DI, FSI> embedFileModel)
            : this(rootModel,
                   embedFileModel.Parent == null ? null :
                        new NavigationItemViewModel<FI, DI, FSI>(rootModel,
                            rootModel.Profile.ConstructDirectoryModel(embedFileModel.Parent)),
                        embedFileModel)
        {

        }


        public NavigationItemViewModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
            NavigationItemViewModel<FI, DI, FSI> parentModel,
            EntryModel<FI, DI, FSI> embedEntryModel)
        {
            _isDirectory = embedEntryModel is DirectoryModel<FI, DI, FSI>;

            if (_isDirectory)
            {
                DirectoryModel<FI, DI, FSI> embedDirectoryModel = embedEntryModel as DirectoryModel<FI, DI, FSI>; ;
                _embeddedEntryModel = _embeddedDirectoryModel = embedDirectoryModel;
                _embeddedEntryViewModel = _embeddedDirectoryViewModel =
                    (DirectoryViewModel<FI, DI, FSI>)embedDirectoryModel.ToViewModel(rootModel.Profile);
            }
            else
            {
                _embeddedEntryModel = embedEntryModel;
                _embeddedEntryViewModel = embedEntryModel.ToViewModel(rootModel.Profile);
            }
            _rootModel = rootModel;
            _parentModel = (parentModel != null && rootModel.Profile.IsInsideScope(parentModel.EmbeddedDirectoryModel)) ? parentModel : null;
            CustomPosition = embedEntryModel.CustomPosition;
            setupCommands();
        }


        public static NavigationItemViewModel<FI, DI, FSI> FromModel(NavigationRootViewModel<FI, DI, FSI> rootModel,
         EntryModel<FI, DI, FSI> embedEntryModel)
        {
            if (embedEntryModel is FileModel<FI, DI, FSI>)
                return new NavigationItemViewModel<FI, DI, FSI>(rootModel, embedEntryModel as FileModel<FI, DI, FSI>);
            else return new NavigationItemViewModel<FI, DI, FSI>(rootModel, embedEntryModel as DirectoryModel<FI, DI, FSI>);
        }


        #endregion

        #region Methods

        #region Equals, Gethashcode and ToString
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            if (EmbeddedEntryModel == null)
                return false;

            if (obj is NavigationItemViewModel<FI, DI, FSI>)
                return EmbeddedEntryModel.Equals((obj as NavigationItemViewModel<FI, DI, FSI>).EmbeddedEntryModel);
            return false;
        }

        public int CompareTo(NavigationItemViewModel<FI, DI, FSI> other)
        {
            return EmbeddedEntryViewModel.CompareTo(other.EmbeddedEntryViewModel);
        }

        public override int GetHashCode()
        {
            return _embeddedEntryModel == null ? 0 : EmbeddedEntryModel.GetHashCode();
        }

        public override string ToString()
        {
            return "IVM:" + (_embeddedEntryModel == null ? "" : EmbeddedEntryModel.ToString());
        }
        #endregion

        #region Subdir loading / list, Bounty, CollapseAll
        /// <summary>
        /// Subdir can only initalized after an TreeViewItem called SubDirectories
        /// </summary>
        private void setupSubdir()
        {
            if (EmbeddedDirectoryModel == null)
                return;

            if (!_isDirectory || EmbeddedDirectoryModel.IsLink)
            {
                _subDirectories = new AsyncObservableCollection<NavigationItemViewModel<FI, DI, FSI>>();
            }
            else
            {
                bool bountyPlaced = false;
                DirectoryModel<FI, DI, FSI> bounty = null;
                _subDirectories = new AsyncObservableCollection<NavigationItemViewModel<FI, DI, FSI>>
                (
                   DispatcherPriority.Background,
                   from item in (_embeddedDirectoryModel.GetSubEntries(true, "*", true, false))
                   select new NavigationItemViewModel<FI, DI, FSI>(_rootModel, this,
                       item as DirectoryModel<FI, DI, FSI>) { IsSubItem = true },
                    //task Func                  
                    () =>
                    {
                        if (_subDirectories.Contains(dummyNode))
                            _subDirectories.Clear();
                        bountyPlaced = false;
                        bounty = _rootModel.Bounty;

                    }, //Begina action
                    (itemAdded) =>
                    {
                        //Debug.WriteLine("Added " + itemAdded.EmbeddedDirectoryModel.EmbeddedEntry.ToString());
                        if (!bountyPlaced && _rootModel.Bounty != null &&
                            itemAdded.EmbeddedEntryModel.CustomPosition == -1)
                        {
                            if (itemAdded.EmbeddedDirectoryModel.EqualsOrHasChild(_rootModel.Bounty))
                            {
                                //BountyIsUp();
                                bountyPlaced = true;
                                itemAdded.PlaceBounty();
                            }
                        }
                    },
                    (removedItem) =>
                    {
                        //Debug.WriteLine("Removed " + removedItem.EmbeddedDirectoryModel.EmbeddedEntry.ToString()); 
                    },
                    (itemList, ex) =>
                    {
                        if (ex != null)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
                            throw ex;
#endif
                        }
                        else
                        {
                            _isInited = true;


                            #region Breadcrumb related code
                            //Custom menu if it's first item of breadcrumb.
                            if (IsFirstItem && _rootModel is BreadcrumbViewModel<FI, DI, FSI>)
                            {
                                BreadcrumbViewModel<FI, DI, FSI> rootBreadcrumbVM = _rootModel as BreadcrumbViewModel<FI, DI, FSI>;
                                int insertCount = 0;

                                foreach (DI dir in Profile.RootDirectories)
                                {
                                    DirectoryModel<FI, DI, FSI> newDirModel =
                                        Profile.ConstructDirectoryModel(dir);
                                    if (!_rootModel.SelectedModel.Equals(dir))
                                    {
                                        NavigationItemViewModel<FI, DI, FSI> newItem
                                            = new NavigationItemViewModel<FI, DI, FSI>(_rootModel, null,
                                               newDirModel) { CustomPosition = ++insertCount };
                                        _subDirectories.Add(newItem);
                                    }
                                }

                                foreach (var rootSubDir in rootBreadcrumbVM.Hierarchy)
                                    if (!rootSubDir.IsItemVisible)
                                    {
                                        NavigationItemViewModel<FI, DI, FSI> cloneItem
                                            = new NavigationItemViewModel<FI, DI, FSI>(_rootModel, rootSubDir.ParentViewModel,
                                                    rootSubDir.EmbeddedEntryModel) { CustomPosition = ++insertCount };
                                        _subDirectories.Add(cloneItem);


                                        //_subDirectories.Insert(0, rootSubDir);
                                    }
                                //new NavigationItemViewModel<FI, DI, FSI>(_rootModel, rootSubDir.ParentViewModel,
                                //rootSubDir.EmbeddedDirectoryModel));



                                if (insertCount > 0)
                                    _subDirectories.Insert(insertCount++, new NavigationItemViewModel<FI, DI, FSI>() { CustomPosition = 0 });
                            }
                            #endregion

                            #region Directory releated code
                            if (bounty != null)
                                if (!bountyPlaced)
                                {

                                    //BountyIsUp();
                                    foreach (NavigationItemViewModel<FI, DI, FSI> subDirVM in
                                        itemList.OrderBy((nivm) => { return -nivm.EmbeddedEntryModel.CustomPosition; }))
                                    {
                                        if (subDirVM.EmbeddedDirectoryModel.EqualsOrHasChild(_rootModel.Bounty))
                                        {
                                            subDirVM.PlaceBounty();
                                            break;
                                        }
                                        else subDirVM.CollapseAll();
                                    }
                                }
                                else

                                    foreach (NavigationItemViewModel<FI, DI, FSI> subDirVM in itemList)
                                    {
                                        if (!subDirVM.EmbeddedDirectoryModel.EqualsOrHasChild(bounty))
                                            subDirVM.CollapseAll();
                                    }

                            #endregion
                            //for (int i = 1; i < itemList.Count; i++)
                            //{
                            //    NavigationItemViewModel<FI, DI, FSI> subDirVM = itemList[i];
                            //    if (subDirVM.EmbeddedDirectoryModel.EqualsOrHasChild(_rootModel.Bounty))
                            //    {
                            //        subDirVM.PlaceBounty();
                            //        break;
                            //    }
                            //}

                            _subDirectories.ChangeSortMethod(SortCriteria.sortByName, ListSortDirection.Ascending);

                        }

                    }
                   );

                if (IsDirectory && EmbeddedDirectoryModel.HasSubDirectories)
                    _subDirectories.Add(dummyNode);

                //if (!IsFirstItem)

            }


        }

        public void CollapseAll()
        {
            if (IsExpanded)
            {
                IsExpanded = false;

                foreach (NavigationItemViewModel<FI, DI, FSI> subDirVM in _subDirectories)
                    subDirVM.CollapseAll();
            }
        }

        public void PlaceBounty()
        {
            if (_rootModel == null || _rootModel.Bounty == null || !IsDirectory || _isSeparator)
                return;

            if (EmbeddedDirectoryModel.EqualsOrHasChild(_rootModel.Bounty))
            {
                if (EmbeddedDirectoryModel.Equals(_rootModel.Bounty))
                {
                    //Debug.WriteLine(_rootModel.Bounty.IsCached);
                    //Debug.WriteLine(EmbeddedDirectoryModel.IsCached);
                    //Exchange bounty if cached.
                    if (!EmbeddedDirectoryModel.IsCached && _rootModel.Bounty.IsCached)
                        _embeddedDirectoryModel = _rootModel.Bounty;
                    _rootModel.RequestBountyReward(this);
                }
                else
                {
                    IsSelected = false;
                    if (_isInited)
                    {
                        IsExpanded = true;
                        foreach (NavigationItemViewModel<FI, DI, FSI> subDirVM in
                            _subDirectories.OrderBy((nivm) => { return -nivm.EmbeddedEntryModel.CustomPosition; }))
                            if (subDirVM.EmbeddedDirectoryModel.EqualsOrHasChild(_rootModel.Bounty))
                            {
                                subDirVM.PlaceBounty();
                                break;
                            }
                            else subDirVM.CollapseAll();
                    }
                    else
                    {
                        //Let greed does all the work....
                        IsExpanded = true;
                    }
                }
            }
            else IsSelected = false;
        }

        private void list(bool forceReload = false)
        {
            if (!_isSeparator)
                if (forceReload || !_isInited || _rootModel.Bounty != null || IsFirstItem)
                {
                    if (IsFirstItem)
                    {
                        //Exception - "This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread."
                        //SubDirectories.Clear();
                        SubDirectories.Load(true);
                    }
                    SubDirectories.Load(forceReload);                    
                }
        }


        internal void BroadcastItemsChanged()
        {
            if (IsFirstItem)// && _rootModel is BreadcrumbViewModel<FI, DI, FSI>)
                ShowCaption = EmbeddedDirectoryModel.Equals(_rootModel.SelectedNavigationViewModel.EmbeddedDirectoryModel);
            //(_rootModel as BreadcrumbViewModel<FI, DI, FSI>).Hierarchy.Count == 1;
        }
        #endregion

        #region Broadcast FileSystem Changes

        public void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            if (EmbeddedDirectoryViewModel != null)
                if ((PathEx.GetDirectoryName(parseName) == "" && parseName.Equals(EmbeddedDirectoryModel.ParseName)) ||
                     (PathEx.GetDirectoryName(parseName).Equals(EmbeddedDirectoryModel.ParseName)))
                //if (EmbeddedDirectoryViewModel.BroadcastChange(parseName, changeType))
                {
                    if (_subDirectories != null)
                        _subDirectories.Load(false);
                }

            //NavigationItemViewModel<FI, DI, FSI>[] _subDirs = SubDirectories.ToArray();
            //for (int i = 0; i < _subDirs.Length; i++)
            //{
            //    _subDirs[i].BroadcastChange(parseName, changeType);
            //    Debug.WriteLine(i);
            //}

            if (SubDirectories != null)
                foreach (NavigationItemViewModel<FI, DI, FSI> subDir in SubDirectories)
                    subDir.BroadcastChange(parseName, changeType);
        }

        #endregion

        #region Commands

        public string ContextMenu(Point pos)
        {
            return Profile.ShowContextmenu(pos, EmbeddedDirectoryModel);
        }

        public void Refresh()
        {
            list(true);
        }


        private void setupCommands()
        {
            ContextMenuCommand = new SimpleCommand()
            {
                ExecuteDelegate = (x) =>
                {
                    NavigationItemViewModel<FI, DI, FSI> vm = this;

                    Point pt = UITools.GetScreenMousePosition();
                    switch (vm.ContextMenu(pt))
                    {
                        case "open": if (OpenCommand != null) OpenCommand.Execute(vm); break;
                        case "rename": if (RenameCommand != null) RenameCommand.Execute(vm); break;
                        case "refresh": if (RefreshCommand != null) RefreshCommand.Execute(vm); break;
                    }

                }
            };

            OpenCommand = new SimpleCommand()
            {
                ExecuteDelegate = (x) =>
                {
                    Profile.Open(EmbeddedDirectoryModel);
                }
            };

            RefreshCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                {
                    return _subDirectories != null && !_subDirectories.IsWorking;
                },
                ExecuteDelegate = (x) =>
                {
                    Refresh();
                }
            };

            PropertiesCommand = new SimpleCommand()
            {
                ExecuteDelegate = x =>
                {
                    System.Windows.Point position = Mouse.GetPosition(null);
                    Profile.ShowProperties(position, EmbeddedDirectoryModel);
                }
            };

            RenameCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                {
                    return !this.EmbeddedDirectoryModel.IsReadonly;
                },
                ExecuteDelegate = (x) =>
                {
                    EmbeddedDirectoryViewModel.IsEditing = true;
                }
            };

            DeleteCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                {
                    return EmbeddedDirectoryModel.IsReadonly;
                },
                ExecuteDelegate = (x) =>
                {
                    try
                    {
                        string caption = String.Format(Texts.strConfirmDelete,
                                1, "");

                        if (new WPFMessageBoxService().ShowYesNo(caption, CustomDialogIcons.Question)
                            == CustomDialogResults.Yes)
                        {
                            Profile.Delete(EmbeddedDirectoryModel, EmbeddedDirectoryViewModel.EmbeddedDirectoryModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        new WPFMessageBoxService().ShowError(ex.Message);
                    }
                }
            };
        }

        #endregion


        #region Drag n Drop
        public bool IsDropEnabled
        {
            get { return this.EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.IsSupportAdd; }
        }

        public DragDropEffects QueryDrop(DragDropEffects sourceEffects, DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.QueryDrop(Profile, sourceEffects, dropInfo);
        }

        public DragDropEffects QueryDrop(DragDropEffects sourceEffects, string[] droppingFiles)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.QueryDrop(Profile, sourceEffects, droppingFiles);
        }

        public DragDropEffects Drop(DragDropEffects sourceEffects, DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.Drop(Profile, sourceEffects, dropInfo);
        }

        public DragDropEffects Drop(DragDropEffects sourceEffects, string[] droppingFiles)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.Drop(Profile, sourceEffects, droppingFiles);
        }

        public void NotifyRefresh()
        {
            throw new NotImplementedException();
        }

        public void StartExpandTimer()
        {
            Timer timer = new Timer(1000);
            ElapsedEventHandler onElapsed = null;
            
            onElapsed = new ElapsedEventHandler(
                 (o, e) =>
                 {
                     if (IsDraggingOver)
                         IsExpanded = true;
                     timer.Elapsed -= onElapsed;
                     timer.Stop();
                     timer.Dispose();
                 }
                 );

            timer.Elapsed += onElapsed;
            timer.Start();
        }

        #endregion

        #endregion

        #region Data

        private bool _isShadowItem = false;
        private bool _isSeparator = false;
        private bool _isDropDownOpen = false;
        private bool _isItemVisible = false;
        private bool _showCaption = true;
        private bool _isSubItem = false;
        private int _customPosition = -1;

        private bool _isExpanded = false;
        private bool _isSelected = false;
        private bool _isDraggingOver = false;
        private bool _isInited = false;
        private bool _isDirectory = false;
        private NavigationRootViewModel<FI, DI, FSI> _rootModel;
        private NavigationItemViewModel<FI, DI, FSI> _parentModel;

        private EntryModel<FI, DI, FSI> _embeddedEntryModel;
        private EntryViewModel<FI, DI, FSI> _embeddedEntryViewModel;
        private DirectoryModel<FI, DI, FSI> _embeddedDirectoryModel;
        private DirectoryViewModel<FI, DI, FSI> _embeddedDirectoryViewModel;
        private AsyncObservableCollection<NavigationItemViewModel<FI, DI, FSI>> _subDirectories;

        #endregion

        #region Public Properties

        internal Profile<FI, DI, FSI> Profile { get { return _rootModel.Profile; } }

        public AsyncObservableCollection<NavigationItemViewModel<FI, DI, FSI>> SubDirectories
        { get { if (_subDirectories == null) setupSubdir(); return _subDirectories; } }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    if (_rootModel != null && value)
                        _rootModel.ReportBeingSelected(this);
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }


        #region Breadcrumb related

        public bool IsFirstItem
        {
            get { return ParentViewModel == null; }
        }

        public bool IsSubItem
        {
            get { return _isSubItem || !IsItemVisible; }
            set { _isSubItem = value; NotifyPropertyChanged("IsSubItem"); }
        }

        static PropertyChangedEventArgs showCaptionChangeArgs =
          ObservableHelper.CreateArgs<NavigationItemViewModel<FI, DI, FSI>>(x => x.ShowCaption);

        public bool ShowCaption
        {
            get { return _showCaption; }
            set
            {
                _showCaption = value;
                NotifyPropertyChanged(showCaptionChangeArgs);
            }
        }

        static PropertyChangedEventArgs isShadowItemChangeArgs =
         ObservableHelper.CreateArgs<NavigationItemViewModel<FI, DI, FSI>>(x => x.IsShadowItem);

        public bool IsShadowItem
        {
            get { return _isShadowItem; }
            set
            {
                _isShadowItem = value;
                NotifyPropertyChanged(isShadowItemChangeArgs);
            }
        }

        static PropertyChangedEventArgs isSeparatorChangeArgs =
         ObservableHelper.CreateArgs<NavigationItemViewModel<FI, DI, FSI>>(x => x.IsSeparator);

        public bool IsSeparator
        {
            get { return _isSeparator; }
            set
            {
                _isSeparator = value;
                NotifyPropertyChanged(isSeparatorChangeArgs);
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value && value != _isExpanded && !_isInited)
                    list();
                _isExpanded = value;                
                NotifyPropertyChanged("IsExpanded");
            }
        }

        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set
            {
                if (value && (!_isDropDownOpen || IsFirstItem))
                    list();
                _isDropDownOpen = value;
                NotifyPropertyChanged("IsDropDownOpen");
            }
        }

        static PropertyChangedEventArgs isItemVisibleChangeArgs =
           ObservableHelper.CreateArgs<NavigationItemViewModel<FI, DI, FSI>>(x => x.IsItemVisible);

        public bool IsItemVisible
        {
            get { return _isItemVisible; }
            set
            {
                _isItemVisible = value;
                NotifyPropertyChanged("IsSubItem");
                NotifyPropertyChanged(isItemVisibleChangeArgs);
            }
        }
        #endregion

        #region CustomPosition
        static PropertyChangedEventArgs CustomPositionChangeArgs =
            ObservableHelper.CreateArgs<NavigationItemViewModel<FI, DI, FSI>>(x => x.CustomPosition);
        /// <summary>
        /// Custom postion in Navigation Item View model level.
        /// </summary>
        public int CustomPosition
        {
            get { return _customPosition; }
            set { _customPosition = value; NotifyPropertyChanged(CustomPositionChangeArgs); }
        }
        #endregion


        public NavigationItemViewModel<FI, DI, FSI> ParentViewModel { get { return _parentModel; } }
        public bool IsDirectory { get { return _isDirectory; } }
        public bool IsDraggingOver
        {
            get { return _isDraggingOver; }
            set
            {
                _isDraggingOver = value;
                if (_rootModel != null)
                    if (value)
                    {
                        _rootModel.ReportDraggedOver(this);
                        if (!IsExpanded)
                            StartExpandTimer();                        
                    }
                    else _rootModel.ReportDraggedLeave(this);
                NotifyPropertyChanged("IsDraggingOver");
            }
        }
        public EntryModel<FI, DI, FSI> EmbeddedEntryModel { get { return _embeddedEntryModel; } }
        public EntryViewModel<FI, DI, FSI> EmbeddedEntryViewModel { get { return _embeddedEntryViewModel; } }
        public DirectoryModel<FI, DI, FSI> EmbeddedDirectoryModel { get { return _embeddedDirectoryModel; } }
        public DirectoryViewModel<FI, DI, FSI> EmbeddedDirectoryViewModel { get { return _embeddedDirectoryViewModel; } }

        #endregion





    }
}
