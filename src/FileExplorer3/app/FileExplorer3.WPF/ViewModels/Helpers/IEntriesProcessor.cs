using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FileExplorer.WPF.Models;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public interface IEntriesProcessor
    {
        void Sort(IComparer comparer, string groupDescription);
        void SetFilters(params ColumnFilter[] filters);
        
        /// <summary>
        /// Specify additional filter 
        /// </summary>
        Func<object, bool> CustomFilter { get; set; }

        ListCollectionView All { get; }
    }

    public interface IEntriesProcessor<VM> : IEntriesProcessor
    {
        IEntriesHelper<VM> EntriesHelper { get; }
        
    }
}
