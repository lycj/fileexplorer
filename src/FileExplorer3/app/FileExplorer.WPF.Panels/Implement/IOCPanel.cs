using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FileExplorer.WPF
{
    public /*abstract*/ class IOCPanel : VirtualizingPanel, IScrollInfo, IIOCPanel, IChildInfo
    {

        #region fields
        private ScrollViewer _scrollOwner;
        #endregion

        #region constructors

        public IOCPanel()
        {
            CanHorizontallyScroll = true;
            CanVerticallyScroll = true;
            Scroll = new NullScrollHelper(this);
        }

        #endregion

        #region events

        #endregion

        #region properties

        public IPanelScrollHelper Scroll { get; set; }
        public IItemGeneratorHelper Generator { get; set; }
        public IPanelLayoutHelper Layout { get; set; }


        public bool CanHorizontallyScroll { get; set; }
        public bool CanVerticallyScroll { get; set; }
        public double ExtentHeight { get { return Scroll.Extent.Height; } }
        public double ExtentWidth { get { return Scroll.Extent.Width; } }
        public double ViewportHeight { get { return Scroll.ViewPort.Height; } }
        public double ViewportWidth { get { return Scroll.ViewPort.Width; } }
        public double HorizontalOffset { get { return Scroll.Offset.X; } }
        public double VerticalOffset { get { return Scroll.Offset.Y; } }



        public ScrollViewer ScrollOwner
        {
            get { return _scrollOwner; }
            set
            {
                _scrollOwner = value;
                Scroll = new PanelScrollHelper(this, value);
            }
        }

        #endregion

        #region dependency properties


        public static readonly DependencyProperty ModeProperty
         = DependencyProperty.Register("Mode", typeof(LayoutType), typeof(IOCPanel),
            new FrameworkPropertyMetadata(LayoutType.FixedStack, FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnModeChanged)));

        // Accessor for arrange mode.
        public LayoutType Mode
        {
            get { return (LayoutType)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }


        public static readonly DependencyProperty ItemWidthProperty
         = DependencyProperty.Register("ItemWidth", typeof(double), typeof(IOCPanel),
            new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnResetLayout)));
        
        // Accessor for the child size dependency property
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty
           = DependencyProperty.Register("ItemHeight", typeof(double), typeof(IOCPanel),
              new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnResetLayout)));

        // Accessor for the child size dependency property
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public Size ChildSize
        {
            get
            {
                double childWidth = ItemWidth == 0 ?
                    Width == 0 || Width.Equals(double.NaN) ? ActualWidth : Width : ItemWidth;
                double childHeight = ItemHeight == 0 ?
                    Height == 0 || Height.Equals(double.NaN) ? ActualHeight : Height : ItemHeight;
                if (childWidth == 0 && Orientation == Orientation.Vertical)
                    childWidth = 500;
                if (childHeight == 0 && Orientation == Orientation.Vertical)
                    childHeight = 500;

                return new Size(childWidth, childHeight);
            }
        }


        public static readonly DependencyProperty OrientationProperty
           = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IOCPanel),
              new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnResetLayout)));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty VerticalContentAlignmentProperty
          = DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(IOCPanel),
             new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure |
             FrameworkPropertyMetadataOptions.AffectsArrange));

        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty
           = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(IOCPanel),
              new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty SmallChangesProperty =
            DependencyProperty.Register("SmallChanges", typeof(uint), typeof(IOCPanel),
              new FrameworkPropertyMetadata((uint)10, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        public uint SmallChanges
        {
            get { return (uint)GetValue(SmallChangesProperty); }
            set { SetValue(SmallChangesProperty, value); }
        }



        public int CacheItemCount
        {
            get { return (int)GetValue(CacheItemCountProperty); }
            set { SetValue(CacheItemCountProperty, value); }
        }

        public static readonly DependencyProperty CacheItemCountProperty =
            DependencyProperty.Register("CacheItemCount", typeof(int),
            typeof(IOCPanel), new UIPropertyMetadata(0));
        private Size _prevAvailableSize;



        #endregion

        #region methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var necessaryChidrenTouch = InternalChildren;
            Generator = new VirtualItemGenerator(this);
            Layout = getLayout();
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            Layout.ResetLayout(Generator[child]);
        }

        protected override Size MeasureOverride(Size availableSize)
        {            
            if (_prevAvailableSize != availableSize)
                Layout.ResetLayout();
            _prevAvailableSize = availableSize;            
            Scroll.UpdateScrollInfo(availableSize);
            Layout.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Scroll.UpdateScrollInfo(finalSize);
            Layout.Arrange(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Layout.ResetLayout();
                    break;
            }            
            base.OnItemsChanged(sender, args);
        }

        private IPanelLayoutHelper getLayout()
        {            
            switch (Mode)
            {
                case LayoutType.VariableStack: return new VariableStackLayout(this, Generator); 
                case LayoutType.FixedStack: return new FixedStackLayout(this, Generator);
                case LayoutType.FixedWrap: return new FixedWrapLayout(this, Generator);
                case LayoutType.HalfVariableWrap: return new HalfVariableWrapLayout(this, Generator);
                default:
                    throw new NotSupportedException();
            }            
        }                

        private void resetScrollOffset()
        {
            if (Scroll != null)
            {
                Scroll.UpdateOffsetX(OffsetType.Fixed, 0);
                Scroll.UpdateOffsetY(OffsetType.Fixed, 0);
            }
            InvalidateMeasure();
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IOCPanel panel = d as IOCPanel;
            if (panel.IsInitialized)
            {
                //panel.RemoveInternalChildRange(0, panel.getInternalChildren().Count);
                foreach (var child in panel.Children)
                    (child as FrameworkElement).InvalidateMeasure();
                panel.Layout = panel.getLayout();
                panel.resetScrollOffset();
            }
        }

        private static void OnResetLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IOCPanel panel = d as IOCPanel;
            if (panel.Layout != null)
                panel.Layout.ResetLayout();
            panel.resetScrollOffset();            
        }



        #region helper methods

        internal int getItemCount()
        {
            if (this.DataContext is System.Windows.Data.CollectionViewGroup)
                return (this.DataContext as System.Windows.Data.CollectionViewGroup).Items.Count;

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
            return itemCount;
        }

        internal void addOrInsertInternalChild(int childIndex, UIElement child)
        {
            if (childIndex >= InternalChildren.Count)
                base.AddInternalChild(child);
            else
                base.InsertInternalChild(childIndex, child);
        }

        internal void removeInternalChild(int childIndex)
        {
            base.RemoveInternalChildRange(childIndex, 1);
        }

        internal UIElementCollection getInternalChildren()
        {
            return InternalChildren;
        }

        internal UIElement getInternalChildren(int childIndex)
        {
            return InternalChildren[childIndex];
        }
        #endregion

        #region IScrollInfo implementations


        public void LineUp()
        {
            Scroll.UpdateOffsetY(OffsetType.Relative, -SmallChanges);
        }

        public void LineDown()
        {
            Scroll.UpdateOffsetY(OffsetType.Relative, +SmallChanges);
        }

        public void LineLeft()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, -SmallChanges);
        }

        public void LineRight()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, +SmallChanges);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {            
            return new Rect();

            //for (int i = 0; i < this.InternalChildren.Count; i++)
            //{

            //    if ((Visual)this.InternalChildren[i] == visual)                    
            //    {
            //        // we found the visual! Let's scroll it into view. First we need to know how big
            //        // each child is.
            //        Rect? arrangedRect = Layout[i].ArrangedRect;
            //        if (arrangedRect.HasValue)
            //        {
            //            // now we can calculate the vertical offset that we need and set it
            //            Scroll.UpdateOffsetX(OffsetType.Fixed, arrangedRect.Value.Left);
            //            Scroll.UpdateOffsetY(OffsetType.Fixed, arrangedRect.Value.Top);
            //        }
                    
            //        // child size is always smaller than viewport, because that is what makes the Panel
            //        // an AnnoyingPanel.
            //        return rectangle;
            //    }
            //}



            //throw new ArgumentException("Given visual is not in this Panel");

        }

        public void MouseWheelUp()
        {
            Scroll.UpdateOffset(OffsetType.Relative, -SmallChanges);
        }

        public void MouseWheelDown()
        {
            Scroll.UpdateOffset(OffsetType.Relative, +SmallChanges);
        }

        public void MouseWheelLeft()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, -SmallChanges);
        }

        public void MouseWheelRight()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, +SmallChanges);
        }

        public void PageUp()
        {
            Scroll.UpdateOffset(OffsetType.Relative, -Scroll.ViewPort.Height);
        }

        public void PageDown()
        {
            Scroll.UpdateOffset(OffsetType.Relative, +Scroll.ViewPort.Height);
        }

        public void PageLeft()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, -Scroll.ViewPort.Width);
        }

        public void PageRight()
        {
            Scroll.UpdateOffsetX(OffsetType.Relative, +Scroll.ViewPort.Width);
        }

        public void SetHorizontalOffset(double offset)
        {
            Scroll.UpdateOffsetX(OffsetType.Fixed, offset);
        }

        public void SetVerticalOffset(double offset)
        {
            Scroll.UpdateOffsetY(OffsetType.Fixed, offset);
        }

        #endregion

        #region IChildInfo

        public Rect GetChildRect(int itemIndex)
        {
            return Layout[itemIndex].ArrangedRect.Value;
        }

        #endregion

        #endregion










    }
}
