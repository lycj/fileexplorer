using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QuickZip.UserControls.MVVM.ViewModel;
using Cinch;
using QuickZip.UserControls.MVVM.Model;
using System.ComponentModel;

namespace QuickZip.UserControls.MVVM
{
    public class BreadcrumbViewModel<FI, DI, FSI> : NavigationRootViewModel<FI, DI, FSI> 
          where DI : FSI
        where FI : FSI
    {
        public BreadcrumbViewModel(Profile<FI, DI, FSI> profile, ExplorerViewModel<FI, DI, FSI> explrVM)
           : base(profile)
        {
            _explrVM = explrVM;

            //_explrVM.DirectoryChanged += (EventHandler<DirectoryChangedEventArgs<FI, DI, FSI>>) 
            //    ((o, e) =>
            //    {
                    
            //    });

            _hierarchy = new ObservableCollection<NavigationItemViewModel<FI, DI, FSI>>();
            setupBackgroundWorker();

            //explrVM.PropertyChanged += (o, e) =>
            //    {
            //        if (e.PropertyName == "CurrentBrowserViewModel")
            //        {

            //        }
            //    };
        }

        #region Methods

        public override void PlaceBounty(EntryModel<FI, DI, FSI> bountyModel)
        {
            if (bountyModel != null && !bountyModel.Equals(CurrentEntryViewModel))
            {
                CurrentEntryViewModel = Profile.ConstructEntryViewModel(bountyModel);
                SelectedNavigationViewModel =
                    NavigationItemViewModel<FI, DI, FSI>.FromModel(this,
                    CurrentEntryViewModel.EmbeddedModel);
                ProcessHierarchy(SelectedNavigationViewModel);
                DirectoryChanged(this, new DirectoryChangedEventArgs<FI, DI, FSI>(bountyModel));
            }
        }

        #region Hierarchy related
        internal void ProcessHierarchy(NavigationItemViewModel<FI, DI, FSI> itemViewModel)
        {
            bgWorker_ProcessHierarchy.WorkerArgument = itemViewModel;
            bgWorker_ProcessHierarchy.RunBackgroundTask();
        }

        internal void BroadcastItemsChanged()
        {
            foreach (NavigationItemViewModel<FI, DI, FSI> item in Hierarchy)
                item.BroadcastItemsChanged();
        }

        private void setupBackgroundWorker()
        {
            bgWorker_ProcessHierarchy = new BackgroundTaskManager
                <NavigationItemViewModel<FI, DI, FSI>,
                List<NavigationItemViewModel<FI, DI, FSI>>>(
                (VM) =>
                {
                    List<NavigationItemViewModel<FI, DI, FSI>> retVal =
                        new List<NavigationItemViewModel<FI, DI, FSI>>();


                    NavigationItemViewModel<FI, DI, FSI> currentModel = VM;

                    while (currentModel != null)
                    {
                        retVal.Add(currentModel);
                        currentModel = currentModel.ParentViewModel;
                    }

                    retVal.Reverse();
                    return new List<NavigationItemViewModel<FI, DI, FSI>>(retVal);
                },
                    (result) =>
                    {
                        foreach (var item in Hierarchy)
                            item.IsDropDownOpen = false;

                        if (result.Count == 1) //Root Directory
                        {
                            while (Hierarchy.Count > 1)
                                Hierarchy.RemoveAt(1);
                            if (Hierarchy.Count == 0)
                                Hierarchy.Add(result[0]);
                            Hierarchy[0].ShowCaption = true;

                        }
                        else
                            for (int i = 0; i < result.Count; i++)
                                if (Hierarchy.Count > i)
                                {
                                    if (result[i].Equals(Hierarchy[i]))
                                    {
                                        //ok, do nothing
                                        Hierarchy[i].IsShadowItem = false;
                                    }
                                    else //Not equal, replace all further directories
                                    {
                                        while (Hierarchy.Count > i)
                                            Hierarchy.RemoveAt(i);
                                        //Then add back the item
                                        Hierarchy.Add(result[i]);
                                    }
                                }
                                else Hierarchy.Add(result[i]);

                        try
                        {
                            if (Hierarchy.Count > result.Count)
                                if (Hierarchy[result.Count - 1].IsItemVisible)
                                    for (int i = result.Count; i < Hierarchy.Count; i++)
                                        Hierarchy[i].IsShadowItem = true;
                                else while (Hierarchy.Count > result.Count)
                                        Hierarchy.RemoveAt(Hierarchy.Count - 1);
                        }
                        catch (ArgumentOutOfRangeException) { } //HierarchyDirectory changed when loading


                        BroadcastItemsChanged();





                        //foreach (NavigationItemViewModel<FI, DI, FSI> hierarchyItem in stack)                        
                        //    _hierarchy.Add(hierarchyItem);
                        //foreach (NavigationItemViewModel<FI, DI, FSI> hierarchyItem in stack)
                        //    hierarchyItem.BroadcastItemsChanged();
                    });

        }
        #endregion
        #endregion

        #region Data

        private ExplorerViewModel<FI, DI, FSI> _explrVM;
        private EntryViewModel<FI, DI, FSI> _currentEntry;
        private ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> _hierarchy;
        private bool _isLoading = false;
        private bool _isBreadcrumbVisible = true;
        private bool _isBreadcrumbEnabled = true;

        private BackgroundTaskManager<NavigationItemViewModel<FI, DI, FSI>,
                List<NavigationItemViewModel<FI, DI, FSI>>>
            bgWorker_ProcessHierarchy;

        #endregion

        #region Public Properties

        public SearchViewModel<FI, DI, FSI> SearchViewModel { get { return _explrVM.SearchViewModel; } }

        public ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> Hierarchy
        { get { return _hierarchy; } }

        public EntryViewModel<FI, DI, FSI> CurrentEntryViewModel
        {    get { return _currentEntry; }
            set { _currentEntry = value; NotifyPropertyChanged("CurrentEntryViewModel"); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged("IsLoading"); } 
        }

        static PropertyChangedEventArgs isBreadcrumbVisibleChangeArgs =
                 ObservableHelper.CreateArgs<BreadcrumbViewModel<FI, DI, FSI>>(x => x.IsBreadcrumbVisible);

        public bool IsBreadcrumbVisible
        {
            get { return _isBreadcrumbVisible; }
            set
            {
                _isBreadcrumbVisible = value;
                NotifyPropertyChanged(isBreadcrumbVisibleChangeArgs);
            }
        }


        static PropertyChangedEventArgs IsBreadcrumbEnabledChangeArgs =
                 ObservableHelper.CreateArgs<BreadcrumbViewModel<FI, DI, FSI>>(x => x.IsBreadcrumbEnabled);

        public bool IsBreadcrumbEnabled
        {
            get { return _isBreadcrumbEnabled; }
            set
            {
                _isBreadcrumbEnabled = value;
                if (!value)
                    IsBreadcrumbVisible = false;
                NotifyPropertyChanged(IsBreadcrumbEnabledChangeArgs);
            }
        }


        #endregion

    }
}
