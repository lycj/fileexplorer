using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region EnterPressCommand
        public static readonly DependencyProperty EnterPressCommandProperty
            = DependencyProperty.RegisterAttached("EnterPressCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnEnterPressInvalidated)));
        public static ICommand GetEnterPressCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(EnterPressCommandProperty);
        }

        public static void SetEnterPressCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(EnterPressCommandProperty, value);
        }
        #endregion

        #region EnterPressCommandParameter
        public static readonly DependencyProperty EnterPressCommandParameterProperty
            = DependencyProperty.RegisterAttached("EnterPressCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetEnterPressCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(EnterPressCommandParameterProperty);
        }

        public static void SetEnterPressCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(EnterPressCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnEnterPressKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ICommand Command = GetEnterPressCommand((DependencyObject)(sender));
                object EnterPressComamndParameter = GetEnterPressCommandParameter((DependencyObject)(sender));

                if (Command != null)
                    Command.Execute(EnterPressComamndParameter);
            }
        }


        private static void OnEnterPressInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, EnterPressCommandProperty, e.NewValue as ICommand);
                element.PreviewKeyDown += new KeyEventHandler(OnEnterPressKeyDown);
                element.AllowDrop = true;
            }
            else
            {
                element.PreviewKeyDown -= new KeyEventHandler(OnEnterPressKeyDown);
                SetAttachedProperty(element, EnterPressCommandProperty, null);
            }
        }
        #endregion



    }
}