using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using QuickZip.UserControls.MVVM.Notification.ViewModel;
using System.Windows.Input;

namespace QuickZip.UserControls.MVVM.Notification.Model
{
    public abstract class NotificationSourceModel : ValidatingObject
    {
        #region Constructor        

        #endregion

        #region Methods


        private NotificationSourceViewModel _viewModel = null;
        public NotificationSourceViewModel ToViewModel()
        {
            if (_viewModel == null)
                _viewModel = new NotificationSourceViewModel(this);
            return _viewModel;
        }

        public abstract IEnumerable<NotificationItemModel> GetNotificationItems();

        public void NotifyItemsUpdated()
        {
            OnSourceUpdated(this, new EventArgs());
        }

        #endregion

        #region Data

        

        #endregion

        #region Public Properties

        /// <summary>
        /// Raise event when notification items required to update.
        /// </summary>
        public EventHandler OnSourceUpdated = (o, e) => { };

       

        #endregion
    }
}
