using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels.Helpers
{
  
    public class ColumnsHelper : NotifyPropertyChanged, IColumnsHelper
    {
        #region Constructor

        public ColumnsHelper(IEntriesProcessor processor, Func<ColumnInfo, ListSortDirection, IComparer> getComparerFunc)
        {
            _processor = processor;
            _getComparerFunc = getComparerFunc;
        }

        #endregion

        #region Methods

        private void NotifyFilterChanged()
        {
            var checkedFilters = ColumnFilters.Where(f => f.IsChecked).ToArray();
            _processor.SetFilters(checkedFilters);

            if (FilterChanged != null)
                FilterChanged(this, EventArgs.Empty);
        }

        private void NotifySortChanged()
        {
            if (SortChanged != null) SortChanged(this, EventArgs.Empty);
            NotifyOfPropertyChanged(() => SortDirection);
            NotifyOfPropertyChanged(() => SortBy);

            var columnInfo = this.ColumnList.Find(_sortBy);
            if (columnInfo != null)
            {
                _processor.Sort(GetComparer(columnInfo, _sortDirection), _sortBy);
            }
        }

        public IComparer GetComparer(ColumnInfo colInfo, ListSortDirection direction)
        {
            return _getComparerFunc(colInfo, direction);
        }

        public void CalculateColumnHeaderCount(IEnumerable<IEntryModel> entryModels)
        {
            foreach (var f in _colFilters)
                f.MatchedCount = 0;

            foreach (var em in entryModels)
                foreach (var f in _colFilters)
                    if (f.Matches(em))
                        f.MatchedCount++;
            NotifyOfPropertyChanged(() => ColumnFilters);
        }

        public void SetColumnFilters(ColumnFilter[] filters)
        {
            _colFilters = filters;
            foreach (var f in filters)
                f.PropertyChanged += (o, e) =>
                    {
                        if (e.PropertyName == "IsChecked")
                            NotifyFilterChanged();
                    };
            NotifyOfPropertyChanged(() => ColumnFilters);
        }

        #endregion

        #region Data

        public event EventHandler FilterChanged;
        public event EventHandler SortChanged;

        private IEntriesProcessor _processor;
        private ColumnFilter[] _colFilters = new ColumnFilter[] { };
        private ColumnInfo[] _colList = new ColumnInfo[]
        {
            ColumnInfo.FromTemplate("Name", "GridLabelTemplate", "EntryModel.Label", null, 200),   
            ColumnInfo.FromBindings("Description", "EntryModel.Description", "", null, 200)   
        };
        private Func<ColumnInfo, ListSortDirection, IComparer> _getComparerFunc;
        private string _sortBy = "EntryModel.Label";
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        #endregion

        #region Public Properties

        public ColumnInfo[] ColumnList
        {
            get { return _colList; }
            set { _colList = value; NotifyOfPropertyChanged(() => ColumnList); }
        }

        public ColumnFilter[] ColumnFilters
        {
            get { return _colFilters; }
            set { SetColumnFilters(value); }
        }

        public string SortBy
        {
            get { return _sortBy; }
            set
            {
                if (_sortBy != value)
                {
                    _sortBy = value;
                    NotifySortChanged();
                }
            }
        }

        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                if (_sortDirection != value)
                {
                    _sortDirection = value;
                    NotifySortChanged();
                }
            }
        }



        #endregion
    }
}
