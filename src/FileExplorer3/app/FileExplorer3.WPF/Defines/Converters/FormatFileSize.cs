using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(UInt64), typeof(string))]
    public class FormatFileSizeConverter : IValueConverter
    {
        public static FormatFileSizeConverter Instance = new FormatFileSizeConverter();

        /// <summary>
        /// Convert size (int) to Kb string more readable to human.
        /// </summary>
        public static string SizeInK(long size)
        {
            if (size == 0)
                return "0 kb";

            float sizeink = ((float)size / 1024);
            if (sizeink <= 999.99)
                return sizeink.ToString("#0.00") + " kb";

            float sizeinm = sizeink / 1024;
            if (sizeinm <= 999.99)
                return sizeinm.ToString("###,###,###,##0.#") + " mb";

            float sizeing = sizeinm / 1024;
            return sizeing.ToString("###,###,###,##0.#") + " GB";
        }

        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {                
                long size = System.Convert.ToInt64(value);
                if (size < 0)
                    return "";
                return SizeInK(size);
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
