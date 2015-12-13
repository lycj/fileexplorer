using FileExplorer.UIEventHub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileExplorer.WPF.BaseControls
{
    /// <summary>
    /// For use in CanvasDrag, so selected items follow cursor around.
    /// </summary>
    public class SelectedItemsAdorner : Adorner
    {
        #region Constructor  

        public SelectedItemsAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
          
            _canvas = new SelfCenteredCanvas()
            {
                Opacity = 0.3,                
                Visibility = System.Windows.Visibility.Visible,
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };


            this.AddLogicalChild(_canvas);
            this.AddVisualChild(_canvas);

            IsHitTestVisible = false;
            
        }


        #endregion

        #region Methods

        protected override Size MeasureOverride(Size constraint)
        {
            _canvas.Measure(constraint);
            return constraint;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            _canvas.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as SelectedItemsAdorner;
            Point initialPosition = adorner.CurrentPosition;
            var draggables = adorner.Items                
                .OfType<IDraggable>()
                .Where(pa => pa is IPositionAware)
                .Where(ipa => ipa.IsDragging).ToArray();
            adorner._canvas.Children.Clear();
            foreach (var item in draggables.Cast<IPositionAware>())
            {
                Vector offset = item.GetMiddlePoint() - initialPosition;
                ContentPresenter cc = new ContentPresenter() { Content = item };
                SelfCenteredCanvas.SetOffsetX(cc, offset.X);
                SelfCenteredCanvas.SetOffsetY(cc, offset.Y);
                adorner._canvas.Children.Add(cc);
            }
        }

        private static void OnCurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var adorner = d as SelectedItemsAdorner;
            adorner._canvas.CentreX = adorner.CurrentPosition.X;
            adorner._canvas.CentreY = adorner.CurrentPosition.Y;
        }


        #endregion

        #region Data

        private SelfCenteredCanvas _canvas;


        #endregion

        #region Public Properties

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index)
        {
            return _canvas;
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return _canvas;
            }
        }

        public Point CurrentPosition
        {
            get { return (Point)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
         DependencyProperty.Register("CurrentPosition", typeof(Point), typeof(SelectedItemsAdorner),
         new UIPropertyMetadata(new Point(-1, -1), new PropertyChangedCallback(OnCurrentPositionChanged)));

     
        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Items, a list of item view models, Items that support IDraggablePositionAware and 
        /// IDraggablePosition.IsSelected will be used to create VisibleItems.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IEnumerable), typeof(SelectedItemsAdorner),
            new UIPropertyMetadata(new System.Collections.ArrayList(), new PropertyChangedCallback(OnItemsChanged)));

        public IEnumerable VisibleItems
        {
            get { return (IEnumerable)GetValue(VisibleItemsProperty); }
            set { SetValue(VisibleItemsProperty, value); }
        }

        /// <summary>
        /// VisibleItems, a list of ContentControls, which contents the Items this is assigned by the adorner.
        /// </summary>
        public static readonly DependencyProperty VisibleItemsProperty =
            DependencyProperty.Register("VisibleItems", typeof(IEnumerable), typeof(SelectedItemsAdorner),
            new UIPropertyMetadata(new List<ContentControl>()));
        
        
        #endregion



    }
}
