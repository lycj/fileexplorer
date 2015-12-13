using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace WPFDemo
{
    public class GetFilesConverterEx : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return null;
                if (value.Equals(DirectoryInfoEx.NetworkDirectory))
                    return null;
                if (value is DirectoryInfoEx)
                    return (value as DirectoryInfoEx).GetFiles();
                return DirectoryInfoEx.DesktopDirectory.GetFiles();
            }
            catch
            {                
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
