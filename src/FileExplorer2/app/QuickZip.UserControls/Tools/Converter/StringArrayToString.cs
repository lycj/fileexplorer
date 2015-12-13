using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string[]), typeof(string))]
    public class StringArrayToStringConverter : IValueConverter
    {
        public static StringArrayToStringConverter Instance = new StringArrayToStringConverter();        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string[])
            {
                string retVal = "";
                foreach (string val in (string[])value)
                    retVal += val + ", ";

                return retVal.TrimEnd(new char[] { ' ', ',' });
            }
            
            return null;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return (value as string).Split(',');
            }
            
            return null;       
        }

        #endregion
    }
}
