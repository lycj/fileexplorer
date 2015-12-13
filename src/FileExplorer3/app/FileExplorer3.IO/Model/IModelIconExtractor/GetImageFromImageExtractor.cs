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
  
  
    public class GetImageFromImageExtractor : IModelIconExtractor<IEntryModel>
    {
        public static GetImageFromImageExtractor Instance = new GetImageFromImageExtractor();

        public async Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            //return Task<byte[]>.Run(() =>
            //    {
                    if (model != null && !String.IsNullOrEmpty(model.FullPath))
                    {
                        using (Bitmap bitmap =
                            ImageExtractor.ExtractImage(model.FullPath, new Size(120, 90), true))
                        {
                            if (bitmap != null)
                                return bitmap.ToByteArray();
                        }
                    }
                    return null;
                //}, ct);

        }
    }

}
