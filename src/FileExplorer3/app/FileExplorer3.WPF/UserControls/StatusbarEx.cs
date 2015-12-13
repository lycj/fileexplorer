using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.Defines;

namespace FileExplorer.WPF.UserControls
{
    public class StatusbarEx : HeaderedItemsControl
    {
        #region Cosntructor

        static StatusbarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusbarEx),
                new FrameworkPropertyMetadata(typeof(StatusbarEx)));
        }

        public StatusbarEx()
        {
            this.AddHandler(StatusbarEx.SizeChangedEvent, (SizeChangedEventHandler)OnSizeChanged);
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StatusbarItemEx();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }

        protected void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (double.IsNaN(IsExpandedDelta))
            {
                if (Orientation == Orientation.Horizontal)
                    IsExpandedDelta = this.ActualHeight + 10;
                else IsExpandedDelta = this.ActualWidth + 10;
            }
            else
            {
                bool isExpanded;
                if (Orientation == Orientation.Horizontal)
                    isExpanded = args.NewSize.Height > IsExpandedDelta;
                else isExpanded = args.NewSize.Width > IsExpandedDelta;
                if (isExpanded != IsExpanded)
                    IsExpanded = isExpanded;
            }
        }

        public static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            StatusbarEx sbar = sender as StatusbarEx;
            if (args.NewValue != args.OldValue)
                if ((bool)args.NewValue)
                    sbar.RaiseEvent(new RoutedEventArgs(ExpandedEvent));
                else sbar.RaiseEvent(new RoutedEventArgs(CollapsedEvent));
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
         typeof(Orientation), typeof(StatusbarEx), new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static DependencyProperty IsExpandedDeltaProperty = DependencyProperty.Register("IsExpandedDelta",
         typeof(double), typeof(StatusbarEx), new PropertyMetadata(30d));

        public double IsExpandedDelta
        {
            get { return (double)GetValue(IsExpandedDeltaProperty); }
            set { SetValue(IsExpandedDeltaProperty, value); }
        }

        public static DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded",
          typeof(bool), typeof(StatusbarEx), new UIPropertyMetadata(false, OnIsExpandedChanged));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }


        public static readonly RoutedEvent ExpandedEvent =
            EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(StatusbarEx));
        
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        public static readonly RoutedEvent CollapsedEvent =
    EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble,
    typeof(RoutedEventHandler), typeof(StatusbarEx));

        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }



        #endregion
    }


    public class StatusbarItemEx : HeaderedContentControl
    {
        #region Cosntructor

        static StatusbarItemEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusbarItemEx),
                new FrameworkPropertyMetadata(typeof(StatusbarItemEx)));
        }

        public StatusbarItemEx()
        {

        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static DependencyProperty IsHeaderProperty = DependencyProperty.Register("IsHeader",
           typeof(bool), typeof(StatusbarItemEx), new PropertyMetadata(false));

        public bool IsHeader
        {
            get { return (bool)GetValue(IsHeaderProperty); }
            set { SetValue(IsHeaderProperty, value); }
        }
        
        public static DependencyProperty TypeProperty = DependencyProperty.Register("Type",
            typeof(DisplayType), typeof(StatusbarItemEx), new PropertyMetadata(DisplayType.Auto));

        public DisplayType Type
        {
            get { return (DisplayType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }


        public static DependencyProperty CaptionWidthProperty = DependencyProperty.Register("CaptionWidth",
            typeof(double), typeof(StatusbarItemEx), new UIPropertyMetadata(100.0));

        public double CaptionWidth
        {
            get { return (double)GetValue(CaptionWidthProperty); }
            set { SetValue(CaptionWidthProperty, value); }
        }

        public static DependencyProperty IsExpandedProperty = StatusbarEx.IsExpandedProperty
            .AddOwner(typeof(StatusbarItemEx));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion
    }


}
