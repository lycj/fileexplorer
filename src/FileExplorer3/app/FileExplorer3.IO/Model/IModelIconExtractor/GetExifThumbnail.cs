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
  
  
    public class GetExifThumbnail : IModelIconExtractor<IEntryModel>
    {
        public async Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            var diskModel = model as DiskEntryModelBase;
            if (diskModel != null && diskModel.IsFileWithExtension(FileExtensions.ExifExtensions))
            {
                using (var stream =
                    await diskModel.DiskProfile.DiskIO.OpenStreamAsync(diskModel,
                        FileExplorer.Defines.FileAccess.Read, ct))
                {
                    using (ExifReader reader = new ExifReader(stream))
                    {
                        var thumbnailBytes = reader.GetJpegThumbnailBytes();
                        if (thumbnailBytes != null && thumbnailBytes.Length > 0)
                            return thumbnailBytes;
                    }
                }

            }

            return null;

        }
    }
}
