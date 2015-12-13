using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region PrevCommand
        public static readonly DependencyProperty PrevCommandProperty
            = DependencyProperty.RegisterAttached("PrevCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnPrevInvalidated)));
        public static ICommand GetPrevCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(PrevCommandProperty);
        }

        public static void SetPrevCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(PrevCommandProperty, value);
        }
        #endregion

        #region PrevCommandParameter
        public static readonly DependencyProperty PrevCommandParameterProperty
            = DependencyProperty.RegisterAttached("PrevCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetPrevCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(PrevCommandParameterProperty);
        }

        public static void SetPrevCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(PrevCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnPrevMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1)
            
            {
                ICommand Command = GetPrevCommand((DependencyObject)(sender));
                object PrevComamndParameter =
                    GetPrevCommandParameter((DependencyObject)(sender));

                if (Command != null)
                    Command.Execute(PrevComamndParameter);
            }
        }

        private static void OnPrevInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, PrevCommandProperty, e.NewValue as ICommand);
                element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnPrevMouseLeftButtonDown);
                element.AllowDrop = true;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(OnPrevMouseLeftButtonDown);
                SetAttachedProperty(element, PrevCommandProperty, null);
            }
        }
        #endregion



    }
}