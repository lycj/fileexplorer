using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace FileExplorer.WPF.UserControls
{
    public class VirtualWrapPanelView : ViewBase
    {
        public static readonly DependencyProperty OrientationProperty =
            VirtualWrapPanel.OrientationProperty.AddOwner(typeof(VirtualWrapPanelView));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty SmallChangesProperty =
           VirtualWrapPanel.SmallChangesProperty.AddOwner(typeof(VirtualWrapPanelView));

        public uint SmallChanges
        {
            get { return (uint)GetValue(SmallChangesProperty); }
            set { SetValue(SmallChangesProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleProperty =
            ItemsControl.ItemContainerStyleProperty.AddOwner(typeof(VirtualWrapPanelView));

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(VirtualWrapPanelView));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            WrapPanel.ItemWidthProperty.AddOwner(typeof(VirtualWrapPanelView));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            WrapPanel.ItemHeightProperty.AddOwner(typeof(VirtualWrapPanelView));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }


        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            WrapPanel.HorizontalAlignmentProperty.AddOwner(typeof(VirtualWrapPanelView));

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }
       
        public static readonly DependencyProperty CacheItemCountProperty =
            DependencyProperty.Register("CacheItemCount", typeof(int),
            typeof(VirtualWrapPanelView), new UIPropertyMetadata(0));

        public int CacheItemCount
        {
            get { return (int)GetValue(CacheItemCountProperty); }
            set { SetValue(CacheItemCountProperty, value); }
        }
        


        private GridViewColumnCollection _columns = new GridViewColumnCollection();
        public GridViewColumnCollection Columns
        {
            get { return _columns; }            
        }

        public static readonly DependencyProperty ColumnHeaderContainerStyleProperty =
            GridView.ColumnHeaderContainerStyleProperty.AddOwner(typeof(VirtualWrapPanelView));
        public Style ColumnHeaderContainerStyle
        {
            get { return (Style)GetValue(ColumnHeaderContainerStyleProperty); }
            set { SetValue(ColumnHeaderContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ColumnHeaderTemplateProperty =
          GridView.ColumnHeaderTemplateProperty.AddOwner(typeof(VirtualWrapPanelView));
        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty =
            GridView.ColumnHeaderTemplateSelectorProperty.AddOwner(typeof(VirtualWrapPanelView));
        public static readonly DependencyProperty ColumnHeaderStringFormatProperty =
             GridView.ColumnHeaderStringFormatProperty.AddOwner(typeof(VirtualWrapPanelView));
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            GridView.AllowsColumnReorderProperty.AddOwner(typeof(VirtualWrapPanelView));
        public static readonly DependencyProperty ColumnHeaderContextMenuProperty =
            GridView.ColumnHeaderContextMenuProperty.AddOwner(typeof(VirtualWrapPanelView));
        public static readonly DependencyProperty ColumnHeaderToolTipProperty =
            GridView.ColumnHeaderToolTipProperty.AddOwner(typeof(VirtualWrapPanelView));

        protected override object DefaultStyleKey
        {
            get
            {
                return new ComponentResourceKey(GetType(), "virtualWrapPanelViewDSK");                
            }
        }

        protected override object ItemContainerDefaultStyleKey
        {
            get
            {
                return new ComponentResourceKey(GetType(), "virtualWrapPanelViewItemDSK");
            }
        }
    }
}
