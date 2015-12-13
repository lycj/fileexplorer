using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls
{
    public static class UCUtils
    {        

        /// <summary>
        /// Add a slash "\" to end of input if not exists.
        /// </summary>
        public static string AppendSlash(string input)
        {
            if (input.EndsWith(@"\")) { return input; }
            else
            { return input + @"\"; }
        }

        public static string SizeInK(UInt64 size)
        {
            if (size == 0)
                return "0 kb";

            float sizeink = ((float)size / 1024);
            if (sizeink <= 999.99)
                return sizeink.ToString("#0.00") + " kb";

            float sizeinm = sizeink / 1024;
            if (sizeinm <= 999.99)
                return sizeinm.ToString("###,###,###,##0.#") + " mb";

            float sizeing = sizeinm / 1024;
            return sizeing.ToString("###,###,###,##0.#") + " GB";
        }

        /// <summary>
        /// Return ProgramFiles path.
        /// </summary>		
        public static string GetProgramPath()
        {
            System.Reflection.Assembly assembly;
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly != null)
                return AppendSlash(PathEx.GetDirectoryName(assembly.Location));
            else
                return AppendSlash(PathEx.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
        }
    }
}
