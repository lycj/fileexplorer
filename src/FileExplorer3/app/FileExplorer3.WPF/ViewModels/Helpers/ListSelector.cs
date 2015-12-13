using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cinch;
using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.UIEventHub;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public class ListSelector<VM, T> : NotifyPropertyChanged, IListSelector<VM, T>
        where VM : ISelectable, IViewModelOf<T>
    {
        #region Constructor

        public ListSelector(IEntriesHelper<VM> entryHelper)
        {
            EntryHelper = entryHelper;
            EntryHelper.EntriesChanged += (o, e) => { notifySelectionChanged(); };
                        
            ExportedCommandBindings = new List<IScriptCommandBinding>()
            {
                new ScriptCommandBinding(ExplorerCommands.InvertSelect, p => true, p => InvertSelect(), null, ScriptBindingScope.Explorer), 
                new ScriptCommandBinding(ExplorerCommands.UnselectAll, p => true, p => UnselectAll(), null, ScriptBindingScope.Explorer), 
                new ScriptCommandBinding(ApplicationCommands.SelectAll, p => true, p => SelectAll(), null, ScriptBindingScope.Explorer)
            };
        }

        #endregion

        #region Methods

        private void notifySelectionChanged()
        {
            NotifyOfPropertyChanged(() => SelectedItems);
            NotifyOfPropertyChanged(() => SelectedViewModels);
            NotifyOfPropertyChanged(() => SelectedModels);
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        public void InvertSelect()
        {
            foreach (var e in EntryHelper.AllNonBindable.ToList())
                e.IsSelected = !e.IsSelected;
            notifySelectionChanged();
        }

        public void UnselectAll()
        {
            foreach (var e in EntryHelper.AllNonBindable.ToList())
                e.IsSelected = false;
            notifySelectionChanged();
        }

        public void SelectAll()
        {
            if (SelectedItems.Count() == EntryHelper.AllNonBindable.Count())
                UnselectAll();
            else
            {
                foreach (var e in EntryHelper.AllNonBindable.ToList())
                    e.IsSelected = true;
                notifySelectionChanged();
            }
        }

        public void Select(Func<VM, bool> querySelectFunc)
        {
            foreach (var item in EntryHelper.AllNonBindable.ToArray())
                item.IsSelected = querySelectFunc(item);
            notifySelectionChanged();
        }

        public void ReportChildSelected(VM viewModel)
        {
            notifySelectionChanged();
        }

        public void ReportChildUnSelected(VM viewModel)
        {
            notifySelectionChanged();
        }

        public IEnumerable<VM> getSelectedItems()
        {
            if (EntryHelper.AllNonBindable != null)
                foreach (var item in EntryHelper.AllNonBindable.ToList())
                    if (item.IsSelected)
                        yield return item;
        }

        public void OnSelectionChanged(IList selectedItems)
        {
            //SelectedItems = selectedItems.Cast<VM>().Distinct().ToList();
        }


        #endregion

        #region Data

        IList<VM> _selectedVms;

        #endregion

        #region Public Properties

        public IEntriesHelper<VM> EntryHelper { get; private set; }

        [Obsolete]
        public IList<VM> SelectedItems
        {
            get { return getSelectedItems().ToList(); }            
        }

        public VM[] SelectedViewModels
        {
            get { return getSelectedItems().ToArray(); }
            set { this.Select(vm => value.Contains(vm)); }
        }

        public T[] SelectedModels
        {
            get { return SelectedViewModels.Select(vm => vm.Model).ToArray(); }
            set { if (value != SelectedModels) this.Select(vm => value.Contains(vm.Model)); }
        }

        public event EventHandler SelectionChanged;

        public IEnumerable<IScriptCommandBinding> ExportedCommandBindings { get; private set; }
        
        #endregion






        
    }
}
