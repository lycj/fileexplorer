using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace QuickZip.Logic
{
    /// <summary>
    /// Model for a command
    /// </summary>
    //http://wekempf.spaces.live.com/blog/cns!D18C3EC06EA971CF!255.entry?wa=wsignin1.0
    public class CommandModel
    {
        RoutedCommand command;
        ExecutedRoutedEventHandler executedHandler;
        CanExecuteRoutedEventHandler canExecuteHandler;

        //Choose not to specify executeHandler / canExecuteHandler
        public CommandModel()
        {
            command = new RoutedUICommand();
        }

        public CommandModel(ExecutedRoutedEventHandler executedHandler)
        {
            command = new RoutedCommand();
            this.executedHandler = executedHandler;
        }

        public CommandModel(ExecutedRoutedEventHandler executedHandler,
            CanExecuteRoutedEventHandler canExecuteHandler)
        {
            command = new RoutedCommand();
            this.executedHandler = executedHandler;
            this.canExecuteHandler = canExecuteHandler;
        }

        public CommandModel(RoutedCommand command, ExecutedRoutedEventHandler executedHandler,
            CanExecuteRoutedEventHandler canExecuteHandler)
        {
            this.command = command;
            this.executedHandler = executedHandler;
            this.canExecuteHandler = canExecuteHandler;
        }

        public RoutedCommand Command
        {
            get { return command; }
        }

        public CommandBinding CommandBinding
        {
            get { return new CommandBinding(Command, OnExecute, OnQueryEnabled); }
        }

        virtual public void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (canExecuteHandler != null)
            {
                canExecuteHandler(sender, e);
            }
            else
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        virtual public void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            executedHandler(sender, e);
        }

        public static void RegisterGesture(Type hostType, CommandModel commandModel, InputGesture inputGesture)
        {
            CommandManager.RegisterClassCommandBinding(hostType, commandModel.CommandBinding);
            CommandManager.RegisterClassInputBinding(hostType, new InputBinding(commandModel.Command, inputGesture));
        }

        public static void Register(Type hostType, CommandModel commandModel)
        {
            CommandManager.RegisterClassCommandBinding(hostType, commandModel.CommandBinding);
        }
    }
}
