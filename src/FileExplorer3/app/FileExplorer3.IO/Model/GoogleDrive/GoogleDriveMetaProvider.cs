using FileExplorer.WPF.Utils;
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

    public class GoogleDriveMetadataProvider : MetadataProviderBase
    {
        public GoogleDriveMetadataProvider()
            : base(new BasicMetadataProvider(), new FileBasedMetadataProvider())
        {

        }

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            retList.AddRange(await base.GetMetadataAsync(selectedModels, modelCount, parentModel));

            if (selectedModels.Count() == 1)
            {
                var itemModel = selectedModels.FirstOrDefault() as GoogleDriveItemModel;
                if (itemModel != null && itemModel.Metadata.ImageMediaMetadata != null)
                {
                    if (itemModel.Metadata.ImageMediaMetadata.Width.HasValue &&
                        itemModel.Metadata.ImageMediaMetadata.Height.HasValue)
                    {
                        string dimension = String.Format("{0} x {1}", itemModel.Metadata.ImageMediaMetadata.Width.Value,
                            itemModel.Metadata.ImageMediaMetadata.Height.Value);
                        retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strImage, MetadataStrings.strDimension,
                                        dimension) { IsVisibleInSidebar = true });
                    }
                }
                if (itemModel.Metadata != null)
                {
                    if (!String.IsNullOrEmpty(itemModel.Metadata.ThumbnailLink))
                        retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                          new BitmapImage(
                                              new Uri(itemModel.Metadata.ThumbnailLink))) { IsVisibleInSidebar = true });
                }
            }


            return retList;
        }

    }
}
