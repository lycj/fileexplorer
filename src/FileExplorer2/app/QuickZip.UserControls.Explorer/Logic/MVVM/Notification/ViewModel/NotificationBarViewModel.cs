using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Notification.Model;
using Cinch;
using System.Windows.Input;
using System.ComponentModel;

namespace QuickZip.UserControls.MVVM.Notification.ViewModel
{
    public class NotificationBarViewModel : Cinch.ViewModelBase
    {
        #region Constructor

        public NotificationBarViewModel(NotificationSourceViewModel[] sources)
        {            
            _sources = new DispatcherNotifiedObservableCollection<NotificationSourceViewModel>();
            foreach (var source in sources)
                AddSource(source);
            setupCommand();
        }

        public NotificationBarViewModel(NotificationSourceViewModel source)
         : this(new NotificationSourceViewModel[] { source })
        {            
        }


        public NotificationBarViewModel()
            : this(new NotificationSourceViewModel[] {  })
        {
        }

        #endregion

        #region Data

        private DispatcherNotifiedObservableCollection<NotificationSourceViewModel> _sources;
        bool _hasNotification = false;
        private ICommand _clearNotificationCommand;

        #endregion

        #region Methods

        public void AddSource(NotificationSourceViewModel sourceModel)
        {
            lock (_sources)
            {
                _sources.Add(sourceModel);                
            }
            sourceModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(sourceModel_PropertyChanged);
            updateHasNotification();
        }

        void sourceModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HasNotification" :
                    updateHasNotification();
                    break;
            }
        }

        public void updateHasNotification()
        {
            lock (_sources)
            {
                foreach (var source in _sources)
                    if (source.HasNotification)
                    {
                        HasNotification = true;
                        return;
                    }
                HasNotification = false;
            }
        }

        public void RemoveSource(NotificationSourceViewModel sourceModel)
        {
            sourceModel.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(sourceModel_PropertyChanged);
            lock (_sources)
                if (_sources.IndexOf(sourceModel) != -1)
                _sources.Remove(sourceModel);
            updateHasNotification();
        }

        /// <summary>
        /// Remove item thats completed successfully.
        /// </summary>
        public void RemoveCompleted()
        {
            lock (_sources)
                foreach (var source in _sources)
                    source.RemoveCompleted();
        }

        private void setupCommand()
        {
            ClearNotificationCommand = new SimpleCommand
            {              
                ExecuteDelegate = x =>
                {
                    RemoveCompleted();
                }
            };
        }

        #endregion

        #region Public Properties

        static PropertyChangedEventArgs clearNotificationCommandCommandChangeArgs =
       ObservableHelper.CreateArgs<NotificationBarViewModel>(x => x.ClearNotificationCommand);

        public ICommand ClearNotificationCommand
        {
            get { return _clearNotificationCommand; }
            set { _clearNotificationCommand = value; NotifyPropertyChanged(clearNotificationCommandCommandChangeArgs); }
        }

        public DispatcherNotifiedObservableCollection<NotificationSourceViewModel> NotificationSources
        {
            get { return _sources; }
        }

        public bool HasNotification { get { return _hasNotification; } private set { _hasNotification = value; NotifyPropertyChanged("HasNotification"); } }

        #endregion
    }
}
