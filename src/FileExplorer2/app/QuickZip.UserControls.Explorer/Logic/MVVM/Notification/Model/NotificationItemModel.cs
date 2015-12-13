using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.Windows.Input;
using System.Drawing;
using QuickZip.UserControls.MVVM.Notification.ViewModel;
using QuickZip.UserControls.MVVM.Command.Model;
using QuickZip.Translation;

namespace QuickZip.UserControls.MVVM.Notification.Model
{
    public enum NotificationMode { Waiting, Processing, Completed, Aborted, Error, PendingToRemove }

    public abstract class NotificationItemModel : ValidatingObject
    {

        #region Methods

        /// <summary>
        /// Invoke work when double clicked (default work).
        /// </summary>
        public abstract void DoWork();
        
        /// <summary>
        /// Return supported commands
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<GenericCommandModel> GetCommands()
        {
            yield return new GenericCommandModel(Texts.strRemoveFromList, 
                (o) => { NotifyRemoveNotification(); }) { Priority = -9999 };
        }

        public NotificationItemViewModel ToViewModel()
        {
            return new NotificationItemViewModel(this);
        }

        public virtual void NotifyRemoveNotification()
        {
            NotificationMode = Model.NotificationMode.PendingToRemove;
            OnRemoveRequested(this, new EventArgs());
        }

        public void NotifyCommandsChanged()
        {
            OnCommandsChanged(this, new EventArgs());
        }

        #endregion


        #region Public Properties

        public EventHandler OnCommandsChanged = (o, e) => { };
        public EventHandler OnRemoveRequested = (o, e) => { };

        static PropertyChangedEventArgs hintTitleChangeArgs =
          ObservableHelper.CreateArgs<NotificationItemModel>(x => x.HintTitle);
        public string HintTitle
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(hintTitleChangeArgs); }
        }

        static PropertyChangedEventArgs hintMessageChangeArgs =
            ObservableHelper.CreateArgs<NotificationItemModel>(x => x.HintMessage);
        public string HintMessage
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged(hintMessageChangeArgs); }
        }

        static PropertyChangedEventArgs headerChangeArgs =
           ObservableHelper.CreateArgs<NotificationItemModel>(x => x.Header);
        public string Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(headerChangeArgs); }
        }       

        static PropertyChangedEventArgs percentCompletedChangeArgs =
           ObservableHelper.CreateArgs<NotificationItemModel>(x => x.PercentCompleted);
        public ushort PercentCompleted
        {
            get { return _percentCompleted; }
            set { _percentCompleted = value; NotifyPropertyChanged(percentCompletedChangeArgs); }
        }

        static PropertyChangedEventArgs isProgressShownChangeArgs =
         ObservableHelper.CreateArgs<NotificationItemModel>(x => x.IsProgressShown);
        public bool IsProgressShown
        {
            get { return _showProgress; }
            set { _showProgress = value; NotifyPropertyChanged(isProgressShownChangeArgs); }
        }

        static PropertyChangedEventArgs isActiveChangeArgs =
      ObservableHelper.CreateArgs<NotificationItemModel>(x => x.IsActive);
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; NotifyPropertyChanged(isActiveChangeArgs); }
        }

        static PropertyChangedEventArgs canDoWorkChangeArgs =
     ObservableHelper.CreateArgs<NotificationItemModel>(x => x.CanDoWork);
        public bool CanDoWork
        {
            get { return _canDoWork; }
            set { _canDoWork = value; NotifyPropertyChanged(canDoWorkChangeArgs); }
        }



        static PropertyChangedEventArgs notificationModeChangeArgs =
         ObservableHelper.CreateArgs<NotificationItemModel>(x => x.NotificationMode);
        public NotificationMode NotificationMode
        {
            get { return _notificationMode; }
            set
            {
                _notificationMode = value; NotifyPropertyChanged(notificationModeChangeArgs);
            }
        }

        static PropertyChangedEventArgs priorityChangeArgs =
           ObservableHelper.CreateArgs<NotificationItemModel>(x => x.Priority);

        public int Priority
        {
            get { return (int)_priority; }
            set
            {
                _priority = value;
                NotifyPropertyChanged(priorityChangeArgs);
            }

        }

        static PropertyChangedEventArgs idChangeArgs =
           ObservableHelper.CreateArgs<NotificationItemModel>(x => x.ID);

        public int ID
        {
            get { return (int)_id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(idChangeArgs);
            }

        }

        static PropertyChangedEventArgs iconChangeArgs =
            ObservableHelper.CreateArgs<NotificationItemModel>(x => x.Icon);
        public System.Drawing.Bitmap Icon
        {
            get { return _icon; }
            set { _icon = value; NotifyPropertyChanged(iconChangeArgs); }
        }



        #endregion

        #region Data

        private bool _isActive = true;        
        private System.Drawing.Bitmap _icon;        
        public bool _canDoWork = false;
        private string _title, _message, _header;
        private ushort _percentCompleted;
        private bool _showProgress;
        private NotificationMode _notificationMode = NotificationMode.Processing;
        private int _priority = -1, _id = -1;
        string _headerText = "";        

        #endregion

    }

    public abstract class NotificationItemModel<T> : NotificationItemModel
    {

        #region Constructor

        public NotificationItemModel(T embeddedNotificationItem)
        {
            EmbeddedNotificationItem = embeddedNotificationItem;
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is NotificationItemModel<T>)
                return this.EmbeddedNotificationItem.Equals((obj as NotificationItemModel<T>).EmbeddedNotificationItem);

            return false;
        }

        public override int GetHashCode()
        {
            return this.EmbeddedNotificationItem.GetHashCode();
        }

        #endregion


        #region Data

        private T _embeddedNotificationItem;

        #endregion

        #region Public Properties

        public T EmbeddedNotificationItem
        {
            get { return _embeddedNotificationItem; }
            protected set { _embeddedNotificationItem = value; NotifyPropertyChanged("EmbeddedNotificationItem"); }
        }

        #endregion

    }
}
