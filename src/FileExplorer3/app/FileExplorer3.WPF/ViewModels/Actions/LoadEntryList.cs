//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;
//using FileExplorer.Models;

//namespace FileExplorer.WPF.ViewModels
//{
//    /// <summary>
//    /// Load a directory content to context["EntryList"], with format IEnumerable&lt;IEntryModel&gt;
//    /// </summary>
//    public class LoadEntryList : IResult
//    {
//        #region Cosntructor

//        public LoadEntryList(IEntryViewModel parentModel, Func<IEntryModel, bool> filter = null)
//        {
//            _parentModel = parentModel;
//            _filter = filter ?? (m => true);
//        }

//        #endregion

//        #region Methods

//        public event EventHandler<ResultCompletionEventArgs> Completed;

//        public void Execute(CoroutineExecutionContext context)
//        {
//            _parentModel.EntryModel.Profile.ListAsync(_parentModel.EntryModel, CancellationToken.None,  _filter).ContinueWithCheck(
//                (prev) =>
//                {
//                    if (prev.IsFaulted)
//                        Completed(this, new ResultCompletionEventArgs { Error = prev.Exception, WasCancelled = prev.IsCanceled });
//                    else
//                    {
//                        var entryModels = from m in prev.Result
//                                          where _filter(m)
//                                          select (IEntryModel)m;
//                        context["EntryList"] = entryModels;
//                        Completed(this, new ResultCompletionEventArgs());
//                    }
//                });

//            //if (listTask.Status != TaskStatus.Running)
//            //    listTask.ContinueWith((prev) =>
//            //        {
//            //            if (prev.IsFaulted)
//            //                Completed(this, new ResultCompletionEventArgs { Error = prev.Exception, WasCancelled = prev.IsCanceled });
//            //            else
//            //            {
//            //                var entryModels = from m in prev.Result
//            //                                  select (IEntryModel)m;
//            //                context["EntryList"] = entryModels;
//            //                Completed(this, new ResultCompletionEventArgs());
//            //            }
//            //        });
//        }

//        #endregion

//        #region Data

//        private IEntryViewModel _parentModel;
//        private Func<IEntryModel, bool> _filter;

//        #endregion

//        #region Public Properties

//        #endregion

//    }
//}
