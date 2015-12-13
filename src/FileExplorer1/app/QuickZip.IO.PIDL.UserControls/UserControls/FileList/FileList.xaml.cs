using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using QuickZip.IO.PIDL.UserControls.Model;
using System.ComponentModel;
using System.Diagnostics;
using QuickZip.IO.PIDL.Tools;
using QuickZip.UserControls;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO.Tools;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace QuickZip.IO.PIDL.UserControls
{
    public enum ViewMode : int
    {
        vmTile = 13,
        vmGrid = 14,
        vmList = 15,
        vmSmallIcon = 16,
        vmIcon = 48,
        vmLargeIcon = 80,
        vmExtraLargeIcon = 120
    }

    /// <summary>
    /// Interaction logic for FileList.xaml
    /// </summary>
    public partial class FileList : ListView, IVirtualListView
    {

        #region Constructor
        public FileList()
        {
            ModelToExConverter = new ModelToExConverter();
            ExModelToIconConverter = new ExModelToIconConverter();
            DataContext = RootModel = new FileListViewModel();
            Commands = new FileListCommands(this, RootModel);         
            
            InitializeComponent();
              
            //0.2
            Binding isLoadingBinding = new Binding("IsLoading");
            isLoadingBinding.Source = RootModel;
            this.SetBinding(IsLoadingProperty, isLoadingBinding);

            Binding sortByBinding = new Binding("SortBy");
            sortByBinding.Source = RootModel;
            sortByBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(SortByProperty, sortByBinding);

            Binding sortDirectionBinding = new Binding("SortDirection");
            sortDirectionBinding.Source = RootModel;
            sortDirectionBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(SortDirectionProperty, sortDirectionBinding);
            UpdateCollumnHeader();

            RootModel.OnProgress += (ProgressEventHandler)delegate(object sender, ProgressEventArgs e)
            {
                RaiseEvent(new ProgressRoutedEventArgs(ProgressEvent, e));
            };

            this.AddHandler(ListView.KeyDownEvent, (KeyEventHandler)delegate(object sender, KeyEventArgs args)
            {
                if ((args as RoutedEventArgs).OriginalSource == this)
                {
                    if ((int)args.Key >= (int)Key.A && (int)args.Key <= (int)Key.Z)
                    {
                        _lookupAdorner.UpdateVisibilty(true);
                        //_lookupAdorner.Focus();
                        Keyboard.Focus(_lookupAdorner);

                        //_lookupAdorner.Text = args.Key.ToString().ToLower();
                    }

                    if (args.Key == Key.Escape)
                        _lookupAdorner.UpdateVisibilty(false);

                }
                //if (args.Key == Key.F3)
                //    _lookupAdorner.UpdateVisibilty(true);

            });

            //0.2
            this.AddHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
            {
                if (args.OriginalSource is GridViewColumnHeader)
                {
                    GridViewColumnHeader header = (GridViewColumnHeader)args.OriginalSource;
                    ExComparer.SortCriteria columnName = GetSortPropertyName(header.Column);

                    //if (string.IsNullOrEmpty(columnName))
                    //    return;

                    if (RootModel.SortBy != columnName)
                        RootModel.SortBy = columnName;
                    else
                        if (RootModel.SortDirection == ListSortDirection.Ascending)
                            RootModel.SortDirection = ListSortDirection.Descending;
                        else RootModel.SortDirection = ListSortDirection.Ascending;

                    UpdateCollumnHeader();
                }
               
            });

            this.AddHandler(ListView.SelectionChangedEvent, (SelectionChangedEventHandler)delegate(object sender, SelectionChangedEventArgs args)
            {
                List<FileSystemInfoEx> selectedList = new List<FileSystemInfoEx>(SelectedEntries);

                foreach (FileListViewItemViewModel removeItem in args.RemovedItems)
                    selectedList.Remove(removeItem.EmbeddedModel.EmbeddedEntry);

                foreach (FileListViewItemViewModel addItem in args.AddedItems)
                    selectedList.Add(addItem.EmbeddedModel.EmbeddedEntry);

                SelectedEntries = selectedList;

                //The following does not work because of virtual items.
                //SelectedEntries = RootModel.CurrentDirectoryModel.SelectedEntries;
            });



            RootModel.PropertyChanged += (PropertyChangedEventHandler)delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "CurrentDirectory")
                {
                    CurrentDirectory = RootModel.CurrentDirectory;
                    ScrollViewer scrollViewer = UITools.FindVisualChild<ScrollViewer>(this);
                    if (scrollViewer != null)
                        scrollViewer.ScrollToHome();

                    ExToIconConverter ati = this.TryFindResource("ati") as ExToIconConverter;
                    if (ati.IconCount > 500)
                        ati.ClearInstanceCache();
                }

            };

            //#region ExpandHandler
            //this.AddHandler(ListViewItem.MouseDoubleClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            //{
            //    DependencyObject lvItem = getListViewItem(e.OriginalSource as DependencyObject);
            //    if (lvItem != null)
            //    {
            //        FileListViewItemViewModel model =
            //            (FileListViewItemViewModel)ItemContainerGenerator.ItemFromContainer(lvItem);
            //        if (model != null)
            //        {
            //            model.Expand();
            //        }
            //    }
            //});
            //#endregion

            #region ContextMenuWrapper - Obsoluted
            //_cmw = new ContextMenuWrapper();

            //this.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
            //    (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
            //    {
            //        if (SelectedValue is FileListViewItemViewModel)
            //        {
            //            var selectedItems = (from FileListViewItemViewModel model in SelectedItems
            //                                 select model.EmbeddedModel.EmbeddedEntry).ToArray();
            //            Point pt = this.PointToScreen(args.GetPosition(this));

            //            string command = _cmw.Popup(selectedItems, new System.Drawing.Point((int)pt.X, (int)pt.Y));
            //            switch (command)
            //            {
            //                case "rename":
            //                    if (this.SelectedValue != null)
            //                    {
            //                        SetIsEditing(ItemContainerGenerator.ContainerFromItem(this.SelectedValue), true);
            //                    }
            //                    break;
            //                case "refresh":
            //                    RootModel.CurrentDirectoryModel.Refresh();
            //                    break;
            //            }
            //        }
            //    }));
            #endregion



        }
        #endregion

        #region Methods    
   
        public new void UnselectAll()
        {
            RootModel.CurrentDirectoryModel.UnselectAll();
        }

        public void Select(FileSystemInfoEx[] itemsToSelect)
        {
            RootModel.CurrentDirectoryModel.Select(itemsToSelect);
        }

        public void Select(FileSystemInfoEx itemToSelect)
        {
            RootModel.CurrentDirectoryModel[itemToSelect].IsSelected = true;
        }

        public void Focus(FileSystemInfoEx itemToFocus)
        {             
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,  new ThreadStart(delegate
            {
                FileListViewItemViewModel selectedVM = RootModel.CurrentDirectoryModel[itemToFocus];
                RootModel.CurrentDirectoryModel.UnselectAll();
                if (selectedVM != null)
                {                    
                    selectedVM.IsSelected = true;
                    if (this.View is GridView)
                        this.ScrollIntoView(selectedVM);
                    else
                    {
                        VirtualWrapPanel vwp = UITools.FindVisualChild<VirtualWrapPanel>(this);
                        int idx = Items.IndexOf(selectedVM);
                        if (vwp != null && idx != -1)
                        {
                            Rect rect = vwp.GetChildRect(idx);
                            if (vwp.Orientation == Orientation.Horizontal)
                                vwp.SetVerticalOffset(rect.Top);
                            else vwp.SetHorizontalOffset(rect.Left);
                        }
                    }
                }                
            }));
        }


        //0.2
        //0.3
        public void UpdateCollumnHeader()
        {
            GridViewColumnCollection collection = null;
            if (View is GridView)

                collection = (View as GridView).Columns;
            else collection = (View as VirtualWrapPanelView).Columns;

            foreach (GridViewColumn col in collection)
            {
                ExComparer.SortCriteria colHeader = GetSortPropertyName(col);
                if (colHeader != RootModel.SortBy)
                    col.HeaderTemplate = (DataTemplate)this.Resources["NormHeaderTemplate"];
                else
                    if (RootModel.SortDirection == ListSortDirection.Ascending)
                        col.HeaderTemplate = (DataTemplate)this.Resources["AscHeaderTemplate"];
                    else col.HeaderTemplate = (DataTemplate)this.Resources["DescHeaderTemplate"];
            }


        }

        static DependencyObject getListViewItem(DependencyObject e)
        {

            while (e != null && !(e is ListViewItem))
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //0.2
            UpdateCollumnHeader();

            _lookupAdorner = new FileListLookupBoxAdorner(this);
            Debug.Assert(_lookupAdorner != null);

            //Binding filterBinding = new Binding("CurrentDirectoryModel.Filter");
            //filterBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //filterBinding.Source = RootModel;
            //_lookupAdorner.SetBinding(FileListLookupBoxAdorner.TextProperty, filterBinding);    
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty
                (FileListLookupBoxAdorner.TextProperty, typeof(FileListLookupBoxAdorner));

            descriptor.AddValueChanged
             (_lookupAdorner, new EventHandler(delegate { RootModel.CurrentDirectoryModel.Filter = _lookupAdorner.Text; }));

            Loaded += delegate
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                if (layer != null)
                    layer.Add(_lookupAdorner);
                _lookupAdorner.UpdateVisibilty(false);

                RaiseEvent(new ProgressRoutedEventArgs(ProgressEvent, new ProgressEventArgs(0, "FileList Loaded",
                    WorkType.Unknown, WorkStatusType.wsCompleted, WorkResultType.wrSuccess)));
            };
        }

        public static void OnCurrentDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FileList fl = (FileList)sender;
            fl.RootModel.CurrentDirectory = (args.NewValue as DirectoryInfoEx);
            if (fl._lookupAdorner != null)
                fl._lookupAdorner.UpdateVisibilty(false);
        }

        public static void OnViewModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!args.NewValue.Equals(args.OldValue))
            {
                FileList fl = (FileList)sender;
                ViewMode viewMode = (ViewMode)args.NewValue;
                string viewName = (viewMode.ToString()).Substring(2) + "View";
                switch (viewMode)
                {
                    case ViewMode.vmExtraLargeIcon:
                    case ViewMode.vmLargeIcon:
                        viewName = "IconView";
                        break;
                }
                ViewBase view = (ViewBase)(fl.TryFindResource(viewName)); ;
                if (view != null)
                    fl.View = view;
                fl.UpdateCollumnHeader();

                if ((int)viewMode > (int)ViewMode.vmIcon && fl.ViewSize < (int)viewMode)
                    fl.ViewSize = (int)viewMode;
            }
        }

        public static void OnViewSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FileList flist = sender as FileList;
            int newSize = (int)args.NewValue;

            ViewMode newViewMode = flist.ViewMode;
            switch (newSize)
            {
                case 13: newViewMode = ViewMode.vmTile; break;
                case 14: newViewMode = ViewMode.vmGrid; break;
                case 15: newViewMode = ViewMode.vmList; break;
                case 16: newViewMode = ViewMode.vmSmallIcon; break;
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

        #region Data

        private FileListLookupBoxAdorner _lookupAdorner;
        //private ContextMenuWrapper _cmw;

        #endregion

        #region Public Properties

        public static readonly RoutedEvent ProgressEvent = ProgressRoutedEventArgs.ProgressEvent.AddOwner(typeof(FileList));

        public FileListLookupBoxAdorner LookupAdorner { get { return _lookupAdorner; } }
        //0.2
        public static readonly DependencyProperty SortPropertyNameProperty = DependencyProperty.RegisterAttached("SortPropertyName",
            typeof(ExComparer.SortCriteria), typeof(FileList));

        public static ExComparer.SortCriteria GetSortPropertyName(DependencyObject sender)
        {
            return (ExComparer.SortCriteria)sender.GetValue(SortPropertyNameProperty);
        }

        public static void SetSortPropertyName(DependencyObject sender, ExComparer.SortCriteria value)
        {
            sender.SetValue(SortPropertyNameProperty, value);
        }


        public static readonly DependencyProperty SelectedEntriesProperty =
         DependencyProperty.Register("SelectedEntries", typeof(IList<FileSystemInfoEx>), typeof(FileList),
         new FrameworkPropertyMetadata(new List<FileSystemInfoEx>()));


        public IList<FileSystemInfoEx> SelectedEntries
        {
            get { return (IList<FileSystemInfoEx>)GetValue(SelectedEntriesProperty); }
            set { SetValue(SelectedEntriesProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached("IsEditing", typeof(bool), typeof(FileList));

        public static bool GetIsEditing(DependencyObject sender)
        {
            return (bool)sender.GetValue(IsEditingProperty);
        }

        public static void SetIsEditing(DependencyObject sender, bool value)
        {
            sender.SetValue(IsEditingProperty, value);
        }

        public static readonly DependencyProperty ModelToExConverterProperty =
          DependencyProperty.Register("ModelToExConverter", typeof(ModelToExConverter), typeof(FileList));

        public ModelToExConverter ModelToExConverter
        {
            get { return (ModelToExConverter)GetValue(ModelToExConverterProperty); }
            set { SetValue(ModelToExConverterProperty, value); }
        }

        public static readonly DependencyProperty CurrentDirectoryProperty =
          DependencyProperty.Register("CurrentDirectory", typeof(DirectoryInfoEx), typeof(FileList),
          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCurrentDirectoryChanged)));

        public DirectoryInfoEx CurrentDirectory
        {
            get { return (DirectoryInfoEx)GetValue(CurrentDirectoryProperty); }
            set { SetValue(CurrentDirectoryProperty, value); }
        }

        public static readonly DependencyProperty RootModelProperty =
          DependencyProperty.Register("RootModel", typeof(FileListViewModel), typeof(FileList),
          new FrameworkPropertyMetadata(null));

        public FileListViewModel RootModel
        {
            get { return (FileListViewModel)GetValue(RootModelProperty); }
            set
            {
                SetValue(RootModelProperty, value);
            }
        }

        public static readonly DependencyProperty CommandsProperty =
          DependencyProperty.Register("Commands", typeof(FileListCommands), typeof(FileList),
          new FrameworkPropertyMetadata(null));

        public FileListCommands Commands
        {
            get { return (FileListCommands)GetValue(CommandsProperty); }
            set
            {
                SetValue(CommandsProperty, value);
            }
        }

        public static readonly DependencyProperty ExModelToIconConverterProperty =
          DependencyProperty.Register("ExModelToIconConverter", typeof(ExModelToIconConverter), typeof(FileList),
          new FrameworkPropertyMetadata(null));

        public ExModelToIconConverter ExModelToIconConverter
        {
            get { return (ExModelToIconConverter)GetValue(ExModelToIconConverterProperty); }
            set
            {
                SetValue(ExModelToIconConverterProperty, value);
            }
        }

        public static readonly DependencyProperty ViewModeProperty =
          DependencyProperty.Register("ViewMode", typeof(ViewMode), typeof(FileList),
          new FrameworkPropertyMetadata(ViewMode.vmGrid, new PropertyChangedCallback(OnViewModeChanged)));

        public ViewMode ViewMode
        {
            get { return (ViewMode)GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        public static readonly DependencyProperty ViewSizeProperty =
            DependencyProperty.Register("ViewSize", typeof(int), typeof(FileList),
            new FrameworkPropertyMetadata(16, new PropertyChangedCallback(OnViewSizeChanged)));

        public int ViewSize
        {
            get { return (int)GetValue(ViewSizeProperty); }
            set { SetValue(ViewSizeProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
          DependencyProperty.Register("IsLoading", typeof(bool), typeof(FileList),
          new FrameworkPropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty SortByProperty =
          DependencyProperty.Register("SortBy", typeof(ExComparer.SortCriteria), typeof(FileList),
          new FrameworkPropertyMetadata(ExComparer.SortCriteria.sortByName));

        public ExComparer.SortCriteria SortBy
        {
            get { return (ExComparer.SortCriteria)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        public static readonly DependencyProperty SortDirectionProperty =
          DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(FileList),
          new FrameworkPropertyMetadata(ListSortDirection.Ascending));

        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }        

        #endregion
    }
}
