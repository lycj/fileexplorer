using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class NullSuggestSource : ISuggestSource
    {

        public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            return Task.Run<IList<object>>(() => new List<object>());
        }
    }
}
