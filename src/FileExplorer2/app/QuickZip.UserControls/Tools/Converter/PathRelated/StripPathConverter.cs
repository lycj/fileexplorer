using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StripPathConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static StripPathConverter Instance = new StripPathConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            return Path.GetFileName(((string)value).TrimEnd('\\'));            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
