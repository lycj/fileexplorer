using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class AddBracketConverter : IValueConverter    
    {
        #region IValueConverter Members
        public static AddBracketConverter Instance = new AddBracketConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return " (" + (string)value + ") ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
