using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region ClickCommand
        public static readonly DependencyProperty ClickCommandProperty
            = DependencyProperty.RegisterAttached("ClickCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnClickInvalidated)));
        public static ICommand GetClickCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(ClickCommandProperty);
        }

        public static void SetClickCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(ClickCommandProperty, value);
        }
        #endregion

        #region ClickCommandParameter
        public static readonly DependencyProperty ClickCommandParameterProperty
            = DependencyProperty.RegisterAttached("ClickCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetClickCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(ClickCommandParameterProperty);
        }

        public static void SetClickCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(ClickCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnClickMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ICommand Command = GetClickCommand((DependencyObject)(sender));
            object ClickComamndParameter =
                GetClickCommandParameter((DependencyObject)(sender));

            if (Command != null)
                Command.Execute(ClickComamndParameter);

        }

        private static void OnClickInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, ClickCommandProperty, e.NewValue as ICommand);
                element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnClickMouseLeftButtonDown);
                element.AllowDrop = true;                
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnClickMouseLeftButtonDown);
                SetAttachedProperty(element, ClickCommandProperty, null);
            }
        }
        #endregion



    }
}