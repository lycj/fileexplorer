using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QuickZip.Logic
{
    /// <summary>
    /// Attached property that can be used to create a binding for a CommandModel. Set the
    /// CreateCommandBinding.Command property to a CommandModel.
    /// </summary>
    public static class CreateCommandBinding
    {
        public static readonly DependencyProperty CommandProperty
           = DependencyProperty.RegisterAttached("Command", typeof(CommandModel), typeof(CreateCommandBinding),
                new PropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));

        public static CommandModel GetCommand(DependencyObject sender)
        {
            return (CommandModel)sender.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject sender, CommandModel command)
        {
            sender.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Callback when the Command property is set or changed.
        /// </summary>
        private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            // If we're given a command model, set up a binding
            CommandModel commandModel = e.NewValue as CommandModel;
            if (commandModel != null)
            {
                element.CommandBindings.Add(new CommandBinding(commandModel.Command, commandModel.OnExecute, commandModel.OnQueryEnabled));
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
