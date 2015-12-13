//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;
//using FileExplorer.Models;

//namespace FileExplorer.WPF.ViewModels
//{
//    public class CalculateColumnHeaderCount : IResult
//    {
//        #region Cosntructor

//        public CalculateColumnHeaderCount(ColumnFilter[] filters)
//        {
//            _filters = filters;
//        }

//        #endregion

//        #region Methods

//        public void Execute(ActionExecutionContext context)
//        {
//            Dictionary<ColumnFilter, int> matchCountDic = new Dictionary<ColumnFilter, int>();
//            foreach (var f in _filters)
//                matchCountDic[f] = 0;

//            var entryModels = context["EntryList"] as IEnumerable<IEntryModel>;
//            if (entryModels != null)
//                foreach (var em in entryModels)
//                    foreach (var f in matchCountDic.Keys)
//                        if (f.Matches(em))
//                            matchCountDic[f]++;

//            foreach (var f in matchCountDic.Keys)
//                f.MatchedCount = matchCountDic[f];
//        }

//        #endregion

//        #region Data

//        private ColumnFilter[] _filters;

//        #endregion

//        #region Public Properties

//        public event EventHandler<ResultCompletionEventArgs> Completed;

//        #endregion


//    }
//}
