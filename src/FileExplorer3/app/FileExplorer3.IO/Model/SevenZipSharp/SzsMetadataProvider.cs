using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models.SevenZipSharp
{
    public class SzsMetadataProvider : MetadataProviderBase
    {
        public SzsMetadataProvider()
            : base(new BasicMetadataProvider(), new FileBasedMetadataProvider())
        {

        }

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            retList.AddRange(await base.GetMetadataAsync(selectedModels, modelCount, parentModel));

            if (selectedModels.Count() == 1)
            {
                var itemModel = selectedModels.First() as ISzsItemModel;

                if (itemModel is SzsChildModel)
                {
                    var metadata = (itemModel as SzsChildModel)._afi;
                    if (metadata != null)
                    {
                        retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strArchive, MetadataStrings.strCRC,
                                        StringUtils.ConvertToHex(metadata.Crc)) { IsVisibleInSidebar = true });
                        if (metadata.Encrypted)
                            retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strArchive, MetadataStrings.strEncrypted,
                                         "") { IsVisibleInSidebar = true });
                        if (!String.IsNullOrEmpty(metadata.Comment))
                            retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strArchive, MetadataStrings.strComment,
                                         metadata.Comment) { IsVisibleInSidebar = true });
                        
                    }
                }
            }


            return retList;
        }
    }
}
