using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement 
    {

        #region Helper functions
        public static object GetAttachedProperty(DependencyObject sender, DependencyProperty property)
        {
            return (object)sender.GetValue(property);
        }

        public static void SetAttachedProperty(DependencyObject sender, DependencyProperty property, object value)
        {
            sender.SetValue(property, value);
        }
        #endregion

        static CommandProvider()
        {
          
        }
    }
}