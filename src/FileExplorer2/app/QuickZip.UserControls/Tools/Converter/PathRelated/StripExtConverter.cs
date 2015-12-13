using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StripExtConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static StripExtConverter Instance = new StripExtConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = (string)value;
            if (path != null && Path.HasExtension(path))
                return path.Replace(Path.GetExtension(path), "");

            return value;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
