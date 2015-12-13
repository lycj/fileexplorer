using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public class ColumnFilter : PropertyChangedBase, IColumnFilter
    {

        #region Cosntructor
        public static ColumnFilter CreateNew(string header, string valuePath, 
            Func<object, bool> filter = null)
        {
            return new ColumnFilter()
            {
                Header = header,
                ValuePath = valuePath,
                Matches = filter == null ? (e) => true : filter
            };
        }

        public static ColumnFilter CreateNew<T>(string header, string valuePath,
           Func<T, bool> filter = null)
        {
            var retVal = new ColumnFilter()
            {
                Header = header,
                ValuePath = valuePath,
            };

            if (filter == null)
                retVal.Matches =  e => true;
            else retVal.Matches = e => filter((T)e);

            return retVal;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Many any filter in a filter group.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Match(IColumnFilter[] filters, object model)
        {
            foreach (var filterGroup in filters.GroupBy(f => f.ValuePath))
            {
                bool match = false;
                foreach (var f in filterGroup)
                    if (f.Matches(model))
                        match = true;
                if (!match) return false;
            }
            return true;
        }

        public static bool MatchAll(IColumnFilter[] filters, object model)
        {
            foreach (var filterGroup in filters.GroupBy(f => f.ValuePath))
            {
                foreach (var f in filterGroup)
                    if (!f.Matches(model))
                        return false;
            }
            return true;
        }

        #endregion

        #region Data

        private string _header;
        private bool _isChecked = false;
        private string _valuePath = "";
        private Func<object, bool> _match;
        private long _matchCount = 0;

        #endregion

        #region Public Properties

        public string Header
        {
            get { return _header; }
            set { _header = value; NotifyOfPropertyChange(() => Header); }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); }
        }

        public string ValuePath
        {
            get { return _valuePath; }
            set { _valuePath = value; NotifyOfPropertyChange(() => ValuePath); }
        }

        public Func<object, bool> Matches
        {
            get { return _match; }
            set { _match = value; NotifyOfPropertyChange(() => Matches); }
        }

        public long MatchedCount
        {
            get { return _matchCount; }
            set { _matchCount = value; NotifyOfPropertyChange(() => MatchedCount); }
        }

        #endregion





       
    }
}
