using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using QuickZip.Tools;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        public static BitmapToBitmapImageConverter Instance = new BitmapToBitmapImageConverter();

        //[System.Runtime.InteropServices.DllImport("gdi32.dll")]
        //public static extern bool DeleteObject(IntPtr hObject);
        //public static BitmapSource loadBitmap(Bitmap source)
        //{
        //    IntPtr hBitmap = source.GetHbitmap();
        //    //Memory Leak fixes, for more info : http://social.msdn.microsoft.com/forums/en-US/wpf/thread/edcf2482-b931-4939-9415-15b3515ddac6/
        //    try
        //    {
        //        BitmapSource retVal = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
        //           BitmapSizeOptions.FromEmptyOptions());
        //        if (retVal.CanFreeze)
        //            retVal.Freeze();
        //        else if (Debugger.IsAttached)
        //            Debugger.Break();
        //        return retVal;
        //    }
        //    catch
        //    {
        //        return new BitmapImage();
        //    }
        //    finally
        //    {
        //        DeleteObject(hBitmap);
        //    }

        //}

        public static BitmapSource loadBitmap(Bitmap source)
        {
            if (source == null)
                source = new Bitmap(1, 1);
            else source = new Bitmap(source);
            MemoryStream ms = new MemoryStream();
            lock (source)
                source.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }
        

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            Bitmap bitmap = value.TryConvertTo<Bitmap>();            
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
