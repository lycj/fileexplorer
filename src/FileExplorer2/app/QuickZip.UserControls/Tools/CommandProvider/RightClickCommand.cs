using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region RightClickCommand
        public static readonly DependencyProperty RightClickCommandProperty
            = DependencyProperty.RegisterAttached("RightClickCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnRightClickInvalidated)));
        public static ICommand GetRightClickCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(RightClickCommandProperty);
        }

        public static void SetRightClickCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(RightClickCommandProperty, value);
        }
        #endregion

        #region RightClickCommandParameter
        public static readonly DependencyProperty RightClickCommandParameterProperty
            = DependencyProperty.RegisterAttached("RightClickCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetRightClickCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(RightClickCommandParameterProperty);
        }

        public static void SetRightClickCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(RightClickCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnClickMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ICommand Command = GetRightClickCommand((DependencyObject)(sender));
            object RightClickComamndParameter =
                GetRightClickCommandParameter((DependencyObject)(sender));

            if (Command != null)
                Command.Execute(RightClickComamndParameter);

        }

        private static void OnRightClickInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, RightClickCommandProperty, e.NewValue as ICommand);
                element.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnClickMouseRightButtonDown);
                element.AllowDrop = true;                
            }
            else
            {
                element.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(OnClickMouseRightButtonDown);
                SetAttachedProperty(element, RightClickCommandProperty, null);
            }
        }
        #endregion



    }
}