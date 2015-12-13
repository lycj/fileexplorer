using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;

namespace QuickZip.UserControls
{
    public class SelectionAdorner : Adorner
    {
        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.Opacity = 0.5;
            IsSelecting = false;
            StartPosition = new Point(0, 0);
            EndPosition = new Point(0, 0);
        }

        public static DependencyProperty IsSelectingProperty = DependencyProperty.Register("IsSelecting", typeof(bool), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsSelectingChanged)));

        public static DependencyProperty StartPositionProperty = DependencyProperty.Register("StartPosition", typeof(Point), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender,  null, new CoerceValueCallback(SelectionHelper.PositionCheck)));

        public static DependencyProperty EndPositionProperty = DependencyProperty.Register("EndPosition", typeof(Point), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(SelectionHelper.PositionCheck)));

        public bool IsSelecting
        {
            get { return (bool)GetValue(IsSelectingProperty); }
            set { SetValue(IsSelectingProperty, value); }
        }

        public Point StartPosition
        {
            get { return (Point)GetValue(StartPositionProperty); }
            set { SetValue(StartPositionProperty, value); }
        }

        public Point EndPosition
        {
            get { return (Point)GetValue(EndPositionProperty); }
            set { SetValue(EndPositionProperty, value); }
        }

       

        private static void OnIsSelectingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ((SelectionAdorner)sender).Visibility = Visibility.Visible;
            else ((SelectionAdorner)sender).Visibility = Visibility.Collapsed;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (IsSelecting)
            {
                drawingContext.DrawRectangle(SystemColors.HotTrackBrush, new Pen(Brushes.Black, 1),
                    new Rect(StartPosition, EndPosition));
            }
            base.OnRender(drawingContext);
        }

    }
}
