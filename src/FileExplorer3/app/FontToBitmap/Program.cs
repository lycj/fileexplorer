using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontToBitmap
{
    class Program
    {

        private static SizeF TextSize(Font font, string str)
        {
            Bitmap bmp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bmp);
            return g.MeasureString(str, font);
        }
        static void Main(string[] args)
        {
            string fontName = args[0];
            int fontSize = Convert.ToInt32(args[1]);
            string outputStr = System.Web.HttpUtility.HtmlDecode(args[2]);
            string outputFileName = args[3];
            
            using (var stream = File.OpenWrite(outputFileName))
            {
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
            }
        }
    }
}
