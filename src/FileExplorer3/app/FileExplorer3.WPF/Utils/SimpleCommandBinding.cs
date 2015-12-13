using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cinch;

namespace FileExplorer.WPF.BaseControls
{
    public class SimpleCommandBinding : CommandBinding
    {
        public SimpleCommandBinding()
        {
            CanExecute += (CanExecuteRoutedEventHandler)((o, e) =>
                {
                    e.CanExecute = SimpleCommand != null && SimpleCommand.CanExecute(e.Parameter);
                });
            Executed += (ExecutedRoutedEventHandler)((o, e) =>
            {
                if (SimpleCommand != null)
                    SimpleCommand.Execute(e.Parameter);
            });
        }
        
        private ICommand _simpleCommand = null;

        public ICommand SimpleCommand { get { return _simpleCommand; } set { _simpleCommand = value; } }

    }
}
