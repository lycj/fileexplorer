///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using QuickZip.IO.PIDL.Tools;
using System.Diagnostics;
using System.IO.Tools;
using System.Windows.Documents;
using System.Collections.Generic;

namespace QuickZip.IO.PIDL.UserControls
{
    public static class DragDropHelperEx
    {
        #region Attached Properties
        public static DependencyProperty EnableDragProperty =
            DependencyProperty.RegisterAttached("EnableDrag", typeof(bool), typeof(DragDropHelperEx),
            new PropertyMetadata(OnEnableDragDropChanged));

        public static DependencyProperty EnableDropProperty =
            DependencyProperty.RegisterAttached("EnableDrop", typeof(bool), typeof(DragDropHelperEx),
            new PropertyMetadata(OnEnableDragDropChanged));

        public static DependencyProperty ConfirmDropProperty =
            DependencyProperty.RegisterAttached("ConfirmDrop", typeof(bool), typeof(DragDropHelperEx),
            new PropertyMetadata(true));

        public static DependencyProperty ConverterProperty =
           DependencyProperty.RegisterAttached("Converter", typeof(IValueConverter),
           typeof(DragDropHelperEx), new PropertyMetadata(null));

        public static DependencyProperty CurrentDirectoryProperty =
          DependencyProperty.RegisterAttached("CurrentDirectory", typeof(DirectoryInfoEx),
          typeof(DragDropHelperEx), new PropertyMetadata(null/*, new PropertyChangedCallback(OnCurrentDirChanged)*/));

        public static DependencyProperty IsDragOverProperty =
          DependencyProperty.RegisterAttached("IsDragOver", typeof(Boolean),
          typeof(DragDropHelperEx), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static void OnCurrentDirChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        //{
        //    Debug.WriteLine("CurrentDirChanged");
        //}

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

        public static IValueConverter GetConverter(DependencyObject target)
        {
            return (IValueConverter)target.GetValue(ConverterProperty);
        }

        public static void SetConverter(DependencyObject target, IValueConverter value)
        {
            target.SetValue(ConverterProperty, value);
        }

        public static DirectoryInfoEx GetCurrentDirectory(DependencyObject target)
        {
            return (DirectoryInfoEx)target.GetValue(CurrentDirectoryProperty);
        }

        public static void SetCurrentDirectory(DependencyObject target, DirectoryInfoEx value)
        {
            target.SetValue(CurrentDirectoryProperty, value);
        }

        public static bool GetIsDragOver(DependencyObject target)
        {
            return (bool)target.GetValue(IsDragOverProperty);
        }

        public static void SetIsDragOver(DependencyObject target, bool value)
        {
            target.SetValue(IsDragOverProperty, value);
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
                        ele.PreviewMouseUp += new MouseButtonEventHandler(OnMouseUp);
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
                        ele.PreviewMouseUp -= new MouseButtonEventHandler(OnMouseUp);
                        ele.MouseMove -= new MouseEventHandler(OnMouseMove);
                    }
                }
                #endregion
            }
            else
            {
                #region EnableDrag
                if ((bool)e.NewValue)
                {
                    if (s is ListBox || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.AllowDrop = true;
                        ele.DragOver += new DragEventHandler(OnDragOver);
                        ele.Drop += new DragEventHandler(OnDrop);
                    }
                    else throw new ArgumentException("Support ListView or TreeView only.");
                }
                else
                {
                    if (s is ListBox || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.DragOver -= new DragEventHandler(OnDragOver);
                        ele.Drop -= new DragEventHandler(OnDrop);
                    }
                }
                #endregion
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
        static bool isOverViewOrScrollBar(IInputElement e)
        {
            if (e is Border)
                return true;
            else
            {
                DependencyObject obj = getParentViewOrViewItemOrScrollBar(e as DependencyObject);
                return (obj is ListView || obj is ScrollBar);
            }
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
        static DataObjectEx dataObj;
        private static bool IsDragging(DependencyObject control)
        {
            return control.Equals(draggingControl);
        }

        private static void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UIElement uiSender = sender as UIElement;
            //ESC pressed
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                uiSender.AllowDrop = true;
                return;
            }
            //Drop!
            if (e.KeyStates == DragDropKeyStates.None)
            {
                dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 0);
                e.Action = DragAction.Drop;
                uiSender.AllowDrop = true;
                return;
            }

