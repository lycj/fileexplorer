using FileExplorer.Defines;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class BasicMetadataProvider : MetadataProviderBase
    {

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();

            foreach (var m in await base.GetMetadataAsync(selectedModels, modelCount, parentModel))
                retList.Add(m);

            retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strCategoryInfo,
                "", String.Format(MetadataStrings.strTotalItems, modelCount)) { IsHeader = true, IsVisibleInStatusbar = true });


            if (selectedModels.Count() > 0)
                retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strCategoryInfo,
                   "", String.Format(MetadataStrings.strSelectedItems,
                   selectedModels.Count())) { IsHeader = true, IsVisibleInStatusbar = true });

            //switch (selectedModels.Count())
            //{
            //    case 1:
            //        var firstEntry = selectedModels.First();
            //        var thumbnailExtractor = firstEntry.Profile.GetThumbnailExtractor(firstEntry);
            //        if (thumbnailExtractor != null)
            //        {
            //            var iconRetVal = await thumbnailExtractor.GetIconForModelAsync(firstEntry, 
            //                CancellationToken.None);
            //            if (iconRetVal != null)
            //                retList.Add(new Metadata(DisplayType.Image, "", "Thumbnail", iconRetVal) 
            //                { IsVisibleInStatusbar = false });
            //        }
            //        break;
            //}


            return retList;
        }
    }
}
