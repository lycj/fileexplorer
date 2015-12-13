using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class MetadataProviderBase : IMetadataProvider
    {
        private IMetadataProvider[] _additionalProvider;

        public MetadataProviderBase(params IMetadataProvider[] additionalProvider)
        {
            _additionalProvider = additionalProvider;
        }




        public async virtual Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            foreach (var p in _additionalProvider)
                retList.AddRange(await p.GetMetadataAsync(selectedModels, modelCount, parentModel));

            return retList;
        }
    }
}
