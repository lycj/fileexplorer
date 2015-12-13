///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.IO.Tools;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.IO.Tools
{
    public static class DragDropHelper
    {
        #region Attached Properties
        public static DependencyProperty EnableDragProperty =
            DependencyProperty.RegisterAttached("EnableDrag", typeof(bool), typeof(DragDropHelper),
            new PropertyMetadata(OnEnableDragDropChanged));

        public static DependencyProperty EnableDropProperty =
            DependencyProperty.RegisterAttached("EnableDrop", typeof(bool), typeof(DragDropHelper),
            new PropertyMetadata(OnEnableDragDropChanged));

        public static DependencyProperty ConfirmDropProperty =
            DependencyProperty.RegisterAttached("ConfirmDrop", typeof(bool), typeof(DragDropHelper),
            new PropertyMetadata(true));

        public static DependencyProperty CurrentDirectoryProperty =
           DependencyProperty.RegisterAttached("CurrentDirectory", typeof(DirectoryInfoEx),
           typeof(DragDropHelper), new PropertyMetadata(null));

        public static bool GetEnableDrag(DependencyObject target)
        {
            return (bool)target.GetValue(EnableDragProperty);
        }

        public static void SetEnableDrag(DependencyObject target, bool value)
        {
            target.SetValue(EnableDragProperty, value);
        }

        public static bool GetEnableDrop(DependencyObject target)
        {
            return (bool)target.GetValue(EnableDropProperty);
        }

        public static void SetEnableDrop(DependencyObject target, bool value)
        {
            target.SetValue(EnableDropProperty, value);
        }

        public static bool GetConfirmDrop(DependencyObject target)
        {
            return (bool)target.GetValue(ConfirmDropProperty);
        }

        public static void SetConfirmDrop(DependencyObject target, bool value)
        {
            target.SetValue(ConfirmDropProperty, value);
        }

        public static DirectoryInfoEx GetCurrentDirectory(DependencyObject target)
        {
            return (DirectoryInfoEx)target.GetValue(CurrentDirectoryProperty);
        }

        public static void SetCurrentDirectory(DependencyObject target, DirectoryInfoEx value)
        {
            target.SetValue(CurrentDirectoryProperty, value);
        }

        public static void OnEnableDragDropChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == EnableDragProperty)
            {
                #region EnableDrag
                if ((bool)e.NewValue)
                {
                    if (s is ListBox || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                        ele.MouseMove += new MouseEventHandler(OnMouseMove);
                    }
                    else throw new ArgumentException("Support ListView or TreeView only.");
                }
                else
                {
                    if (s is ListBox || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.PreviewMouseDown -= new MouseButtonEventHandler(OnPreviewMouseDown);
                        ele.MouseMove -= new MouseEventHandler(OnMouseMove);
                    }
                }
                #endregion
            }
            else
            {
                //#region EnableDrop
                //if ((bool)e.NewValue)
                //{
                //    if (s is ListBox || s is TreeView || s is ISupportDrop)
                //    {
                //        UIElement ele = s as UIElement;
                //        ele.AllowDrop = true;
                //        ele.DragOver += new DragEventHandler(OnDragOver);
                //        ele.Drop += new DragEventHandler(OnDrop);
                //    }
                //    else throw new ArgumentException("Support ListView or TreeView only.");
                //}
                //else
                //{
                //    if (s is ListBox || s is TreeView || s is ISupportDrop)
                //    {
                //        UIElement ele = s as UIElement;
                //        ele.DragOver -= new DragEventHandler(OnDragOver);
                //        ele.Drop -= new DragEventHandler(OnDrop);
                //    }
                //}
                //#endregion
            }

        }
        #endregion

        #region Tools
        static DependencyObject getParentViewOrViewItemOrScrollBar(DependencyObject e)
        {

            while (e != null && !(e is ListView) && !(e is ListViewItem) && !(e is ScrollBar))
                try
                {
                    e = VisualTreeHelper.GetParent(e);
                }
                catch
                {
                    return e;
                }
            if (e != null)
                return e;

            return null;
        }
        static bool isOverViewOrScrollBar(DependencyObject e)
        {
            DependencyObject obj = getParentViewOrViewItemOrScrollBar(e);
            return (obj is ListView || obj is ScrollBar);
        }

        static bool IsMouseOverScrollbar(UIElement sender, MouseButtonEventArgs e)
        {
            Point ptMouse = e.GetPosition(sender);
            HitTestResult res = VisualTreeHelper.HitTest(sender, ptMouse);
            if (res == null)
                return false;

            DependencyObject depObj = res.VisualHit;
            while (depObj != null)
            {
                if (depObj is ScrollBar)
                    return true;

                // VisualTreeHelper works with objects of type Visual or Visual3D.
                // If the current object is not derived from Visual or Visual3D,
                // then use the LogicalTreeHelper to find the parent element.
                if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }

            return false;
        }
        #endregion

        #region Drag n Drop - Drag
        static Point _startPoint;
        static DependencyObject draggingControl;
        static DragWrapper wrapper = new DragWrapper();

        private static bool IsDragging(DependencyObject control)
        {
            return control.Equals(draggingControl);
        }


        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            UIElement uiSender = sender as UIElement;
            bool isSelected = sender is TreeView ?
                (sender as TreeView).SelectedValue != null :
                (sender as Selector).SelectedIndex != -1;

            if ((e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                && !IsDragging(uiSender))
            {
                Point position = e.GetPosition(null);

                if (!e.MouseDevice.DirectlyOver.Focusable)
                    if (Math.Abs(position.X - _startPoint.X) < SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - _startPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
                    {
                        if (isSelected)
                            try
                            {
                                StartDrag(sender, e);
                            }
                            catch { }
                    }
            };
            //else
            //    base.OnMouseMove(e);
        }

        static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            UIElement uiElement = sender as UIElement;
            bool isSelected = sender is TreeView ?
                (sender as TreeView).SelectedValue != null :
                (sender as Selector).SelectedIndex != -1;

            bool _mouseOverFileList = isOverViewOrScrollBar(e.MouseDevice.DirectlyOver as DependencyObject);
            if (_mouseOverFileList)
            {
                _startPoint = new Point(-10000, 10000);
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                if (isSelected && !IsMouseOverScrollbar(uiElement, e))
                    _startPoint = e.GetPosition(null);
                else
                    _startPoint = new Point(-10000, 10000);

        }

        static FileSystemInfoEx[] getSelectedItems(object sender)
        {
            if (sender is ListBox)
            {
                ListBox lbSender = (ListBox)sender;
                if (lbSender.SelectedValue is FileSystemInfoEx)
                    return (from FileSystemInfoEx item in lbSender.SelectedItems
                            select
                                item).ToArray();
            }
            if (sender is TreeView)
            {
                TreeView tvSender = (TreeView)sender;
                if (tvSender.SelectedValue is FileSystemInfoEx)
                    return new FileSystemInfoEx[] { tvSender.SelectedValue as FileSystemInfoEx };
            }

            return new FileSystemInfoEx[] { };
        }

        //http://blogs.msdn.com/jaimer/archive/2007/07/12/drag-drop-in-wpf-explained-end-to-end.aspx
        static void StartDrag(object sender, MouseEventArgs e)
        {
            UIElement control = sender as UIElement;
            draggingControl = control;
            try
            {
                FileSystemInfoEx[] selectionList = getSelectedItems(sender);
                if (e.RightButton == MouseButtonState.Pressed)
                    wrapper.StartDrag(System.Windows.Forms.MouseButtons.Right, selectionList);
                else wrapper.StartDrag(System.Windows.Forms.MouseButtons.Left, selectionList);
            }
            finally
            {
                draggingControl = null;
            }
        }
        #endregion



        //    #region Drag n Drop - Drop

        //    static TreeViewItem getParentTreeViewItem(FrameworkElement obj)
        //    {
        //        FrameworkElement temp = obj;
        //        while (!(temp is TreeViewItem) && (temp != null))
        //            temp = (FrameworkElement)VisualTreeHelper.GetParent(temp);

        //        return (TreeViewItem)temp;
        //    }

        //    static TreeViewItem getSelectedTreeViewItem(DragEventArgs e)
        //    {
        //        HitTestResult htr = VisualTreeHelper.HitTest((Visual)e.Source,
        //            e.GetPosition((IInputElement)e.Source));
        //        return getParentTreeViewItem((FrameworkElement)htr.VisualHit);
        //    }

        //    static IDirectoryInfoExA locateCurrentDirectory(DependencyObject sender, DragEventArgs e)
        //    {
        //        if (sender is ISupportDrop)
        //            return (sender as ISupportDrop).SelectedDirectory;

        //        if (GetCurrentDirectory(sender) != null)
        //            return GetCurrentDirectory(sender);

        //        if (sender is TreeView)
        //        {
        //            TreeViewItem tvItem = getSelectedTreeViewItem(e);
        //            //Debug.WriteLine(tvItem.DataContext);
        //            if (tvItem != null)
        //                if (tvItem.DataContext is IDirectoryInfoExA)
        //                    return tvItem.DataContext as IDirectoryInfoExA;
        //                else if (tvItem.DataContext is ISupportDrop)
        //                    return (tvItem.DataContext as ISupportDrop).SelectedDirectory;
        //        }

        //        //ListView should specify CurrentDirectory or implement ISupportDrop
        //        return null;
        //    }

        //    static bool CanDrop(string targetDir, string file1)
        //    {
        //        return !(String.Equals(file1, targetDir, StringComparison.InvariantCultureIgnoreCase)) &&
        //                        !(file1.Length > targetDir.Length ?
        //                        file1.StartsWith(targetDir) : targetDir.StartsWith(file1));

        //    }

        //    static void OnDragOver(object sender, DragEventArgs e)
        //    {
        //        e.Effects = DragDropEffects.None;
        //        UIElement uiSender = sender as UIElement;
        //        if (uiSender.AllowDrop)
        //        {
        //            IDirectoryInfoExA currentDir = locateCurrentDirectory(uiSender, e);
        //            if (currentDir == null ||
        //                (currentDir.Attributes & FileAttributes.ReadOnly) != 0) return;

        //            if (uiSender is TreeView || !IsDragging(uiSender))
        //                if (e.Data.GetDataPresent(DataObjectExA.DataFormats_EntryString))
        //                {
        //                    string[] files = (string[])e.Data.GetData(DataObjectExA.DataFormats_EntryString);
        //                    string targetDir = currentDir.ParseName;

        //                    if (CanDrop(targetDir, files[0]))
        //                        e.Effects = e.AllowedEffects | DragDropEffects.All;
        //                    e.Handled = true;
        //                }
        //                else
        //                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //                    {
        //                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //                        FileSystemInfoExA item0 = FileSystemInfoExA.FromStringParse(files[0]);
        //                        if (!(item0.Equals(currentDir)))
        //                            e.Effects = e.AllowedEffects | DragDropEffects.All;

        //                        e.Handled = true;
        //                    }
        //        }
        //    }

        //    static void refresh(DependencyObject sender, DragEventArgs e, bool refreshFolder)
        //    {
        //        //if (sender is TreeView)
        //        //{
        //        //    if (refreshFolder)
        //        //    {
        //        //        TreeViewItem tvItem = getSelectedTreeViewItem(e);
        //        //        if (tvItem != null)
        //        //            if (tvItem.DataContext is ISupportDrop)
        //        //                (tvItem.DataContext as ISupportDrop).Refresh();
        //        //            else
        //        //            {
        //        //                BindingExpression ItemSourceBinding =
        //        //                BindingOperations.GetBindingExpression(sender, TreeViewItem.ItemsSourceProperty);
        //        //                ItemSourceBinding.UpdateTarget();
        //        //            }
        //        //    }
        //        //}
        //        //else
        //            if (sender is ISupportDrop)
        //                (sender as ISupportDrop).Refresh();
        //            else if (sender is ListBox)
        //            {
        //                BindingExpression ItemSourceBinding =
        //                    BindingOperations.GetBindingExpression(sender, ListBox.ItemsSourceProperty);
        //                ItemSourceBinding.UpdateTarget();
        //            }

        //    }

        //    static void OnDrop(object s, DragEventArgs e)
        //    {
        //        DependencyObject sender = (DependencyObject)s;
        //        IDirectoryInfoExA currentDir = locateCurrentDirectory(sender, e);
        //        if (currentDir == null) return;

        //        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //        {
        //            (sender as FrameworkElement).Cursor = Cursors.Wait;
        //            bool refreshFolder = false;
        //            try
        //            {
        //                string[] addList =
        //                    e.Data.GetDataPresent(DataObjectExA.DataFormats_EntryString) ?
        //                    (string[])e.Data.GetData(DataObjectExA.DataFormats_EntryString) :
        //                    (string[])e.Data.GetData(DataFormats.FileDrop);

        //                if (CanDrop(currentDir.ParseName, addList[0]))
        //                {
        //                    string queryText = String.Format("Add {0} items to {1}?", addList.Length, currentDir.ParseName);
        //                    if (!GetConfirmDrop(sender) ||
        //                        MessageBox.Show(queryText, "Add", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //                        foreach (string item in addList)
        //                        {
        //                            FileSystemInfoExA parsedItem = FileSystemInfoExA.FromStringParse(item);
        //                            (currentDir as IInternalDirectoryInfoExA).put(parsedItem);
        //                            if (parsedItem is IDirectoryInfoExA) refreshFolder = true;
        //                        }
        //                }
        //            }
        //            finally
        //            {
        //                (sender as FrameworkElement).Cursor = Cursors.Arrow;
        //            }

        //            refresh(sender, e, refreshFolder);
        //        }
        //    }

        //    #endregion

    }
}
