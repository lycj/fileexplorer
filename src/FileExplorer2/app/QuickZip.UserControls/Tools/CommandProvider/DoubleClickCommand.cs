using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region DoubleClickCommand
        public static readonly DependencyProperty DoubleClickCommandProperty
            = DependencyProperty.RegisterAttached("DoubleClickCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnDoubleClickInvalidated)));
        public static ICommand GetDoubleClickCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(DoubleClickCommandProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(DoubleClickCommandProperty, value);
        }
        #endregion

        #region DoubleClickCommandParameter
        public static readonly DependencyProperty DoubleClickCommandParameterProperty
            = DependencyProperty.RegisterAttached("DoubleClickCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetDoubleClickCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(DoubleClickCommandParameterProperty);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(DoubleClickCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnDoubleClickMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2) //DoubleClick
            {
                ICommand Command = GetDoubleClickCommand((DependencyObject)(sender));
                object doubleClickComamndParameter =
                    GetDoubleClickCommandParameter((DependencyObject)(sender));

                if (Command != null)
                    Command.Execute(doubleClickComamndParameter);
            }
        }

        private static void OnDoubleClickInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, DoubleClickCommandProperty, e.NewValue as ICommand);
                element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnDoubleClickMouseLeftButtonDown);
                element.AllowDrop = true;                
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnDoubleClickMouseLeftButtonDown);
                SetAttachedProperty(element, DoubleClickCommandProperty, null);
            }
        }
        #endregion



    }
}