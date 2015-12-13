//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Caliburn.Micro;

//namespace FileExplorer.WPF.ViewModels.Actions
//{
//    public class DoSomething : IResult
//    {
//        #region Cosntructor

//        public DoSomething(System.Action<CoroutineExecutionContext> action)
//        {
//            _action = action;
//        }

//        #endregion

//        #region Methods

//        public event EventHandler<ResultCompletionEventArgs> Completed;

      

//        public void Execute(CoroutineExecutionContext context)
//        {
//            try
//            {
//                _action(context);
//                Completed(this, new ResultCompletionEventArgs());
//            }
//            catch (Exception ex)
//            {
//                Completed(this, new ResultCompletionEventArgs() { Error = ex });
//            }
//        }

//        #endregion

//        #region Data

//        private System.Action<CoroutineExecutionContext> _action;

//        #endregion

//        #region Public Properties

//        #endregion


        
//    }
//}
