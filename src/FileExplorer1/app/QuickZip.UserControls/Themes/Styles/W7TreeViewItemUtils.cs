using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace QuickZip.UserControls
{
    public class W7TreeViewItemUtils
    {
        public static DependencyProperty ArrowOpacityProperty = 
            DependencyProperty.RegisterAttached("ArrowOpacity", typeof(double), typeof(W7TreeViewItemUtils),
            new UIPropertyMetadata(1.0d));
        public static double GetArrowOpacity(UIElement element, double value)
        {
            return (double)(element.GetValue(ArrowOpacityProperty));
        }

        public static void SetArrowOpacity(UIElement element, double value)
        {
            element.SetValue(ArrowOpacityProperty, value);
        }


        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TreeView && e.NewValue != e.OldValue)
            {
                TreeView tvSender = (TreeView)sender;
                if ((bool)e.NewValue == true)
                {
                    tvSender.MouseEnter += new MouseEventHandler(tvSender_MouseEnter);
                    tvSender.MouseLeave += new MouseEventHandler(tvSender_MouseLeave);
                    SetArrowOpacity(tvSender, 0.1d);
                }
                else
                {
                    tvSender.MouseEnter -= new MouseEventHandler(tvSender_MouseEnter);
                    tvSender.MouseLeave -= new MouseEventHandler(tvSender_MouseLeave);
                    SetArrowOpacity(tvSender, 1.0d);
                }
            }
        }

        static void tvSender_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TreeView tvSender = (TreeView)sender;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1.0d;
            da.To = 0.1d;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            tvSender.BeginAnimation(W7TreeViewItemUtils.ArrowOpacityProperty, da);
        }

        static void tvSender_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TreeView tvSender = (TreeView)sender;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0.1d;
            da.To = 1.0d;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            tvSender.BeginAnimation(W7TreeViewItemUtils.ArrowOpacityProperty, da);
        }

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(W7TreeViewItemUtils),
            new UIPropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

        public static bool GetIsEnabled(UIElement element, bool value)
        {
            return (bool)(element.GetValue(IsEnabledProperty));
        }

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

    }
}
