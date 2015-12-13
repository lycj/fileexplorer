using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace FileExplorer.UIEventHub.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class BoolToVisibilityCollapsedConverter : IValueConverter
    {
        public static BoolToVisibilityCollapsedConverter Instance = new BoolToVisibilityCollapsedConverter();

        #region IValueConverter Members        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value != Visibility.Collapsed)
                return true;
            return false;
        }

        #endregion
    }
}
