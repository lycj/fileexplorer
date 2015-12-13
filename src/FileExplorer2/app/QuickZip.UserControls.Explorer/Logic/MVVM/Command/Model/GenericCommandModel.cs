using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Windows.Input;
using QuickZip.UserControls.MVVM.Command.ViewModel;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    public class GenericCommandModel : CommandModel
    {
        #region Constructor


        public GenericCommandModel(string header, ICommand command, object param = null)
        {
            IsExecutable = true;
            Header = header;
            _command = command; 
            _param = param;
            
        }

        public GenericCommandModel(string header, Action<object> executeDelegate, Predicate<object> canExecuteDelegate = null,
            object param = null)
        {
            IsExecutable = true;
            Header = header;
            _command = new SimpleCommand()
                {
                    ExecuteDelegate = executeDelegate, 
                    CanExecuteDelegate = canExecuteDelegate
                };
            _param = param;

        }

        public GenericCommandModel(RoutedUICommand uicommand, object param = null)
            : this(uicommand.Text, uicommand, param)
        {
            IsExecutable = true;
        }

        #endregion

        #region Methods        

        public override bool CanExecute(object param)
        {
            if (_command != null)
                return _command.CanExecute(_param != null ? _param : param);
            else
                return false;
        }

        public override void Execute(object param)
        {
            if (CanExecute(param) && _command != null)
                _command.Execute(_param != null ? _param : param);
        }

        #endregion

        #region Data

    
        private ICommand _command = null;
        private object _param = null;
        

        #endregion

        #region Public Properties

        public RoutedUICommand UICommand { get { return _command as RoutedUICommand; } }
        public ICommand Command { get { return _command; } }
        public object CommandParameter { get { return _param; } }

       
        #endregion
    }
}
