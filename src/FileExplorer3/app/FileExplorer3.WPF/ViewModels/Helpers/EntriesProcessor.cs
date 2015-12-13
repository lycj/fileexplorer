using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FileExplorer.WPF.Models;

namespace FileExplorer.WPF.ViewModels.Helpers
{


    public class EntriesProcessor<VM> : IEntriesProcessor<VM>
    {
        #region Constructor

        public EntriesProcessor(IEntriesHelper<VM> entriesHelper,
            Func<VM, object> filterObjectGetter)
        {
            EntriesHelper = entriesHelper;
            _filterObjectGetter = filterObjectGetter;
        }

        #endregion

        #region Methods

        public void Sort(IComparer comparer, string groupDescription)
        {
            All.CustomSort = comparer;
            All.GroupDescriptions.Add(new PropertyGroupDescription(groupDescription));
        }


        void updateFilters()
        {
            //if (All.Filter == null)
            All.Filter = (e) => e != null &&
                CustomFilter(e) &&
                ColumnFilter.Match(_colFilters, _filterObjectGetter((VM)e));
            //else
            //    All.Filter = (e) =>
            //        All.Filter(e) && CustomFilter(e) &&
            //        ColumnFilter.Match(_colFilters, (e as IEntryViewModel).EntryModel);
        }

        public void SetFilters(params ColumnFilter[] filters)
        {
            _colFilters = filters;
            updateFilters();
        }



        #endregion

        #region Data

        ListCollectionView _processedItems = null;
        private ColumnFilter[] _colFilters = new ColumnFilter[] { };
        private Func<object, bool> _customFilter = e => true;
        private Func<VM, object> _filterObjectGetter;


        #endregion

        #region Public Properties

        public Func<object, bool> CustomFilter
        {
            get { return _customFilter; }
            set { _customFilter = value ?? (e => true); updateFilters(); }
        }

        public ListCollectionView All
        {
            get
            {
                if (_processedItems == null)
                    _processedItems = CollectionViewSource.GetDefaultView(EntriesHelper.All) as ListCollectionView;
                return _processedItems;
            }
        }
        public IEntriesHelper<VM> EntriesHelper { get; private set; }
        public IColumnsHelper ColumnHelper { get; private set; }


        #endregion



    }
}
