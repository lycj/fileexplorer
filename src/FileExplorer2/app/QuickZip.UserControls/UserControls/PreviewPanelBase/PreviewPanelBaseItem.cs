using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuickZip.UserControls;

namespace QuickZip.UserControls
{

    public class PreviewPanelBaseItem : Control
    {
        #region Constructor
        //static PreviewPanelBaseItem()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewPanelBaseItem),
        //        new FrameworkPropertyMetadata(typeof(PreviewPanelBaseItem)));
        //}

        public PreviewPanelBaseItem()
        {            
        }
        #endregion

        #region Methods

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            //PreviewPanel ppanel = UITools.FindAncestor<PreviewPanel>(this);
            //Selector.SetIsSelected(this, true);
            (UITools.FindAncestor<PreviewPanelBase>(this)).SelectedValue = DataContext;
        }

        #endregion

        #region Dependency Properties

        //public static readonly DependencyProperty RootModelProperty =
        //DependencyProperty.Register("RootModel", typeof(PreviewPanelBaseItemViewModel), typeof(PreviewPanelBaseItem),
        //new FrameworkPropertyMetadata(null));

        //public PreviewPanelBaseItemViewModel RootModel
        //{
        //    get { return (PreviewPanelBaseItemViewModel)GetValue(RootModelProperty); }
        //    set
        //    {
        //        SetValue(RootModelProperty, value);
        //    }
        //}

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            ListBoxItem.IsSelectedProperty.AddOwner(typeof(PreviewPanelBaseItem));            

        

        #endregion
    }

    
}
