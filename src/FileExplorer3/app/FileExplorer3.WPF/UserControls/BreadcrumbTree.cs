using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF.UserControls
{
    public class BreadcrumbTree : TreeView
    {
        #region Constructor

        static BreadcrumbTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTree),
                new FrameworkPropertyMetadata(typeof(BreadcrumbTree)));
        }

        #endregion

        #region Methods 

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbTreeItem() {  };
        }

        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        public static readonly DependencyProperty OverflowedItemContainerStyleProperty =
                   DependencyProperty.Register("OverflowedItemContainerStyle", typeof(Style), typeof(BreadcrumbTree));

        public Style OverflowedItemContainerStyle
        {
            get { return (Style)GetValue(OverflowedItemContainerStyleProperty); }
            set { SetValue(OverflowedItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty MenuItemTemplateProperty =
                 DependencyProperty.Register("MenuItemTemplate", typeof(DataTemplate), typeof(BreadcrumbTree));

        public DataTemplate MenuItemTemplate
        {
            get { return (DataTemplate)GetValue(MenuItemTemplateProperty); }
            set { SetValue(MenuItemTemplateProperty, value); }
        }
        
        #endregion
    }
}
