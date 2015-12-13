using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.WPF.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _handler;
        private readonly Func<object, bool> _canHandle;
        private bool _isEnabled = true;

        public RelayCommand(Action<object> handler, Func<object, bool> canHandle = null)
        {
            _handler = handler;
            _canHandle = canHandle ?? (pm => true);
        }



        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled && _canHandle(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }
}
