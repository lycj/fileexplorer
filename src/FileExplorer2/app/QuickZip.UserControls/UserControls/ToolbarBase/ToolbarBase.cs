using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;

namespace QuickZip.UserControls
{
    public class ToolbarBase : Menu
    {
        static ToolbarBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarBase), new FrameworkPropertyMetadata(typeof(ToolbarBase)));
        }

        public ToolbarBase()
        {

        }

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolbarMenuItem();
        }

        #endregion


        #region Public Properties

        public static readonly DependencyProperty IsDraggingOverProperty = DependencyProperty.RegisterAttached("IsDraggingOver",
            typeof(bool), typeof(ToolbarBase), new UIPropertyMetadata(false));

        public static void SetIsDraggingOver(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDraggingOverProperty, value);
        }

        public static bool GetIsDraggingOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDraggingOverProperty);
        }





        #endregion
    }
}
