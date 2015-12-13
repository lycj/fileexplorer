using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileExplorer.WPF.BaseControls
{
    public class SelectionAdorner : Adorner
    {
        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.Opacity = 0.5;
            IsHitTestVisible = false;
            IsSelecting = false;
            StartPosition = new Point(0, 0);
            EndPosition = new Point(0, 0);            
        }


        internal static object PositionCheck(DependencyObject sender, object value)
        {
            if (value is Point)
            {
                Point ptValue = (Point)value;
                SelectionAdorner adorner = sender as SelectionAdorner;
                ptValue.X = Math.Max(ptValue.X, 0);
                if (adorner.ActualWidth != 0)
                    ptValue.X = Math.Min(ptValue.X, adorner.ActualWidth);
                ptValue.Y = Math.Max(ptValue.Y, 0);
                if (adorner.ActualHeight != 0)
                    ptValue.Y = Math.Min(ptValue.Y, adorner.ActualHeight);
                return ptValue;
            }
            return value;
        }

        public static DependencyProperty IsSelectingProperty = DependencyProperty.Register("IsSelecting", typeof(bool), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsSelectingChanged)));

        public static DependencyProperty AdjustVectorProperty = DependencyProperty.Register("AdjustVector", typeof(Vector), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(new Vector(0, 0), FrameworkPropertyMetadataOptions.AffectsRender, null));


        public static DependencyProperty StartPositionProperty = DependencyProperty.Register("StartPosition", typeof(Point), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(PositionCheck)));

        public static DependencyProperty EndPositionProperty = DependencyProperty.Register("EndPosition", typeof(Point), typeof(SelectionAdorner),
           new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(PositionCheck)));

        public bool IsSelecting
        {
            get { return (bool)GetValue(IsSelectingProperty); }
            set { SetValue(IsSelectingProperty, value); }
        }

        public Vector AdjustVector
        {
            get { return (Vector)GetValue(AdjustVectorProperty); }
            set { SetValue(AdjustVectorProperty, value); }
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
                //drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1),
                //    new Rect(0, 0, 1000, 1000));
                Point startPos = new Point(StartPosition.X + AdjustVector.X, StartPosition.Y + AdjustVector.Y);
                Point endPos = new Point(EndPosition.X + AdjustVector.X, EndPosition.Y + AdjustVector.Y);

                drawingContext.DrawRectangle(SystemColors.HotTrackBrush, new Pen(Brushes.Black, 1), new Rect(startPos, endPos));
            }
            base.OnRender(drawingContext);
        }

    }
}
