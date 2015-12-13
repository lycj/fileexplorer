using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class FormatDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DateTime time = (DateTime)value;
                if (time.Equals(DateTime.MinValue))
                    return "";
                else
                    return time.ToString(Mask);
            }
            catch { return ""; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string _mask = "yyyy/MM/dd HH:mm:ss";
        public string Mask { get { return _mask; } set { _mask = value; } }

        #endregion
    }
}
