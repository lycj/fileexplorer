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
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using QuickZip.IO.PIDL.Tools;
using Cinch;
using System.IO.Tools;
using System.Threading;
using QuickZip.UserControls;
using System.Windows.Media.Animation;

namespace QuickZip.IO.PIDL.UserControls
{
    /// <summary>
    /// Interaction logic for DirectoryTree.xaml
    /// </summary>
    public partial class DirectoryTree : TreeView
    {
        internal TreeViewItem _lastSelectedContainer = null;

        static DirectoryTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryTree),
                new FrameworkPropertyMetadata(typeof(DirectoryTree)));
        }

        public DirectoryTree()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            DataContext = RootModel = new DirectoryTreeViewModel();
            RootModel.RootDirectory = DirectoryInfoEx.DesktopDirectory;
            Commands = new DirectoryTreeCommands(this, RootModel);

            RootModel.OnProgress += (ProgressEventHandler)delegate(object sender, ProgressEventArgs e)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                {
                    RaiseEvent(new ProgressRoutedEventArgs(ProgressEvent, e));
                }));
            };

            W7TreeViewItemUtils.SetIsEnabled(this, true);            

            #region Selection
            this.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(
                (RoutedEventHandler)delegate(object obj, RoutedEventArgs args)
                {
                    if (SelectedValue is DirectoryTreeItemViewModel)
                    {
                        DirectoryTreeItemViewModel selectedModel = SelectedValue as DirectoryTreeItemViewModel;
                        SelectedDirectory = selectedModel.EmbeddedDirectoryModel.EmbeddedDirectoryEntry;
                        if (args.OriginalSource is TreeViewItem)
                            (args.OriginalSource as TreeViewItem).BringIntoView();

                        _lastSelectedContainer = (args.OriginalSource as TreeViewItem);
                    }
                }));

            //this.SelectedDirectoryPath <---> this.SelectedDirectory
            Binding selectedDirectoryPathBinding = new Binding("SelectedDirectory");
            selectedDirectoryPathBinding.Mode = BindingMode.TwoWay;
            selectedDirectoryPathBinding.Converter = new ExToStringConverter();
            selectedDirectoryPathBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            selectedDirectoryPathBinding.Source = this;
            this.SetBinding(DirectoryTree.SelectedDirectoryPathProperty, selectedDirectoryPathBinding);

            //this.SelectedDirectory <---> RootModel.SelectedDirectory
            Binding selectedDirectoryBinding = new Binding("SelectedDirectory");
            selectedDirectoryBinding.Mode = BindingMode.TwoWay;
            selectedDirectoryBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            selectedDirectoryBinding.Source = RootModel;
            this.SetBinding(DirectoryTree.SelectedDirectoryProperty, selectedDirectoryBinding);

            //this.AutoCollapse <---> RootModel.AutoCollapse
            Binding autoCollapseBinding = new Binding("AutoCollapse");
            autoCollapseBinding.Mode = BindingMode.OneWayToSource;
            autoCollapseBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            autoCollapseBinding.Source = RootModel;
            this.SetBinding(DirectoryTree.AutoCollapseProperty, autoCollapseBinding);
            #endregion

            #region ContextMenuWrapper - Obsoluted
            //_cmw = new ContextMenuWrapper();

            //this.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
            //    (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
            //    {
            //        if (SelectedValue is DirectoryTreeItemViewModel)
            //        {
            //            DirectoryTreeItemViewModel selectedModel = SelectedValue as DirectoryTreeItemViewModel;
            //            Point pt = this.PointToScreen(args.GetPosition(this));
            //            string command = _cmw.Popup(new FileSystemInfoEx[] { selectedModel.EmbeddedDirectoryModel.EmbeddedDirectoryEntry },
            //                new System.Drawing.Point((int)pt.X, (int)pt.Y));
            //            switch (command)
            //            {
            //                case "rename":
            //                    if (this.SelectedValue != null)
            //                    {
            //                        if (_lastSelectedContainer != null)
            //                        {
            //                            SetIsEditing(_lastSelectedContainer, true);
            //                        }
            //                    }
            //                    break;
            //                case "refresh":
            //                    if (this.SelectedValue is DirectoryTreeItemViewModel)
            //                    {
            //                        (this.SelectedValue as DirectoryTreeItemViewModel).Refresh();
            //                    }
            //                    break;
            //            }
            //        }
            //    }));
            #endregion           

        }

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _adorner = new LoadingAdorner(this);

            Binding isLoadingBinding = new Binding("IsLoading");
            isLoadingBinding.Source = RootModel;
            _adorner.SetBinding(LoadingAdorner.IsLoadingProperty, isLoadingBinding);

            Loaded += delegate
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
                if (layer != null)                                   
                    layer.Add(_adorner);     

                RaiseEvent(new ProgressRoutedEventArgs(ProgressEvent, new ProgressEventArgs(0, "DirectoryTree Loaded",
                    WorkType.Unknown, WorkStatusType.wsCompleted, WorkResultType.wrSuccess)));
            };
        }

        #endregion

        #region Data        
        //private ContextMenuWrapper _cmw;
        private LoadingAdorner _adorner;
        #endregion

        #region Public Properties
        public static readonly RoutedEvent ProgressEvent = ProgressRoutedEventArgs.ProgressEvent.AddOwner(typeof(DirectoryTree));

        public static ModelToExConverter ModelToExConverter = new ModelToExConverter();              

        public static readonly DependencyProperty SelectedDirectoryProperty =
          DependencyProperty.Register("SelectedDirectory", typeof(DirectoryInfoEx), typeof(DirectoryTree),
          new FrameworkPropertyMetadata());

        public DirectoryInfoEx SelectedDirectory
        {
            get { return (DirectoryInfoEx)GetValue(SelectedDirectoryProperty); }
            set { SetValue(SelectedDirectoryProperty, value); }
        }

        public static readonly DependencyProperty AutoCollapseProperty =
          DependencyProperty.Register("AutoCollapse", typeof(bool), typeof(DirectoryTree),
          new FrameworkPropertyMetadata(true));

        public bool AutoCollapse
        {
            get { return (bool)GetValue(AutoCollapseProperty); }
            set { SetValue(AutoCollapseProperty, value); }
        }

        public static readonly DependencyProperty RootDirectoryProperty =
          DependencyProperty.Register("RootDirectory", typeof(DirectoryInfoEx), typeof(DirectoryTree),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRootDirectoryChanged)));

        public DirectoryInfoEx RootDirectory
        {
            get { return (DirectoryInfoEx)GetValue(RootDirectoryProperty); }
            set { SetValue(RootDirectoryProperty, value); }
        }

        public static readonly DependencyProperty RootModelProperty =
          DependencyProperty.Register("RootModel", typeof(DirectoryTreeViewModel), typeof(DirectoryTree));

        public DirectoryTreeViewModel RootModel
        {
            get { return (DirectoryTreeViewModel)GetValue(RootModelProperty); }
            set { SetValue(RootModelProperty, value); }
        }

        public static readonly DependencyProperty CommandsProperty =
          DependencyProperty.Register("Commands", typeof(DirectoryTreeCommands), typeof(DirectoryTree),
          new FrameworkPropertyMetadata(null));

        public DirectoryTreeCommands Commands
        {
            get { return (DirectoryTreeCommands)GetValue(CommandsProperty); }
            set
            {
                SetValue(CommandsProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDirectoryPathProperty =
          DependencyProperty.Register("SelectedDirectoryPath", typeof(string), typeof(DirectoryTree));

        public string SelectedDirectoryPath
        {
            get { return (string)GetValue(SelectedDirectoryPathProperty); }
            set { SetValue(SelectedDirectoryPathProperty, value); }
        }        

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached("IsEditing", typeof(bool), typeof(DirectoryTree));
        
        public static bool GetIsEditing(DependencyObject sender)
        {
            return (bool)sender.GetValue(IsEditingProperty);
        }

        public static void SetIsEditing(DependencyObject sender, bool value)
        {
            sender.SetValue(IsEditingProperty, value);
        }

        #endregion

        #region Static Methods
        //public static void OnSelectedDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        //{
        //    DirectoryTree dt = (DirectoryTree)sender;
        //    dt.RootModel.SelectedDirectory = args.NewValue as DirectoryInfoEx;
        //}

        public static void OnRootDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DirectoryTree dt = (DirectoryTree)sender;
            dt.RootModel.RootDirectory = args.NewValue as DirectoryInfoEx;
        }


        #endregion

    }
}
