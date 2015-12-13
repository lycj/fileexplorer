using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace QuickZip.UserControls
{
    public class DirectoryTree2 : TreeView
    {
        #region Constructor

        public DirectoryTree2()
        {

            #region ContextMenu
            this.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
                (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
                {
                    TreeViewItem sourceItem = UITools.GetParentTreeViewItem(args.OriginalSource as FrameworkElement);
                    if (sourceItem != null)
                    {
                        if (!sourceItem.IsSelected)
                        {
                            TreeViewItemAutomationPeer peer = new TreeViewItemAutomationPeer(sourceItem);
                            ISelectionItemProvider invokeProv = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                            invokeProv.Select();
                        }

                        if (sourceItem.IsSelected)
                            if (ContextMenuCommand != null && ContextMenuCommand.CanExecute(this.SelectedItem))
                                ContextMenuCommand.Execute(this.SelectedItem);
                    }
                }));

            #endregion

            W7TreeViewItemUtils.SetIsEnabled(this, true);    

        }

        #endregion


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DirectoryTreeItem2();
        }

        #region Public Properties

        public ICommand ContextMenuCommand
        {
            get { return (ICommand)GetValue(ContextMenuCommandProperty); }
            set { SetValue(ContextMenuCommandProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuCommandProperty =
            DependencyProperty.Register("ContextMenuCommand", typeof(ICommand), typeof(DirectoryTree2), new UIPropertyMetadata(null));


        #endregion
    }

    public class DirectoryTreeItem2 : TreeViewItem
    {
        public DirectoryTreeItem2()
        {
            this.AddHandler(TreeViewItem.DragEnterEvent, (RoutedEventHandler)((o, e) =>
            {
                if (UITools.GetParentTreeViewItem(e.OriginalSource as FrameworkElement).Equals(this))                
                    IsDraggingOver = true;
            }));

            this.AddHandler(TreeViewItem.DropEvent, (RoutedEventHandler)((o, e) =>
            {
                if (UITools.GetParentTreeViewItem(e.OriginalSource as FrameworkElement).Equals(this))
                    IsDraggingOver = false;
            }));

            this.AddHandler(TreeViewItem.DragLeaveEvent, (RoutedEventHandler)((o, e) =>
            {
                if (UITools.GetParentTreeViewItem(e.OriginalSource as FrameworkElement).Equals(this))                
                    IsDraggingOver = false;
            }));
        }

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DirectoryTreeItem2();
        }

        #endregion

        #region Public Properties

        public bool IsDraggingOver
        {
            get { return (bool)GetValue(IsDraggingOverProperty); }
            set { SetValue(IsDraggingOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDraggingOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDraggingOverProperty =
            DependencyProperty.Register("IsDraggingOver", typeof(bool), typeof(DirectoryTreeItem2), new UIPropertyMetadata(false));



        #endregion



    }
}
