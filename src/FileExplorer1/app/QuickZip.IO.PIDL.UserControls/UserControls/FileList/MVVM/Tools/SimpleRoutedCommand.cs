using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Cinch;
using System.Diagnostics;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class SimpleRoutedCommand : SimpleCommand, ICommand
    {
        public SimpleRoutedCommand()
        {
            _routedCommand = new RoutedUICommand();
            init();
        }

        public SimpleRoutedCommand(string text)
        {
            RoutedUICommand routedCommand = new RoutedUICommand();
            routedCommand.Text = text;            
            _routedCommand = routedCommand;
            init();
        }

        public SimpleRoutedCommand(RoutedUICommand routedCommand)
        {
            _routedCommand = routedCommand;
            init();
        }

        public SimpleRoutedCommand(RoutedUICommand routedCommand, SimpleCommand simpleCommand)
        {
            _routedCommand = routedCommand;
            _embeddedCommand = simpleCommand;
            ExecuteDelegate = x =>
            {
                _embeddedCommand.Execute(x);
                this.CommandSucceeded = simpleCommand.CommandSucceeded;
            };

            CanExecuteDelegate = x =>
            {
                return _embeddedCommand.CanExecute(x);
            };            

            init();
        }

        private void init()
        {
            _commandBinding = new CommandBinding(_routedCommand,
               (ExecutedRoutedEventHandler)delegate(object sender, ExecutedRoutedEventArgs e)
               {
                   this.ExecuteDelegate(e.Parameter);
                   e.Handled = true;
               },
               (CanExecuteRoutedEventHandler)delegate(object sender, CanExecuteRoutedEventArgs e)
               {
                   e.CanExecute = this.CanExecuteDelegate(e.Parameter);
               });
        }

        public override string ToString()
        {
            return _routedCommand.ToString();
        }

        #region Data
        private CommandBinding _commandBinding;
        private RoutedCommand _routedCommand;
        private SimpleCommand _embeddedCommand = null;
        #endregion
        #region Public Properties        
        public CommandBinding CommandBinding { get { return _commandBinding; } }
        #endregion

        #region Static Methods
        //http://wekempf.spaces.live.com/blog/cns!D18C3EC06EA971CF!255.entry?wa=wsignin1.0
        public static void Register(Type hostType, SimpleRoutedCommand simpleRoutedCommand, InputGesture inputGesture)
        {
            CommandManager.RegisterClassCommandBinding(hostType, simpleRoutedCommand.CommandBinding);
            CommandManager.RegisterClassInputBinding(hostType, new InputBinding(simpleRoutedCommand, inputGesture));
            simpleRoutedCommand._routedCommand.InputGestures.Add(inputGesture);
        }

        public static void Register(Type hostType, SimpleRoutedCommand simpleRoutedCommand)
        {
            CommandManager.RegisterClassCommandBinding(hostType, simpleRoutedCommand.CommandBinding);
        }
        #endregion
    }
}
