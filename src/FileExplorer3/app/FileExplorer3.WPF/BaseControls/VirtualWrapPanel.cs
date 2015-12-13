
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Dan Crevier's and Ben Constable's work                                                  //
//(http://blogs.msdn.com/dancre/archive/2006/02/16/implementing-a-virtualizingpanel-part-4-the-goods.aspx)       //
//(http://blogs.msdn.com/bencon/)                                                                                //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.UserControls
{
    public class VirtualWrapPanel : AnimatedVirtualizingPanel, IScrollInfo, IChildInfo
    {
        public VirtualWrapPanel()
        {
            // For use in the IScrollInfo implementation
            this.RenderTransform = _trans;
        }

        #region Methods

        #region Layout specific code
        // I've isolated the layout specific code to this region. If you want to do something other than tiling, this is
        // where you'll make your changes

        /// <summary>
        /// Calculate the extent of the view based on the available size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        /// <returns></returns>
        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            if (Orientation == Orientation.Horizontal)
            {
                int childPerRow = CalculateChildrenPerRow(availableSize);
                return new Size(childPerRow * this.ChildSize.Width,
                    this.ChildSize.Height * Math.Ceiling((double)itemCount / childPerRow));
            }
            else
            {
                int childPerCol = CalculateChildrenPerCol(availableSize);
                return new Size(this.ChildSize.Width * Math.Ceiling((double)itemCount / childPerCol),
                    childPerCol * this.ChildSize.Height);
            }
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            if (Orientation == Orientation.Horizontal)
            {
                int childPerRow = CalculateChildrenPerRow(_extent);

                firstVisibleItemIndex = (int)Math.Floor(_offset.Y / this.ChildSize.Height) * childPerRow;
                lastVisibleItemIndex = (int)Math.Ceiling((_offset.Y + _viewport.Height) / this.ChildSize.Height) * childPerRow - 1;

                firstVisibleItemIndex -= CacheItemCount;
                if (firstVisibleItemIndex < 0) firstVisibleItemIndex = 0;
                lastVisibleItemIndex += CacheItemCount;   

                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
                if (lastVisibleItemIndex >= itemCount)
                    lastVisibleItemIndex = itemCount - 1;
            }
            else
            {
                int childPerCol = CalculateChildrenPerCol(_extent);

                firstVisibleItemIndex = (int)Math.Floor(_offset.X / this.ChildSize.Width) * childPerCol;
                lastVisibleItemIndex = (int)Math.Ceiling((_offset.X + _viewport.Width) / this.ChildSize.Width) * childPerCol - 1;

                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
                if (lastVisibleItemIndex >= itemCount)
                    lastVisibleItemIndex = itemCount - 1;
            }

        }

        private Rect GetChildRect(int itemIndex, Size finalSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                int childPerRow = CalculateChildrenPerRow(finalSize);

                int row = itemIndex / childPerRow;
                int column = itemIndex % childPerRow;

                return new Rect(column * this.ChildSize.Width, row * this.ChildSize.Height, this.ChildSize.Width, this.ChildSize.Height);
            }
            else
            {
                int childPerCol = CalculateChildrenPerCol(finalSize);

                int column = itemIndex / childPerCol;
                int row = itemIndex % childPerCol;

                return new Rect(column * this.ChildSize.Width, row * this.ChildSize.Height, this.ChildSize.Width, this.ChildSize.Height);
            }
        }

       


        #region Animated arrange mode

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            Rect finalRect = GetChildRect(itemIndex, finalSize);
            //Point initPos = GetOriginalPosition(child);
            Point finalPos = new Point(finalRect.Left, finalRect.Top);
            
            //child.Arrange(finalRect);

            base.AnimatedArrange(child, finalRect);

            //if (initPos == new Point(-1, -1) || initPos == finalPos)
            //{
            //    child.Arrange(finalRect);
            //    SetOriginalPosition(child, finalPos);
            //}
            //else
            //{
            //    ArrangeChildAnimated((FrameworkElement)child, initPos, finalPos, new Size(finalRect.Width, finalRect.Height));
            //}

            //Debug.WriteLine(String.Format("{0}: {1} --> {2}", itemIndex, initPos, finalPos));
        }


        //private Duration duration = new Duration(TimeSpan.FromSeconds(0.5));


        //public static Point GetOriginalPosition(DependencyObject obj)
        //{
        //    return (Point)obj.GetValue(OriginalPositionProperty);
        //}

        //public static void SetOriginalPosition(DependencyObject obj, Point value)
        //{
        //    obj.SetValue(OriginalPositionProperty, value);
        //}
        
        //public static readonly DependencyProperty OriginalPositionProperty =
        //    DependencyProperty.RegisterAttached("OriginalPosition", typeof(Point), 
        //    typeof(VirtualWrapPanel), new UIPropertyMetadata(new Point(-1, -1)));

        
        

        ////http://codeblitz.wordpress.com/2009/03/20/wpf-auto-arrange-animated-panel/
        //private void ArrangeChildAnimated(FrameworkElement element, Point initPosition, Point finalPosition, Size allocatedSize)
        //{            
        //    TranslateTransform trans = element.RenderTransform as TranslateTransform;

        //    // create new Translate Transform
        //    if (trans == null)
        //    {
        //        trans = new TranslateTransform();
        //        element.RenderTransform = trans;
        //    }
        //    // tell the child element to arrange itself
        //    element.Arrange(new Rect(initPosition, allocatedSize));

        //    // keep translation point and store it in it's Tag property for rendering later.
        //    Point translatePosition = new Point(finalPosition.X - initPosition.X, finalPosition.Y - initPosition.Y);

        //    Dispatcher.BeginInvoke(new Action<FrameworkElement, Point>(BeginAnimation),
        //            DispatcherPriority.Render, element, translatePosition);
        //}

        //private void BeginAnimation(FrameworkElement element, Point translatePosition)
        //{
            
        //    // hook up X and Y translate transformations
        //    element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, 
        //        new DoubleAnimation(translatePosition.X, duration), HandoffBehavior.Compose);
        //    element.RenderTransform.BeginAnimation(TranslateTransform.YProperty,
        //        new DoubleAnimation(translatePosition.Y, duration), HandoffBehavior.Compose);
        //}

        #endregion

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int CalculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow;
            if (availableSize.Width == Double.PositiveInfinity)
                childrenPerRow = this.Children.Count;
            else
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ChildSize.Width));
            return childrenPerRow;
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int CalculateChildrenPerCol(Size availableSize)
        {
            // Figure out how many children fit on each row            
            if (availableSize.Height == Double.PositiveInfinity)
                return this.Children.Count;
            else
                return Math.Max(1, (int)Math.Floor(availableSize.Height / this.ChildSize.Height));
        }

        #endregion

        #region VirtualizingPanel
        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);

            // Figure out range that's visible based on layout algorithm
            int firstVisibleItemIndex, lastVisibleItemIndex;
            GetVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);

            // We need to access InternalChildren before the generator to work around a bug
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

            // Get index where we'd insert the child for this position. If the item is realized
            // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
            // insert after the corresponding child
            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;

                    if (newlyRealized)
                    {
                        // Figure out if we need to insert the child at the end or somewhere in the middle
                        if (childIndex >= children.Count)
                        {
                            base.AddInternalChild(child);
                        }
                        else
                        {
                            base.InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    else
                    {
                        // The child has already been created, let's be sure it's in the right spot
                        Debug.Assert(child == children[childIndex], "Wrong child was generated");
                    }

                    // Measurements will depend on layout algorithm
                    child.Measure(ChildSize);
                }
            }

            // Note: this could be deferred to idle time for efficiency
            CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return availableSize;//base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            UpdateScrollInfo(finalSize);

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                // Map the child offset to an item offset
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                ArrangeChild(itemIndex, child, finalSize);
            }

            return  base.ArrangeOverride(finalSize);;
        }

        /// <summary>
        /// Revirtualize items that are no longer visible
        /// </summary>
        /// <param name="minDesiredGenerated">first item index that should be visible</param>
        /// <param name="maxDesiredGenerated">last item index that should be visible</param>
        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

            }
            base.OnItemsChanged(sender, args);
        }
        #endregion

        #region IScrollInfo
        // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/
        private void UpdateScrollInfo(Size availableSize)
        {
            // See how many items there are
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            Size extent = CalculateExtent(availableSize, itemCount);
            // Update extent
            if (extent != _extent)
            {
                _extent = extent;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }

            // Update viewport
            if (availableSize != _viewport)
            {
                _viewport = availableSize;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }
        }

        public ScrollViewer ScrollOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool CanHorizontallyScroll
        {
            get { return _canHScroll; }
            set { _canHScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _canVScroll; }
            set { _canVScroll = value; }
        }

        public double HorizontalOffset
        {
            get { return _offset.X; }
        }

        public double VerticalOffset
        {
            get { return _offset.Y; }
        }

        public double ExtentHeight
        {
            get { return _extent.Height; }
        }

        public double ExtentWidth
        {
            get { return _extent.Width; }
        }

        public double ViewportHeight
        {
            get { return _viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return _viewport.Width; }
        }        

        public void AddOffset(bool largeChange)
        {
            double yChanges = (largeChange ? _viewport.Height : SmallChanges);
            double xChanges = (largeChange ? _viewport.Width : SmallChanges);

            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(this.VerticalOffset + yChanges);
            else SetHorizontalOffset(this.HorizontalOffset + xChanges);            
        }

        public void SubtractOffset(bool largeChange)
        {
            double yChanges = (largeChange ? _viewport.Height : SmallChanges);
            double xChanges = (largeChange ? _viewport.Width : SmallChanges);

            if (Orientation == Orientation.Horizontal)
                SetVerticalOffset(this.VerticalOffset - yChanges);
            else SetHorizontalOffset(this.HorizontalOffset - xChanges);            
        }

        public void PageUp()
        {
            SubtractOffset(false); 
        }

        public void PageDown()
        {
            AddOffset(true);
        }

        public void MouseWheelUp()
        {
            SubtractOffset(false);
        }

        public void MouseWheelDown()
        {
            AddOffset(false);
        }

        public void LineUp()
        {
            SetVerticalOffset(this.VerticalOffset - SmallChanges);
        }

        public void LineDown()
        {
            SetVerticalOffset(this.VerticalOffset + SmallChanges);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - SmallChanges);
        }

        public void LineRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + SmallChanges);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - SmallChanges);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + SmallChanges);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - _viewport.Width);
        }

        public void PageRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + _viewport.Width);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || _viewport.Width >= _extent.Width)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Width >= _extent.Width)
                {
                    offset = _extent.Width - _viewport.Width;
                }
            }

            _offset.X = offset;

            if (_owner != null)
                _owner.InvalidateScrollInfo();

            _trans.X = -offset;

            // Force us to realize the correct children
            InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Height >= _extent.Height)
                {
                    offset = _extent.Height - _viewport.Height;
                }
            }

            _offset.Y = offset;

            if (_owner != null)
                _owner.InvalidateScrollInfo();

            _trans.Y = -offset;

            // Force us to realize the correct children
            InvalidateMeasure();
        }
        #endregion

        #region IChildInfo

        public Rect GetChildRect(int itemIndex)
        {
            return GetChildRect(itemIndex, _extent);
        }

        #endregion

        #endregion


        #region Data
        private TranslateTransform _trans = new TranslateTransform();
        private ScrollViewer _owner;
        private bool _canHScroll = false;
        private bool _canVScroll = false;
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;
        #endregion

        #region Public Properties

        public static readonly DependencyProperty ItemWidthProperty
           = DependencyProperty.Register("ItemWidth", typeof(double), typeof(VirtualWrapPanel),
              new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        // Accessor for the child size dependency property
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty
           = DependencyProperty.Register("ItemHeight", typeof(double), typeof(VirtualWrapPanel),
              new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

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
             
                return new Size(childWidth, childHeight);
            }
        }

        public static readonly DependencyProperty OrientationProperty
           = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualWrapPanel),
              new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty SmallChangesProperty =
            DependencyProperty.Register("SmallChanges", typeof(uint), typeof(VirtualWrapPanel),
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
            typeof(VirtualWrapPanel), new UIPropertyMetadata(0));


        #endregion
    }


  
}
