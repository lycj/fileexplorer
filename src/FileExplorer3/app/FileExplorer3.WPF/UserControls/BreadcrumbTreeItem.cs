using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.WPF.BaseControls;

namespace FileExplorer.WPF.UserControls
{
    [TemplateVisualState(Name = "ShowCaption", GroupName = "CaptionStates")]
    [TemplateVisualState(Name = "HideCaption", GroupName = "CaptionStates")]
    public class BreadcrumbTreeItem : TreeViewItem
    {
        #region Constructor

        static BreadcrumbTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTreeItem),
                new FrameworkPropertyMetadata(typeof(BreadcrumbTreeItem)));
        }

        public BreadcrumbTreeItem()
        {
     
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbTreeItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)((o, e) =>
                {
                    if (e.Source is Button)
                    {
                        this.SetValue(IsCurrentSelectedProperty, true);
                        e.Handled = true;
                    }
                }));

            //this.AddHandler(OverflowItem.SelectedEvent, (RoutedEventHandler)((o, e) =>
            //    {
            //        if (e.Source is OverflowItem)
            //        {
            //            IsExpanded = false;
            //        }
            //    }));
        }

        public static void OnIsCaptionVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as BreadcrumbTreeItem).UpdateStates(true);
        }

        public static void OnOverflowItemCountChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as BreadcrumbTreeItem).SetValue(IsOverflowedProperty, ((int)args.NewValue) > 0);
        }

        private void UpdateStates(bool useTransition)
        {
            if (IsCaptionVisible)
                VisualStateManager.GoToState(this, "ShowCaption", useTransition);
            else VisualStateManager.GoToState(this, "HideCaption", useTransition);
        }

        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        public static DependencyProperty OverflowItemCountProperty = OverflowableStackPanel.OverflowItemCountProperty
            .AddOwner(typeof(BreadcrumbTreeItem), new PropertyMetadata(OnOverflowItemCountChanged));

        public int OverflowItemCount
        {
            get { return (int)GetValue(OverflowItemCountProperty); }
            set { SetValue(OverflowItemCountProperty, value); }
        }

        public static DependencyProperty IsOverflowedProperty = DependencyProperty.Register("IsOverflowed", typeof(bool),
         typeof(BreadcrumbTreeItem), new PropertyMetadata(false));

        public bool IsOverflowed
        {
            get { return (bool)GetValue(IsOverflowedProperty); }
            set { SetValue(IsOverflowedProperty, value); }
        }

        public static readonly DependencyProperty OverflowedItemContainerStyleProperty =
                BreadcrumbTree.OverflowedItemContainerStyleProperty.AddOwner(typeof(BreadcrumbTreeItem));

        public Style OverflowedItemContainerStyle
        {
            get { return (Style)GetValue(OverflowedItemContainerStyleProperty); }
            set { SetValue(OverflowedItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty SelectedChildProperty =
          DependencyProperty.Register("SelectedChild", typeof(object), typeof(BreadcrumbTreeItem),
              new UIPropertyMetadata(null));

        public object SelectedChild
        {
            get { return (object)GetValue(SelectedChildProperty); }
            set { SetValue(SelectedChildProperty, value); }
        }

        public static readonly DependencyProperty ValuePathProperty =
         DependencyProperty.Register("ValuePath", typeof(string), typeof(BreadcrumbTreeItem),
             new UIPropertyMetadata(""));

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }
        

        public static readonly DependencyProperty IsChildSelectedProperty =
            DependencyProperty.Register("IsChildSelected", typeof(bool), typeof(BreadcrumbTreeItem), 
                new UIPropertyMetadata());

        public bool IsChildSelected
        {
            get { return (bool)GetValue(IsChildSelectedProperty); }
            set { SetValue(IsChildSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentSelectedProperty =
           DependencyProperty.Register("IsCurrentSelected", typeof(bool), typeof(BreadcrumbTreeItem),
               new UIPropertyMetadata(false));

        public bool IsCurrentSelected
        {
            get { return (bool)GetValue(IsCurrentSelectedProperty); }
            set { SetValue(IsCurrentSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsCaptionVisibleProperty =
                    DependencyProperty.Register("IsCaptionVisible", typeof(bool), typeof(BreadcrumbTreeItem),
                    new UIPropertyMetadata(true, OnIsCaptionVisibleChanged));

        /// <summary>
        /// Display Caption
        /// </summary>
        public bool IsCaptionVisible
        {
            get { return (bool)GetValue(IsCaptionVisibleProperty); }
            set { SetValue(IsCaptionVisibleProperty, value); }
        }

        public static readonly DependencyProperty MenuItemTemplateProperty =
                BreadcrumbTree.MenuItemTemplateProperty.AddOwner(typeof(BreadcrumbTreeItem));

        public DataTemplate MenuItemTemplate
        {
            get { return (DataTemplate)GetValue(MenuItemTemplateProperty); }
            set { SetValue(MenuItemTemplateProperty, value); }
        }

        
        #endregion
    }
}
