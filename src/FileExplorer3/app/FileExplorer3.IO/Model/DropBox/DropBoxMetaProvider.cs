using FileExplorer.WPF.Utils;
using DropNet;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileExplorer.Models
{

    public class DropBoxMetadataProvider : MetadataProviderBase
    {
        private Func<DropNetClient> _clientFunc;
        public DropBoxMetadataProvider(Func<DropNetClient> clientFunc)
            : base(new BasicMetadataProvider(), new FileBasedMetadataProvider())
        {
            _clientFunc = clientFunc;
        }

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            retList.AddRange(await base.GetMetadataAsync(selectedModels, modelCount, parentModel));

            if (selectedModels.Count() == 1)
            {
                var itemModel = selectedModels.First() as DropBoxItemModel;
                if (itemModel.Metadata != null)
                {
                    if (itemModel.Metadata.Thumb_Exists)
                    {
                        var thumbnailBytes =  (await _clientFunc().GetThumbnailTask(itemModel.Metadata, 
                                DropNet.Models.ThumbnailSize.Large)).RawBytes;
                        if (thumbnailBytes != null && thumbnailBytes.Length > 0)
                            retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                W32ConverterUtils.ToBitmapImage(thumbnailBytes)) { IsVisibleInSidebar = true });
                    }
                }
            }


            return retList;
        }

    }
}
