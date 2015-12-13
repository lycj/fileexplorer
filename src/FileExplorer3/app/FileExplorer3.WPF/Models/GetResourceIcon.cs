using FileExplorer.Models;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer3.WPF.Models
{
    public class GetResourceIcon : IModelIconExtractor<IEntryModel>
    {
        private byte[] IconResource { get; set; }

        public GetResourceIcon(object sender, string path2Resource)
        {
            IconResource = ResourceUtils.GetEmbeddedResourceAsByteArray(sender, path2Resource);
        }

        public Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            return Task.FromResult(IconResource);
        }
    }
}
