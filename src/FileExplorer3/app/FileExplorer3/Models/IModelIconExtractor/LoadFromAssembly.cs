using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;
using System.Reflection;

namespace FileExplorer.Models
{
    public class LoadFromAssembly : IModelIconExtractor<IEntryModel>
    {
        private Assembly _assembly;
        private string _path;

        public LoadFromAssembly(Assembly assembly, string path)
        {
            _assembly = assembly;
            _path = path;
        }

        public async Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            return _assembly.GetManifestResourceStream(_path).ToByteArray();            
        }
    }
}
