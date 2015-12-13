using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF.BaseControls
{

    /// <summary>
    /// ScrollViewer that contains Top/Right/Bottom/LeftContent so you can insert fixed content to it.
    /// </summary>
    public class DockableScrollViewer : ScrollViewer
    {

        #region Constructors

        static DockableScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockableScrollViewer),
                new FrameworkPropertyMetadata(typeof(DockableScrollViewer)));
        }

        public DockableScrollViewer()
        {

        }

        #endregion

        #region Methods

        private void updateContentParams(DependencyProperty contentProperty)
        {
            if (_templateApplied)
            {

                var content = this.GetValue(contentProperty) as FrameworkElement;
                if (content != null)
                {
                    double contentSize = (double)content.GetValue(DockableScrollViewer.ContentSizeProperty);
                    if (contentSize != 0)
                    {
                        var eleContent = this.Template.FindName(contentProperty.Name, this) as ContentPresenter;
                        if (eleContent != null)
                            eleContent.SetValue(ContentPresenter.WidthProperty, contentSize);
                    }
                }
            }
        }
        private void updateContentParams()
        {
            updateContentParams(DockableScrollViewer.TopContentProperty);
            updateContentParams(DockableScrollViewer.RightContentProperty);
            updateContentParams(DockableScrollViewer.BottomContentProperty);
            updateContentParams(DockableScrollViewer.LeftContentProperty);
            updateContentParams(DockableScrollViewer.OuterTopContentProperty);
            updateContentParams(DockableScrollViewer.OuterRightContentProperty);
            updateContentParams(DockableScrollViewer.OuterBottomContentProperty);
            updateContentParams(DockableScrollViewer.OuterLeftContentProperty);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templateApplied = true;
            updateContentParams();
        }

        private static void OnContentChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as DockableScrollViewer).updateContentParams(args.Property);
        }

        #endregion

        #region Data

        private bool _templateApplied = false;

        #endregion

        #region Public Properties

        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.RegisterAttached("IsResizable", typeof(bool), typeof(DockableScrollViewer), new PropertyMetadata(false));

        public static bool GetIsResizable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsResizableProperty);
        }


        public static void SetIsResizable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsResizableProperty, value);
        }


        public static readonly DependencyProperty IsContentVisibleProperty =
           DependencyProperty.RegisterAttached("IsContentVisible", typeof(bool), typeof(DockableScrollViewer), new PropertyMetadata(true));

        public static bool GetIsContentVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsContentVisibleProperty);
        }

        public static void SetIsContentVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsContentVisibleProperty, value);
        }


        public static readonly DependencyProperty ContentSizeProperty =
           DependencyProperty.RegisterAttached("ContentSize", typeof(double), typeof(DockableScrollViewer));

        public static double GetContentSize(DependencyObject obj)
        {
            return (double)obj.GetValue(ContentSizeProperty);
        }

        public static void SetContentSize(DependencyObject obj, double value)
        {
            obj.SetValue(ContentSizeProperty, value);
        }


        public static readonly DependencyProperty TopContentProperty =
            DependencyProperty.Register("TopContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object TopContent
        {
            get { return (object)GetValue(TopContentProperty); }
            set { SetValue(TopContentProperty, value); }
        }

        public static readonly DependencyProperty RightContentProperty =
           DependencyProperty.Register("RightContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object RightContent
        {
            get { return (object)GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }

        public static readonly DependencyProperty BottomContentProperty =
           DependencyProperty.Register("BottomContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null,OnContentChanged));
        public object BottomContent
        {
            get { return (object)GetValue(BottomContentProperty); }
            set { SetValue(BottomContentProperty, value); }
        }

        public static readonly DependencyProperty LeftContentProperty =
           DependencyProperty.Register("LeftContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object LeftContent
        {
            get { return (object)GetValue(LeftContentProperty); }
            set { SetValue(LeftContentProperty, value); }
        }


        public static readonly DependencyProperty OuterTopContentProperty =
           DependencyProperty.Register("OuterTopContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object OuterTopContent
        {
            get { return (object)GetValue(OuterTopContentProperty); }
            set { SetValue(OuterTopContentProperty, value); }
        }

        public static readonly DependencyProperty OuterRightContentProperty =
           DependencyProperty.Register("OuterRightContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object OuterRightContent
        {
            get { return (object)GetValue(OuterRightContentProperty); }
            set { SetValue(OuterRightContentProperty, value); }
        }

        public static readonly DependencyProperty OuterBottomContentProperty =
           DependencyProperty.Register("OuterBottomContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object OuterBottomContent
        {
            get { return (object)GetValue(OuterBottomContentProperty); }
            set { SetValue(OuterBottomContentProperty, value); }
        }

        public static readonly DependencyProperty OuterLeftContentProperty =
           DependencyProperty.Register("OuterLeftContent", typeof(object), typeof(DockableScrollViewer), new PropertyMetadata(null, OnContentChanged));
        public object OuterLeftContent
        {
            get { return (object)GetValue(OuterLeftContentProperty); }
            set { SetValue(OuterLeftContentProperty, value); }
        }


        #endregion
    }
}
