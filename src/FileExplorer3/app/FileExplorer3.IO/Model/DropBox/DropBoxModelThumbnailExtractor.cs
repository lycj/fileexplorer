using DropNet;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileExplorer.Models
{
    public class DropBoxModelThumbnailExtractor : IModelIconExtractor<IEntryModel>
    {
        private Func<DropNetClient> _clientFunc;
        public DropBoxModelThumbnailExtractor(Func<DropNetClient> clientFunc)
        {
            _clientFunc = clientFunc;
        }


        public async Task<byte[]>
            GetIconBytesForModelAsync(IEntryModel model, System.Threading.CancellationToken ct)
        {
            var dboxModel = model as DropBoxItemModel;
            if (dboxModel != null && dboxModel.Metadata != null && dboxModel.Metadata.Thumb_Exists)
            {
                byte[] bytes = (await _clientFunc().GetThumbnailTask(dboxModel.Metadata, 
                    DropNet.Models.ThumbnailSize.Large)).RawBytes;

                return bytes;
            }
            return null;
        }
    }
}
