using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public partial class CommandProvider : FrameworkElement
    {
        #region TVSelectionChangedCommand
        public static readonly DependencyProperty TVSelectionChangedCommandProperty
            = DependencyProperty.RegisterAttached("TVSelectionChangedCommand",
                typeof(ICommand), typeof(CommandProvider),
                new PropertyMetadata(new PropertyChangedCallback(OnTVSelectionChangedInvalidated)));
        public static ICommand GetTVSelectionChangedCommand(DependencyObject sender)
        {
            return (ICommand)sender.GetValue(TVSelectionChangedCommandProperty);
        }

        public static void SetTVSelectionChangedCommand(DependencyObject sender, ICommand value)
        {
            sender.SetValue(TVSelectionChangedCommandProperty, value);
        }
        #endregion

        #region TVSelectionChangedCommandParameter
        public static readonly DependencyProperty TVSelectionChangedCommandParameterProperty
            = DependencyProperty.RegisterAttached("TVSelectionChangedCommandParameter",
                typeof(object), typeof(CommandProvider));

        public static object GetTVSelectionChangedCommandParameter(DependencyObject sender)
        {
            return (object)sender.GetValue(TVSelectionChangedCommandParameterProperty);
        }

        public static void SetTVSelectionChangedCommandParameter(DependencyObject sender, ICommand value)
        {
            sender.SetValue(TVSelectionChangedCommandParameterProperty, value);
        }
        #endregion

        #region Events
        protected static void OnTVSelectionChangedMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2) //TVSelectionChanged
            {
                ICommand Command = GetTVSelectionChangedCommand((DependencyObject)(sender));
                object TVSelectionChangedComamndParameter =
                    GetTVSelectionChangedCommandParameter((DependencyObject)(sender));

                if (Command != null)
                    Command.Execute(TVSelectionChangedComamndParameter);
            }
        }

        private static void OnTVSelectionChangedInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TreeView element = (TreeView)dependencyObject;

            if (e.NewValue != null)
            {
                SetAttachedProperty(element, TVSelectionChangedCommandProperty, e.NewValue as ICommand);
                element.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeElement_SelectedItemChanged);
                element.AllowDrop = true;
            }
            else
            {
                element.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(treeElement_SelectedItemChanged);
                SetAttachedProperty(element, TVSelectionChangedCommandProperty, null);
            }
        }

        static void treeElement_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null) 
            {
                ICommand Command = GetTVSelectionChangedCommand((DependencyObject)(sender));
                object TVSelectionChangedComamndParameter =
                    GetTVSelectionChangedCommandParameter((DependencyObject)(sender));

                if (Command != null)
                    Command.Execute(TVSelectionChangedComamndParameter);
            }
        }
        #endregion



    }
}