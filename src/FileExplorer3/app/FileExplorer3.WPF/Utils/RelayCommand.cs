//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;

//namespace FileExplorer.WPF.Utils
//{
//    //http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090051
//    public class RelayCommand : ICommand
//    {
//        #region Fields

//        readonly Action<object> _execute;
//        readonly Predicate<object> _canExecute;

//        #endregion // Fields

//        #region Constructors

//        public RelayCommand(Action<object> execute)
//            : this(execute, null)
//        {
//        }

//        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
//        {
//            if (execute == null)
//                throw new ArgumentNullException("execute");

//            _execute = execute;
//            _canExecute = canExecute;
//        }
//        #endregion // Constructors

//        #region ICommand Members

//        [DebuggerStepThrough]
//        public bool CanExecute(object parameter)
//        {
//            return _canExecute == null ? true : _canExecute(parameter);
//        }

//        public event EventHandler CanExecuteChanged
//        {
//            add { CommandManager.RequerySuggested += value; }
//            remove { CommandManager.RequerySuggested -= value; }
//        }

//        public void Execute(object parameter)
//        {
//            _execute(parameter);
//        }

//        #endregion // ICommand Members


//    }


//    //// To use this class within your viewmodel class:
//    //RelayCommand _myCommand;
//    //public ICommand MyCommand
//    //{
//    //get
//    //{
//    //if (_myCommand == null)
//    //{
//    //_myCommand = new RelayCommand(p => this.DoMyCommand(p),
//    //p => this.CanDoMyCommand(p) );
//    //}
//    //return _myCommand;
//    //}
//    //}

//    /// <summary>
//    /// A command whose sole purpose is to 
//    /// relay its functionality to other
//    /// objects by invoking delegates. The
//    /// default return value for the CanExecute
//    /// method is 'true'.
//    /// 
//    /// Source: http://www.codeproject.com/Articles/31837/Creating-an-Internationalized-Wizard-in-WPF
//    /// </summary>
//    public class RelayCommand<T> : ICommand
//    {
//        #region Fields
//        private readonly Action<T> mExecute = null;
//        private readonly Predicate<T> mCanExecute = null;
//        #endregion // Fields

//        #region Constructors
//        /// <summary>
//        /// Class constructor
//        /// </summary>
//        /// <param name="execute"></param>
//        public RelayCommand(Action<T> execute)
//            : this(execute, null)
//        {
//        }

//        /// <summary>
//        /// Creates a new command.
//        /// </summary>
//        /// <param name="execute">The execution logic.</param>
//        /// <param name="canExecute">The execution status logic.</param>
//        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
//        {
//            if (execute == null)
//                throw new ArgumentNullException("execute");

//            this.mExecute = execute;
//            this.mCanExecute = canExecute;
//        }

//        #endregion // Constructors

//        #region events
//        /// <summary>
//        /// Eventhandler to re-evaluate whether this command can execute or not
//        /// </summary>
//        public event EventHandler CanExecuteChanged
//        {
//            add
//            {
//                if (this.mCanExecute != null)
//                    CommandManager.RequerySuggested += value;
//            }

//            remove
//            {
//                if (this.mCanExecute != null)
//                    CommandManager.RequerySuggested -= value;
//            }
//        }
//        #endregion

//        #region methods
//        /// <summary>
//        /// Determine whether this pre-requisites to execute this command are given or not.
//        /// </summary>
//        /// <param name="parameter"></param>
//        /// <returns></returns>
//        [DebuggerStepThrough]
//        public bool CanExecute(object parameter)
//        {
//            return this.mCanExecute == null ? true : this.mCanExecute((T)parameter);
//        }

//        /// <summary>
//        /// Execute the command method managed in this class.
//        /// </summary>
//        /// <param name="parameter"></param>
//        public void Execute(object parameter)
//        {
//            this.mExecute((T)parameter);
//        }
//        #endregion methods
//    }
//}
