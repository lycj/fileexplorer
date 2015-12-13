using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    /// <summary>
    /// Converter < to [b] and > to [/b]
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class TagToBoldConverter : IValueConverter
    {        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return (value as string).Replace("<", "[b]").Replace(">", "[/b]");
            }
            return null;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return (value as string).Replace("[b]", "<").Replace("[/b]", ">");
            }
            return null;       
        }

        #endregion
    }
}
