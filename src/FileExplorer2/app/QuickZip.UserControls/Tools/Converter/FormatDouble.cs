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
    [ValueConversion(typeof(double), typeof(string))]
    public class FormatDoubleConverter : IValueConverter
    {
        private string _format = "{0:F2}";

        public string Format { get { return _format; } set { _format = value; } }

        #region IValueConverter Members        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return String.Format(_format, value);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
