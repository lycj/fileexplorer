using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(long))]
    public class TimeSpanToTicksConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static TimeSpanToStrConverter Instance = new TimeSpanToStrConverter();

        public static long Convert(TimeSpan span)
        {
            return span.Ticks;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan span = (TimeSpan)value;
            return Convert(span);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new TimeSpan((long)value);
        }

        #endregion
    }
}
