using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.UserControls
{
    public enum SortCriteria
    {
        sortByName, sortByFullName, sortByLabel, sortByType, sortByLength,
        sortByCreationTime, sortByLastWriteTime, sortByLastAccessTime
    };
    public enum SortDirectionType { sortAssending, sortDescending };

}
