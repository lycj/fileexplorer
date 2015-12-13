using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{  
    public interface IModelIconExtractor<T>
    {
        Task<byte[]> GetIconBytesForModelAsync(T model, CancellationToken ct);
    }

}
