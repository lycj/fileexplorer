using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public enum ItemType { itString };

    [DefaultProperty("Value")]
    [ContentProperty("Value")]
    public class StatusbarItem : ContentControl
    {        
        static StatusbarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusbarItem), new FrameworkPropertyMetadata(typeof(StatusbarItem)));
        }

        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register("ItemType", typeof(ItemType), typeof(StatusbarItem),
            new FrameworkPropertyMetadata(ItemType.itString));

        public ItemType ItemType
        {
            get { return (ItemType)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(StatusbarItem),
            new FrameworkPropertyMetadata("Header"));
        
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(StatusbarItem),
            new FrameworkPropertyMetadata("Value"));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            try
            {
                return base.MeasureOverride(constraint);
            }
            catch  (ArithmeticException)
            {
                //Debug.WriteLine(ex.Message);
                return new Size(0, 0);
            }
        }
    }
}
