using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Models
{
  


    public class NullMetadataProvider : MetadataProviderBase
    {

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, 
            IEntryModel parentModel)
        {
            return new List<IMetadata>();
        }
    }
}
