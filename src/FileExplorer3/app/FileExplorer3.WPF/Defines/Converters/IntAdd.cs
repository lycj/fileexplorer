using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(int), typeof(int))]
    public class IntAddConverter : IValueConverter
    {
        public static IntAddConverter Instance = new IntAddConverter();

        public int Add { get; set; }
        public float AddPercent { get; set; }
        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value + Add + (int)((int)value * AddPercent));
            }

            if (value is double)
            {
                return ((double)value + Add + ((double)value * AddPercent));
            }
            return null;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value - Add);
            }

            if (value is double)
            {
                return ((double)value - Add);
            }
            return null;       
        }

        #endregion
    }
}
