using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class GetExtConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static GetExtConverter Instance = new GetExtConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Path.GetExtension((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
