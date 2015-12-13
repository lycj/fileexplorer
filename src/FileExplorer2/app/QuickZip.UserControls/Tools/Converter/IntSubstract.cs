using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(int), typeof(int))]
    public class IntSubstractConverter : IValueConverter
    {
        public static IntSubstractConverter Instance = new IntSubstractConverter();

        public int Subtract { get; set; }

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value - Subtract);
            }

            if (value is double)
            {
                return ((double)value - Subtract);
            }
            return null;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value + Subtract);
            }

            if (value is double)
            {
                return ((double)value + Subtract);
            }

            return null;       
        }

        #endregion
    }
}
