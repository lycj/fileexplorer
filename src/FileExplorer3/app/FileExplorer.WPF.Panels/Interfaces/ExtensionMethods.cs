using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF
{
    public static partial class ExtensionMethods
    {      
        /// <summary>
        /// Add and subtract CacheItemCount to StartIdx and EndIdx.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="startIdx"></param>
        /// <param name="endIdx"></param>
        public static void UpdateCacheItemCount(this IOCPanel panel, 
            ref int startIdx, ref int endIdx)
        {
            //Add and subtract CacheItemCount to StartIdx and EndIdx.
            int itemCount = panel.getItemCount();
            startIdx -= panel.CacheItemCount;
            endIdx += panel.CacheItemCount;
            if (startIdx < 0)
                startIdx = 0;
            if (endIdx > itemCount - 1)
                endIdx = itemCount - 1;
        }

    }
}
