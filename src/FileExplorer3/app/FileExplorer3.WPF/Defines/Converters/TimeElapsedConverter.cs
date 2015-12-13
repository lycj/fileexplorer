using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Diagnostics;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimeElapsedConverter : IValueConverter
    {
        #region IValueConverter Members
        public static TimeSpanToStrConverter Instance = new TimeSpanToStrConverter();

        public static string Convert(DateTime time)
        {
            if (time == DateTime.MinValue)
                return "";

            var elapsed = DateTime.UtcNow.Subtract(time);
            string retVal = null;
            if (elapsed.TotalDays > 365)
                retVal = string.Format("{0:0} years {1:0} days", elapsed.TotalDays / 365, 
                    elapsed.TotalDays % 365 );
            else
                if (elapsed.TotalDays > 0)
                    retVal = string.Format("{0:0} days", elapsed.TotalDays);
                else if (elapsed.TotalHours > 0)
                    retVal = string.Format("{0:0} hours", elapsed.TotalHours);
                else if (elapsed.TotalSeconds > 0)
                    retVal = string.Format("{0:0} seconds", elapsed.TotalSeconds);

            if (retVal != null)
                return retVal + " ago";

            return "Unknown";
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            return Convert(dt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
