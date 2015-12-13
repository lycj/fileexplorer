using System;
using System.Collections.Generic;
using System.Text;
using NaturalSorting;
using System.IO;
using System.Collections;
using QuickZip.IO.PIDL.UserControls.Model;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls
{
    //0.2
    public class ExModelComparer : IComparer<ExModel>, IComparer<ExViewModel>, IComparer
    {
        public ExComparer.SortCriteria SortBy = ExComparer.SortCriteria.sortByName;
        public ExComparer.SortDirectionType SortDirection = ExComparer.SortDirectionType.sortAssending;
        public bool IsFolderFirst = true;

        private static NaturalComparer nc = new NaturalComparer();

        public ExModelComparer(ExComparer.SortCriteria criteria)
        {
            SortBy = criteria;
        }

        public ExModelComparer(ExComparer.SortCriteria criteria, ExComparer.SortDirectionType direction)
        {
            SortBy = criteria;
            SortDirection = direction;
        }

        #region IComparer<ExModel> Members

        public int Compare(ExModel x, ExModel y)
        {
            int retVal = 0;
            if (IsFolderFirst && (x is DirectoryModel) != (y is DirectoryModel))
                retVal = (x is DirectoryModel).CompareTo((y is DirectoryModel)) * -1;
            else
            {
                switch (SortBy)
                {
                    case ExComparer.SortCriteria.sortByName: retVal = nc.Compare(x.Name, y.Name); break;
                    //case ExComparer.SortCriteria.sortByType: retVal = nc.Compare(PathEx.GetExtension(x.Name).ToLower(),
                    //        PathEx.GetExtension(y.Name).ToLower()); break;
                    case ExComparer.SortCriteria.sortByFullName: retVal = nc.Compare(x.FullName, y.FullName); break;
                    case ExComparer.SortCriteria.sortByLabel: retVal = nc.Compare(x.Label, y.Label); break;
                    case ExComparer.SortCriteria.sortByLength:
                        long xSize = x is FileModel ? (x as FileModel).Length : 0;
                        long ySize = y is FileModel ? (y as FileModel).Length : 0;
                        retVal = xSize.CompareTo(ySize); break;
                    case ExComparer.SortCriteria.sortByCreationTime:
                        retVal = x.CreationTime.CompareTo(y.CreationTime); break;
                    case ExComparer.SortCriteria.sortByLastWriteTime:
                        retVal = x.LastWriteTime.CompareTo(y.LastWriteTime); break;
                    case ExComparer.SortCriteria.sortByLastAccessTime:
                        retVal = x.LastAccessTime.CompareTo(y.LastAccessTime); break;
                }
            }
            return SortDirection == ExComparer.SortDirectionType.sortAssending ? retVal : -retVal;
        }

        #endregion

        #region IComparer<ExViewModel> Members

        public int Compare(ExViewModel x, ExViewModel y)
        {
            return Compare(x.EmbeddedModel, y.EmbeddedModel);
        }

        #endregion

        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x is ExModel && y is ExModel)
                return Compare(x as ExModel, y as ExModel);
            else if (x is ExViewModel && y is ExViewModel)
                return Compare(x as ExViewModel, y as ExViewModel);
            return 0;
        }
        #endregion



        
    }
}
