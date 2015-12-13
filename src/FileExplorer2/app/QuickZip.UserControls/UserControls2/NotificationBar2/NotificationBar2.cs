using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace QuickZip.UserControls
{
    public class NotificationBar2 : ItemsControl
    {
         #region Constructor

        public NotificationBar2()
        {
            
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationBarBase();
        }
        

        #endregion

        #region Dependency Properties

              
        #endregion
    }
}
