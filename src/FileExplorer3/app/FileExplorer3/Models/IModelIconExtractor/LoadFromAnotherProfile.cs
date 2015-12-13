using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class LoadFromLinkPath : IModelIconExtractor<IEntryModel>
    {
        private IProfile[] _profiles;
        public LoadFromLinkPath(params IProfile[] profiles)
        {
            _profiles = profiles;
        }

        public async Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, System.Threading.CancellationToken ct)
        {
            string linkPath = (model as IEntryLinkModel).LinkPath;

            IEntryModel foundEntry = await _profiles.ParseAsync(linkPath);
            if (foundEntry != null)
                return await foundEntry.Profile.GetIconExtractSequence(foundEntry)
                    .Last().GetIconBytesForModelAsync(foundEntry, ct);

            return new byte[] { };
        }
    }
}
