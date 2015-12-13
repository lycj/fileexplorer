using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace QuickZip.UserControls
{
    public class StatusbarBase : ItemsControl
    {
         #region Constructor

        static StatusbarBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusbarBase), new FrameworkPropertyMetadata(typeof(StatusbarBase)));
        }

        public StatusbarBase()
        {
            
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StatusbarItem();
        }

        #endregion

        #region Dependency Properties

        public static DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(StatusbarBase));

        public ImageSource IconSource 
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        #endregion
    }
}
