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
  
  
    public class GetFromIconExtractIcon : IModelIconExtractor<IEntryModel>
    {
        public static GetFromIconExtractIcon Instance = new GetFromIconExtractIcon();

        public Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            if (model == null || model.IsDirectory)
                return Task.FromResult<byte[]>(null);

            using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(model.FullPath))
            using (var bitmap = icon.ToBitmap())
                return Task.FromResult<byte[]>(
                    bitmap.ToByteArray());
        }
    }


}
