using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickZip.UserControls.Themes
{
    public class CommonProperties : DependencyObject
    {

        #region ViewSize
        public static readonly DependencyProperty ViewSizeProperty =
            DependencyProperty.RegisterAttached("ViewSize", 
            typeof(int), typeof(CommonProperties), new PropertyMetadata(16));

        public static int GetViewSize(DependencyObject sender)
        {
            return (int)sender.GetValue(ViewSizeProperty);
        }

        public static void SetViewSize(DependencyObject sender, int value)
        {
            sender.SetValue(ViewSizeProperty, value);
        }

        #endregion

        #region PropertyName (attached property)
        public static readonly DependencyProperty SortPropertyNameProperty = 
            DependencyProperty.RegisterAttached("SortPropertyName",
            typeof(SortCriteria), typeof(CommonProperties));

        public static SortCriteria GetSortPropertyName(DependencyObject sender)
        {
            return (SortCriteria)sender.GetValue(SortPropertyNameProperty);
        }

        public static void SetSortPropertyName(DependencyObject sender, SortCriteria value)
        {
            sender.SetValue(SortPropertyNameProperty, value);
        }
        #endregion

        #region IsEditing (attached property)
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached
            ("IsEditing", typeof(bool), typeof(CommonProperties));

        public static bool GetIsEditing(DependencyObject sender)
        {
            return (bool)sender.GetValue(IsEditingProperty);
        }

        public static void SetIsEditing(DependencyObject sender, bool value)
        {
            sender.SetValue(IsEditingProperty, value);
        }
        #endregion
    }
}
