//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;
//using FileExplorer.Models;

//namespace FileExplorer.WPF.ViewModels.Actions
//{

//    /// <summary>
//    /// Lookup context[EntryViewModelList] and use matchFunc, 
//    /// and return first matched item to context[MatchedItem]
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class FindMatched<T> : IResult
//    { 
//        #region Cosntructor

//        public FindMatched(IEntryModel model, IObservableCollection<T> lookupList, Func<T, IEntryModel, bool> matchFunc)
//        {
//            _model = model;            
//            _matchFunc = matchFunc;
//            _lookupList = lookupList;
//        }
        
//        #endregion

//        #region Methods

      
//        public void Execute(ActionExecutionContext context)
//        {            
//            foreach(var evm in _lookupList)
//                if (_matchFunc(evm, _model))
//                {
//                    context["MatchedItem"] = evm;
//                    Completed(this, new ResultCompletionEventArgs());
//                    return;
//                }
//            Completed(this, new ResultCompletionEventArgs() 
//            { Error = new Exception("FindMatched: No matched item found.") });
//        }
        
//        #endregion

//        #region Data
//        IEntryModel _model;
//        private Func<T, IEntryModel, bool> _matchFunc;
//        private IObservableCollection<T> _lookupList;
        
//        #endregion

//        #region Public Properties
        
//        public event EventHandler<ResultCompletionEventArgs> Completed;
        
//        #endregion
        
//    }
//}
