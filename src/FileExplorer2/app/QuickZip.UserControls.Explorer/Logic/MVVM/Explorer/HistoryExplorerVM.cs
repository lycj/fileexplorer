using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM;
using System.Windows.Data;
using Cinch;
using System.ComponentModel;

namespace QuickZip.UserControls.MVVM
{

    internal static class listTools
    {
        public static T Pop<T>(this List<T> list)
        {
            lock (list)
            {
                T retVal = list.Last();
                list.RemoveAt(list.Count - 1);
                return retVal;
            }
        }

        public static void Push<T>(this List<T> list, T item)
        {
            lock (list)
            {
                list.Add(item);
            }
        }

        public static void Push<T>(this List<T> list, T item, int maxListSize)
        {
            lock (list)
            {
                list.Add(item);
                if (list.Count > maxListSize)
                    list.RemoveAt(0);
            }
        }
    }

    public class HistoryExplorerViewModel<FI, DI, FSI> : ExplorerViewModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {

        #region Constructor

        public HistoryExplorerViewModel(Profile<FI, DI, FSI> profile)
            : base(profile)
        {
            setupCommands();
        }

        #endregion

        #region Methods
        private static int MAX_FULL_CACHE_COUNT = 5;

        private void AddNavigationHistory(ViewerBaseVM item)
        {
            if (NavigationPosition != -1)
                for (int i = 0; i < NavigationPosition; i++)
                    NavigationHistory.RemoveAt(0);
            while (NavigationHistory.Count > 10)
                NavigationHistory.RemoveAt(NavigationHistory.Count - 1);

            if (NavigationHistory.IndexOf(item) != -1)
                NavigationHistory.Remove(item);
            NavigationHistory.Insert(0, item);
            NavigationPosition = 0;

            //if (jpList != null)
            //{
            //    jpList.AddToRecent(item.EmbeddedDirectoryModel.EmbeddedDirectoryEntry.ParseName);
            //    jpList.Refresh();
            //}
        }

        public void ClearNavigationHistory()
        {
            NavigationHistory.Clear();
            NavigationPosition = -1;
        }

        protected override void ChangeCurrentBrowserViewModel(ViewerBaseVM viewModel)
        {
            ChangeCurrentBrowserViewModel(viewModel, true);
        }

        protected void ChangeCurrentBrowserViewModel(ViewerBaseVM viewModel, bool updateHistory)
        {
            ViewerBaseVM currentBrowserViewModel = CurrentBrowserViewModel;

            base.ChangeCurrentBrowserViewModel(viewModel);
            
            if (updateHistory && !viewModel.Equals(currentBrowserViewModel))
                AddNavigationHistory(viewModel);
        }

        #region ChangeNavigationPosition


        internal void ChangeNavigationPosition(int newPosition)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _navigationPosition = newPosition;
            NotifyPropertyChanged(navigationPositionChangeArgs);
            if (newPosition != -1 && newPosition < NavigationHistory.Count)
            {
                ChangeCurrentBrowserViewModel(NavigationHistory[newPosition], false);
            }
            else //Fault
            {
                //Does nothing
            }

            //NotifyPropertyChanged(SelectedDirectoryChangeArgs);
            //NotifyPropertyChanged(SelectedDirectoryModelChangeArgs);

            CanGoNext = newPosition > 0;
            CanGoBack = newPosition < NavigationHistory.Count - 1;
        }
        #endregion

        internal void ChangeNavigationPosition(ViewerBaseVM item)
        {
            int pos = NavigationHistory.IndexOf(item);
            if (pos != -1)
                ChangeNavigationPosition(pos);
        }

        public void GoBack()
        {
            ChangeNavigationPosition(NavigationPosition + 1);
        }

        public void GoNext()
        {
            ChangeNavigationPosition(NavigationPosition - 1);
        }

        private void setupCommands()
        {
            _goBackCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) => { return CanGoBack; },
                ExecuteDelegate = (x) => { if (CanGoBack) GoBack(); }
            };

            _goNextCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) => { return CanGoNext; },
                ExecuteDelegate = (x) => { if (CanGoNext) GoNext(); }
            };

            //SimpleRoutedCommand.RegisterClass(typeof(Explorer2), _refreshCommand);
        }

        #endregion

        #region Data

        private bool _canGoBack = false, _canGoNext = false;


        private DispatcherNotifiedObservableCollection<ViewerBaseVM> _navigationHistory =
            new DispatcherNotifiedObservableCollection<ViewerBaseVM>();
        private int _navigationPosition = -1;


        private SimpleCommand _goBackCommand = null, _goNextCommand;        

        #endregion

        #region Public Properties

        public SimpleCommand GoBackCommand { get { return _goBackCommand; } }
        public SimpleCommand GoNextCommand { get { return _goNextCommand; } }

        #region Navigation History, Position
        static PropertyChangedEventArgs navigatorHistoryChangeArgs =
          ObservableHelper.CreateArgs<HistoryExplorerViewModel<FI, DI, FSI>>(x => x.NavigationHistory);

        public DispatcherNotifiedObservableCollection<ViewerBaseVM> NavigationHistory
        {
            get { return _navigationHistory; }
            set
            {
                _navigationHistory = value;
                NotifyPropertyChanged(navigatorHistoryChangeArgs);
            }
        }

        static PropertyChangedEventArgs navigationPositionChangeArgs =
           ObservableHelper.CreateArgs<HistoryExplorerViewModel<FI, DI, FSI>>(x => x.NavigationPosition);

        public int NavigationPosition
        {
            get { return _navigationPosition; }
            set
            {
                ChangeNavigationPosition(value);
            }
        }
        #endregion

        #region CanGoBack, CanGoNext
        static PropertyChangedEventArgs canGoBackChangeArgs =
        ObservableHelper.CreateArgs<HistoryExplorerViewModel<FI, DI, FSI>>(x => x.CanGoBack);

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set
            {
                _canGoBack = value;
                NotifyPropertyChanged(canGoBackChangeArgs);
            }
        }

        static PropertyChangedEventArgs canGoNextChangeArgs =
          ObservableHelper.CreateArgs<HistoryExplorerViewModel<FI, DI, FSI>>(x => x.CanGoNext);

        public bool CanGoNext
        {
            get { return _canGoNext; }
            set
            {
                _canGoNext = value;
                NotifyPropertyChanged(canGoNextChangeArgs);
            }
        }
        #endregion

        #endregion
    }


}
