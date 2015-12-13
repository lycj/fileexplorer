using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.Defines;

namespace FileExplorer.Models
{
    public class FileSystemInfoMetadataProvider : MetadataProviderBase
    {
        public FileSystemInfoMetadataProvider()
            : base(new BasicMetadataProvider(), new FileBasedMetadataProvider(), new ExifMetadataProvider(), new ImageMetadataProvider())
        { }

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels,
            int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();

            foreach (var m in await base.GetMetadataAsync(selectedModels, modelCount, parentModel))
                retList.Add(m);

            //if (selectedModels.Count() == 0)
            //{
            //    retList.Add(new Metadata(DisplayType.Number, MetadataStrings.strCategoryTest, "Number", 10000) { IsVisibleInSidebar = true });
            //    retList.Add(new Metadata(DisplayType.Percent, MetadataStrings.strCategoryTest, "Percent", 10) { IsVisibleInSidebar = true });
            //    retList.Add(new Metadata(DisplayType.Boolean, MetadataStrings.strCategoryTest, "Boolean", true, false) { IsVisibleInSidebar = true });
            //}
            return retList;

        }

    }
}
