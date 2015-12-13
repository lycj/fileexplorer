using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class InvertSignConverter : IValueConverter
    {
        public static IntAddConverter Instance = new IntAddConverter();

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return (double)value * -1;
            }

            if (value is int)
            {
                return (int)value * -1;
            }
            return null;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return (double)value * -1;
            }

            if (value is int)
            {
                return (int)value * -1;
            }
            return null;       
        }

        #endregion
    }
}
