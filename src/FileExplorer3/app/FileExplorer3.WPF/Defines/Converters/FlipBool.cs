using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class FlipBoolConverter : IValueConverter
    {     
        #region IValueConverter Members        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return false;
            else return true;
        }

        #endregion
    }
}
