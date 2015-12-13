using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;
using FileExplorer.Defines;

namespace FileExplorer.WPF.BaseControls
{
    public class DragAdorner : Adorner
    {

        public DragAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _canvas = new Canvas() { Background = null };
            _text = new TextBlock() { Foreground = Brushes.Black, FontWeight = FontWeights.Bold,                
                 };
            _border = new Border() { Child = _text, BorderThickness = new Thickness(1), BorderBrush = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Left, Padding= new Thickness(2,0, 2, 0),
                        Background = Brushes.White
            };
            _border.RenderTransform = new TranslateTransform(10, 0);
            //FrameworkElementFactory overlapPanelfactory = new FrameworkElementFactory(typeof(OverlappingPanel));
            //overlapPanelfactory.SetValue(OverlappingPanel.OverlapXProperty, 5.0d);
            //overlapPanelfactory.SetValue(OverlappingPanel.OverlapYProperty, 5.0d);

            _items = new ItemsControl()
                {
                    MaxHeight = 250,
                    MaxWidth = 1000,
                    Visibility = System.Windows.Visibility.Collapsed,
                    Opacity = 0.8,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center, 
            
                    //ItemsPanel = new ItemsPanelTemplate(overlapPanelfactory)
                };

           
            _stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
            _stackPanel.Children.Add(_border);
            _stackPanel.Children.Add(_items);
            Canvas.SetTop(_stackPanel, -50);
            Canvas.SetLeft(_stackPanel, -50);


            this.ContextMenu = new ContextMenu() { PlacementTarget = _items };
            SetSupportedDragDropEffects(DragDropEffectsEx.All);


            this.ContextMenu.AddHandler(MenuItem.ClickEvent, (RoutedEventHandler)((o, e) =>
            {
                MenuItem mi =  e.Source as MenuItem;
                if (mi.Tag is DragDropEffectsEx)
                {
                    this.SetValue(DragDropEffectProperty, mi.Tag);
                    this.ContextMenu.StaysOpen = false;
                }
            }));

            _canvas.Children.Add(_stackPanel);
            this.AddLogicalChild(_canvas);
            this.AddVisualChild(_canvas);
        }


        #region Methods

        public void SetSupportedDragDropEffects(DragDropEffectsEx effects, DragDropEffectsEx defaultEffect = DragDropEffectsEx.Copy)
        {
            ContextMenu.Items.Clear();
            foreach (var e in Enum.GetValues(typeof(DragDropEffectsEx)))
            {
                DragDropEffectsEx curEffect = (DragDropEffectsEx)e;
                if (curEffect != DragDropEffectsEx.None && effects.HasFlag(curEffect))
                {
                    var header = new TextBlock() { Text = curEffect.ToString() };
                    if (curEffect.Equals(defaultEffect))
                    {
                        header.FontWeight = FontWeights.Bold;
                        ContextMenu.Items.Insert(0, new MenuItem() { Tag = e, Header = header });
                    }
                    else ContextMenu.Items.Add(new MenuItem() { Tag = e, Header = header });
                }
            }
        }

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index)
        {
            return _canvas;
        }

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

        public static void OnDraggingItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner adorner = o as DragAdorner;
            if (adorner != null)
            {
                adorner._items.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        public static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner adorner = o as DragAdorner;
            if (adorner != null)
            {
                string newValue = e.NewValue as string;
                adorner._text.Text = newValue;
                adorner._border.Visibility = String.IsNullOrEmpty(newValue) ? Visibility.Collapsed : Visibility.Visible;                
            }
        }

        public static void OnIsDraggingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner adorner = o as DragAdorner;
            if (adorner != null)
            {
                //Debug.WriteLine(e.NewValue);
                if ((bool)e.NewValue)
                    adorner._items.Visibility =  Visibility.Visible;
                else adorner._items.Visibility = Visibility.Collapsed;

                adorner._border.Visibility = adorner._items.Visibility;
            }
        }

        public static void OnIsContextMenuVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Write(e.NewValue);
            // DragAdorner adorner = o as DragAdorner;
            // if (adorner != null)
            // {
            //     Debug.WriteLine(adorner._canvas.ContextMenu.Visibility);
            // }
            // //DragAdorner adorner = o as DragAdorner;
            // //if (adorner != null)
            // //{
                 
                 
                 
            // //    adorner._canvas.ContextMenu.Visibility = Visibility.Visible;
            // //}
        }

        public static void OnPointerPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner adorner = o as DragAdorner;
            if (adorner != null)
            {
                Point newPos = (Point)e.NewValue;
                Canvas.SetLeft(adorner._stackPanel, newPos.X + 10);
                Canvas.SetTop(adorner._stackPanel, newPos.Y + 10);
            }
        }

        public static void OnDraggingItemTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DragAdorner adorner = o as DragAdorner;
            if (adorner != null)
            {
                adorner._items.ItemTemplate = e.NewValue as DataTemplate;
            }
        }

        

        #endregion

        #region Data

        private Canvas _canvas;
        private ItemsControl _items;
        private StackPanel _stackPanel;
        public TextBlock _text;
        private Border _border;

        #endregion

        #region Public Properties


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DragAdorner),
            new UIPropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));


        public DragDropEffectsEx DragDropEffect
        {
            get { return (DragDropEffectsEx)GetValue(DragDropEffectProperty); }
            set { SetValue(DragDropEffectProperty, value); }
        }
        
        public static readonly DependencyProperty DragDropEffectProperty =
            DependencyProperty.Register("DragDropEffect", typeof(DragDropEffectsEx), typeof(DragAdorner),
            new UIPropertyMetadata(DragDropEffectsEx.None));




        public IEnumerable DraggingItems
        {
            get { return (IEnumerable)GetValue(DraggingItemsProperty); }
            set { SetValue(DraggingItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DraggingItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DraggingItemsProperty =
            DependencyProperty.Register("DraggingItems", typeof(IEnumerable), typeof(DragAdorner),
            new UIPropertyMetadata(new System.Collections.ArrayList(), new PropertyChangedCallback(OnDraggingItemsChanged)));




        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDragging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(DragAdorner), new UIPropertyMetadata(false,
             new PropertyChangedCallback(OnIsDraggingChanged)));




        public bool IsContextMenuVisible
        {
            get { return (bool)GetValue(IsContextMenuVisibleProperty); }
            set { SetValue(IsContextMenuVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextMenuVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContextMenuVisibleProperty =
            DependencyProperty.Register("IsContextMenuVisible", typeof(bool), typeof(DragAdorner), new UIPropertyMetadata(false,
                 new PropertyChangedCallback(OnIsContextMenuVisibleChanged)));



        public Point PointerPosition
        {
            get { return (Point)GetValue(PointerPositionProperty); }
            set { SetValue(PointerPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerPositionProperty =
            DependencyProperty.Register("PointerPosition", typeof(Point), typeof(DragAdorner), 
            new UIPropertyMetadata(new Point(-50, -50), new PropertyChangedCallback(OnPointerPositionChanged)));




        public DataTemplate DraggingItemTemplate
        {
            get { return (DataTemplate)GetValue(DraggingItemTemplateProperty); }
            set { SetValue(DraggingItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DraggingItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DraggingItemTemplateProperty =
            DependencyProperty.Register("DraggingItemTemplate", typeof(DataTemplate), typeof(DragAdorner), 
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnDraggingItemTemplateChanged)));

        


        #endregion



    }
}
