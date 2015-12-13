using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class PathHierarchyComparer<T> : IEntryHierarchyComparer
        where T : IEntryModel
    {        
        private StringComparison _comparsion;
        private char _separator;

        public PathHierarchyComparer(StringComparison comparsion, char separator = '\\')
        {            
            _comparsion = comparsion;
            _separator = separator;
        }

        public HierarchicalResult CompareHierarchy(IEntryModel value1, IEntryModel value2)
        {
            if (value1 is T && value2 is T)
            {
                if (value1 == null || value2 == null)
                    return HierarchicalResult.Unrelated;

                if (value1.FullPath.Equals(value2.FullPath, _comparsion))
                    return HierarchicalResult.Current;

                if (value1.FullPath.StartsWith(value2.FullPath + _separator, _comparsion))
                    return HierarchicalResult.Parent;

                if (value2.FullPath.StartsWith(value1.FullPath + _separator, _comparsion))
                    return HierarchicalResult.Child;
            }
            return HierarchicalResult.Unrelated;
        }
    }
}
