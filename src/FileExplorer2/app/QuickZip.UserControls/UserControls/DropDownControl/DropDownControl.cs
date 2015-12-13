using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace QuickZip.UserControls
{
    public class DropDownControl : HeaderedContentControl
    {
        #region Constructor
        static DropDownControl()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicMediaPlayer),
            //new FrameworkPropertyMetadata(typeof(BasicMediaPlayer)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownControl),
                new System.Windows.FrameworkPropertyMetadata(typeof(DropDownControl)));
        }

        public DropDownControl()
        {

        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = (Popup)this.Template.FindName("PART_Popup", this);
            _content = (ContentPresenter)this.Template.FindName("PART_Content", this);

            _popup.AddHandler(Popup.LostFocusEvent,
               new RoutedEventHandler((o, e) =>
               {
                   //(o as DropDownControl).                   
                   //IsDropDownOpen = false;
               }));
        }

        private static void OnIsDropDownOpenChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DropDownControl ddc = (DropDownControl)sender;
            if (ddc._popup != null)
            {
                ddc._popup.IsOpen = (bool)args.NewValue;
            }
            //if (ddc._content != null)
            //{
            //    ddc._content.Focus();
            //}
            //if (((bool)args.NewValue) && ddc._dropDownGrid != null)
            //{
            //    //Setfocu
            //    //ddc._dropDownGrid.
            //    //Debug.WriteLine(ddc._dropDownGrid.IsFocused);
            //}
        }

        #endregion

        #region Data

        Popup _popup = null;
        ContentPresenter _content = null;

        #endregion

        #region DependencyProperties

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool),
            typeof(DropDownControl), new UIPropertyMetadata(false,
                new PropertyChangedCallback(OnIsDropDownOpenChanged)));




        public bool IsDropDownAlignLeft
        {
            get { return (bool)GetValue(IsDropDownAlignLeftProperty); }
            set { SetValue(IsDropDownAlignLeftProperty, value); }
        }
        
        public static readonly DependencyProperty IsDropDownAlignLeftProperty =
            DependencyProperty.Register("IsDropDownAlignLeft", typeof(bool),
            typeof(DropDownControl), new UIPropertyMetadata(false));

        


        //IsHeaderEnabled
        //IsDropDownOpen

        #endregion
    }
}
