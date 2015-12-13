//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;

//namespace FileExplorer.WPF.ViewModels
//{
//    /// <summary>
//    /// Append entrylist context["EntryList"] to context.Target (IEntryListViewModel)
//    /// </summary>
//    public class UnselectAll : IResult
//    {
//        #region Cosntructor

//        public UnselectAll(IEnumerable<IEntryViewModel> entries)
//        {
//            _entries = entries;
//        }

//        #endregion

//        #region Methods

//        public event EventHandler<ResultCompletionEventArgs> Completed;

//        public void Execute(CoroutineExecutionContext context)
//        {
//            foreach (var e in _entries)
//                e.IsSelected = false;
//            Completed(this, new ResultCompletionEventArgs());
//        }

//        #endregion

//        #region Data

//        private IEnumerable<IEntryViewModel> _entries;

//        #endregion

//        #region Public Properties

//        #endregion
       
//    }
//}
