using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public enum ViewMode : int
    {
        vmTile = 13,
        vmGrid = 14,
        vmList = 15,
        vmSmallIcon = 16,
        vmIcon = 48,
        vmLargeIcon = 80,
        vmExtraLargeIcon = 120,
        vmViewer = 256
    }

    public class FileList2 : ListView, IVirtualListView
    {
        private class fileListItemStyleSelector : StyleSelector
        {
            public override Style SelectStyle(object item, DependencyObject container)
            {
                FileList2 flist = UITools.FindAncestor<FileList2>(container);
                if (flist != null)
                {
                    if (!(flist.View is GridView))
                        return (Style)flist.FindResource("qz_FileListItem_Style");
                }
                return base.SelectStyle(item, container);
            }
        }

        public FileList2()
        {
            //<Setter Property="SelectedItems" Value="{Binding SelectedItems, Mode=OneWayToSource}" />
            this.ItemContainerStyleSelector = new fileListItemStyleSelector();


            this.AddHandler(FileList2.SelectionChangedEvent, (RoutedEventHandler)((o, e) =>
                {
                    BindableSelectedItems = SelectedItems;

                }));

            this.AddHandler(UIElement.KeyDownEvent, (KeyEventHandler)delegate(object sender, KeyEventArgs args)
            {
                switch (args.Key)
                {
                    case Key.F2 :
                        if (RenameCommand != null && RenameCommand.CanExecute(DataContext))
                            RenameCommand.Execute(DataContext);
                        break;
                }
                    
            });


            this.AddHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
            {
                if (args.OriginalSource is GridViewColumnHeader)
                {
                    GridViewColumnHeader header = (GridViewColumnHeader)args.OriginalSource;
                    if (header.Column != null)
                    {
                        SortCriteria columnName = GetSortPropertyName(header.Column);

                        //if (string.IsNullOrEmpty(columnName))
                        //    return;

                        if (SortBy != columnName)
                            SetValue(SortByProperty, columnName);
                        else
                            if (SortDirection == ListSortDirection.Ascending)
                                SetValue(SortDirectionProperty, ListSortDirection.Descending);
                            else SetValue(SortDirectionProperty, ListSortDirection.Ascending);

                        UpdateCollumnHeader();
                    }
                }
            });



        }


        #region Methods


        protected override DependencyObject GetContainerForItemOverride()
        {
            ListViewItem item = new ListViewItem();
         
            if (ViewMode == ViewMode.vmGrid)
                item.Style = (Style)this.FindResource("qz_GridFileListItem_Style");
            return item;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SelectionHelper.SetEnableSelection(this, true);

            UpdateCollumnHeader();


            #region Update scroll position if scroll bar disappear
            ScrollViewer scrollView = UITools.FindVisualChild<ScrollViewer>(this);

            DependencyPropertyDescriptor vertScrollbarVisibilityDescriptor =
                DependencyPropertyDescriptor.FromProperty(
                ScrollViewer.ComputedVerticalScrollBarVisibilityProperty, typeof(ScrollViewer));

            vertScrollbarVisibilityDescriptor.AddValueChanged
                      (scrollView, delegate
                      {
                          if (scrollView.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Collapsed)
                          {
                              VirtualizingPanel panel = UITools.FindVisualChild<VirtualizingPanel>(this);
                              if (panel is IScrollInfo)
                              {
                                  (panel as IScrollInfo).SetVerticalOffset(0);
                              }
                          }
                      });

            DependencyPropertyDescriptor horzScrollbarVisibilityDescriptor =
                DependencyPropertyDescriptor.FromProperty(
                ScrollViewer.ComputedHorizontalScrollBarVisibilityProperty, typeof(ScrollViewer));

            horzScrollbarVisibilityDescriptor.AddValueChanged
                      (scrollView, delegate
                      {
                          if (scrollView.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Collapsed)
                          {
                              VirtualizingPanel panel = UITools.FindVisualChild<VirtualizingPanel>(this);
                              if (panel is IScrollInfo)
                              {
                                  (panel as IScrollInfo).SetHorizontalOffset(0);
                              }
                          }
                      });


            #endregion

            #region ContextMenu

            Point mouseDownPt = new Point(-100,-100);

            this.AddHandler(TreeViewItem.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(
                 (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
                 {
                     mouseDownPt = args.GetPosition(this);                        
                }));
            this.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
                (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
                {
                    Point mouseUpPt = args.GetPosition(this);

                    if ((Math.Abs(mouseDownPt.X - mouseUpPt.X) < 5) &&
                        (Math.Abs(mouseDownPt.Y - mouseUpPt.Y) < 5))
                    {
                        args.Handled = true;
                        if (ContextMenuCommand != null && ContextMenuCommand.CanExecute(this.DataContext))
                            if (SelectedValue != null)
                                ContextMenuCommand.Execute(this.DataContext);
                    }
                }));

            #endregion
            
            //Memory Leak
            //Unloaded += (o, e) =>
            //{
            //    SelectionHelper.SetEnableSelection(o as FileList2, false);
            //    (o as FileList2).View = null;

            //};

            
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);


        }

        public void UpdateCollumnHeader()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            GridViewColumnCollection collection = null;
            if (View is GridView)

                collection = (View as GridView).Columns;
            else
                if (View is VirtualWrapPanelView)
                    collection = (View as VirtualWrapPanelView).Columns;
                else if (View is VirtualStackPanelView)
                    collection = (View as VirtualStackPanelView).Columns;

            foreach (GridViewColumn col in collection)
            {
                SortCriteria colHeader = GetSortPropertyName(col);
                if (colHeader != SortBy)
                    col.HeaderTemplate = this.TryFindResource("NormHeaderTemplate") as DataTemplate;
                else
                    if (SortDirection == ListSortDirection.Ascending)
                        col.HeaderTemplate = this.TryFindResource("AscHeaderTemplate") as DataTemplate;
                    else col.HeaderTemplate = this.TryFindResource("DescHeaderTemplate") as DataTemplate;
            }
        }


        public static void OnViewModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!args.NewValue.Equals(args.OldValue))
            {
                FileList2 fl = (FileList2)sender;
                ViewMode viewMode = (ViewMode)args.NewValue;
                if (viewMode == 0)
                    return;

                string viewName = (viewMode.ToString()).Substring(2) + "View";
                switch (viewMode)
                {
                    case ViewMode.vmExtraLargeIcon:
                    case ViewMode.vmLargeIcon:
                        viewName = "IconView";
                        break;
                }
                ViewBase view = (ViewBase)(fl.TryFindResource(viewName)); ;

                if (fl.ViewSize < (int)viewMode)
                    fl.ViewSize = (int)viewMode;

                if (view != null)
                    fl.View = view;
                fl.UpdateCollumnHeader();
            }
        }



        public static void OnViewSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FileList2 flist = sender as FileList2;
            int newSize = (int)args.NewValue;

            ViewMode newViewMode = flist.ViewMode;
            switch (newSize)
            {
                case 13: newViewMode = ViewMode.vmTile; break;
                case 14: newViewMode = ViewMode.vmGrid; break;
                case 15: newViewMode = ViewMode.vmList; break;
                case 16: newViewMode = ViewMode.vmSmallIcon; break;
                case 256: newViewMode = ViewMode.vmViewer; break;
                default:
                    if (newSize <= 16) newViewMode = ViewMode.vmSmallIcon;
                    else
                        if (newSize <= 48)
                            newViewMode = ViewMode.vmIcon;
                        else if (newSize <= 80)
                            newViewMode = ViewMode.vmLargeIcon;
                        else newViewMode = ViewMode.vmExtraLargeIcon;
                    break;
            }

            if (newViewMode != flist.ViewMode)
                flist.ViewMode = newViewMode;


        }

        #endregion

        #region Public Properties

        #region SortPropertyName (attached property)
        public static readonly DependencyProperty SortPropertyNameProperty = DependencyProperty.RegisterAttached("SortPropertyName",
            typeof(SortCriteria), typeof(FileList2));

        public static SortCriteria GetSortPropertyName(DependencyObject sender)
        {
            return (SortCriteria)sender.GetValue(SortPropertyNameProperty);
        }

        public static void SetSortPropertyName(DependencyObject sender, SortCriteria value)
        {
            sender.SetValue(SortPropertyNameProperty, value);
        }
        #endregion

        #region IsEditing (attached property)
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached("IsEditing", typeof(bool), typeof(FileList2));

        public static bool GetIsEditing(DependencyObject sender)
        {
            return (bool)sender.GetValue(IsEditingProperty);
        }

        public static void SetIsEditing(DependencyObject sender, bool value)
        {
            sender.SetValue(IsEditingProperty, value);
        }
        #endregion


        #region SortBy, SortDirection
        public static readonly DependencyProperty SortByProperty =
         DependencyProperty.Register("SortBy", typeof(SortCriteria), typeof(FileList2),
         new FrameworkPropertyMetadata(SortCriteria.sortByFullName, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SortCriteria SortBy
        {
            get { return (SortCriteria)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        public static readonly DependencyProperty SortDirectionProperty =
          DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(FileList2),
          new FrameworkPropertyMetadata(ListSortDirection.Ascending, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }
        #endregion


        #region ViewMode, ViewSize
        public static readonly DependencyProperty ViewModeProperty =
          DependencyProperty.Register("ViewMode", typeof(ViewMode), typeof(FileList2),
          new FrameworkPropertyMetadata(ViewMode.vmGrid, new PropertyChangedCallback(OnViewModeChanged)));

        public ViewMode ViewMode
        {
            get { return (ViewMode)GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        public static readonly DependencyProperty ViewSizeProperty =
            DependencyProperty.Register("ViewSize", typeof(int), typeof(FileList2),
            new FrameworkPropertyMetadata(16, new PropertyChangedCallback(OnViewSizeChanged)));

        public int ViewSize
        {
            get { return (int)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }
        #endregion

        #region BindableSelectedItems

        public IList BindableSelectedItems
        {
            get { return (IList)GetValue(BindableSelectedItemsProperty); }
            set { SetValue(BindableSelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindableSelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.Register("BindableSelectedItems", typeof(IList), typeof(FileList2));
        #endregion




        public ICommand RenameCommand
        {
            get { return (ICommand)GetValue(RenameCommandProperty); }
            set { SetValue(RenameCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenameCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenameCommandProperty =
            DependencyProperty.Register("RenameCommand", typeof(ICommand), typeof(FileList2), new UIPropertyMetadata(null));

        

        public ICommand ContextMenuCommand
        {
            get { return (ICommand)GetValue(ContextMenuCommandProperty); }
            set { SetValue(ContextMenuCommandProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuCommandProperty =
            DependencyProperty.Register("ContextMenuCommand", typeof(ICommand), typeof(FileList2), new UIPropertyMetadata(null));

       
        public ICommand UnSelectAllCommand
        {
            get { return (ICommand)GetValue(UnSelectAllCommandProperty); }
            set { SetValue(UnSelectAllCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnSelectAllCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnSelectAllCommandProperty =
            DependencyProperty.Register("UnSelectAllCommand", typeof(ICommand), typeof(FileList2), new UIPropertyMetadata(null));

        void IVirtualListView.UnselectAll()
        {
            if (UnSelectAllCommand != null)
                UnSelectAllCommand.Execute(this);
            this.SelectedItems.Clear();
        }

        #endregion



       
    }
}
