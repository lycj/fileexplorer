using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM;
using System.Collections;
using System.Windows.Documents;
using System.ComponentModel;
using Cinch;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using QuickZip.Translation;
using System.Windows.Input;
using QuickZip.UserControls.Logic.Tools.DragnDrop;

namespace QuickZip.UserControls.MVVM
{
    /// <summary>
    /// File Listing View.
    /// </summary>
    /// <typeparam name="FI"></typeparam>
    /// <typeparam name="DI"></typeparam>
    /// <typeparam name="FSI"></typeparam>
    public abstract class BaseDirectoryViewerViewModel<FI, DI, FSI> : ViewerBaseViewModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {
        #region Constructor

        public BaseDirectoryViewerViewModel(Profile<FI, DI, FSI> profile,
           DirectoryModel<FI, DI, FSI> embedDirectoryModel)
            : base(profile, embedDirectoryModel)
        {
            //if (specialStatusbar)
            //    StatusbarViewModel = new DirectoryStatusbarViewModel<FI, DI, FSI>(_profile, EmbeddedDirectoryViewModel);
            Reload(true);
            _subEntries = new CollectionViewSource() { Source = EmbeddedDirectoryViewModel.SubEntries };
            _subEntries.ChangeSortMethod<FI, DI, FSI>(_sortBy, _sortDirection);
            IsDirectoryTreeEnabled = true;
            CurrentViewerMode = ViewerMode.Directory;
        }

        public BaseDirectoryViewerViewModel(Profile<FI, DI, FSI> profile)
            : base(profile)
        {
            IsDirectoryTreeEnabled = true;
        }
        #endregion

        #region Data

        private CollectionViewSource _subEntries;
        private SortCriteria _sortBy = SortCriteria.sortByFullName;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        #endregion

        #region Methods

        protected override void setupCommands()
        {
            base.setupCommands();


            CopyCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                    {
                        return (SelectedViewModels != null && SelectedViewModels.Count > 0);
                    },
                ExecuteDelegate = (x) =>
                    {
                        try
                        {
                            _profile.CopyToClipboard(SelectedModels.ToArray());
                        }
                        catch (Exception ex)
                        {
                            new WPFMessageBoxService().ShowError(ex.Message);
                        }
                    }
            };

            NewFolderCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                {
                    return !EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.IsReadonly;
                },
                ExecuteDelegate = (x) =>
                {
                    try
                    {
                        NewFolder("New Folder", x as string);
                    }
                    catch (Exception ex)
                    {
                        new WPFMessageBoxService().ShowError(ex.Message);
                    }
                }
            };




            PasteCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                    {
                        return System.Windows.Clipboard.ContainsFileDropList() &&
                            EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.IsSupportAdd;
                    },
                ExecuteDelegate = (x) =>
                    {
                        try
                        {
                            _profile.PasteFromClipboard(EmbeddedDirectoryViewModel.EmbeddedDirectoryModel);
                        }
                        catch (Exception ex)
                        {
                            new WPFMessageBoxService().ShowError(ex.Message);
                        }
                    }
            };

            DeleteCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                {

                    if (SelectedViewModels != null && SelectedViewModels.Count > 0)
                    {
                        foreach (var vm in SelectedViewModels)
                            if (vm.EmbeddedModel.IsReadonly)
                                return false;
                        return true;
                    }
                    return false;
                },
                ExecuteDelegate = (x) =>
                {
                    try
                    {
                        string caption = String.Format(Texts.strConfirmDelete,
                                SelectedViewModels.Count, SelectedViewModels.Count > 1 ? "s" : "");

                        if (new WPFMessageBoxService().ShowYesNo(caption, CustomDialogIcons.Question)
                            == CustomDialogResults.Yes)
                        {
                            _profile.Delete(SelectedModels.ToArray(), EmbeddedDirectoryViewModel.EmbeddedDirectoryModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        new WPFMessageBoxService().ShowError(ex.Message);
                    }
                }
            };

            PropertiesCommand = new SimpleCommand()
            {
                CanExecuteDelegate = x =>
                {
                    return SelectedModels != null && SelectedModels.Count > 0;
                },
                ExecuteDelegate = x =>
                {
                    System.Windows.Point position = Mouse.GetPosition(null);
                    _profile.ShowProperties(position, SelectedModels.ToArray());
                }
            };

            SelectAllCommand = new SimpleCommand()
            {
                ExecuteDelegate = (x) =>
                    {
                        if (SelectedViewModels == null ||
                            SelectedViewModels.Count < EmbeddedDirectoryViewModel.SubDirectories.Count)
                        {
                            foreach (var item in EmbeddedDirectoryViewModel.SubEntries)
                                item.IsSelected = true;
                            SelectedViewModels =
                                new List<EntryViewModel<FI, DI, FSI>>(from vm in
                                                                          EmbeddedDirectoryViewModel.SubEntries
                                                                      select vm);

                        }
                        else
                        {
                            foreach (var item in EmbeddedDirectoryViewModel.SubEntries)
                                item.IsSelected = false;
                            SelectedViewModels = new List<EntryViewModel<FI, DI, FSI>>();
                        }

                    }
            };


        }

        public void NewFolder(string name = "New Folder", string type = null)
        {
            DI createdDirectory = _profile.CreateDirectory(this.EmbeddedDirectoryViewModel.EmbeddedDirectoryModel, name, type);
            if (createdDirectory == null)
                throw new IOException("Creation Failed");
            else
            {
                Reload(true,
                    () =>
                    {
                        DirectoryModel<FI, DI, FSI> createdDirectoryModel = _profile.ConstructDirectoryModel(createdDirectory);
                        foreach (var item in EmbeddedDirectoryViewModel.SubEntries)
                            item.IsSelected = item.EmbeddedModel.Equals(createdDirectoryModel);
                    }
                    );



            }
        }


        public void Reload(bool forceReload = true, Action afterCompleted = null)
        {
            IsLoading = true;
            if (EmbeddedDirectoryViewModel != null)
                EmbeddedDirectoryViewModel.List(forceReload,
                    () =>
                    {
                        NotifyPropertyChanged("SubEntries");
                        IsLoading = false;
                        UpdateStatusbar();
                        if (afterCompleted != null)
                            afterCompleted();
                        //DirectoryStatusbarViewModel.ChildViewModels = EmbeddedDirectoryViewModel.SubEntries.ToArray();
                        //StatusbarViewModel = new DirectoryStatusbarViewModel<FI, DI, FSI>(_profile, EmbeddedDirectoryViewModel,
                        //    EmbeddedDirectoryViewModel.SubEntries.ToArray());
                    });

        }

        public override void Expand()
        {
            {
                if (IsExpandEnabled)
                    lock (SelectedViewModels)
                    {
                        if (SelectedViewModels.Count == 1)
                        {

                            bool fileSelected = SelectedViewModels[0].EmbeddedModel is FileModel<FI, DI, FSI>;

                            OpenMode mode =
                                SelectedViewModels[0].EmbeddedModel is DirectoryModel<FI, DI, FSI> ?
                                DirectoryOpenMode : fileSelected ?
                                FileOpenMode : OpenMode.None; ;

                            switch (mode)
                            {
                                case OpenMode.OpenInner:
                                    DirectoryChanged(this, new DirectoryChangedEventArgs<FI, DI, FSI>(
                                        SelectedViewModels[0].EmbeddedModel));

                                    break;
                                case OpenMode.OpenOuter:
                                    SelectedViewModels[0].EmbeddedModel.Open();
                                    break;
                            }
                        }
                    }
            }
        }

        public override void Refresh()
        {
            Reload(true);
        }

        public override void OnUnload()
        {
            if (EmbeddedDirectoryViewModel != null)
                EmbeddedDirectoryViewModel.OnUnload();
        }

        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            IsContextMenuEnabled = SelectedViewModels.Count > 0;
            //DirectoryStatusbarViewModel.SelectedViewModels = SelectedViewModels.ToArray();

            //new DirectoryStatusbarViewModel<FI, DI, FSI>(
            //_profile, EmbeddedDirectoryViewModel,
            //EmbeddedDirectoryViewModel.SubEntries.ToArray(),
            //SelectedItems.ToArray());
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            NotifyPropertyChanged("EmbeddedDirectoryViewModel");
        }

        public override string ToString()
        {
            if (EmbeddedDirectoryViewModel == null)
                return "DirectoryViewerVM;Null";
            else
                return "DirectoryViewerVM;" + EmbeddedDirectoryViewModel.ToString();
        }

        protected override string getLabel()
        {
            if (EmbeddedEntryViewModel != null)
                return EmbeddedEntryViewModel.EmbeddedModel.Label;

            return "";
        }

        protected override string getToolTip()
        {
            if (EmbeddedEntryViewModel != null)
                return EmbeddedEntryViewModel.EmbeddedModel.ParseName;
            else return "";
        }

        protected override ImageSource getSmallIcon()
        {
            return EmbeddedDirectoryViewModel.SmallIcon.Item1.Value;
        }

        protected override void unselectAll()
        {
            if (EmbeddedDirectoryViewModel != null)
                foreach (EntryViewModel<FI, DI, FSI> vm in EmbeddedDirectoryViewModel.SubEntries)
                    vm.IsSelected = false;
        }

        public override void BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            if (EmbeddedDirectoryViewModel != null)
                EmbeddedDirectoryViewModel.BroadcastChange(parseName, changeType);
        }

        #endregion

        #region Public Properties

        //public DirectoryStatusbarViewModel<FI, DI, FSI> DirectoryStatusbarViewModel { get { return StatusbarViewModel as DirectoryStatusbarViewModel<FI, DI, FSI>; } }

        public CollectionViewSource SubEntries { get { return _subEntries; } }

        #region SortBy, SortDirection
        static PropertyChangedEventArgs sortByChangeArgs =
         ObservableHelper.CreateArgs<BaseDirectoryViewerViewModel<FI, DI, FSI>>(x => x.SortBy);

        /// <summary>
        /// Specify how subentries sort it's items.
        /// </summary>
        public SortCriteria SortBy
        {
            get { return _sortBy; }
            set
            {
                _subEntries.ChangeSortMethod<FI, DI, FSI>(value, _sortDirection);
                _sortBy = value;
                NotifyPropertyChanged(sortByChangeArgs);
            }
        }

        static PropertyChangedEventArgs sortDirectionChangeArgs =
          ObservableHelper.CreateArgs<BaseDirectoryViewerViewModel<FI, DI, FSI>>(x => x.SortDirection);

        /// <summary>
        /// Specify the direction of sorting (ascending or descending).
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                _subEntries.ChangeSortMethod<FI, DI, FSI>(_sortBy, value);
                _sortDirection = value;
                NotifyPropertyChanged(sortDirectionChangeArgs);
            }
        }
        #endregion

        public DirectoryViewModel<FI, DI, FSI> EmbeddedDirectoryViewModel
        {
            get { return (DirectoryViewModel<FI, DI, FSI>)EmbeddedEntryViewModel; }
        }


        #endregion


    }
}
