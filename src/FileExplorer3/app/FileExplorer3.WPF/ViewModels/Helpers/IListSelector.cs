using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public interface IReportSelected<VM>
    {
        void ReportChildSelected(VM viewModel);
        void ReportChildUnSelected(VM viewModel);
    }

    public interface IListSelector<VM, T> : IReportSelected<VM>, INotifyPropertyChanged, IExportCommandBindings
        where VM : IViewModelOf<T>
    {
        #region Constructor

        #endregion

        #region Methods


        ///// <summary>
        ///// Called by FileListViewModel to notify selectedItems is changed.
        ///// </summary>
        ///// <param name="selectedItems"></param>
        //void OnSelectionChanged(IList selectedItems);

        void SelectAll();
        void UnselectAll();

        void Select(Func<VM, bool> querySelectFunc);


        #endregion

        #region Data

        #endregion

        #region Public Properties
        event EventHandler SelectionChanged;

        IList<VM> SelectedItems { get; }
        
        VM[] SelectedViewModels { get; set; }
        T[] SelectedModels { get; set; }

        #endregion
        
    }
}
