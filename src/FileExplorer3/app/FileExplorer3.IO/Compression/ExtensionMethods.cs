using FileExplorer.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.IO
{
    public static partial class ExtensionMethods
    {
        public static bool CompressOne(this ICompressorWrapper wrapper, string type, Stream stream, string fileName, Stream fileStream)
        {
            return wrapper.CompressMultiple(type, stream, new Dictionary<string, Stream>() { { fileName, fileStream } });
        }

        public static bool Delete(this ICompressorWrapper wrapper, string type, Stream stream, string path)
        {
            return wrapper.Delete(type, stream, (p) => PathFE.MatchFileMask(p, path));
        }
    }
}
