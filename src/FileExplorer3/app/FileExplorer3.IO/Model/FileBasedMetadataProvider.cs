using FileExplorer.Defines;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{

    public class FileBasedMetadataProvider : MetadataProviderBase
    {
        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();


            if (selectedModels.Count() > 0)
            {
                long size = 0;
                DateTime creationTime = DateTime.MinValue, lastUpdateTime = DateTime.MinValue;
                foreach (var m in selectedModels)
                {
                    if (m is DiskEntryModelBase)
                        size += (m as DiskEntryModelBase).Size;
                    if (m.CreationTimeUtc > creationTime)
                        creationTime = m.CreationTimeUtc;
                    if (m.LastUpdateTimeUtc > lastUpdateTime)
                        lastUpdateTime = m.LastUpdateTimeUtc;
                }

                if (creationTime != DateTime.MinValue)
                    retList.Add(new Metadata(DisplayType.TimeElapsed, MetadataStrings.strCategoryInfo,
                       MetadataStrings.strCreationTime, creationTime.ToLocalTime()) { IsVisibleInStatusbar = false });

                if (lastUpdateTime != DateTime.MinValue && lastUpdateTime.Subtract(creationTime).TotalSeconds > 5)
                    retList.Add(new Metadata(DisplayType.TimeElapsed, MetadataStrings.strCategoryInfo,
                       MetadataStrings.strLastUpdateTime, lastUpdateTime.ToLocalTime()) { IsVisibleInStatusbar = false });

                if (size > 0)
                    retList.Add(new Metadata(DisplayType.Kb, MetadataStrings.strCategoryInfo,
                        MetadataStrings.strSize, size) { IsVisibleInStatusbar = false });
                retList.Add(new Metadata(DisplayType.Image, "", "Thumbnail", "http://rs.sinahk.net/ne/i/120517/sinalogo.gif"));

            }

            return retList;
        }

    }
}
