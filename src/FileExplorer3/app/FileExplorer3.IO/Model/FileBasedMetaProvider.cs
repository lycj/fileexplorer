using FileExplorer.WPF.Utils;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.IO;

namespace FileExplorer.Models
{

    public class FileBasedMetadataProvider : MetadataProviderBase
    {
        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            retList.AddRange(await base.GetMetadataAsync(selectedModels, modelCount, parentModel));

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
                       MetadataStrings.strCreationTime, creationTime.ToLocalTime()) { IsVisibleInSidebar = true });

                if (lastUpdateTime != DateTime.MinValue && lastUpdateTime.Subtract(creationTime).TotalSeconds > 5)
                    retList.Add(new Metadata(DisplayType.TimeElapsed, MetadataStrings.strCategoryInfo,
                       MetadataStrings.strLastUpdateTime, lastUpdateTime.ToLocalTime()) { IsVisibleInSidebar = true });

                if (size > 0)
                    retList.Add(new Metadata(DisplayType.Kb, MetadataStrings.strCategoryInfo,
                        MetadataStrings.strSize, size) { IsVisibleInSidebar = true });

                if (selectedModels.Count() == 1)
                {
                    string extension = selectedModels.First().GetExtension();
                    string fileType, mimeType;
                    if (!String.IsNullOrEmpty(extension))
                    {
                        W32RegistryUtils.GetTypeInformation(extension, out fileType, out mimeType);
                        if (fileType != null)
                            retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strCategoryInfo,
                                MetadataStrings.strFileType, fileType) { IsVisibleInSidebar = true });
                        if (mimeType != null)
                            retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strCategoryInfo,
                                MetadataStrings.strMimeType, mimeType) { IsVisibleInSidebar = true });
                    }                    
                }
            }

            return retList;
        }

    }
}
