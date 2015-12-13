using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using QuickZip.UserControls.MVVM.Notification.Model;

namespace QuickZip.UserControls.MVVM.Notification.ViewModel
{
    /// <summary>
    /// A Notification Bar may have multiple NotificationSourceViewModel
    /// </summary>
    public class NotificationSourceViewModel : Cinch.ViewModelBase
    {
        #region Constructor 
        
        public NotificationSourceViewModel(NotificationSourceModel embeddedSourceModel)
        {
            _embeddedSourceModel = embeddedSourceModel;
            _notificationItemList = new Cinch.DispatcherNotifiedObservableCollection<NotificationItemViewModel>();
            setupBgWorker();
            
            _embeddedSourceModel.OnSourceUpdated += new EventHandler(sourceUpdated);            
        }

        #endregion

        #region Methods


        public void setupBgWorker()
        {
            bgWorker_updateItems = new BackgroundTaskManager<object, NotificationItemViewModel[]>(

                (x) =>
                {                    
                    var vmEnum = from m in _embeddedSourceModel.GetNotificationItems() 
                                 where !_removedList.Contains(m.ID) 
                                 select m.ToViewModel();
                    return new List<NotificationItemViewModel>(vmEnum).ToArray();
                },
                (vms) =>
                {
                    _notificationItemList.Clear();
                    foreach (var vm in vms)
                    {
                        vm.ParentModel = this;
                        _notificationItemList.Add(vm);
                    }
                    HasNotification = _notificationItemList.Count > 0;
                });

            
            bgWorker_updateItems.RunBackgroundTask();            
        }

        private void sourceUpdated(object sender, EventArgs e)
        {
            bgWorker_updateItems.RunBackgroundTask();
        }

        public void Add(NotificationItemViewModel vm)
        {
            lock(_notificationItemList)
                _notificationItemList.Add(vm);
        }

        public int IntexOf(NotificationItemViewModel vm)
        {
            lock (_notificationItemList)
                return _notificationItemList.IndexOf(vm);
        }

        //public void Clear()
        //{
        //    lock (_notificationItemList)
        //        _notificationItemList.Clear();
        //}

        public void Remove(NotificationItemViewModel vm)
        {
            lock (_notificationItemList)
                remove(vm);
        }

        private void remove(NotificationItemViewModel vm)
        {
            if (_notificationItemList.Contains(vm))
            {
                _removedList.Add(vm.EmbeddedModel.ID);
                _notificationItemList.Remove(vm);
            }
            HasNotification = _notificationItemList.Count > 0;
        }

        #region removeNotificationMode, RemovePending, RemoveCompleted
        /// <summary>
        /// Remove if mode equals to the specified ones.
        /// </summary>
        /// <param name="mode"></param>
        private void removeNotificationMode(NotificationMode[] modes)
        {
            lock (_notificationItemList)
                for (int i = _notificationItemList.Count - 1; i >= 0; i--)
                {
                    foreach (var mode in modes)
                    {
                        if (_notificationItemList[i].EmbeddedModel.NotificationMode == mode)
                        {
                            remove(_notificationItemList[i]);
                            break;
                        }
                    }

                }
        }

        private void removeNotificationMode(NotificationMode mode)
        {
            removeNotificationMode(new[] { mode });
        }

        /// <summary>
        /// Remove item thats pending to remove (NotifictionMode)
        /// </summary>
        public void RemovePending()
        {
            removeNotificationMode(NotificationMode.PendingToRemove);
        }

        /// <summary>
        /// Remove item thats completed successfully.
        /// </summary>
        public void RemoveCompleted()
        {
            removeNotificationMode(new NotificationMode[] { NotificationMode.PendingToRemove, NotificationMode.Completed });
        }

        #endregion

        #endregion

        #region Data

        NotificationSourceModel _embeddedSourceModel;
        List<int> _removedList = new List<int>();
        DispatcherNotifiedObservableCollection<NotificationItemViewModel> _notificationItemList;
        BackgroundTaskManager<object, NotificationItemViewModel[]> bgWorker_updateItems;
        bool _hasNotification = false;

        #endregion

        #region Public properties

        public DispatcherNotifiedObservableCollection<NotificationItemViewModel> NotificationItemList { get { return _notificationItemList; } }
        public bool HasNotification { get { return _hasNotification; } private set { _hasNotification = value; NotifyPropertyChanged("HasNotification"); } }

        #endregion


    }
}