            e.Action = DragAction.Continue;
            //base.OnQueryContinueDrag(e);
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

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetIsDragOver(null);
        }

        static ListBoxItem getSelectedItem(Visual sender, Point position)
        {
            HitTestResult r = VisualTreeHelper.HitTest(sender, position);
            if (r == null) return null;

            DependencyObject obj = r.VisualHit;
            while (!(obj is ListBox) && (obj != null))
            {
                obj = VisualTreeHelper.GetParent(obj);

                if (obj is ListBoxItem)
                    return obj as ListBoxItem;
            }

            return null;
        }

        static bool listBoxMouseOverSelectedItem(ListBox lbSender)
        {
            ListBoxItem _selectedItem = getSelectedItem(lbSender, Mouse.GetPosition(lbSender));
            if (_selectedItem != null && lbSender.SelectedItem != null)
                return lbSender.SelectedItems.Contains(lbSender.ItemContainerGenerator.ItemFromContainer(_selectedItem));
            return false;
        }

        static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = e.OriginalSource as FrameworkElement;
            UIElement uiElement = sender as UIElement;
            bool isSelected = sender is TreeView ?
                (sender as TreeView).SelectedValue != null :
                sender is ListBox ? listBoxMouseOverSelectedItem(sender as ListBox) :
                (sender as Selector).SelectedIndex != -1;

            bool _mouseOverFileList = isOverViewOrScrollBar(e.MouseDevice.DirectlyOver);
            if (_mouseOverFileList)
            {
                _startPoint = new Point(-10000, 10000);
                return;
            }



            if (isSelected && !IsMouseOverScrollbar(uiElement, e))
            {
                _startPoint = e.GetPosition(null);
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1 && sender is ListBox)
                {
                    e.Handled = true;
                }
            }
            else
                _startPoint = new Point(-10000, 10000);

        }


