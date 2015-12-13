using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF.BaseControls
{
    public class OverflowPanel : ListView
    {
        #region Constructor

        static OverflowPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverflowPanel),
             new FrameworkPropertyMetadata(typeof(OverflowPanel)));
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new OverflowItem() { };
        }

        

        #endregion

        #region Data



        #endregion

        #region Public Properties

        //public static readonly DependencyProperty HeaderTemplateProperty =
        //   DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(OverflowPanel), new PropertyMetadata(null));

        //public DataTemplate HeaderTemplate
        //{
        //    get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        //    set { SetValue(HeaderTemplateProperty, value); }
        //}

        //public static readonly DependencyProperty IconTemplateProperty =
        //   DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(OverflowPanel), new PropertyMetadata(null));

        //public DataTemplate IconTemplate
        //{
        //    get { return (DataTemplate)GetValue(IconTemplateProperty); }
        //    set { SetValue(IconTemplateProperty, value); }
        //}

        #endregion
    }
}
