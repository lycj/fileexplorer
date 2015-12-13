using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Tools;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using FileExplorer.WPF.Models;
using QuickZip.Converters;
using QuickZip.UserControls.Logic.Tools.IconExtractor;
using ExifLib;
using System.Windows.Media.Imaging;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.IO.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.IO;

namespace FileExplorer.Models
{
  
    public class DrawOverlayTextExtractor //: IModelIconExtractor<IEntryModel>
    {
        private static async Task<byte[]> getIcon(IModelIconExtractor<IEntryModel> baseExtractor, IEntryModel em,
             Func<IEntryModel, string> text2DrawFunc, System.Drawing.Color color)
        {
            if (em != null && !String.IsNullOrEmpty(em.FullPath))
            {
                byte[] baseBytes = await baseExtractor.GetIconBytesForModelAsync(em, CancellationToken.None);
                Bitmap baseBitmap =
                    new Bitmap(new MemoryStream(baseBytes));

                using (Graphics g = Graphics.FromImage(baseBitmap))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    string text = text2DrawFunc(em);
                    Font font = new Font("Comic Sans MS", Math.Max(baseBitmap.Width / 5, 1),
                        System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    float height = g.MeasureString(text, font).Height;
                    float rightOffset = baseBitmap.Width / 5;

                    //if (size == IconSize.small)
                    //{
                    //    font = new Font("Arial", 5, System.Drawing.FontStyle.Bold);
                    //    height = g.MeasureString(ext, font).Height;
                    //    rightOffset = 0;
                    //}


                    g.DrawString(text, font,
                                new System.Drawing.SolidBrush(color),                                
                                new RectangleF(0, baseBitmap.Height - height, baseBitmap.Width - rightOffset, height),
                                new StringFormat(StringFormatFlags.DirectionRightToLeft));
                    return baseBitmap.ToByteArray();
                }
            }
            return new byte[] { };
        }


        public static IModelIconExtractor<IEntryModel> Create(IModelIconExtractor<IEntryModel> baseExtractor, 
            Func<IEntryModel, string> keyFunc,
            Func<IEntryModel, string> text2DrawFunc, System.Drawing.Color color)
        {          
            return ModelIconExtractor<IEntryModel>.FromTaskFuncCachable(
          keyFunc,
          em => getIcon(baseExtractor, em, text2DrawFunc, color)
          );
        }

        //private Func<IEntryModel, string> _text2DrawFunc;

        //public DrawOverlayTextExtractor(IModelIconExtractor<IEntryModel> baseExtractor, Func<IEntryModel, string> text2DrawFunc)
        //{
        //    _text2DrawFunc = text2DrawFunc;
        //}

        //public Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        //{
        //    return Task<byte[]>.Run(() =>
        //    {
        //        if (model != null && !String.IsNullOrEmpty(model.FullPath))
        //        {
        //            using (Bitmap bitmap =
        //                ImageExtractor.ExtractImage(model.FullPath, new Size(120, 90), true))
        //            {
        //                if (bitmap != null)
        //                    return bitmap.ToByteArray();
        //            }
        //        }
        //        return null;
        //    });

        //}

    }

}
