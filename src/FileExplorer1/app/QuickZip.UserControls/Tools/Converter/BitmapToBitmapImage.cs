using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        public static BitmapToBitmapImageConverter Instance = new BitmapToBitmapImageConverter();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static BitmapSource loadBitmap(Bitmap source)
        {
            try
            {
                IntPtr hBitmap = source.GetHbitmap();
                //Memory Leak fixes, for more info : http://social.msdn.microsoft.com/forums/en-US/wpf/thread/edcf2482-b931-4939-9415-15b3515ddac6/
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
            catch
            {
                return new BitmapImage();
            }

        }
        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            Bitmap bitmap = value as Bitmap;
            if (bitmap != null)
                return loadBitmap(bitmap);
            return null;   
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
