using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Models;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public class EntryViewModelComparer : IComparer<IEntryViewModel>, IComparer
    {
        IComparer<IEntryModel> _modelComparer;
        int  _modifier;
        public EntryViewModelComparer(IComparer<IEntryModel> modelComparer, ListSortDirection sortDirection)
        {
            _modelComparer = modelComparer;
            _modifier = sortDirection == ListSortDirection.Ascending ? 1 : -1;
        }

        public int Compare(IEntryViewModel x, IEntryViewModel y)
        {
            return _modelComparer.Compare(x.EntryModel, y.EntryModel);
        }

        public int Compare(object x, object y)
        {            
            if (x is IEntryViewModel && y is IEntryViewModel)
                return _modifier * Compare(x as IEntryViewModel, y as IEntryViewModel);
            return -1;
        }
    }
}
