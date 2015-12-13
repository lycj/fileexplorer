using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;

namespace FileExplorer.Defines
{


    public static partial class UIEventHubProperties
    {
        #region SelectionAdorner
        public static DependencyProperty SelectionAdornerProperty =
            DependencyProperty.RegisterAttached("SelectionAdorner", typeof(SelectionAdorner), typeof(UIEventHubProperties));

        public static SelectionAdorner GetSelectionAdorner(DependencyObject target)
        {
            return (SelectionAdorner)target.GetValue(SelectionAdornerProperty);
        }

        public static void SetSelectionAdorner(DependencyObject target, SelectionAdorner value)
        {
            target.SetValue(SelectionAdornerProperty, value);
        }

        #endregion

        #region SelectedItemsAdorner
        public static DependencyProperty SelectedItemsAdornerProperty =
            DependencyProperty.RegisterAttached("SelectedItemsAdorner", typeof(SelectedItemsAdorner), typeof(UIEventHubProperties));

        public static SelectedItemsAdorner GetSelectedItemsAdorner(DependencyObject target)
        {
            return (SelectedItemsAdorner)target.GetValue(SelectedItemsAdornerProperty);
        }

        public static void SetSelectedItemsAdorner(DependencyObject target, SelectedItemsAdorner value)
        {
            target.SetValue(SelectedItemsAdornerProperty, value);
        }

        #endregion

        #region ResizeItemAdorner
        public static DependencyProperty ResizeItemAdornerProperty =
            DependencyProperty.RegisterAttached("ResizeItemAdorner", typeof(ResizeItemAdorner), typeof(UIEventHubProperties));

        public static ResizeItemAdorner GetResizeItemAdorner(DependencyObject target)
        {
            return (ResizeItemAdorner)target.GetValue(ResizeItemAdornerProperty);
        }

        public static void SetResizeItemAdorner(DependencyObject target, ResizeItemAdorner value)
        {
            target.SetValue(ResizeItemAdornerProperty, value);
        }

        #endregion

        #region LastScrollContentPresenter

        public static DependencyProperty LastScrollContentPresenterProperty =
           DependencyProperty.RegisterAttached("LastScrollContentPresenter", typeof(ScrollContentPresenter), typeof(UIEventHubProperties));

        public static ScrollContentPresenter GetLastScrollContentPresenter(DependencyObject target)
        {
            return (ScrollContentPresenter)target.GetValue(LastScrollContentPresenterProperty);
        }

        public static void SetLastScrollContentPresenter(DependencyObject target, ScrollContentPresenter value)
        {
            target.SetValue(LastScrollContentPresenterProperty, value);
        }
        #endregion

        #region StartSelectedItem

        public static DependencyProperty StartSelectedItemProperty =
          DependencyProperty.RegisterAttached("StartSelectedItem", typeof(Control), typeof(UIEventHubProperties));

        public static Control GetStartSelectedItem(DependencyObject target)
        {
            return (Control)target.GetValue(StartSelectedItemProperty);
        }

        public static void SetStartSelectedItem(DependencyObject target, Control value)
        {
            target.SetValue(StartSelectedItemProperty, value);
        }
        #endregion

        #region IsSelecting

        public static DependencyProperty IsSelectingProperty =
       DependencyProperty.RegisterAttached("IsSelecting", typeof(bool), typeof(UIEventHubProperties), new PropertyMetadata(false));


        public static bool GetIsSelecting(DependencyObject target)
        {
            return (bool)target.GetValue(IsSelectingProperty);
        }

        public static void SetIsSelecting(DependencyObject target, bool value)
        {
            target.SetValue(IsSelectingProperty, value);
        }
        #endregion
    }
}