static FileSystemInfoEx[] getSelectedItems(object sender)
{
    FileSystemInfoEx[] retVal = null;
    if (sender is ListBox)
    {
        ListBox lbSender = (ListBox)sender;
        if (lbSender.SelectedValue is FileSystemInfoEx)
            retVal = (from FileSystemInfoEx item in lbSender.SelectedItems
                    select
                        item).ToArray();
        else
        {
            IValueConverter converter = GetConverter(lbSender);
            if (converter != null)
                retVal = (from object item in lbSender.SelectedItems
                        select
                            converter.Convert(item, null, null, null) as FileSystemInfoEx).ToArray();
        }
    }
    if (sender is TreeView)
    {
        TreeView tvSender = (TreeView)sender;
        if (tvSender.SelectedValue is FileSystemInfoEx)
            retVal = new FileSystemInfoEx[] { tvSender.SelectedValue as FileSystemInfoEx };
        else
        {
            IValueConverter converter = GetConverter(tvSender);
            if (converter != null)
                retVal = new FileSystemInfoEx[] { converter.Convert(tvSender.SelectedValue, null, null, null) as FileSystemInfoEx };
        }
    }

    List<FileSystemInfoEx> retList = new List<FileSystemInfoEx>();
    if (retVal != null)
    {
        foreach (FileSystemInfoEx ex in retVal)
            if (!ex.FullName.StartsWith("::"))
                retList.Add(ex);
        return retList.ToArray();
    }
    else return new FileSystemInfoEx[] { };
}

        //http://blogs.msdn.com/jaimer/archive/2007/07/12/drag-drop-in-wpf-explained-end-to-end.aspx
        static void StartDrag(object sender, MouseEventArgs e)
        {
            UIElement control = sender as UIElement;
            draggingControl = control;
            try
            {
                control.QueryContinueDrag += new QueryContinueDragEventHandler(OnQueryContinueDrag);

                //control.AllowDrop = false;
                FileSystemInfoEx[] selectionList = getSelectedItems(sender);

                if (selectionList.Length > 0)
                {
                    dataObj = new DataObjectEx(selectionList);
                    dataObj.SetData(DataFormats.FileDrop, dataObj.PathnameList());
                    dataObj.SetData(DataObjectEx.DataFormats_EntryString, dataObj.PathnameList());
                    dataObj.SetData(ShellClipboardFormats.CFSTR_PREFERREDDROPEFFECT, DragDropEffects.Copy);
                    dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 1);
                    DragDropEffects de = System.Windows.DragDrop.DoDragDrop(control, dataObj, DragDropEffects.Copy);
                }
            }
            finally
            {
                draggingControl = null;
                SetIsDragOver(null);
            }
        }
        #endregion


        #region Drag n Drop - Drop

        static TreeViewItem getParentTreeViewItem(FrameworkElement obj)
        {
            FrameworkElement temp = obj;
            while (!(temp is TreeViewItem) && (temp != null))
                temp = (FrameworkElement)VisualTreeHelper.GetParent(temp);

            return (TreeViewItem)temp;
        }

        static TreeViewItem getSelectedTreeViewItem(DragEventArgs e)
        {
            HitTestResult htr = VisualTreeHelper.HitTest((Visual)e.Source,
                e.GetPosition((IInputElement)e.Source));
            return getParentTreeViewItem((FrameworkElement)htr.VisualHit);
        }


        static UIElement _lastIsDragOverItem = null;
        static void SetIsDragOver(UIElement element)
        {
            if (_lastIsDragOverItem != null)
                SetIsDragOver(_lastIsDragOverItem, false);
            _lastIsDragOverItem = element;
            if (element != null)
                SetIsDragOver(element, true);
        }

        static DirectoryInfoEx locateCurrentDirectory(DependencyObject sender, DragEventArgs e)
        {
            if (sender is ISupportDrop)
                return (sender as ISupportDrop).SelectedDirectory;

            if (GetCurrentDirectory(sender) != null)
                return GetCurrentDirectory(sender);

            if (sender is TreeView)
            {
                TreeViewItem tvItem = getSelectedTreeViewItem(e);
                SetIsDragOver(tvItem);
                //Debug.WriteLine(tvItem.DataContext);
                if (tvItem != null)
                    if (tvItem.DataContext is DirectoryInfoEx)
                        return tvItem.DataContext as DirectoryInfoEx;
                    else if (GetConverter(sender) != null)
                        return (GetConverter(sender).Convert(tvItem.DataContext, null, null, null) as DirectoryInfoEx);
            }

            //ListView should specify CurrentDirectory or implement ISupportDrop
            return null;
        }

        static bool CanDrop(string targetDir, string file1)
        {
            return !(String.Equals(file1, targetDir, StringComparison.InvariantCultureIgnoreCase)) &&
                            !(file1.Length > targetDir.Length ?
                            file1.StartsWith(targetDir) : targetDir.StartsWith(file1));

        }

        static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            UIElement uiSender = sender as UIElement;
            if (uiSender.AllowDrop)
            {
                DirectoryInfoEx currentDir = locateCurrentDirectory(uiSender, e);
                if (currentDir == null ||
                    (currentDir.Attributes & FileAttributes.ReadOnly) != 0) return;

                if (uiSender is TreeView || !IsDragging(uiSender))
                    if (e.Data.GetDataPresent(DataObjectEx.DataFormats_EntryString))
                    {
                        string[] files = (string[])e.Data.GetData(DataObjectEx.DataFormats_EntryString);
                        string targetDir = currentDir.FullName;

                        if (CanDrop(targetDir, files[0]))
                            e.Effects = e.AllowedEffects | DragDropEffects.All;
                        e.Handled = true;
                    }
                    else
                        if (e.Data.GetDataPresent(DataFormats.FileDrop))
                        {
                            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            FileSystemInfoEx item0 = FileSystemInfoEx.FromString(files[0]);
                            if (!(item0.Equals(currentDir)))
                                e.Effects = e.AllowedEffects | DragDropEffects.All;

                            e.Handled = true;
                        }
            }
        }

        static void refresh(DependencyObject sender, DragEventArgs e, bool refreshFolder)
        {
            //if (sender is TreeView)
            //{
            //    if (refreshFolder)
            //    {
            //        TreeViewItem tvItem = getSelectedTreeViewItem(e);
            //        if (tvItem != null)
            //            if (tvItem.DataContext is ISupportDrop)
            //                (tvItem.DataContext as ISupportDrop).Refresh();
            //            else
            //            {
            //                BindingExpression ItemSourceBinding =
            //                BindingOperations.GetBindingExpression(sender, TreeViewItem.ItemsSourceProperty);
            //                ItemSourceBinding.UpdateTarget();
            //            }
            //    }
            //}
            //else


        }

        private static FileSystemInfoEx[] stringToEx(string[] pathList)
        {
            List<FileSystemInfoEx> retList = new List<FileSystemInfoEx>();

            foreach (string path in pathList)
                retList.Add(FileSystemInfoEx.FromString(path));

            return retList.ToArray();
        }

        static void OnDrop(object s, DragEventArgs e)
        {
            DependencyObject sender = (DependencyObject)s;
            DirectoryInfoEx currentDir = locateCurrentDirectory(sender, e);
            if (currentDir == null) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                (sender as FrameworkElement).Cursor = Cursors.Wait;
                bool refreshFolder = false;
                try
                {
                    string[] addList =
                        e.Data.GetDataPresent(DataObjectEx.DataFormats_EntryString) ?
                        (string[])e.Data.GetData(DataObjectEx.DataFormats_EntryString) :
                        (string[])e.Data.GetData(DataFormats.FileDrop);

                    //0.2: Fixed DropDropHelper strange drag behavior. (folder dropped to itself, caused by OnDragOver skipped)
                    if (CanDrop(currentDir.FullName, addList[0]))
                    {
                        string queryText = String.Format("Add {0} items to {1}?", addList.Length, currentDir.FullName);
                        if (!GetConfirmDrop(sender) ||
                            MessageBox.Show(queryText, "Add", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            foreach (string item in addList)
                            {
                                //FileSystemInfoEx parsedItem = FileSystemInfoEx.FromString(item);
                                //IOTools.Copy(parsedItem.FullName, PathEx.Combine(currentDir.FullName, parsedItem.Name));
                                //if (parsedItem is DirectoryInfoEx) refreshFolder = true;

                                FileSystemInfoEx[] addListExA = stringToEx(addList);
                                int id = WorkSpawner.SpawnCopyWork(addListExA, currentDir, true);
                                System.IO.Tools.WorkFinishedEventHandler handler = null;
                                handler = (System.IO.Tools.WorkFinishedEventHandler)delegate(object send, System.IO.Tools.WorkFinishedEventArgs args)
                                {
                                    refresh(sender, e, refreshFolder);
                                    WorkSpawner.Works[id].WorkFinished -= handler;
                                };


                                WorkSpawner.Works[id].WorkFinished += handler;
                                WorkSpawner.Start(id);
                            }
                    }
                }
                finally
                {
                    (sender as FrameworkElement).Cursor = Cursors.Arrow;
                    SetIsDragOver(null);
                }

                refresh(sender, e, refreshFolder);
            }
        }

        #endregion

    }
}
