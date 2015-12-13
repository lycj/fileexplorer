using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using QuickZip.UserControls;
using QuickZip.UserControls.MVVM.Model;
using System.IO;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Windows.Data;

namespace QuickZip.UserControls.MVVM
{

    public static class EntryComparerHelper
    {
        public static void ChangeSortMethod<FI, DI, FSI>(
            this CollectionViewSource collection,
            SortCriteria sortBy, System.ComponentModel.ListSortDirection sortDirection)
            where FI : FSI
            where DI : FSI
        {

            System.Windows.Data.ListCollectionView dataView =
                (System.Windows.Data.ListCollectionView)
                (System.Windows.Data.CollectionViewSource.GetDefaultView(collection.View));

            dataView.SortDescriptions.Clear();
            dataView.CustomSort = null;

            SortDirectionType direction = sortDirection == System.ComponentModel.ListSortDirection.Ascending ?
               SortDirectionType.sortAssending : SortDirectionType.sortDescending;

            dataView.CustomSort = new EntryComparer<FI, DI, FSI>(sortBy, direction) { IsFolderFirst = true };

        }

        public static void ChangeSortMethod<FI, DI, FSI>(
            this System.Collections.ObjectModel.ObservableCollection<EntryViewModel<FI, DI, FSI>> collection,
            SortCriteria sortBy, System.ComponentModel.ListSortDirection sortDirection)
            where FI : FSI
            where DI : FSI
        {

            System.Windows.Data.ListCollectionView dataView =
                (System.Windows.Data.ListCollectionView)
                (System.Windows.Data.CollectionViewSource.GetDefaultView(collection));

            dataView.SortDescriptions.Clear();
            dataView.CustomSort = null;

            SortDirectionType direction = sortDirection == System.ComponentModel.ListSortDirection.Ascending ?
               SortDirectionType.sortAssending : SortDirectionType.sortDescending;

            dataView.CustomSort = new EntryComparer<FI, DI, FSI>(sortBy, direction) { IsFolderFirst = true };

        }

        public static void ChangeSortMethod<FI, DI, FSI>(
            this System.Collections.ObjectModel.ObservableCollection<NavigationItemViewModel<FI, DI, FSI>> collection,
            SortCriteria sortBy, System.ComponentModel.ListSortDirection sortDirection)
            where FI : FSI
            where DI : FSI
        {

            System.Windows.Data.ListCollectionView dataView =
                (System.Windows.Data.ListCollectionView)
                (System.Windows.Data.CollectionViewSource.GetDefaultView(collection));

            dataView.SortDescriptions.Clear();
            dataView.CustomSort = null;

            SortDirectionType direction = sortDirection == System.ComponentModel.ListSortDirection.Ascending ?
               SortDirectionType.sortAssending : SortDirectionType.sortDescending;

            dataView.CustomSort = new EntryComparer<FI, DI, FSI>(sortBy, direction) { IsFolderFirst = true };

        }
    }

