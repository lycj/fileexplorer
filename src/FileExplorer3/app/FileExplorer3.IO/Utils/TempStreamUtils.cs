using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public static class TempStreamUtils
    {
        public static Stream NewTempStream(out string fileName, string ext)
        {
            if (ext.StartsWith("."))
                ext = ext.TrimStart('.');
            do
            {
                fileName = PathFE.Combine(Path.GetTempPath(), StringUtils.RandomString(8) + "." + ext);
            }
            while (File.Exists(fileName));

            return new FileStream(fileName, FileMode.CreateNew);
        }

    }
}
