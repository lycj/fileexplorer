using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;


namespace FileExplorer.WPF.UserControls
{
    //TreeViewEx and TreeViewItemEx

    public class TreeViewEx : TreeView
    {
        #region Cosntructor

        public TreeViewEx()
        {

        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static readonly DependencyProperty OuterTopContentProperty =
           DockableScrollViewer.OuterTopContentProperty.AddOwner(typeof(TreeViewEx));
        public static readonly DependencyProperty OuterRightContentProperty =
           DockableScrollViewer.OuterRightContentProperty.AddOwner(typeof(TreeViewEx));
        public static readonly DependencyProperty OuterBottomContentProperty =
          DockableScrollViewer.OuterBottomContentProperty.AddOwner(typeof(TreeViewEx));
        public static readonly DependencyProperty OuterLeftContentProperty =
          DockableScrollViewer.OuterLeftContentProperty.AddOwner(typeof(TreeViewEx));

        public static readonly DependencyProperty TopContentProperty =
         DockableScrollViewer.TopContentProperty.AddOwner(typeof(TreeViewEx));
        public static readonly DependencyProperty RightContentProperty =
         DockableScrollViewer.RightContentProperty.AddOwner(typeof(TreeViewEx));

        public static readonly DependencyProperty BottomContentProperty =
        DockableScrollViewer.BottomContentProperty.AddOwner(typeof(TreeViewEx));
        public object BottomContent
        {
            get { return (object)GetValue(BottomContentProperty); }
            set { SetValue(BottomContentProperty, value); }
        }

        public static readonly DependencyProperty LeftContentProperty =
         DockableScrollViewer.LeftContentProperty.AddOwner(typeof(TreeViewEx));

        #endregion
    }

    public class TreeViewItemEx : TreeViewItem
    {
        #region Cosntructor

        public TreeViewItemEx()
        {
            this.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(
                 (RoutedEventHandler)delegate(object obj, RoutedEventArgs args)
                 {
                     if (args.OriginalSource is TreeViewItem)
                         (args.OriginalSource as TreeViewItem).BringIntoView();

                 }));

            this.AddValueChanged(TreeViewItemEx.IsBringIntoViewProperty, (o, e) =>
            {
                TreeViewItemEx tvItem = o as TreeViewItemEx;
                if (tvItem.IsBringIntoView)
                {
                    this.BringIntoView();
                    tvItem.IsBringIntoView = false;
                }
            });
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AddValueChanged(TreeViewItemEx.IsDraggingOverProperty, (o, e) =>
                {
                    TreeViewItemEx tvItem = o as TreeViewItemEx;
                    if (tvItem.IsDraggingOver && ExpandIfDragOver)
                    {
                        DispatcherTimer dispatcherTimer = new DispatcherTimer();
                        EventHandler onTick = null;
                        onTick = (o1, e1) =>
                        {
                            if (tvItem.IsDraggingOver)
                                tvItem.SetValue(TreeViewItem.IsExpandedProperty, true);
                            dispatcherTimer.Tick -= onTick;
                            dispatcherTimer.Stop();
                        };
                        dispatcherTimer.Tick += onTick;
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                        dispatcherTimer.Start();
                    }
                });
          
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);


        }
        //override hittestvi

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public bool IsItemUpdateRequired
        {
            get { return (bool)GetValue(IsItemUpdateRequiredProperty); }
            set { SetValue(IsItemUpdateRequiredProperty, value); }
        }

        public static readonly DependencyProperty IsItemUpdateRequiredProperty =
            DependencyProperty.Register("IsItemUpdateRequired", typeof(bool),
            typeof(TreeViewItemEx), new UIPropertyMetadata(false));

        public bool IsDraggingOver
        {
            get { return (bool)GetValue(IsDraggingOverProperty); }
            set { SetValue(IsDraggingOverProperty, value); }
        }

        public static readonly DependencyProperty IsDraggingOverProperty =
            DependencyProperty.Register("IsDraggingOver", typeof(bool),
            typeof(TreeViewItemEx), new UIPropertyMetadata(false));


        /// <summary>
        /// Changing this property to true will Bring the Item into View then set to false again.
        /// </summary>
        public bool IsBringIntoView
        {
            get { return (bool)GetValue(IsBringIntoViewProperty); }
            set { SetValue(IsBringIntoViewProperty, value); }
        }

        public static readonly DependencyProperty IsBringIntoViewProperty =
            DependencyProperty.Register("IsBringIntoView", typeof(bool),
            typeof(TreeViewItemEx), new UIPropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool),
            typeof(TreeViewItemEx), new UIPropertyMetadata(false));

        public bool ExpandIfDragOver
        {
            get { return (bool)GetValue(ExpandIfDragOverProperty); }
            set { SetValue(ExpandIfDragOverProperty, value); }
        }

        public static readonly DependencyProperty ExpandIfDragOverProperty =
            DependencyProperty.Register("ExpandIfDragOver", typeof(bool),
            typeof(TreeViewItemEx), new UIPropertyMetadata(true));

        #endregion
    }
}
