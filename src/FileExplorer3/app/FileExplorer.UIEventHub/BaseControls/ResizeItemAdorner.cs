using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileExplorer.WPF.BaseControls
{
    /// <summary>
    /// Shows a canvas with a ResizeThumb.
    /// </summary>
    public class ResizeItemAdorner : Adorner
    {
        #region Constructor


        public ResizeItemAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _resize = new ResizeDecorator() { IsHitTestVisible = true, Background = null, DataContext = null };
            //_resize.SetBinding(Control.WidthProperty, "SelectedItem.Size.Width");
            //_resize.SetBinding(Control.HeightProperty, "SelectedItem.Size.Height");
            //_resize.SetBinding(CenteredCanvas.XProperty, "Position.X");
            //_resize.SetBinding(CenteredCanvas.YProperty, "Position.Y");


            //_resize.AddHandler(Thumb.DragDeltaEvent, (DragDeltaEventHandler)((o, e) =>
            //    {
            //        Thumb thumb = e.OriginalSource as Thumb;
            //        double deltaVertical = 0, deltaHorizontal = 0;

            //        switch (thumb.VerticalAlignment)
            //        {
            //            case System.Windows.VerticalAlignment.Bottom:
            //                deltaVertical = Math.Min(-e.VerticalChange, _resize.ActualHeight - _resize.MinHeight);
            //                break;
            //            case System.Windows.VerticalAlignment.Top:
            //                deltaVertical = Math.Min(e.VerticalChange, _resize.ActualHeight - _resize.MinHeight);
            //                break;
            //        }

            //        switch (thumb.HorizontalAlignment)
            //        {
            //            case System.Windows.HorizontalAlignment.Left:
            //                deltaHorizontal = Math.Min(e.HorizontalChange, _resize.ActualWidth - _resize.MinWidth);
            //                break;
            //            case System.Windows.HorizontalAlignment.Right:
            //                deltaHorizontal = Math.Min(-e.HorizontalChange, _resize.ActualWidth - _resize.MinWidth);
            //                break;
            //        }


            //        if (deltaHorizontal != 0)
            //            _resizable.Size = new Size(deltaVertical + _resizable.Size.Height, deltaHorizontal + _resizable.Size.Width);
            //            //_resize.ScaleX = 1 + (deltaHorizontal / _resizable.Size.Width);
            //        //if (deltaVertical != 0) _resize.ScaleY += (deltaVertical / _resize.ActualHeight);
            //        Console.WriteLine(e.HorizontalChange);

            //    }));

            //_canvas = new CenteredCanvas() { Background = null };
            //_canvas.Children.Add(_resize);

            this.AddLogicalChild(_resize);
            this.AddVisualChild(_resize);


        }

        #endregion

        #region Methods

        protected override Size MeasureOverride(Size constraint)
        {
            _resize.Measure(constraint);
            return new Size(_resizable.Width, _resizable.Height);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            _resize.Arrange(new Rect(0, 0, _resizable.Width, _resizable.Height));
            return new Size(_resizable.Width, _resizable.Height);
        }

        //public void SetTargetItem(IResizable selected)
        //{
        //    _resize.DataContext = selected;
        //}

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizeItemAdorner)._resizable = (IResizable)e.NewValue;
            //(d as ResizeItemAdorner)._resize.DataContext = e.NewValue;
            (d as ResizeItemAdorner)._resize.Content = e.NewValue;
        }


        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (d as ResizeItemAdorner);
            if (e.Property == OffsetWidthProperty)
                adorner._resize.ScaleX = 1.0d + (double)e.NewValue / adorner._resizable.Width;
            else if (e.Property == OffsetHeightProperty)                            
                adorner._resize.ScaleY = 1.0d + (double)e.NewValue / adorner._resizable.Height;            
            else if (e.Property == OffsetLeftProperty)
                adorner._resize.TranslateX = (double)e.NewValue;
            else if (e.Property == OffsetTopProperty)
                adorner._resize.TranslateY = (double)e.NewValue;

            //Console.WriteLine(String.Format("Scale={0},{1} Size={2},{3} Offset={4},{5}", adorner._resize.ScaleX,
            //    adorner._resize.ScaleY, adorner._resizable.Width, adorner._resizable.Height, adorner.OffsetX, adorner.OffsetY));
        }


        #endregion

        #region Data

        private ResizeDecorator _resize;
        //private ContentPresenter _cc;
        private IResizable _resizable;
        //private CenteredCanvas _canvas;

        #endregion

        #region Public Properties

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index)
        {
            return _resize;
        }

        public double OffsetWidth
        {
            get { return (double)GetValue(OffsetWidthProperty); }
            set { SetValue(OffsetWidthProperty, value); }
        }

        public static readonly DependencyProperty OffsetWidthProperty =
         DependencyProperty.Register("OffsetWidth", typeof(double), typeof(ResizeItemAdorner),
         new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));

        public double OffsetHeight
        {
            get { return (double)GetValue(OffsetHeightProperty); }
            set { SetValue(OffsetHeightProperty, value); }
        }

        public static readonly DependencyProperty OffsetHeightProperty =
         DependencyProperty.Register("OffsetHeight", typeof(double), typeof(ResizeItemAdorner),
         new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));


        public double OffsetLeft
        {
            get { return (double)GetValue(OffsetLeftProperty); }
            set { SetValue(OffsetLeftProperty, value); }
        }

        public static readonly DependencyProperty OffsetLeftProperty =
         DependencyProperty.Register("OffsetLeft", typeof(double), typeof(ResizeItemAdorner),
         new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));

        public double OffsetTop
        {
            get { return (double)GetValue(OffsetTopProperty); }
            set { SetValue(OffsetTopProperty, value); }
        }

        public static readonly DependencyProperty OffsetTopProperty =
         DependencyProperty.Register("OffsetTop", typeof(double), typeof(ResizeItemAdorner),
         new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));


        public IResizable SelectedItem
        {
            get { return (IResizable)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
         DependencyProperty.Register("SelectedItem", typeof(IResizable), typeof(ResizeItemAdorner),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));


        #endregion
    }
}
