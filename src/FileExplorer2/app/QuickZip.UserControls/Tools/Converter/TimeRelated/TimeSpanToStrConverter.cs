using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToStrConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static TimeSpanToStrConverter Instance = new TimeSpanToStrConverter();

        public static string Convert(TimeSpan span)
        {
            string outStr = "";

            if (span.Hours > 0) outStr += span.Hours.ToString() + " Hours ";
            if (span.Minutes > 0) outStr += span.Minutes.ToString() + " Minutes ";
            if (span.Seconds > 0) outStr += span.Seconds.ToString() + " Seconds ";

            if (outStr == "") outStr = "0 Seconds ";

            return outStr;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan span = (TimeSpan)value;
            return Convert(span);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
