//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;
//using FileExplorer.WPF.Models;

//namespace FileExplorer.WPF.ViewModels.Actions
//{
//    /// <summary>
//    /// Append entrylist context["EntryList"] to context.Target (IEntryListViewModel)
//    /// </summary>
//    public class AppendEntryList : IResult
//    {
//        #region Cosntructor

//        public AppendEntryList(IEntryViewModel parentModel, IFileListViewModel targetModel)
//        {
//            _parentModel = parentModel;
//            _targetModel = targetModel;
//        }

//        #endregion

//        #region Methods

//        public event EventHandler<ResultCompletionEventArgs> Completed;

//        public void Execute(ActionExecutionContext context)
//        {
//            var entryModels = context["EntryList"] as IEnumerable<IEntryModel>;
//            foreach (var em in entryModels)
//            {
//                var evm = EntryViewModel.FromEntryModel(em);                
//                _targetModel.Items.Add(evm);
//            }
//            Completed(this, new ResultCompletionEventArgs());
//        }

//        #endregion

//        #region Data

//        private IEntryViewModel _parentModel;
//        private IFileListViewModel _targetModel;

//        #endregion

//        #region Public Properties

//        #endregion
       
//    }
//}
