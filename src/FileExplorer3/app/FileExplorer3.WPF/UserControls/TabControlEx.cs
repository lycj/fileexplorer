using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer.WPF.UserControls
{
    public class TabControlEx : TabControl
    {

        #region Constructors

        static TabControlEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlEx),
                new FrameworkPropertyMetadata(typeof(TabControlEx)));
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            var newTabItem = new TabItemEx();
            return newTabItem;
            //return base.GetContainerForItemOverride();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ancestorWindow = UITools.FindAncestor<Window>(this);
            _ancestorWindow.AddValueChanged(Window.WindowStateProperty, (o, e) =>
                {
                    this.WindowState = _ancestorWindow.WindowState;
                });
            //_titlebar = this.Template.FindName("PART_TitleBar", this) as Titlebar;
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            var parentTabItem =
                UITools.FindAncestor<TabItem>(e.OriginalSource as System.Windows.DependencyObject, null);
            var parentButton =
                UITools.FindAncestor<Button>(e.OriginalSource as System.Windows.DependencyObject, null);

            if ((e.OriginalSource as FrameworkElement).Name ==
                "HeaderPanelScrollViewer")
                if (parentTabItem == null && parentButton == null &&
                    _ancestorWindow.WindowState == WindowState.Maximized)
                    _ancestorWindow.WindowState = WindowState.Normal;
                else
                    base.OnMouseDoubleClick(e);
        }

        private static void OnShowTabPanelPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var tc = obj as TabControlEx;
            tc.ShowTabPanelUI = tc.ShowTabPanel || tc.WindowState == WindowState.Maximized;
        }

        #endregion

        #region Data

        private Window _ancestorWindow;

        #endregion

        #region Public Properties


        public static DependencyProperty LeftTabHeaderContentProperty = DependencyProperty.Register(
            "LeftTabHeaderContent", typeof(object), typeof(TabControlEx));

        public object LeftTabHeaderContent
        {
            get { return GetValue(LeftTabHeaderContentProperty); }
            set { SetValue(LeftTabHeaderContentProperty, value); }
        }

        public static DependencyProperty RightTabHeaderContentProperty = DependencyProperty.Register(
            "RightTabHeaderContent", typeof(object), typeof(TabControlEx));

        public object RightTabHeaderContent
        {
            get { return GetValue(RightTabHeaderContentProperty); }
            set { SetValue(RightTabHeaderContentProperty, value); }
        }

        public static DependencyProperty WindowStateProperty = DependencyProperty.Register(
            "WindowState", typeof(WindowState), typeof(TabControlEx),
            new PropertyMetadata(new PropertyChangedCallback(OnShowTabPanelPropertyChanged)));

        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public static DependencyProperty ShowTabPanelProperty = DependencyProperty.Register(
            "ShowTabPanel", typeof(bool), typeof(TabControlEx), 
            new PropertyMetadata(new PropertyChangedCallback(OnShowTabPanelPropertyChanged)));

        public bool ShowTabPanel
        {
            get { return (bool)GetValue(ShowTabPanelProperty); }
            set { SetValue(ShowTabPanelProperty, value); }
        }


        public static DependencyProperty ShowTabPanelUIProperty = DependencyProperty.Register(
            "ShowTabPanelUI", typeof(bool), typeof(TabControlEx), new PropertyMetadata(true));

        public bool ShowTabPanelUI
        {
            get { return (bool)GetValue(ShowTabPanelUIProperty); }
            set { SetValue(ShowTabPanelUIProperty, value); }
        }

        #endregion
    }

    public class TabItemEx : TabItem
    {
        #region Constructors

        static TabItemEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemEx),
                new FrameworkPropertyMetadata(typeof(TabItemEx)));
        }

        public TabItemEx()
        {


        }


        #endregion

        #region Methods



        #endregion

        #region Data


        #endregion

        #region Public Properties

        public static DependencyProperty ShowPlaceHolderProperty = DependencyProperty.Register("ShowPlaceHolder",
            typeof(bool), typeof(TabItemEx), new PropertyMetadata(false));

        public bool ShowPlaceHolder
        {
            get { return (bool)GetValue(ShowPlaceHolderProperty); }
            set { SetValue(ShowPlaceHolderProperty, value); }
        }

        public static DependencyProperty HeaderOpacityProperty = DependencyProperty.Register("HeaderOpacity",
           typeof(float), typeof(TabItemEx), new PropertyMetadata(1.0f));

        public float HeaderOpacity
        {
            get { return (float)GetValue(HeaderOpacityProperty); }
            set { SetValue(HeaderOpacityProperty, value); }
        }

        #endregion
    }
}
