using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public class TreeRootSelector<VM, T> : TreeSelector<VM,T>, ITreeRootSelector<VM, T>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        /// <param name="compareFunc"></param>
        /// <param name="rootLevel">Level of TreeItem to consider as root, root items should shown in expander 
        /// (e.g. in OverflowedAndRootItems) and have caption and expander hidden when the path is longer than it.</param>
        public TreeRootSelector(IEntriesHelper<VM> entryHelper)// int rootLevel = 0,
            //params Func<T, T, HierarchicalResult>[] compareFuncs)
            : base(entryHelper)
        {
            //_rootLevel = rootLevel;
            //_compareFuncs = compareFuncs;
            //Comparers = new [] { PathComparer.LocalDefault };
        }

        #endregion

        #region Methods

        public override void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
            ITreeSelector<VM, T> _prevSelector = _selectedSelector;
            T _prevSelectedValue = _selectedValue;
            _prevPath = path;

            _selectedSelector = path.Last();
            _selectedValue = path.Last().Value;
            if (_prevSelectedValue != null && !_prevSelectedValue.Equals(path.Last().Value))
            {
                _prevSelector.IsSelected = false;
            }
            NotifyOfPropertyChanged(() => SelectedValue);
            NotifyOfPropertyChanged(() => SelectedViewModel);
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);

            updateRootItems(path);
        }

        public override void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
        {
        }


        private async Task updateRootItemsAsync(ITreeSelector<VM, T> selector, ObservableCollection<VM> rootItems, int level)
        {
            if (level == 0)
                return;

            List<ITreeSelector<VM, T>> rootTreeSelectors = new List<ITreeSelector<VM, T>>();
            await selector.LookupAsync(default(T), BroadcastNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
                new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
                    {                        
                        rootTreeSelectors.Add(c);                                                
                        return true;
                    }));

            foreach (var c in rootTreeSelectors)
            {
                rootItems.Add(c.ViewModel);
                c.IsRoot = true;
                await updateRootItemsAsync(c, rootItems, level - 1);
            }
        }

        private void updateRootItems(Stack<ITreeSelector<VM, T>> path = null)
        {
            if (_rootItems == null)
                _rootItems = new ObservableCollection<VM>();
            else _rootItems.Clear();
            if (path != null && path.Count() > 0)
            {
                foreach (var p in path.Reverse())
                {                    
                    if (!(this.EntryHelper.AllNonBindable.Contains(p.ViewModel)))
                        _rootItems.Add(p.ViewModel);
                }
                _rootItems.Add(default(VM)); //Separator
            }

            updateRootItemsAsync(this, _rootItems, 2);
        }
     
        public async Task SelectAsync(T value)
        {
            if (_selectedValue == null || CompareHierarchy(_selectedValue, value) != HierarchicalResult.Current)
            {
                await LookupAsync(value, RecrusiveSearch<VM, T>.LoadSubentriesIfNotLoaded,
                    SetSelected<VM, T>.WhenSelected, SetChildSelected<VM, T>.ToSelectedChild);                
            }
        }

        public HierarchicalResult CompareHierarchy(T value1, T value2)
        {
            foreach (var c in Comparers)
            {
                var retVal = c.CompareHierarchy(value1, value2);
                if (retVal != HierarchicalResult.Unrelated)
                    return retVal;
            }
            return HierarchicalResult.Unrelated;
        }

        #endregion

        #region Data

        T _selectedValue = default(T);
        ITreeSelector<VM, T> _selectedSelector;
        Stack<ITreeSelector<VM, T>> _prevPath = null;
        private IEnumerable<ICompareHierarchy<T>> _comparers;
        private ObservableCollection<VM> _rootItems = null;

        #endregion

        #region Public Properties

        public event EventHandler SelectionChanged;
        //private int _rootLevel;

        public ObservableCollection<VM> OverflowedAndRootItems
        {
            get { if (_rootItems == null) updateRootItems(); return _rootItems; }
            set { _rootItems = value; NotifyOfPropertyChanged(() => OverflowedAndRootItems); }
        }

        public ITreeSelector<VM, T> SelectedSelector
        {
            get { return _selectedSelector; }
        }

        public VM SelectedViewModel
        {
            get { return (SelectedSelector == null ? default(VM) : SelectedSelector.ViewModel); }
        }

        public T SelectedValue
        {
            get { return _selectedValue; }
            set { SelectAsync(value); }
        }


        //public int RootLevel { get { return _rootLevel; } set { _rootLevel = value; } }

        public IEnumerable<ICompareHierarchy<T>> Comparers { get { return _comparers; } set { _comparers = value; } }

        #endregion





  
    }
}
