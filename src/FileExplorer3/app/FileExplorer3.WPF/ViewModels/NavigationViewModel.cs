using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.Script;

namespace FileExplorer.WPF.ViewModels
{
    public class NavigationViewModel : ViewAware, INavigationViewModel,
        IHandle<SelectionChangedEvent>
    {
        #region Constructor

        public NavigationViewModel(IEventAggregator events)
        {
            Events = events;
            if (events != null)
                events.Subscribe(this);

            Commands = new NavigationCommandManager(this, events);            
        }

        #endregion

        #region Methods       

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (!_viewAttached)
            {
                var uiEle = view as System.Windows.UIElement;
                this.Commands.RegisterCommand(uiEle, ScriptBindingScope.Local);
                _viewAttached = true;
            }
        }

        public void Handle(SelectionChangedEvent message)
        {
            if (message.Sender is IDirectoryTreeViewModel || message.Sender is IBreadcrumbViewModel)
            {
                //Is directory change                
                var destFolder = message.SelectedViewModels.First();
                if (!_updatingNavPosition && (_currentFolder == null || !(_currentFolder.Equals(destFolder))))
                {
                    AddAndBroadcast(destFolder);
                }
            }
        }

        private DateTime _lastBroadcast = DateTime.UtcNow;

        public void AddAndBroadcast(IEntryViewModel destFolder)
        {
            //Prevent two events from publishing simultaneously.
            //if (_navigationHistory.Count > 1 && this._navigationHistory[1].Equals(destFolder) &&
            //    DateTime.UtcNow.Subtract(_lastBroadcast).TotalSeconds < 1)
            //    return;

            Add(destFolder);                        
            Events.PublishOnUIThread(new DirectoryChangedEvent(this, destFolder, _currentFolder));
            _currentFolder = destFolder;
            _lastBroadcast = DateTime.UtcNow;
        }

        public void Add(IEntryViewModel item)
        {
            _updatingNavigationHistory = true;
            try
            {
                //GC.Collect(0, GCCollectionMode.Forced, true);
                if (NavigationPosition != -1)
                    for (int i = 0; i < NavigationPosition; i++)
                        NavigationHistory.RemoveAt(0);
                while (NavigationHistory.Count > 10)
                    NavigationHistory.RemoveAt(NavigationHistory.Count - 1);

                if (NavigationHistory.IndexOf(item) != -1)
                    NavigationHistory.Remove(item);
                NavigationHistory.Insert(0, item);
                NavigationPosition = 0;

                UpdateState();
            }
            finally
            {
                _updatingNavigationHistory = false;
            }
        }

        private void UpdateState()
        {
            CanGoNext = NavigationPosition > 0;
            CanGoBack = NavigationPosition < NavigationHistory.Count - 1;
            CanGoUp = _currentFolder != null && _currentFolder.EntryModel.Parent != null;
        }

        public void Clear()
        {
            NavigationHistory.Clear();
            NavigationPosition = -1;
        }

        internal void ChangeNavigationPosition(int newPosition)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _updatingNavPosition = true;
            try
            {
                int orgPosition = NavigationPosition;
                _position = newPosition;
                NotifyOfPropertyChange(() => NavigationPosition);

                if (newPosition != -1 && newPosition < NavigationHistory.Count)
                {
                    if (!_updatingNavigationHistory)
                    {
                        Events.PublishOnUIThread(new DirectoryChangedEvent(this,
                            NavigationHistory[newPosition], NavigationHistory[orgPosition]));
                        _currentFolder = NavigationHistory[newPosition];
                    }
                }
            }
            finally
            {
                _updatingNavPosition = false;
                UpdateState();
            }
        }

        public void GoUp()
        {
            if (!_updatingNavPosition && _currentFolder != null && _currentFolder.EntryModel.Parent != null)
                AddAndBroadcast(EntryViewModel.FromEntryModel(_currentFolder.EntryModel.Parent));
            else UpdateState();
        }

        public void GoBack()
        {
            ChangeNavigationPosition(NavigationPosition + 1);
        }

        public void GoNext()
        {
            ChangeNavigationPosition(NavigationPosition - 1);
        }

        #endregion

        #region Data

        bool _viewAttached = false;
        bool _updatingNavPosition = false, _updatingNavigationHistory = false;
        int _position = 0;
        IEntryViewModel _currentFolder;
        IObservableCollection<IEntryViewModel> _navigationHistory = new BindableCollection<IEntryViewModel>();
        private bool _canGoBack;
        private bool _canGoNext;
        private bool _canGoUp;

        #endregion

        #region Public Properties

        public ICommandManager Commands { get; protected set; }
        public IEventAggregator Events { get; set; }
        public int NavigationPosition { get { return _position; } set {  ChangeNavigationPosition(value); } }
        public IObservableCollection<IEntryViewModel> NavigationHistory { get { return _navigationHistory; } }
        public bool CanGoBack { get { return _canGoBack; } private set { _canGoBack = value; NotifyOfPropertyChange(() => CanGoBack); } }
        public bool CanGoNext { get { return _canGoNext; } private set { _canGoNext = value; NotifyOfPropertyChange(() => CanGoNext); } }
        public bool CanGoUp { get { return _canGoUp; } private set { _canGoUp = value; NotifyOfPropertyChange(() => CanGoUp); } }

        #endregion





       
    }
}
