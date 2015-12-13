using FileExplorer.Defines;
using FileExplorer.WPF.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace FileExplorer.WPF.BaseControls
{
    public class DisplayContentControl : ContentControl
    {
        #region Cosntructor

        //static DisplayContentControl()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DisplayContentControl),
        //        new FrameworkPropertyMetadata(typeof(DisplayContentControl)));
        //}

        public DisplayContentControl()
        {
            //this.ContentTemplateSelector = new DisplayTemplateSelector(
            //    DisplayTemplateSelector.FromDisplayContentControl);
            //ContenteC
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
          
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties


        public static DependencyProperty TypeProperty = DependencyProperty.Register("Type",
            typeof(DisplayType), typeof(DisplayContentControl), new PropertyMetadata(DisplayType.Auto));

        public DisplayType Type
        {
            get { return (DisplayType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }



        #endregion
    }
}
