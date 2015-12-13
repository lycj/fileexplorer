using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using QuickZip.Tools;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(int))]
    public class TimeSpanToMsecConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                TimeSpan ts = (TimeSpan)value;
                return ts.TotalMilliseconds;
            }
            catch { return 0; }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int ms = (int)(double)value;                
                return new TimeSpan(0, 0, 0, 0, ms);
            }
            catch { return TimeSpan.MinValue; }
        }

        #endregion
    }
}
