using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FileExplorer.WPF.Utils;

namespace QuickZip.Converters
{
    public class StreamToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Stream)
                value = (value as Stream).ToByteArray();
            if (value is byte[])
                return FileExplorer.WPF.Utils.BitmapSourceUtils.CreateBitmapSourceFromBitmap(value as byte[]);
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
