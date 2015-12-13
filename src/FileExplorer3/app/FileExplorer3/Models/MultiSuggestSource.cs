using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;

namespace FileExplorer.Models
{   

 
    public class MultiSuggestSource : ISuggestSource
    {
        private ISuggestSource[] _suggestSources;
        public MultiSuggestSource(params ISuggestSource[] suggestSources)
        {
            _suggestSources = suggestSources;
        }

        public MultiSuggestSource(ISuggestSource source1, params ISuggestSource[] moreSources)
        {
            _suggestSources = (new ISuggestSource[] { source1 }).Concat(moreSources).ToArray();
        }

        public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            List<object> retVal = new List<object>();
            foreach (var ss in _suggestSources)
                retVal.AddRange(await ss.SuggestAsync(data, input, helper));
            return retVal;
        }
    }
}
