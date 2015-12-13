using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace QuickZip.UserControls
{
    public class NotificationBarBase : ItemsControl
    {
        #region Constructor

        static NotificationBarBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBarBase), new FrameworkPropertyMetadata(typeof(NotificationBarBase)));
        }

        public NotificationBarBase()
        {

        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationBarItem();
        }

        #endregion

        #region Dependency Properties

        

        #endregion
    }
}
