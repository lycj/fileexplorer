using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public static class BitmapUtils
    {
        private static SizeF TextSize(Font font, string str)
        {
            Bitmap bmp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bmp);
            return g.MeasureString(str, font);
        }

        public static Stream DrawTextToBitmapStream(string fontName, int fontSize, string outputStr)
        {
            var stream = new MemoryStream();
            Font f = new Font(fontName, fontSize);
            var size = TextSize(f, outputStr);
            Bitmap bmp = new Bitmap((int)size.Width, (int)size.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(outputStr, f, Brushes.Black, 0, 0);

            g.Flush();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
