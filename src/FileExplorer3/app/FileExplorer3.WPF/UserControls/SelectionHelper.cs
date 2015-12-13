using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.UserControls
{
    #region Enums and Interfaces
   

    public interface IVirtualListView
    {
        void UnselectAll();
    }

    #endregion


    /// <summary>
    /// For ListView's MultiSelect support.
    /// Obsoluted, use UIEventHub instead.
    /// </summary>
    public static class SelectionHelper
    {
        #region Attached Properties
        public static DependencyProperty EnableSelectionProperty =
            DependencyProperty.RegisterAttached("EnableSelection", typeof(bool), typeof(SelectionHelper),
            new PropertyMetadata(OnEnableSelectionpChanged));

        public static bool GetEnableSelection(DependencyObject target)
        {
            return (bool)target.GetValue(EnableSelectionProperty);
        }

        public static void SetEnableSelection(DependencyObject target, bool value)
        {
            target.SetValue(EnableSelectionProperty, value);
        }

        private static DependencyProperty LastScrollContentPresenterProperty =
            DependencyProperty.RegisterAttached("LastScrollContentPresenter", typeof(ScrollContentPresenter), typeof(SelectionHelper));

        public static ScrollContentPresenter GetLastScrollContentPresenter(DependencyObject target)
        {
            return (ScrollContentPresenter)target.GetValue(LastScrollContentPresenterProperty);
        }

        public static void SetLastScrollContentPresenter(DependencyObject target, ScrollContentPresenter value)
        {
            target.SetValue(LastScrollContentPresenterProperty, value);
        }

        private static DependencyProperty SelectionAdornerProperty =
            DependencyProperty.RegisterAttached("SelectionAdorner", typeof(SelectionAdorner), typeof(SelectionHelper));

        public static SelectionAdorner GetSelectionAdorner(DependencyObject target)
        {
            return (SelectionAdorner)target.GetValue(SelectionAdornerProperty);
        }

        public static void SetSelectionAdorner(DependencyObject target, SelectionAdorner value)
        {
            target.SetValue(SelectionAdornerProperty, value);
        }

        private static DependencyProperty StartScrollbarPositionProperty =
            DependencyProperty.RegisterAttached("StartScrollbarPosition", typeof(Point), typeof(SelectionHelper));

        public static Point GetStartScrollbarPosition(DependencyObject target)
        {
            return (Point)target.GetValue(StartScrollbarPositionProperty);
        }

        public static void SetStartScrollbarPosition(DependencyObject target, Point value)
        {
            target.SetValue(StartScrollbarPositionProperty, value);
        }

        private static DependencyProperty StartPositionProperty =
            DependencyProperty.RegisterAttached("StartPosition", typeof(Point), typeof(SelectionHelper));

        public static Point GetStartPosition(DependencyObject target)
        {
            return (Point)target.GetValue(StartPositionProperty);
        }

        public static void SetStartPosition(DependencyObject target, Point value)
        {
            target.SetValue(StartPositionProperty, value);
        }

        private static DependencyProperty HighlightCountProperty =
            DependencyProperty.RegisterAttached("HighlightCount", typeof(int), typeof(SelectionHelper));

        public static int GetHighlightCount(DependencyObject target)
        {
            return (int)target.GetValue(HighlightCountProperty);
        }

        public static void SetHighlightCount(DependencyObject target, int value)
        {
            target.SetValue(HighlightCountProperty, value);
        }

        private static DependencyProperty IsDraggingProperty =
           DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(SelectionHelper), new PropertyMetadata(false));

        public static bool GetIsDragging(DependencyObject target)
        {
            return (bool)target.GetValue(IsDraggingProperty);
        }
        public static bool GetIsDragging(ListView sender)
        {
            ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(sender);
            ScrollContentPresenter p = UITools.FindAncestor<ScrollContentPresenter>(ip);
            return GetIsDragging(p);
        }
        public static void SetIsDragging(DependencyObject target, bool value)
        {
            target.SetValue(IsDraggingProperty, value);
        }

        private static void OnMouseDown(object sender, RoutedEventArgs args)
        {
            MouseButtonEventArgs mbArgs = args as MouseButtonEventArgs;
            if (mbArgs.ChangedButton == MouseButton.Left)
                ClearSelection(sender as ListView);
        }

        private static void OnScrollChange(object sender, RoutedEventArgs args)
        {
            ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(sender as ListView);
            ScrollContentPresenter p = UITools.FindAncestor<ScrollContentPresenter>(ip);

            if (GetIsDragging(p) && Mouse.LeftButton == MouseButtonState.Pressed)
                UpdatePosition(p, true);
        }

        public static void OnEnableSelectionpChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {            
            if (s is ListView)
            {
                ListView control = s as ListView;


                var OnSizeChanged = (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
                {
                    ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(sender as ListView);
                    ScrollContentPresenter p = UITools.FindAncestor<ScrollContentPresenter>(ip);
                    EndDragging(p);
                };

                Func<ScrollContentPresenter, AdornerLayer> getAdornerLayer = (p) =>
                    {
                        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(p);
                        if (adornerLayer != null)
                            return adornerLayer;

                        ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(s as ListView);
                        AdornerDecorator ad = UITools.FindAncestor<AdornerDecorator>(ip);
                        try
                        {
                            adornerLayer = AdornerLayer.GetAdornerLayer(ad);
                            if (adornerLayer != null)
                                return adornerLayer;
                        }
                        catch { }
                        return p.AdornerLayer; ;
                    };

                Action unloadAdorner = () =>
                    {
                        ScrollContentPresenter p = GetLastScrollContentPresenter(control);
                        if (p != null)
                        {
                            SelectionAdorner _adorner = GetSelectionAdorner(p);
                            if (_adorner != null)
                            {
                                getAdornerLayer(p).Remove(_adorner);
                                _adorner.PreviewMouseDown -= new MouseButtonEventHandler(OnPreviewMouseDown);
                                _adorner.MouseMove -= new MouseEventHandler(OnMouseMove);
                                _adorner.MouseUp -= new MouseButtonEventHandler(OnMouseUp);
                            }

                            control.MouseUp -= new MouseButtonEventHandler(OnMouseUp);
                            control.MouseMove -= new MouseEventHandler(OnMouseMove);

                            control.RemoveHandler(ListView.SizeChangedEvent, OnSizeChanged);

                            SetSelectionAdorner(p, null);
                        }

                    };

                Action attachAdorner = () =>
                {
                    unloadAdorner();
                    ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(control);
                    ScrollContentPresenter p = UITools.FindAncestor<ScrollContentPresenter>(ip);
                    if (p != null)
                    {
                        SelectionAdorner _adorner = new SelectionAdorner(p);
                        SetSelectionAdorner(p, _adorner);

                        AdornerLayer adornerLayer = getAdornerLayer(p);

                        if (adornerLayer != null)
                        {
                            adornerLayer.Add(_adorner);
                            _adorner.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                            _adorner.MouseMove += new MouseEventHandler(OnMouseMove);
                            _adorner.MouseUp += new MouseButtonEventHandler(OnMouseUp);
                        }

                        control.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                        control.MouseUp += new MouseButtonEventHandler(OnMouseUp);
                        control.MouseMove += new MouseEventHandler(OnMouseMove);

                        control.AddHandler(ListView.SizeChangedEvent, OnSizeChanged);

                        SetLastScrollContentPresenter(control, p);
                    }
                };

                if ((bool)e.NewValue == true)
                {
                    if (control.IsLoaded)
                        attachAdorner();
                    else
                        control.Loaded += delegate { attachAdorner(); };

                    control.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(OnScrollChange));
                    control.AddHandler(ListView.MouseDownEvent, new RoutedEventHandler(OnMouseDown));                    

                    //Monitor view change, and reattach handlers.
                    DependencyPropertyDescriptor viewDescriptor = DependencyPropertyDescriptor.FromProperty(ListView.ViewProperty, typeof(ListView));

                    viewDescriptor.AddValueChanged
                      (control, delegate
                      {
                          control.Dispatcher.BeginInvoke(DispatcherPriority.Input, attachAdorner);
                      });
                }
                else //If EnableSelection = False
                {
                    unloadAdorner();

                    control.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(OnScrollChange));
                    control.RemoveHandler(ListView.MouseDownEvent, new RoutedEventHandler(OnMouseDown));

                    SetSelectionAdorner(control, null);
                }

            }
        }
        #endregion

        #region Methods

        //static ListViewItem getSelectedItem(ScrollContentPresenter lvSender, Point position)
        //{
        //    HitTestResult r = VisualTreeHelper.HitTest(lvSender, position);

        //    if (r == null) return null;

        //    DependencyObject obj = r.VisualHit;
        //    while (!(obj is ListView) && (obj != null))
        //    {
        //        obj = VisualTreeHelper.GetParent(obj);

        //        if (obj is ListViewItem)
        //            return obj as ListViewItem;
        //    }

        //    return null;
        //}
        static ListViewItem getSelectedItem(Visual sender, Point position)
        {            
            DependencyObject obj = null;
            //Bug#66
            VisualTreeHelper.HitTest(
            sender,
            (o) =>
            {
                if (UITools.FindAncestor<ListViewItem>(o) != null)
                    return HitTestFilterBehavior.Continue;
                else return HitTestFilterBehavior.ContinueSkipSelf;
            },
            (r) =>
            {
                obj = r.VisualHit;
                return HitTestResultBehavior.Stop;
            },
            new PointHitTestParameters(position));
            //if (r == null) return null;

            //DependencyObject obj = r.VisualHit;
            while (!(obj is ListBox) && (obj != null))
            {
                obj = VisualTreeHelper.GetParent(obj);

                if (obj is ListViewItem)
                    return obj as ListViewItem;
            }

            return null;
        }

        static Point GetScrollbarPosition(ScrollContentPresenter p)
        {
            ScrollViewer scrollViewer = UITools.FindAncestor<ScrollViewer>(p);
            return new Point(p.ActualWidth / scrollViewer.ViewportWidth * scrollViewer.HorizontalOffset,
                p.ActualHeight / scrollViewer.ViewportHeight * scrollViewer.VerticalOffset);
        }

        static void BeginDragging(ScrollContentPresenter p)
        {
            SetStartScrollbarPosition(p, GetScrollbarPosition(p));
            SetIsDragging(p, true);
        }

        internal static object PositionCheck(DependencyObject sender, object value)
        {
            if (value is Point)
            {
                Point ptValue = (Point)value;
                SelectionAdorner adorner = sender as SelectionAdorner;
                ptValue.X = Math.Max(ptValue.X, 0);
                ptValue.X = Math.Min(ptValue.X, adorner.ActualWidth);
                ptValue.Y = Math.Max(ptValue.Y, 0);
                ptValue.Y = Math.Min(ptValue.Y, adorner.ActualHeight);
                return ptValue;
            }
            return value;
        }

        static void UpdateSelectedItems(ListView lvControl, IList<object> newList)
        {
            ClearSelection(lvControl);

            List<object> addList = new List<object>();
            List<object> removeList = new List<object>(lvControl.SelectedItems as IList<object>);

            foreach (object obj in newList)
                if (removeList.Contains(obj))
                    removeList.Remove(obj);
                else addList.Add(obj);

            foreach (object obj in removeList)
                lvControl.SelectedItems.Remove(obj);
            foreach (object obj in addList)
                lvControl.SelectedItems.Add(obj);
        }


        static void HighlightItems(ListView lvControl, IList<int> newList)
        {
            for (int i = 0; i < lvControl.Items.Count; i++)
            {
                ListViewItem item = lvControl.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (item != null)
                    SetIsDragging(item, newList.Contains(i));
            }
        }

        static void UnhighlightItems(ListView lvControl)
        {
            for (int i = 0; i < lvControl.Items.Count; i++)
            {
                ListViewItem item = lvControl.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (item != null)
                    SetIsDragging(item, false);
            }
        }

        static void UpdateSelection(ScrollContentPresenter p, Rect selectionBounds, bool highlightOnly)
        {
            try
            {
                ListView lvControl = UITools.FindAncestor<ListView>(p);
                if (lvControl != null)
                {
                    IChildInfo icInfo = UITools.FindVisualChild<Panel>(p) as IChildInfo;

                    List<object> newList = new List<object>();
                    List<int> newIntList = new List<int>();

                    if (icInfo != null)
                    {


                        for (int i = 0; i < lvControl.Items.Count; i++)
                            if (icInfo.GetChildRect(i).IntersectsWith(selectionBounds))
                            {
                                newList.Add(lvControl.Items[i]);
                                newIntList.Add(i);
                            }


                    }
                    else
                        //0.4 For GridView, only support selection if drag occur inside the first column
                        if (lvControl.View is GridView)
                        {
                            Point curPos = (Point)PositionCheck(GetSelectionAdorner(p), Mouse.GetPosition(p));

                            if ((lvControl.View as GridView).Columns.Count > 0)
                            {
                                double col0width = (lvControl.View as GridView).Columns[0].ActualWidth;
                                if (curPos.X < col0width || GetStartPosition(p).X < col0width)
                                {
                                    if (_firstSelectedItem == null)
                                        _firstSelectedItem = getSelectedItem(p, curPos);
                                    if (getSelectedItem(p, curPos) != null)
                                        _curSelectedItem = getSelectedItem(p, curPos);

                                    if (_firstSelectedItem != null && _curSelectedItem != null)
                                    {

                                        int startIdx = lvControl.ItemContainerGenerator.IndexFromContainer(_firstSelectedItem);
                                        int endIdx = lvControl.ItemContainerGenerator.IndexFromContainer(_curSelectedItem);


                                        for (int i = Math.Min(startIdx, endIdx); i <= Math.Max(startIdx, endIdx); i++)
                                        {
                                            newList.Add(lvControl.Items[i]);
                                            newIntList.Add(i);
                                        }
                                    }
                                }
                            }
                        }

                    if (highlightOnly)
                    {
                        SetHighlightCount(lvControl, newIntList.Count);
                        HighlightItems(lvControl, newIntList);
                    }
                    else
                    {
                        SetHighlightCount(lvControl, 0);
                        UnhighlightItems(lvControl);
                        UpdateSelectedItems(lvControl, newList);
                    }

                }
            }
            catch { }
        }

        static void UpdatePosition(ScrollContentPresenter p, bool highlightOnly)
        {
            ScrollViewer scrollViewer = UITools.FindAncestor<ScrollViewer>(p);
            SelectionAdorner _adorner = GetSelectionAdorner(p);

            if (GetIsDragging(p))
            {
                Point startScrollbarPosition = GetStartScrollbarPosition(p);
                Point curScrollbarPosition = GetScrollbarPosition(p);
                Point startPosition = GetStartPosition(p);
                Point curPosition = Mouse.GetPosition(p);

                if (!_adorner.IsSelecting)
                {
                    if (Math.Abs(startPosition.X - curPosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(startPosition.Y - curPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        _adorner.IsSelecting = true;
                        Mouse.Capture(p);
                    }
                }
                else
                {
                    Vector offset = Point.Subtract(startScrollbarPosition, curScrollbarPosition);
                    _adorner.StartPosition = Point.Add(startPosition, offset);
                    _adorner.EndPosition = curPosition;
                    UpdateSelection(p, new Rect(
                        new Point(startPosition.X + startScrollbarPosition.X, startPosition.Y + startScrollbarPosition.Y),
                        new Point(curPosition.X + curScrollbarPosition.X, curPosition.Y + curScrollbarPosition.Y)), highlightOnly);
                }
            }
        }

        static void EndDragging(ScrollContentPresenter p)
        {
            SelectionAdorner _adorner = GetSelectionAdorner(p);
            if (_adorner.IsSelecting)
            {
                UpdatePosition(p, false);
                _adorner.IsSelecting = false;
                SetIsDragging(p, false);
            }
        }

        static bool _itemAlreadySelected = false;
        static ListViewItem _itemUnderMouse = null;
        static ListViewItem _firstSelectedItem = null;
        static ListViewItem _curSelectedItem = null;

        static ScrollContentPresenter getScrollContentPresenter(object sender)
        {
            if (sender is ListView)
            {
                ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(sender as ListView);
                return UITools.FindAncestor<ScrollContentPresenter>(ip);
            }
            else
                if (sender is SelectionAdorner)
                    return (ScrollContentPresenter)((SelectionAdorner)sender).AdornedElement;
                else return (ScrollContentPresenter)sender;
        }

        static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.ClickCount > 1)
                return;

            //DependencyObject obj = (DependencyObject)e.OriginalSource;
            //while (obj != null)
            //{
            //    Debug.WriteLine(obj);
            //    obj = VisualTreeHelper.GetParent(obj);
            //}
            //0.4: Fixed click on GridView Header recognize as drag start.
            bool isOverGridViewHeader = UITools.FindAncestor<GridViewColumnHeader>(e.OriginalSource as DependencyObject) != null;
            bool isOverScrollBar = UITools.FindAncestor<ScrollBar>(e.OriginalSource as DependencyObject) != null;

            bool spButtonPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            ScrollContentPresenter p = getScrollContentPresenter(sender);
            _itemUnderMouse = getSelectedItem(p, e.GetPosition(p));
            //Debug.WriteLine(_itemUnderMouse == null ? "NONE" : _itemUnderMouse.ToString());
            if (!spButtonPressed && !isOverScrollBar && !isOverGridViewHeader)
            {
                SetStartPosition(p, Mouse.GetPosition(p));

                _firstSelectedItem = _itemUnderMouse;
                _itemAlreadySelected = (_itemUnderMouse != null && _itemUnderMouse.IsSelected);


                if (!_itemAlreadySelected && e.ClickCount == 1)
                {
                    BeginDragging(p);
                    e.Handled = true;
                }
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ListView lvSender = UITools.FindAncestor<ListView>(sender as DependencyObject);
                int startIdx = lvSender.SelectedIndex;
                int endIdx = lvSender.Items.Count - 1;
                if (_itemUnderMouse != null)
                    endIdx = lvSender.ItemContainerGenerator.IndexFromContainer(_itemUnderMouse);
                if (startIdx != -1 && endIdx != -1)
                {
                    ClearSelection(lvSender);
                    for (int i = Math.Min(startIdx, endIdx); i <= Math.Max(startIdx, endIdx); i++)
                        lvSender.SelectedItems.Add(lvSender.Items[i]);
                    e.Handled = true;
                }

            }
        }

        //0.4: If the ListView is Virtual and ListViewItem.IsSelected is bound to a ViewModel's property (e.g. IsSelected), 
        //     you have to implement IVirtualListView in your ListView, which clear your ViewModel's IsSelected property.
        static void ClearSelection(ListView lvSender)
        {
            if (lvSender is IVirtualListView)
                (lvSender as IVirtualListView).UnselectAll();
            else lvSender.SelectedItems.Clear();
        }

        static void OnMouseMove(object sender, MouseEventArgs e)
        {
            ScrollContentPresenter p = getScrollContentPresenter(sender);

            if (GetIsDragging(p))
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //0.5: Auto Scroll
                    Point mousePos = e.GetPosition(p);
                    IScrollInfo isInfo = UITools.FindVisualChild<Panel>(p) as IScrollInfo;
                    ListView lv = UITools.FindAncestor<ListView>(p);

                    if (isInfo.CanHorizontallyScroll)
                        if (mousePos.X < 0)
                            isInfo.SetHorizontalOffset(isInfo.HorizontalOffset - 1);
                        else if (mousePos.X > lv.ActualWidth)
                            isInfo.SetHorizontalOffset(isInfo.HorizontalOffset + 1);
                    if (isInfo.CanVerticallyScroll)
                        if (mousePos.Y < 0)
                            isInfo.SetVerticalOffset(isInfo.VerticalOffset - 1);
                        else if (mousePos.Y > lv.ActualHeight) //isInfo.ViewportHeight is bugged.
                            isInfo.SetVerticalOffset(isInfo.VerticalOffset + 1);

                    UpdatePosition(p, true);
                }
        }

        static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ScrollContentPresenter p = getScrollContentPresenter(sender);

            if (e.ChangedButton != MouseButton.Left)
                return;

            if (GetIsDragging(p))
            {
                EndDragging(p);
                Mouse.Capture(null);
                UITools.FindAncestor<ListView>(p).Focus();

                ListViewItem curSelectedItem = getSelectedItem(p, Mouse.GetPosition(p));
                if (curSelectedItem != null && curSelectedItem.Equals(_itemUnderMouse))
                {
                    ListView lvControl = UITools.FindAncestor<ListView>(p);
                    ClearSelection(lvControl);
                    _itemUnderMouse.IsSelected = true;
                }
            }

            if (Point.Subtract(GetStartPosition(p), Mouse.GetPosition(p)).Length < 10.0d)
            {
                ListView lvControl = UITools.FindAncestor<ListView>(p);
                ClearSelection(lvControl);
            }

            if (_firstSelectedItem != null)
                _firstSelectedItem.IsSelected = true;

            _firstSelectedItem = null;
            _curSelectedItem = null;
            SetIsDragging(p, false);


        }

        #endregion

        #region Data


        #endregion
    }
}
