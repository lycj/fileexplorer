using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#else
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using FileExplorer;
#endif


namespace FileExplorer.WPF.Utils
{
    public static class W32ConverterUtils
    {
       

        private static BitmapEncoder getEncoder(MediaType mt)
        {
            switch (mt)
            {
#if !NETFX_CORE
                case MediaType.Png: return new PngBitmapEncoder();
                case MediaType.Jpeg : return new JpegBitmapEncoder();
#endif

                default: throw new NotSupportedException();
            }
        }

        

#if NETFX_CORE
        public static InMemoryRandomAccessStream ToStream(this BitmapSource bitmapSource, MediaType mt)
        {

            if (bitmapSource is WriteableBitmap)
            {
                var ms = new InMemoryRandomAccessStream();
                BitmapSourceUtils.WriteToStream(
                       (WriteableBitmap)bitmapSource, ms, BitmapSourceUtils.getEncoderId(mt)).Wait();
                return ms;
            }
            else throw new NotSupportedException();

        }

#else
        public static byte[] ToByteArray(this Image image, ImageFormat fmt)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, fmt);
            return ms.ToArray();
        }

        public static byte[] ToByteArray(this Image image)
        {
            return ToByteArray(image, ImageFormat.Png);
        }

        //http://stackoverflow.com/questions/6597676/bitmapimage-to-byte
        public static Stream ToStream(this BitmapSource bitmapSource, BitmapEncoder encoder)
        {            

            MemoryStream ms = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));            
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static byte[] ToBytes(this BitmapSource bitmapSource, MediaType mediaType = MediaType.Png)
        {
            return bitmapSource.ToBytes(getEncoder(mediaType));
        }

        public static Stream ToStream(this BitmapSource bitmapSource, MediaType mediaType = MediaType.Png)
        {
            return bitmapSource.ToStream(getEncoder(mediaType));
        }
#endif

        

#if NETFX_CORE

        public static byte[] ToBytes(this IRandomAccessStream stream)
        {
            using (var dr = new DataReader(stream))
            {
                byte[] bytes = new byte[stream.Size];
                dr.ReadBytes(bytes);
                return bytes;
            }
        }

        public static byte[] ToBytes(this BitmapSource bitmapSource, MediaType mt = MediaType.Jpeg)
        {
            return ToStream(bitmapSource, mt).ToBytes();
        }

#else
        public static byte[] ToBytes(this BitmapSource bitmapSource, BitmapEncoder encoder)
        {
            byte[] data;
            
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
#endif



        //http://stackoverflow.com/questions/7187528/wpf-convert-file-to-byte-to-bitmapsource-using-jpegbitmapdecoder
        public static BitmapSource ToBitmapSource(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {

#if NETFX_CORE
                return null;
#else

                var decoder = new PngBitmapDecoder(stream,
                                                BitmapCreateOptions.PreservePixelFormat,
                                                BitmapCacheOption.OnLoad);

                var retVal = decoder.Frames[0];
                retVal.Freeze();
                return retVal;
#endif
            }
        }

        public static BitmapImage ToBitmapImage(byte[] bytes)
        {
            MemoryStream byteStream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;

            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
