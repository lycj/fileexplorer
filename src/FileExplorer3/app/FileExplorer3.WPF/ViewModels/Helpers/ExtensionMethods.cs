using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.WPF.ViewModels;

namespace FileExplorer.WPF
{
    public static partial class ExtensionMethods
    {
        public static ITreeRootSelector<VM, T> AsRoot<VM, T>(this ITreeSelector<VM, T> selector)
        {
            return selector as ITreeRootSelector<VM, T>;
        }

        /// <summary>
        /// Whether current directory is root directory
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static bool IsFirstLevelSelector<VM, T>(this ITreeSelector<VM, T> selector)
        {
            return selector.ParentSelector.Equals(selector.RootSelector);
        }
        /// <summary>
        /// Broadcast changes, so the tree can refresh changed items.
        /// </summary>
        public static async Task BroascastAsync<VM, T>(this ITreeRootSelector<VM, T> rootSelector, T changedItem)
        {
            await rootSelector.LookupAsync(changedItem,
                    RecrusiveSearch<VM, T>.SkipIfNotLoaded,
                    RefreshDirectory<VM, T>.WhenFound);
        }


     
    }
}
