using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(UInt64), typeof(string))]
    public class FormatNumberConverter : IValueConverter
    {
        public static FormatNumberConverter Instance = new FormatNumberConverter();
                
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                long size = System.Convert.ToInt64(value);
                string format = (parameter as string) ?? "###,###,###,##0.##";
                return size.ToString(format);
            }
            catch { return ""; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
