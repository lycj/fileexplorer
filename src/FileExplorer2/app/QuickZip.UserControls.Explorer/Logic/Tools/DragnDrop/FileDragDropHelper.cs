using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics;
using System.IO.Tools;
using System.Windows.Documents;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    public static class FileDragDropHelper<T>
    {
        #region Attached Properties

        public static readonly DependencyProperty DragAdornerProperty =
            DependencyProperty.RegisterAttached("DragAdorner", typeof(DragAdorner<T>), typeof(FileDragDropHelper<T>), new UIPropertyMetadata(null));

        private static DependencyProperty IsDraggingProperty =
          DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(FileDragDropHelper<T>));

        public static DependencyProperty EnableDragProperty =
           DependencyProperty.RegisterAttached("EnableDrag", typeof(bool), typeof(FileDragDropHelper<T>),
           new PropertyMetadata(OnEnableDragChanged));

        public static DependencyProperty EnableDropProperty =
            DependencyProperty.RegisterAttached("EnableDrop", typeof(bool), typeof(FileDragDropHelper<T>),
            new PropertyMetadata(OnEnableDropChanged));


        public static readonly DependencyProperty DragItemTemplateProperty =
            DependencyProperty.RegisterAttached("DragItemTemplate", typeof(DataTemplate), typeof(FileDragDropHelper<T>),
            new UIPropertyMetadata(null, OnDragItemTemplateChanged));



        public static DragAdorner<T> GetDragAdorner(DependencyObject obj)
        {
            return (DragAdorner<T>)obj.GetValue(DragAdornerProperty);
        }

        public static void SetDragAdorner(DependencyObject obj, DragAdorner<T> value)
        {
            obj.SetValue(DragAdornerProperty, value);
        }

        public static bool GetIsDragging(DependencyObject target)
        {
            return (bool)target.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(DependencyObject target, bool value)
        {
            target.SetValue(IsDraggingProperty, value);
        }

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

        public static DataTemplate GetDragItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragItemTemplateProperty);
        }

        public static void SetDragItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragItemTemplateProperty, value);
        }


        #endregion

        #region Methods


        private static Window getParentWindow(DependencyObject s)
        {
            Window window = UITools.FindAncestor<Window>(s);
            return window;
        }

        private static DragAdorner<T> getDragDropAdorner(DependencyObject s, bool createIfNotExists = true)
        {
            Window parentWindow = Window.GetWindow(s);
            if (parentWindow != null)
            {
                AdornerDecorator decorator = UITools.FindVisualChildByName<AdornerDecorator>
                    (parentWindow, "PART_DragDropAdorner");

                if (decorator == null)
                {
                    throw new ArgumentException("PART_DragDropAdorner not found, or not an AdornerDecorator");
                }
                else
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(decorator);

                    Adorner[] adorners = adornerLayer.GetAdorners(adornerLayer);
                    if (adorners != null)
                        foreach (var adorner in adorners)
                            if (adorner is DragAdorner<T>)
                                return adorner as DragAdorner<T>;

                    if (createIfNotExists)
                    {
                        DragAdorner<T> newAdorner = new DragAdorner<T>(adornerLayer);
                        adornerLayer.Add(newAdorner);
                        return newAdorner;
                    }
                }
            }
            return null;
        }

        public static void OnEnableDragChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == EnableDragProperty)
            {
                if ((bool)e.NewValue)
                {
                    if (s is Selector || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
                        ele.PreviewMouseUp += new MouseButtonEventHandler(OnMouseUp);
                        ele.MouseMove += new MouseEventHandler(OnMouseMove);

                        DragAdorner<T> adorner = getDragDropAdorner(s);
                        SetDragAdorner(s, adorner);
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
            }
        }
        
        public static void OnEnableDropChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == EnableDropProperty)
            {
                if ((bool)e.NewValue)
                {
                    if (s is Selector || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.AllowDrop = true;
                        ele.DragOver += new DragEventHandler(OnDragOver);
                        ele.DragLeave += new DragEventHandler(OnDragLeave);                        
                        ele.Drop += new DragEventHandler(OnDrop);

                        DragAdorner<T> adorner = getDragDropAdorner(s);
                        SetDragAdorner(s, adorner);
                    }
                    else throw new ArgumentException("Support ListView or TreeView only.");
                }
                else
                {
                    if (s is ListBox || s is TreeView)
                    {
                        UIElement ele = s as UIElement;
                        ele.DragOver -= new DragEventHandler(OnDragOver);
                        ele.DragLeave -= new DragEventHandler(OnDragLeave);                        
                        ele.Drop += new DragEventHandler(OnDrop);
                    }
                }
            }
        }

        public static void OnDragItemTemplateChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is Selector || s is TreeView)
            {
                DragAdorner<T> adorner = getDragDropAdorner(s);
                adorner.DraggingItemTemplate = e.NewValue as DataTemplate;
            }
        }


        #endregion

        #region Methods - Drag

        private static void SetDraggingItems(FrameworkElement control, T[] items)
        {
            DragAdorner<T> dragAdorner = GetDragAdorner(control);
            if (dragAdorner != null)
                dragAdorner.DraggingItems = items;
        }

        private static void HideAdorner(FrameworkElement control)
        {
            _isAdornerVisible = false;
            //Debug.WriteLine("HideAdorner");
            DragAdorner<T> dragAdorner = GetDragAdorner(control);
            if (dragAdorner != null)
                dragAdorner.IsDragging = false;
        }

        private static void ShowAdorner(FrameworkElement control)
        {
            _isAdornerVisible = true;
            //Debug.WriteLine("ShowAdorner");
            DragAdorner<T> dragAdorner = GetDragAdorner(control);
            if (dragAdorner != null)
                dragAdorner.IsDragging = true;
        }

        private static void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            //ESC pressed
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                control.AllowDrop = true;

                //control.QueryContinueDrag -= new QueryContinueDragEventHandler(OnQueryContinueDrag);
                HideAdorner(control);
            }
            else
                //Drop!
                if (e.KeyStates == DragDropKeyStates.None)
                {
                    _dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 0);
                    e.Action = DragAction.Drop;
                    control.AllowDrop = true;

                    //control.QueryContinueDrag -= new QueryContinueDragEventHandler(OnQueryContinueDrag);
                    HideAdorner(control);
                }
                else
                    e.Action = DragAction.Continue;

            e.Handled = true;
            //Debug.WriteLine(e.Action);
            //base.OnQueryContinueDrag(e);
        }


        /// <summary>
        /// Check if drag enabled for the listview or treeview.
        /// 1. Implement ISupportDrag interface
        /// 2. SupportedDragDropEffedt != None
        /// 3. Have items selected, and mouse over selected item
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static bool isDragEnabled(FrameworkElement control)
        {
            if (control.DataContext is ISupportDrag<T>)
            {
                ISupportDrag<T> iSupportDrag = control.DataContext as ISupportDrag<T>;
                if (iSupportDrag != null && iSupportDrag.SupportedDragDropEffects != DragDropEffects.None)
                {
                    if ((control is ListBox) &&
                        (UITools.IsMouseOverSelectedItem(control as ListBox) && !UITools.IsMouseOverScrollbar(control)))
                        return true;
                    if (control is TreeView && (control as TreeView).SelectedValue != null)
                        return true;

                }
            }
            return false;
        }


        static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //The dragging element (should be T)
            FrameworkElement sourceElement = e.OriginalSource as FrameworkElement;
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;


            _startPoint = _invalidPoint;
            SetIsDragging(control, false);

            if (isDragEnabled(control) && UITools.IsMouseOverSelectedItem(control))
            {
                _startPoint = e.GetPosition(null);
                //e.Handled = true;
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            //The dragging element (should be T)
            FrameworkElement sourceElement = e.OriginalSource as FrameworkElement;
            //Debug.Assert(sourceElement.DataContext is ISupportDrag<T>);
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            if ((e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                && !GetIsDragging(control))
            {
                Point position = e.GetPosition(null);

                if (!e.MouseDevice.DirectlyOver.Focusable)
                    if (isDragEnabled(control))
                    {
                        _dragSource = control.DataContext as ISupportDrag<T>;

                        if (_dragSource != null)
                            if ((_dragSource.SupportedDragDropEffects & DragDropEffects.Copy) != 0 ||
                                (_dragSource.SupportedDragDropEffects & DragDropEffects.Link) != 0)
                                if (!_startPoint.Equals(_invalidPoint)) //Bug#65
                                    if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                        Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                                        try
                                        {
                                            _dataObj = new VirtualDataObject<T>(_dragSource);
                                            if (_dragSource.BeforeDrag(_dataObj.DropInfo))
                                            {
                                                SetIsDragging(control, true);

                                                var draggingItems = from si in _dataObj.DropInfo.SelectedItems select si.EmbeddedItem;
                                                SetDraggingItems(control, draggingItems.ToArray());
                                                ShowAdorner(control);

                                                if (_dataObj.PrepareDataObject())
                                                {
                                                    //control.QueryContinueDrag += new QueryContinueDragEventHandler(OnQueryContinueDrag);
                                                    System.Windows.DragDrop.AddQueryContinueDragHandler(control,
                                                        new QueryContinueDragEventHandler(OnQueryContinueDrag));

                                                    DragDropEffects de =
                                                        System.Windows.DragDrop.DoDragDrop(control,
                                                        _dataObj, _dataObj.DropInfo.SupportedEffects);
                                                    e.Handled = true;

                                                    System.Windows.DragDrop.RemoveQueryContinueDragHandler(control,
                                                      new QueryContinueDragEventHandler(OnQueryContinueDrag));
                                                }
                                            }
                                        }
                                        catch (Exception ex) { Trace.WriteLine("FileDragDropHelper.OnMouseMove Failed - " + ex.Message); }
                                        finally { _dataObj = null; _dragSource = null; }
                    }

            };
        }

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //The dragging element (should be T)
            FrameworkElement sourceElement = e.OriginalSource as FrameworkElement;
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            if (GetIsDragging(control))
            {
                Point position = e.GetPosition(null);
                SetIsDragging(control, false);

                HideAdorner(control);
            }

            //SetIsDragOver(null);
        }

        #endregion

        #region Methods - Drop

        static IDropTarget<T> locateDropTarget(FrameworkElement control)
        {
            ISupportDrop<T> iSupportDrop = control.DataContext as ISupportDrop<T>;
            if (iSupportDrop != null)
                return iSupportDrop.CurrentDropTarget;
            return null;
        }

        public static Func<string, T> ConstructItem = null;        

        static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Scroll;
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            DragAdorner<T> dragAdorner = GetDragAdorner(control);
            if (dragAdorner != null)
                dragAdorner.PointerPosition = e.GetPosition(dragAdorner);

            if (control.AllowDrop)
            {
                IDropTarget<T> dropTarget = locateDropTarget(control);
                if (dropTarget != null && !(_dragSource != null && _dragSource.Equals(dropTarget)))
                {
                    if (dropTarget.IsDropEnabled)
                    {                       
                        if (!e.Handled && _dataObj != null && _dataObj.IsSameDataObject(e.Data))
                        {
                            e.Effects |= dropTarget.QueryDrop(e.AllowedEffects, _dataObj.DropInfo);

                            if (!_isAdornerVisible && ConstructItem != null && e.Effects != DragDropEffects.Scroll)
                            {
                                var draggingItems = from si in _dataObj.DropInfo.SelectedItems select si.EmbeddedItem;
                                SetDraggingItems(control, draggingItems.Take(5).ToArray());
                                ShowAdorner(control);
                            }

                            e.Handled = true;

                        }

                        if (!e.Handled && e.Data.GetDataPresent(DataFormats.FileDrop))
                        {
                            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            e.Effects |= dropTarget.QueryDrop(e.AllowedEffects, files);

                            if (!_isAdornerVisible && ConstructItem != null && e.Effects != DragDropEffects.Scroll)
                            {
                                var draggingItems = from file in files select ConstructItem(file);
                                SetDraggingItems(control, draggingItems.Take(5).ToArray());
                                ShowAdorner(control);
                            }


                            e.Handled = true;
                        }
                    }
                }
            }

            //if (e.Effects == DragDropEffects.Scroll)
            //    HideAdorner(control);
            e.Handled = true;
        }

        static void OnDragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            //HideAdorner(control);
        }


        static void OnDrop(object sender, DragEventArgs e)
        {
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            DragAdorner<T> dragAdorner = GetDragAdorner(control);
           
            HideAdorner(control);
            if (control.AllowDrop)
            {              
                IDropTarget<T> dropTarget = locateDropTarget(control);
                if (dropTarget != null && !(_dragSource != null && _dragSource.Equals(dropTarget)))
                {
                    if (dropTarget.IsDropEnabled)
                    {                        
                        if (!e.Handled && _dataObj != null && _dataObj.IsSameDataObject(e.Data))
                        {
                            DragDropEffects effects = dropTarget.QueryDrop(e.AllowedEffects, _dataObj.DropInfo);
                            if ((effects & DragDropEffects.Copy) != 0 || (effects & DragDropEffects.Link) != 0)
                                dropTarget.Drop(e.AllowedEffects, _dataObj.DropInfo);
                            e.Handled = true;
                        }

                        if (!e.Handled && e.Data.GetDataPresent(DataFormats.FileDrop))
                        {
                            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            DragDropEffects effects = dropTarget.QueryDrop(e.AllowedEffects, files);
                            if ((effects & DragDropEffects.Copy) != 0 || (effects & DragDropEffects.Link) != 0)
                                dropTarget.Drop(e.AllowedEffects, files);
                            e.Handled = true;
                        }
                    }

                }

            }
        }

        #endregion

        #region Data

        private static bool _isAdornerVisible = false;
        private static Point _startPoint;
        private static Point _invalidPoint = new Point(int.MinValue, int.MinValue);
        private static VirtualDataObject<T> _dataObj = null;
        private static ISupportDrag<T> _dragSource;

        #endregion

        #region Public Properties

        #endregion


    }
}