    public class EntryComparer<FI, DI, FSI> : IComparer<EntryViewModel<FI, DI, FSI>>,
        IComparer<EntryModel<FI, DI, FSI>>, IComparer<NavigationItemViewModel<FI, DI, FSI>>,
        IComparer
        where FI : FSI
        where DI : FSI
    {


        public SortCriteria SortBy = SortCriteria.sortByName;
        public SortDirectionType SortDirection = SortDirectionType.sortAssending;
        public bool IsFolderFirst = true;
        private int sortDirectionModifier { get { return SortDirection == SortDirectionType.sortAssending ? -1 : 1; } }

        static NaturalSorting.NaturalComparer nc = new NaturalSorting.NaturalComparer();
        public static int CompareStringNatural(string str1, string str2)
        {
            return nc.Compare(str1, str2);
        }


        public EntryComparer(SortCriteria criteria)
        {
            SortBy = criteria;
        }

        public EntryComparer(SortCriteria criteria, SortDirectionType direction)
        {
            SortBy = criteria;
            SortDirection = direction;
        }

        #region IComparer<IFileSystemInfoExA> Members

        private static bool isFolder(EntryModel<FI, DI, FSI> entry)
        {
            return entry is DirectoryModel<FI, DI, FSI>;
        }

        public int Compare(EntryModel<FI, DI, FSI> x, EntryModel<FI, DI, FSI> y)
        {

            if (SortBy == SortCriteria.sortByName &&
                x.CustomPosition != -1 && y.CustomPosition != -1)
            {
                return -x.CustomPosition.CompareTo(y.CustomPosition);
            }
            //if ((x.COFEAttributes & COFEAttributes.CustomSortMethod) != 0 && 
            //    x is IComparable<IFileSystemInfoExA>)
            //    return (x as IComparable<IFileSystemInfoExA>).CompareTo(y);
            //if ((y.COFEAttributes & COFEAttributes.CustomSortMethod) != 0 && 
            //    y is IComparable<IFileSystemInfoExA>)
            //    return -1 * (y as IComparable<IFileSystemInfoExA>).CompareTo(x);

            int retVal = 0;
            if (IsFolderFirst && (isFolder(x) != isFolder(y)))
                retVal = isFolder(x).CompareTo(isFolder(y)) * sortDirectionModifier;
            //else if ((SpecialPriorityList.Contains(x.ParseName) || SpecialPriorityList.Contains(y.ParseName)) &&
            //    (SortBy == SortCriteria.sortByFullName || SortBy == SortCriteria.sortByLabel || SortBy == SortCriteria.sortByName))
            //{
            //    int xIdx = SpecialPriorityList.IndexOf(x.ParseName);
            //    int yIdx = SpecialPriorityList.IndexOf(y.ParseName);
            //    retVal = xIdx.CompareTo(yIdx) * -1;
            //}
            else
            {
                switch (SortBy)
                {
                    case SortCriteria.sortByType: retVal = CompareStringNatural(PathEx.GetExtension(x.Name).ToLower(),
                        PathEx.GetExtension(y.Name).ToLower()); break;
                    case SortCriteria.sortByName: retVal = CompareStringNatural(x.Name, y.Name); break;
                    case SortCriteria.sortByFullName: retVal = CompareStringNatural(x.ParseName, y.ParseName); break;
                    case SortCriteria.sortByLabel: retVal = CompareStringNatural(x.Label, y.Label); break;
                    case SortCriteria.sortByLength:
                        long xSize = x is FileInfoEx ? (x as FileInfoEx).Length : 0;
                        long ySize = y is FileInfoEx ? (y as FileInfoEx).Length : 0;
                        retVal = xSize.CompareTo(ySize); break;
                    case SortCriteria.sortByCreationTime:
                        retVal = x.CreationTime.CompareTo(y.CreationTime); break;
                    case SortCriteria.sortByLastWriteTime:
                        retVal = x.LastWriteTime.CompareTo(y.LastWriteTime); break;
                    case SortCriteria.sortByLastAccessTime:
                        retVal = x.LastAccessTime.CompareTo(y.LastAccessTime); break;
                }
            }
            return SortDirection == SortDirectionType.sortAssending ? retVal : -retVal;
        }

        #endregion


        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x is EntryViewModel<FI, DI, FSI> && y is EntryViewModel<FI, DI, FSI>)
                return Compare(x as EntryViewModel<FI, DI, FSI>, y as EntryViewModel<FI, DI, FSI>);
            if (x is EntryModel<FI, DI, FSI> && y is EntryModel<FI, DI, FSI>)
                return Compare(x as EntryModel<FI, DI, FSI>, y as EntryModel<FI, DI, FSI>);
            if (x is NavigationItemViewModel<FI, DI, FSI> && y is NavigationItemViewModel<FI, DI, FSI>)
                return Compare(x as NavigationItemViewModel<FI, DI, FSI>, y as NavigationItemViewModel<FI, DI, FSI>);
            return 0;
        }

        #endregion

        #region IComparer<EntryViewModel<FI,DI,FSI>> Members

        public int Compare(EntryViewModel<FI, DI, FSI> x, EntryViewModel<FI, DI, FSI> y)
        {
            if (x.CustomPosition != -1 || y.CustomPosition != -1)
                return x.CustomPosition.CompareTo(y.CustomPosition) * sortDirectionModifier;

            return Compare(x.EmbeddedModel, y.EmbeddedModel);
        }

        #endregion

        #region IComparer<NavigationItemViewModel<FI,DI,FSI>> Members

        public int Compare(NavigationItemViewModel<FI, DI, FSI> x, NavigationItemViewModel<FI, DI, FSI> y)
        {
            if (x.CustomPosition != -1 || y.CustomPosition != -1)
                return x.CustomPosition.CompareTo(y.CustomPosition) * sortDirectionModifier;
            
            if (x.EmbeddedEntryModel != null && y.EmbeddedEntryModel != null)
                return Compare(x.EmbeddedEntryModel, y.EmbeddedEntryModel);

            return -1;
        }

        #endregion
    }
}
