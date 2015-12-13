   //----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------
 
//#define Profiling
  
using MS.Internal;
using MS.Internal.Controls;
using MS.Utility;
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
  
namespace System.Windows.Controls1
{
    /// <summary>
    /// VirtualizingStackPanel is used to arrange children into single line.
    /// </summary>
    public class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------
 
        #region Constructors
 
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VirtualizingStackPanel()
        {
        }
 
        static VirtualizingStackPanel()
        {
            lock (DependencyProperty.Synchronized)
            {
                _desiredSizeStorageIndex = DependencyProperty.GetUniqueGlobalIndex(null, null);
                DependencyProperty.RegisteredPropertyList.Add();
            }
        }
 
        #endregion Constructors
 
        //--------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------
  
        #region Public Methods
  
        //------------------------------------------------------------
        //  IScrollInfo Methods
        //------------------------------------------------------------
        #region IScrollInfo Methods
 
        /// <summary>
        ///     Scroll content by one line to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one line to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one line to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        ///     Scroll content by one line to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }
 
        /// <summary>
        ///     Scroll content by one page to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }
 
        /// <summary>
        ///     Scroll content by one page to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }
 
        /// <summary>
        ///     Scroll content by one page to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }
  
        /// <summary>
        ///     Scroll content by one page to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - SystemParameters.WheelScrollLines * ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        ///     Scroll content by one page to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + SystemParameters.WheelScrollLines * ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 3.0 * ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + 3.0 * ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        /// Set the HorizontalOffset to the passed value.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            EnsureScrollData();
  
            double scrollX = ScrollContentPresenter.ValidateInputOffset(offset, "HorizontalOffset");
            if (!DoubleUtil.AreClose(scrollX, _scrollData._offset.X))
            {
                Vector oldViewportOffset = _scrollData._offset;
 
                // Store the new offset
                _scrollData._offset.X = scrollX;
 
                // Report the change in offset
                OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);
 
                InvalidateMeasure();
            }
        }
 
        /// <summary>
        /// Set the VerticalOffset to the passed value.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            EnsureScrollData();
 
            double scrollY = ScrollContentPresenter.ValidateInputOffset(offset, "VerticalOffset");
            if (!DoubleUtil.AreClose(scrollY, _scrollData._offset.Y))
            {
                Vector oldViewportOffset = _scrollData._offset;
  
                // Store the new offset
                _scrollData._offset.Y = scrollY;
  
                // Report the change in offset
                OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);
 
                InvalidateMeasure();
            }
        }
  
        /// <summary>
        /// VirtualizingStackPanel implementation of <seealso cref="IScrollInfo.MakeVisible">.
        /// </seealso></summary>
        // The goal is to change offsets to bring the child into view, and return a rectangle in our space to make visible.
        // The rectangle we return is in the physical dimension the input target rect transformed into our pace.
        // In the logical dimension, it is our immediate child's rect.
        // Note: This code presently assumes we/children are layout clean.  See work item 22269 for more detail.
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            Vector newOffset = new Vector();
            Rect newRect = new Rect();
            Rect originalRect = rectangle;
            bool isHorizontal = (Orientation == Orientation.Horizontal);
 
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (    rectangle.IsEmpty
                || visual == null
                || visual == (Visual)this
                ||  !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }
  
#pragma warning disable 1634, 1691
#pragma warning disable 56506
            // Compute the child's rect relative to (0,0) in our coordinate space.
            // This is a false positive by PreSharp. visual cannot be null because of the 'if' check above
            GeneralTransform childTransform = visual.TransformToAncestor(this);
#pragma warning restore 56506
#pragma warning restore 1634, 1691
            rectangle = childTransform.TransformBounds(rectangle);
  
            // We can't do any work unless we're scrolling.
            if (!IsScrolling)
            {
                return rectangle;
            }
 
            // Make ourselves visible in the non-stacking direction
            MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, !isHorizontal);
  
            if (IsPixelBased)
            {
                MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, isHorizontal);
            }
            else
            {
                // Bring our child containing the visual into view.
                // For non-pixel based scrolling the offset is in logical units in the stacking direction
                // and physical units in the other. Hence the logical helper call here.
                int childIndex = FindChildIndexThatParentsVisual(visual);
                MakeVisibleLogicalHelper(childIndex, rectangle, ref newOffset, ref newRect);
            }
 
            // We have computed the scrolling offsets; validate and scroll to them.
            newOffset.X = ScrollContentPresenter.CoerceOffset(newOffset.X, _scrollData._extent.Width, _scrollData._viewport.Width);
            newOffset.Y = ScrollContentPresenter.CoerceOffset(newOffset.Y, _scrollData._extent.Height, _scrollData._viewport.Height);
 
            if (!DoubleUtil.AreClose(newOffset, _scrollData._offset))
            {
                Vector oldOffset = _scrollData._offset;
                _scrollData._offset = newOffset;
 
                OnViewportOffsetChanged(oldOffset, newOffset);
 
                InvalidateMeasure();
                OnScrollChange();
                if (ScrollOwner != null)
                {
                    // When layout gets updated it may happen that visual is obscured by a ScrollBar
                    // We call MakeVisible again to make sure element is visible in this case
                    ScrollOwner.MakeVisible(visual, originalRect);
                }
 
                _bringIntoViewContainer = null;
            }
  
            // Return the rectangle
            return newRect;
        }
 
        /// <summary>
        /// Generates the item at the specified index and calls BringIntoView on it.
        /// </summary>
        /// <param name="index">Specify the item index that should become visible
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        protected internal override void BringIndexIntoView(int index)
        {
            if (index < 0 || index >= ItemCount)
                throw new ArgumentOutOfRangeException("index");
 
            IItemContainerGenerator generator = Generator;
            int childIndex;
            bool visualOrderChanged = false;
            GeneratorPosition position = IndexToGeneratorPositionForStart(index, out childIndex);
            using (generator.StartAt(position, GeneratorDirection.Forward, true))
            {
                bool newlyRealized;
                UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                if (child != null)
                {
  
                    visualOrderChanged = AddContainerFromGenerator(childIndex, child, newlyRealized);
  
                    if (visualOrderChanged)
                    {
                        Debug.Assert(IsVirtualizing && InRecyclingMode, "We should only modify the visual order when in recycling mode");
                        InvalidateZState();
                    }
 
                    FrameworkElement element = child as FrameworkElement;
                    if (element != null)
                    {
                        element.BringIntoView();
                        _bringIntoViewContainer = element;
                    }
                }
            }
        }
  
        #endregion
  
        #endregion
 
        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //--------------------------------------------------------------------
 
        #region Public Properties
  
        /// <summary>
        /// Specifies dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
  
        /// <summary>
        /// This property is always true because this panel has vertical or horizontal orientation
        /// </summary>
        protected internal override bool HasLogicalOrientation
        {
            get { return true; }
        }
  
        /// <summary>
        ///     Orientation of the panel if its layout is in one dimension.
        /// Otherwise HasLogicalOrientation is false and LogicalOrientation should be ignored
        /// </summary>
        protected internal override Orientation LogicalOrientation
        {
            get { return this.Orientation; }
        }
  
        /// <summary>
        /// DependencyProperty for <see cref="Orientation"> property.
        /// </see></summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(OnOrientationChanged)),
                new ValidateValueCallback(ScrollBar.IsValidOrientation));
  
        /// <summary>
        ///     Attached property for use on the ItemsControl that is the host for the items being
        ///     presented by this panel. Use this property to turn virtualization on/off.
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(true));
  
        /// <summary>
        ///     Retrieves the value for <see cref="IsVirtualizingProperty">.
        /// </see></summary>
        /// <param name="o">The object on which to query the value.
        /// <returns>True if virtualizing, false otherwise.</returns>
        public static bool GetIsVirtualizing(DependencyObject o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
  
            return (bool)o.GetValue(IsVirtualizingProperty);
        }
 
        /// <summary>
        ///     Sets the value for <see cref="IsVirtualizingProperty">.
        /// </see></summary>
        /// <param name="element">The element on which to set the value.
        /// <param name="value">True if virtualizing, false otherwise.
        public static void SetIsVirtualizing(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
  
            element.SetValue(IsVirtualizingProperty, value);
        }
  
 
 
        /// <summary>
        ///     Attached property for use on the ItemsControl that is the host for the items being
        ///     presented by this panel. Use this property to modify the virtualization mode.
        ///
        ///     Note that this property can only be set before the panel has been initialized
        /// </summary>
        public static readonly DependencyProperty VirtualizationModeProperty =
            DependencyProperty.RegisterAttached("VirtualizationMode", typeof(VirtualizationMode), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(VirtualizationMode.Standard));
 
        /// <summary>
        ///     Retrieves the value for <see cref="VirtualizationModeProperty">.
        /// </see></summary>
        /// <param name="o">The object on which to query the value.
        /// <returns>The current virtualization mode.</returns>
        public static VirtualizationMode GetVirtualizationMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
  
            return (VirtualizationMode)element.GetValue(VirtualizationModeProperty);
        }
  
        /// <summary>
        ///     Sets the value for <see cref="VirtualizationModeProperty">.
        /// </see></summary>
        /// <param name="element">The element on which to set the value.
        /// <param name="value">The desired virtualization mode.
        public static void SetVirtualizationMode(DependencyObject element, VirtualizationMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
 
            element.SetValue(VirtualizationModeProperty, value);
        }
  
  
        //-----------------------------------------------------------
        //  IScrollInfo Properties
        //-----------------------------------------------------------
        #region IScrollInfo Properties
 
        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing its child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanHorizontallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowHorizontal;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowHorizontal != value)
                {
                    _scrollData._allowHorizontal = value;
                    InvalidateMeasure();
                }
            }
        }
 
        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing its child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanVerticallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowVertical;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowVertical != value)
                {
                    _scrollData._allowVertical = value;
                    InvalidateMeasure();
                }
            }
        }
  
        /// <summary>
        /// ExtentWidth contains the horizontal size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Width;
            }
        }
  
        /// <summary>
        /// ExtentHeight contains the vertical size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Height;
            }
        }
 
        /// <summary>
        /// ViewportWidth contains the horizontal size of content's visible range in 1/96"
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Width;
            }
        }
  
        /// <summary>
        /// ViewportHeight contains the vertical size of content's visible range in 1/96"
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Height;
            }
        }
  
        /// <summary>
        /// HorizontalOffset is the horizontal offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double HorizontalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.X;
            }
        }
 
        /// <summary>
        /// VerticalOffset is the vertical offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double VerticalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.Y;
            }
        }
  
        /// <summary>
        /// ScrollOwner is the container that controls any scrollbars, headers, etc... that are dependant
        /// on this IScrollInfo's properties.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollViewer ScrollOwner
        {
            get
            {
                EnsureScrollData();
                return _scrollData._scrollOwner;
            }
            set
            {
                EnsureScrollData();
                if (value != _scrollData._scrollOwner)
                {
                    ResetScrolling(this);
                    _scrollData._scrollOwner = value;
                }
            }
        }
 
        #endregion IScrollInfo Properties
  
        #endregion Public Properties
  
        //-------------------------------------------------------------------
        //
        //  Public Events
        //
        //--------------------------------------------------------------------
 
  
        #region Public Events
  
        /// <summary>
        ///     Called on the ItemsControl that owns this panel when an item is being re-virtualized.
        /// </summary>
        public static readonly RoutedEvent CleanUpVirtualizedItemEvent = EventManager.RegisterRoutedEvent("CleanUpVirtualizedItemEvent", RoutingStrategy.Direct, typeof(CleanUpVirtualizedItemEventHandler), typeof(VirtualizingStackPanel));
 
 
        /// <summary>
        ///     Adds a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event
        /// <param name="handler">Event Handler to be added
        public static void AddCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        {
            FrameworkElement.AddHandler(element, CleanUpVirtualizedItemEvent, handler);
        }
  
        /// <summary>
        ///     Removes a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event
        /// <param name="handler">Event Handler to be removed
        public static void RemoveCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        {
            FrameworkElement.RemoveHandler(element, CleanUpVirtualizedItemEvent, handler);
        }
 
        /// <summary>
        ///     Called when an item is being re-virtualized.
        /// </summary>
        protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (itemsControl != null)
            {
                itemsControl.RaiseEvent(e);
            }
        }
  
        #endregion
 
        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //--------------------------------------------------------------------
 
        #region Protected Methods
 
        /// <summary>
        /// General VirtualizingStackPanel layout behavior is to grow unbounded in the "stacking" direction (Size To Content).
        /// Children in this dimension are encouraged to be as large as they like.  In the other dimension,
        /// VirtualizingStackPanel will assume the maximum size of its children.
        /// </summary>
        /// <remarks>
        /// When scrolling, VirtualizingStackPanel will not grow in layout size but effectively add the children on a z-plane which
        /// will probably be clipped by some parent (typically a ScrollContentPresenter) to Stack's size.
        /// </remarks>
        /// <param name="constraint">Constraint
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if Profiling
            if (Panel.IsAboutToGenerateContent(this))
                return MeasureOverrideProfileStub(constraint);
            else
                return RealMeasureOverride(constraint);
        }
 
        // this is a handy place to start/stop profiling
        private Size MeasureOverrideProfileStub(Size constraint)
        {
            return RealMeasureOverride(constraint);
        }
  
        private Size RealMeasureOverride(Size constraint)
        {
#endif
            bool etwTracingEnabled = IsScrolling && EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal);
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.StartEvent, "VirtualizingStackPanel :MeasureOverride");
            }
  
            Debug.Assert(MeasureData == null || constraint == MeasureData.AvailableSize, "MeasureData needs to be passed down in [....] with size");
 
            MeasureData measureData = MeasureData;
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstViewport;                              // First child index in the viewport.
            double firstItemOffset;                         // Offset of the top of the first child relative to the top of the viewport.
            double virtualizedItemsSize = 0d;               // Amount that virtualized children contribute to the desired size in the stacking direction
            int lastViewport = -1;                          // Last child index in the viewport.  -1 indicates we have not yet iterated through the last child.
            double logicalVisibleSpace, childLogicalSize;
            Rect originalViewport = Rect.Empty;             // Only used if this is the scrolling panel.  Saves off the given viewport for scroll computations.
  
            // Collect information from the ItemsControl, if there is one.
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = (itemsControl != null) ? itemsControl.Items.Count : 0;
            SetVirtualizationState(itemsControl, /* hasMeasureData = */ measureData != null && measureData.HasViewport);
  
            IList children = RealizedChildren;  // yes, this is weird, but this property ensures the Generator is properly initialized.
            IItemContainerGenerator generator = Generator;
 
            // Adjust the viewport
            if (IsPixelBased)
            {
                if (IsScrolling)
                {
  
                    // We're the top level scrolling panel.  Set the viewport and extend it to add a focus trail
                    originalViewport = new Rect(_scrollData._offset.X, _scrollData._offset.Y, constraint.Width, constraint.Height);
                    measureData = new MeasureData(constraint, originalViewport);
  
                    // The way we have a focus trail when pixel-based is to artificially extend the viewport.  All calculations are done
                    // with this 'artificial' viewport with the exception of the scroll offset, extent, etc.
                    measureData = AddFocusTrail(measureData, fHorizontal);
                    Debug.Assert(!object.ReferenceEquals(originalViewport, measureData.Viewport), "original viewport should not have a focus trail");
                }
                else
                {
                    measureData = AdjustViewportOffset(measureData, itemsControl, fHorizontal);
                    Debug.Assert(!object.ReferenceEquals(MeasureData, measureData), "The value set in the MeasureData property should not be modified");
                }
            }
  
 
            //
            // Initialize child sizing and iterator data
            // Allow children as much size as they want along the stack.
            //
            if (fHorizontal)
            {
                layoutSlotSize.Width = Double.PositiveInfinity;
                if (IsScrolling && CanVerticallyScroll) { layoutSlotSize.Height = Double.PositiveInfinity; }
                logicalVisibleSpace = constraint.Width;
            }
            else
            {
                layoutSlotSize.Height = Double.PositiveInfinity;
                if (IsScrolling && CanHorizontallyScroll) { layoutSlotSize.Width = Double.PositiveInfinity; }
                logicalVisibleSpace = constraint.Height;
            }
  
            // Compute index of first item in the viewport
            firstViewport = ComputeIndexOfFirstVisibleItem(measureData, itemsControl, fHorizontal, out firstItemOffset);
 
            if (IsPixelBased)
            {
                // Acount for the size of items that won't be generated
                Debug.Assert(stackDesiredSize.Width == 0 && stackDesiredSize.Height == 0, "stack desired size must be 0 for virtualizedItemsSize to work");
                stackDesiredSize = ExtendDesiredSize(itemsControl, stackDesiredSize, firstViewport, /*before = */ true, fHorizontal);
  
                virtualizedItemsSize = fHorizontal ? stackDesiredSize.Width : stackDesiredSize.Height;
            }
 
 
            debug_AssertRealizedChildrenEqualVisualChildren();
  
            // If recycling clean up before generating children.
            if (IsVirtualizing && InRecyclingMode)
            {
                CleanupContainers(firstViewport, itemsControl);
                debug_VerifyRealizedChildren();
            }
 
            //
            // Figure out the position of the first visible item
            //
            GeneratorPosition startPos = IndexToGeneratorPositionForStart(IsVirtualizing ? firstViewport : 0, out _firstVisibleChildIndex);
            int childIndex = _firstVisibleChildIndex;
 
            //
            // Main loop: generate and measure all children (or all visible children if virtualizing).
            //
            bool ranOutOfItems = true;
            bool visualOrderChanged = false;
            _visibleCount = 0;
            if (itemCount > 0)
            {
                _afterTrail = 0;
                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    for (int i = IsVirtualizing ? firstViewport : 0, count = itemCount; i < count; ++i)
                    {
                        // Get next child.
                        bool newlyRealized;
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
  
                        if (child == null)
                        {
                            Debug.Assert(!newlyRealized, "The generator realized a null value.");
 
                            // We reached the end of the items (because of a group)
                            break;
                        }
  
                        visualOrderChanged |= AddContainerFromGenerator(childIndex, child, newlyRealized);
 
                        childIndex++;
                        _visibleCount++;
 
                        if (IsPixelBased)
                        {
                            // Pass along MeasureData so it continues down the tree.
                            child.MeasureData = CreateChildMeasureData(measureData, layoutSlotSize, stackDesiredSize, fHorizontal);
                        }
 
                        Size childDesiredSize = child.DesiredSize;
                        child.Measure(layoutSlotSize);
 
                        if (childDesiredSize != child.DesiredSize)
                        {
                            childDesiredSize = child.DesiredSize;
  
                            // Reset the _maxDesiredSize cache if child DesiredSize changes
                            if (_scrollData != null)
                                _scrollData._maxDesiredSize = new Size();
                        }
 
 
                        // Accumulate child size.
                        if (fHorizontal)
                        {
                            stackDesiredSize.Width += childDesiredSize.Width;
                            stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                            childLogicalSize = childDesiredSize.Width;
                        }
                        else
                        {
                            stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                            stackDesiredSize.Height += childDesiredSize.Height;
                            childLogicalSize = childDesiredSize.Height;
                        }
 
  
 
                        // Adjust remaining viewport space if we are scrolling and within the viewport region.
                        // While scrolling (not virtualizing), we always measure children before and after the viewport.
                        if (IsScrolling && lastViewport == -1 && i >= firstViewport)
                        {
                            logicalVisibleSpace -= childLogicalSize;
                            if (DoubleUtil.LessThanOrClose(logicalVisibleSpace, 0.0))
                            {
                                lastViewport = i;
                            }
                        }
 
  
                        // When under a viewport, virtualizing and at or beyond the first element, stop creating elements when out of space.
                        if (IsVirtualizing && (i >= firstViewport))
                        {
                            double viewportSize;
                            double totalGenerated;
 
                            //
                            // Decide if the end of the item is outside the viewport.
                            //
                            // StackDesiredSize, with some adjustment, is a measure of exactly how much viewport space we have used.
                            //
                            // StackDesiredSize is the sum of all generated children (starting with the first visible item).  The first
                            // visible item doesn't always start at the top of the viewport, so we have to adjust by the firstItemoffset.
                            //
                            // When pixel-based we add the sum of all virtualized children to the stackDesiredSize; this has to be removed as well.
                            //
                            Debug.Assert(IsPixelBased || virtualizedItemsSize == 0d);
  
                            if (fHorizontal)
                            {
                                viewportSize = IsPixelBased ? measureData.Viewport.Width : constraint.Width;
                                totalGenerated = stackDesiredSize.Width - virtualizedItemsSize + firstItemOffset;
  
                            }
                            else
                            {
                                viewportSize = IsPixelBased ? measureData.Viewport.Height : constraint.Height;
                                totalGenerated = stackDesiredSize.Height - virtualizedItemsSize + firstItemOffset;
                            }
  
 
                            if (totalGenerated > viewportSize)
                            {
                                // The end of this child is outside the viewport.  Check if we want to generate some more.
 
                                if (IsPixelBased)
                                {
                                    // For pixel-based virtualization (specifically TreeView virtualization) we deal with
                                    // the after trail later, since it has to function hierarchically.
                                    break;
                                }
                                else
                                {
                                    // We want to keep a focusable item after the end so that keyboard navigation
                                    // can work, but we want to limit that to FocusTrail number of items
                                    // in case all the items are not focusable.
                                    if (_afterTrail > 0 && ( _afterTrail >= FocusTrail || Keyboard.IsFocusable(child)))
                                    {
                                        // Either we passed the limit or the child was focusable
                                        ranOutOfItems = false;
                                        break;
                                    }
 
                                    _afterTrail++;
                                    // Loop around and generate another item
                                }
                            }
                        }
                    }
                }
            }
 
#if DEBUG
            if (IsVirtualizing && InRecyclingMode)
            {
                debug_VerifyRealizedChildren();
            }
#endif
 
            _visibleStart = firstViewport;
 
  
            if (IsPixelBased)
            {
                // Acount for the size of items that won't be generated
                stackDesiredSize = ExtendDesiredSize(itemsControl, stackDesiredSize, firstViewport + _visibleCount, /*before = */ false, fHorizontal);
            }
 
 
            //
            // Adjust the scroll offset, extent, etc.
            //
            if (IsScrolling)
            {
                if (IsPixelBased)
                {
 
                    Vector offset = new Vector(originalViewport.Location.X, originalViewport.Location.Y);
                    SetAndVerifyScrollingData(originalViewport.Size, stackDesiredSize, offset);
                }
                else
                {
                    // Compute the extent before we fill remaining space and modify the stack desired size
                    Size extent = ComputeLogicalExtent(stackDesiredSize, itemCount, fHorizontal);
  
                    if (ranOutOfItems)
                    {
                        // If we or children have resized, it's possible that we can now display more content.
                        // This is true if we started at a nonzero offeset and still have space remaining.
                        // In this case, we loop back through previous children until we run out of space.
                        FillRemainingSpace(ref firstViewport, ref logicalVisibleSpace, ref stackDesiredSize, layoutSlotSize, fHorizontal);
                    }
 
                    // Create the Before focus trail
                    // NOTE: the call here (under IsScrolling) implicitly assumes that only a scrolling panel can virtualize and thus requires
                    // a focus trail.  That's not true for hierarchical (pixel-based) virtualization, but it handles the focus trail differently anyway.
                    EnsureTopCapGenerated(layoutSlotSize);
  
                    // Compute Scrolling data such as extent, viewport, and offset.
                    stackDesiredSize = UpdateLogicalScrollData(stackDesiredSize, constraint, logicalVisibleSpace,
                                                               extent, firstViewport, lastViewport, itemCount, fHorizontal);
                }
            }
 
            //
            // Cleanup items no longer in the viewport
            //
            if (IsVirtualizing && !InRecyclingMode)
            {
                if (IsPixelBased)
                {
                    // Immediate cleanup
                    CleanupContainers(firstViewport, itemsControl);
                }
                else
                {
                    // Less aggressive backwards-compat background cleanup operation
                    EnsureCleanupOperation(false /* delay */);
                }
            }
  
 
            if (IsVirtualizing && InRecyclingMode)
            {
                DisconnectRecycledContainers();
 
                if (visualOrderChanged)
                {
                    // We moved some containers in the visual tree without firing changed events.  ZOrder is now invalid.
                    InvalidateZState();
                }
            }
 
  
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.EndEvent, "VirtualizingStackPanel :MeasureOverride");
            }
  
            debug_AssertRealizedChildrenEqualVisualChildren();
 
            return stackDesiredSize;
        }
 
  
  
        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="arrangeSize">Arrange size
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            Rect rcChild = new Rect(arrangeSize);
  
            IList children;
            double previousChildSize = 0.0;
            ItemsControl itemsControl = null;
            bool childrenAreContainers = true;
 
            bool etwTracingEnabled = IsScrolling && EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal);
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.StartEvent, "VirtualizingStackPanel :ArrangeOverride");
            }
  
            //
            // Compute scroll offset and seed it into rcChild.
            //
            if (IsScrolling)
            {
                if (fHorizontal)
                {
                    double offsetX = _scrollData._computedOffset.X;
                    rcChild.X = IsPixelBased ? -offsetX : ComputePhysicalFromLogicalOffset(IsVirtualizing ? _firstVisibleChildIndex : offsetX, true);
                    rcChild.Y = -1.0 * _scrollData._computedOffset.Y;
                }
                else
                {
                    double offsetY = _scrollData._computedOffset.Y;
                    rcChild.X = -1.0 * _scrollData._computedOffset.X;
                    rcChild.Y = IsPixelBased ? -offsetY : ComputePhysicalFromLogicalOffset(IsVirtualizing ? _firstVisibleChildIndex : offsetY, false);
                }
            }
 
            //
            // Arrange and Position Children.
            //
            // If we're virtualizing and pixel-based we loop through the entire items collection (the policy is to arrange items exactly where they
            // should appear regardless of the virtualization state of siblings).  This is required to properly virtualize hiearchically.
            // Otherwise we loop through the children collection (when virtualizing in items mode VSP arranges children in a simple stack order).
            //
  
            if (IsPixelBased && IsVirtualizing)
            {
                // This is a pixel-based internal panel.  It must behave externally exactly the way a non-virtualizing panel does in Arrange.
                // Specifically, it arranges its children in the 'proper' place, regardless of whether or not their siblings are virtualized.
 
                itemsControl = ItemsControl.GetItemsOwner(this);
                children = itemsControl.Items;
                childrenAreContainers = false;
            }
            else
            {
                debug_AssertRealizedChildrenEqualVisualChildren();  // RealizedChildren only differs from InternalChildren inside of Measure when container recycling is on.
                children = RealizedChildren;
            }
  
  
            for (int i = 0; i < children.Count; ++i)
            {
                UIElement container = null;
                Size childSize;
 
                if (childrenAreContainers)
                {
                    // we are looping through the actual containers; the visual children of this panel.
                    container = (UIElement)children[i];
                    childSize = container.DesiredSize;
                }
                else
                {
                    // We are looping through items and may or may not have a container for each given item.
                    childSize = ContainerSizeForItem(itemsControl, children[i], i, out container);
                }
  
  
                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = childSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, childSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = childSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(arrangeSize.Width, childSize.Width);
                }
  
                if (container != null)
                {
                    container.Arrange(rcChild);
                }
            }
 
 
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.EndEvent, "VirtualizingStackPanel :ArrangeOverride");
            }
  
            return arrangeSize;
        }
 
 
        /// <summary>
        ///     Called when the Items collection associated with the containing ItemsControl changes.
        /// </summary>
        /// <param name="sender">sender
        /// <param name="args">Event arguments
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
 
            bool resetMaximumDesiredSize = false;
  
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    OnItemsRemove(args);
                    resetMaximumDesiredSize = true;
                    break;
 
                case NotifyCollectionChangedAction.Replace:
                    OnItemsReplace(args);
                    resetMaximumDesiredSize = true;
                    break;
  
                case NotifyCollectionChangedAction.Move:
                    OnItemsMove(args);
                    break;
 
                case NotifyCollectionChangedAction.Reset:
                    resetMaximumDesiredSize = true;
                    break;
            }
  
            if (resetMaximumDesiredSize && IsScrolling)
            {
                // The items changed such that the maximum size may no longer be valid.
                // The next layout pass will update this value.
                _scrollData._maxDesiredSize = new Size();
            }
 
        }
  
        /// <summary>
        ///     Called when the UI collection of children is cleared by the base Panel class.
        /// </summary>
        protected override void OnClearChildren()
        {
            base.OnClearChildren();
            _realizedChildren = null;
            _visibleStart = _firstVisibleChildIndex = _visibleCount = 0;
        }
 
        // Override of OnGotKeyboardFocus.  Called when focus moves to any child or subchild of this VSP
        // Used by TreeView virtualization to keep track of the focused item.
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            FocusChanged(e);
        }
  
        // Override of OnLostKeyboardFocus.  Called when focus moves away from this VSP.
        // Used by TreeView virtualization to keep track of the focused item.
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            FocusChanged(e);
        }
  
  
 
        #endregion Protected Methods
 
        #region Internal Methods
 
        // Tells the Generator to clear out all containers for this ItemsControl.  This is called by the ItemValueStorage
        // service when the ItemsControl this panel is a host for is about to be thrown away.  This allows the VSP to save
        // off any properties it is interested in and results in a call to ClearContainerForItem on the ItemsControl, allowing
        // the Item Container Storage to do so as well.
 
        // Note: A possible perf improvement may be to make 'fast' RemoveAll on the Generator that simply calls ClearContainerForItem
        // for us without walking through its data structures to actually clean out items.
        internal void ClearAllContainers(ItemsControl itemsControl)
        {
            Debug.Assert(IsVirtualizing,
                         "We should only clear containers for ItemsControls that are virtualizing");
  
            Debug.Assert(itemsControl == ItemsControl.GetItemsOwner(this),
                        "We can only clear containers that this panel is a host for");
  
            IItemContainerGenerator generator = Generator;
 
            if (IsPixelBased)
            {
                IList children = RealizedChildren;
                UIElement child;
  
                for (int i = 0; i < children.Count; i++)
                {
                    child = (UIElement)children[i];
                    itemsControl.StoreItemValue(((ItemContainerGenerator)generator).ItemFromContainer(child), child.DesiredSize, _desiredSizeStorageIndex);
                }
  
            }
  
            if (generator != null)
            {
                generator.RemoveAll();
            }
        }
 
        #endregion
 
        //------------------------------------------------------
        //
        //  Private Methods
        //
        //-----------------------------------------------------
 
        #region Private Methods
  
 
        //
        // MeasureOverride Helpers
        //
  
        #region MeasureOverride Helpers
 
        /// <summary>
        /// Extends the viewport of the given MeasureData to give a focus trail.  Returns by how much it extended the viewport.
        /// </summary>
        /// <param name="childData">
        /// <returns></returns>
        private MeasureData AddFocusTrail(MeasureData measureData, bool isHorizontal)
        {
            //
            // Create the before / after focus trail for interior panels that use MeasureData's viewport to virtualize.
            // We expand the viewport so that roughly two extra items are generated at the top and the bottom.
            //
            // For the before / after focus trail good values are
            //  padding = header height * 4;
            //
            // To make page up / down work without rewriting TreeView's algorithm we actually extend the viewport one extra page
            // top and bottm.
            //
            Debug.Assert(IsScrolling, "The scrolling panel is the only one that should extend the viewport");
            Invariant.Assert(IsPixelBased, "If we're sending down a viewport to the children we should be doing pixel-based computations");
  
            double page = isHorizontal ? ViewportWidth : ViewportHeight;
            Rect viewport = measureData.Viewport;
  
            if (isHorizontal)
            {
                viewport.Width += page * 2;
                viewport.X -= page;
            }
            else
            {
                viewport.Height += page * 2;
                viewport.Y -= page;
            }
  
            measureData.Viewport = viewport;
            return measureData;
        }
  
        #region Scroll Computation Helpers
  
        /// <summary>
        /// Returns the extent in logical units in the stacking direction.
        /// </summary>
        /// <param name="stackDesiredSize">
        /// <param name="itemCount">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private Size ComputeLogicalExtent(Size stackDesiredSize, int itemCount, bool isHorizontal)
        {
            bool accumulateExtent = false;
            Size extent = new Size();
  
            if (ScrollOwner != null)
            {
                accumulateExtent = ScrollOwner.InChildInvalidateMeasure;
                ScrollOwner.InChildInvalidateMeasure = false;
            }
  
            if (isHorizontal)
            {
                extent.Width = itemCount;
                extent.Height = accumulateExtent ? Math.Max(stackDesiredSize.Height, _scrollData._extent.Height) : stackDesiredSize.Height;
            }
            else
            {
                extent.Width = accumulateExtent ? Math.Max(stackDesiredSize.Width, _scrollData._extent.Width) : stackDesiredSize.Width;
                extent.Height = itemCount;
            }
 
            return extent;
        }
 
 
  
        /// <summary>
        /// Called when we ran out of children before filling up the viewport.
        /// </summary>
        private void FillRemainingSpace(ref int firstViewport, ref double logicalVisibleSpace, ref Size stackDesiredSize, Size layoutSlotSize, bool isHorizontal)
        {
            Debug.Assert(IsScrolling, "Only the scrolling panel can fill remaining space");
            Debug.Assert(!IsPixelBased, "This is a logical operation");
 
            double projectedLogicalVisibleSpace;
            Size childDesiredSize;
            IList children = RealizedChildren;
            int childIndex = IsVirtualizing ? _firstVisibleChildIndex : firstViewport;
 
            while (childIndex > 0)
            {
                if (!PreviousChildIsGenerated(childIndex))
                {
                    GeneratePreviousChild(childIndex, layoutSlotSize);
                    childIndex++; // We just inserted a child, so increment the index
                }
                else if (childIndex <= _firstVisibleChildIndex)
                {
                    ((UIElement)children[childIndex - 1]).Measure(layoutSlotSize);
                }
 
                projectedLogicalVisibleSpace = logicalVisibleSpace;
  
                childDesiredSize = ((UIElement)children[childIndex - 1]).DesiredSize;
  
                if (isHorizontal)
                {
                    projectedLogicalVisibleSpace -= childDesiredSize.Width;
                }
                else
                {
                    projectedLogicalVisibleSpace -= childDesiredSize.Height;
                }
  
                // If we have run out of room, break.
                if (DoubleUtil.LessThan(projectedLogicalVisibleSpace, 0.0)) { break; }
  
                // Account for the child in the panel's desired size
                if (isHorizontal)
                {
                    stackDesiredSize.Width += childDesiredSize.Width;
                    stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                }
                else
                {
                    stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                    stackDesiredSize.Height += childDesiredSize.Height;
                }
 
                // Adjust viewport
                childIndex--;
                logicalVisibleSpace = projectedLogicalVisibleSpace;
                _visibleCount++;
            }
            if ((childIndex < _firstVisibleChildIndex) || !IsVirtualizing)
            {
                _firstVisibleChildIndex = childIndex;
            }
  
            _visibleStart = firstViewport = (IsItemsHost && children.Count != 0) ? GetGeneratedIndex(_firstVisibleChildIndex) : 0;
        }
  
 
        /// <summary>
        /// Updates ScrollData's offset, extent, and viewport in logical units.
        /// </summary>
        /// <param name="stackDesiredSize">
        /// <param name="constraint">
        /// <param name="logicalVisibleSpace">
        /// <param name="extent">
        /// <param name="firstViewport">
        /// <param name="lastViewport">
        /// <param name="itemCount">
        /// <param name="fHorizontal">
        /// <returns></returns>
        private Size UpdateLogicalScrollData(Size stackDesiredSize, Size constraint, double logicalVisibleSpace, Size extent,
                                             int firstViewport, int lastViewport, int itemCount, bool fHorizontal)
        {
            Debug.Assert(IsScrolling && !IsPixelBased, "this computes logical scroll data");
  
            Size viewport = constraint;
            Vector offset = _scrollData._offset;
 
            // If we have not yet set the last child in the viewport, set it to the last child.
            if (lastViewport == -1) { lastViewport = itemCount - 1; }
  
            int logicalExtent = itemCount;
            int logicalViewport = lastViewport - firstViewport;
  
            //
            // Compute the logical viewport size.
            //
 
            // We are conservative when estimating a viewport, not including the last element in case it is only partially visible.
            // We want to count it if it is fully visible (>= 0 space remaining) or the only element in the viewport.
            if (logicalViewport == 0 || DoubleUtil.GreaterThanOrClose(logicalVisibleSpace, 0.0)) { logicalViewport++; }
  
            if (fHorizontal)
            {
                viewport.Width = logicalViewport;
                offset.X = firstViewport;
                offset.Y = Math.Max(0, Math.Min(offset.Y, extent.Height - viewport.Height));
 
                // In case last item is visible because we scroll all the way to the right and scrolling is on
                // we want desired size not to be smaller than constraint to avoid another relayout
                if (logicalExtent > logicalViewport && !Double.IsPositiveInfinity(constraint.Width))
                {
                    stackDesiredSize.Width = constraint.Width;
                }
            }
            else
            {
                viewport.Height = logicalViewport;
                offset.Y = firstViewport;
                offset.X = Math.Max(0, Math.Min(offset.X, extent.Width - viewport.Width));
  
                // In case last item is visible because we scroll all the way to the bottom and scrolling is on
                // we want desired size not to be smaller than constraint to avoid another relayout
                if (logicalExtent > logicalViewport && !Double.IsPositiveInfinity(constraint.Height))
                {
                    stackDesiredSize.Height = constraint.Height;
                }
            }
  
  
 
            // Since we can offset and clip our content, we never need to be larger than the parent suggestion.
            // If we returned the full size of the content, we would always be so big we didn't need to scroll.  :)
            stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
            stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
  
            // When scrolling, the maximum horizontal or vertical size of items can cause the desired size of the
            // panel to change, which can cause the owning ScrollViewer re-layout as well when it is not necessary.
            // We will thus remember the maximum desired size and always return that. The actual arrangement and
            // clipping still be calculated from actual scroll data values.
            // The maximum desired size is reset when the items change.
            _scrollData._maxDesiredSize.Width = Math.Max(stackDesiredSize.Width, _scrollData._maxDesiredSize.Width);
            _scrollData._maxDesiredSize.Height = Math.Max(stackDesiredSize.Height, _scrollData._maxDesiredSize.Height);
            stackDesiredSize = _scrollData._maxDesiredSize;
  
            // Verify Scroll Info, invalidate ScrollOwner if necessary.
            SetAndVerifyScrollingData(viewport, extent, offset);
  
            return stackDesiredSize;
        }
 
        #endregion
 
  
        /// <summary>
        /// DesiredSize is normally computed by summing up the size of all items we've generated.  Pixel-based virtualization uses a 'full' desired size.
        /// This extends the given desired size beyond the visible items.  It will extend it by the items before or after the set of generated items.
        /// The given pivotIndex is the index of either the first or last item generated.
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="stackDesiredSize">
        /// <param name="pivotIndex">
        /// <param name="before">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private Size ExtendDesiredSize(ItemsControl itemsControl, Size stackDesiredSize, int pivotIndex, bool before, bool isHorizontal)
        {
            Debug.Assert(IsPixelBased, "MeasureOverride should have already computed desiredSize if non-virtualizing or items-based");
 
            //
            // If we're virtualizing the sum of all generated containers is not the true desired size since not all containers were generated.
            // In the old items-based mode it didn't matter because only the scrolling panel could virtualize and scrollviewer doesn't *really*
            // care about desired size.
            //
            // In pixel-based mode we need to compute the same desired size as if we weren't virtualizing.
            //
            // Note: there are faster ways to do this than loop through items, but the cost isn't significant and the other possible implementations are nasty.
            //
 
            Size containerSize;
            ItemCollection items = itemsControl.Items;
 
            for (int i = (before ? 0 : pivotIndex); i < (before ? pivotIndex : items.Count); i++)
            {
                containerSize = ContainerSizeForItem(itemsControl, items[i], i);
  
                if (isHorizontal)
                {
                    stackDesiredSize.Width += containerSize.Width;
                }
                else
                {
                    stackDesiredSize.Height += containerSize.Height;
                }
            }
 
            return stackDesiredSize;
        }
  
 
        //
        // Returns the index of the first item visible (even partially) in the viewport.
        //
        private int ComputeIndexOfFirstVisibleItem(MeasureData measureData, ItemsControl itemsControl, bool isHorizontal, out double firstItemOffset)
        {
            firstItemOffset = 0d;   // offset of the top of the first visible child from the top of the viewport.  The child always
                                    // starts before the top of the viewport so this is always negative.
  
            if (itemsControl != null)
            {
                ItemCollection items = itemsControl.Items;
                int itemsCount = items.Count;
  
                if (!IsPixelBased)
                {
                    //
                    // Classic case that shipped with V1
                    //
                    // If the panel is implementing IScrollInfo then _scrollData keeps track of the
                    // current offset, extent, etc in logical units
                    //
                    if (IsScrolling)
                    {
                        return CoerceIndexToInteger(isHorizontal ? _scrollData._offset.X : _scrollData._offset.Y, itemsCount);
                    }
                }
                else
                {
                    Size containerSize;
                    double totalSpan = 0.0;      // total height or width in the stacking direction
                    double containerSpan = 0.0;
                    double viewportOffset = isHorizontal ? measureData.Viewport.X : measureData.Viewport.Y;
 
                    for (int i = 0; i < itemsCount; i++)
                    {
                        containerSize = ContainerSizeForItem(itemsControl, items[i], i);
                        containerSpan = isHorizontal ? containerSize.Width : containerSize.Height;
                        totalSpan += containerSpan;
 
                        if (totalSpan > viewportOffset)
                        {
                            // This is the first item that starts before the viewportOffset but ends after it; i is thus the index
                            // to the first item in the viewport.
                            firstItemOffset = totalSpan - containerSpan - viewportOffset;
                            return i;
                        }
                    }
                }
            }
 
            return 0;
        }
  
 
        private Size ContainerSizeForItem(ItemsControl itemsControl, object item, int index)
        {
            UIElement temp;
            return ContainerSizeForItem(itemsControl, item, index, out temp);
        }
 
        /// <summary>
        /// Returns the size of the container for a given item.  The size can come from the container, a lookup, or a guess depending
        /// on the virtualization state of the item.
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="item">
        /// <param name="index">
        /// <param name="container">returns the container for the item; null if the container wasn't found
        /// <returns></returns>
        private Size ContainerSizeForItem(ItemsControl itemsControl, object item, int index, out UIElement container)
        {
            Size containerSize;
            container = index >= 0 ? ((ItemContainerGenerator)Generator).ContainerFromIndex(index) as UIElement : null;
  
            if (container != null)
            {
                containerSize = container.DesiredSize;
            }
            else
            {
                // It's virtualized; grab the height off the item if available.
                object value = itemsControl.ReadItemValue(item, _desiredSizeStorageIndex);
                if (value != null)
                {
                    containerSize = (Size)value;
                }
                else
                {
                    //
                    // No stored container height; simply guess.
                    //
                    containerSize = new Size();
 
  
                    if (Orientation == Orientation.Horizontal)
                    {
                        containerSize.Width = ContainerStackingSizeEstimate(itemsControl, /*isHorizontal = */ true);
                        containerSize.Height = DesiredSize.Height;
                    }
                    else
                    {
                        containerSize.Height = ContainerStackingSizeEstimate(itemsControl, /*isHorizontal = */ false);
                        containerSize.Width = DesiredSize.Width;
                    }
                }
            }
  
            Debug.Assert(!containerSize.IsEmpty, "We can't estimate an empty size");
            return containerSize;
        }
 
  
        private double ContainerStackingSizeEstimate(ItemsControl itemsControl, bool isHorizontal)
        {
            return ContainerStackingSizeEstimate(itemsControl as IProvideStackingSize, isHorizontal);
        }
 
        /// <summary>
        /// Estimates a container size in the stacking direction for the given ItemsControl
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="IsHorizontal">
        /// <returns></returns>
        private double ContainerStackingSizeEstimate(IProvideStackingSize estimate, bool isHorizontal)
        {
            double stackingSize = 0d;
  
            if (estimate != null)
            {
                stackingSize = estimate.EstimatedContainerSize(isHorizontal);
            }
 
            if (stackingSize <= 0d || DoubleUtil.IsNaN(stackingSize))
            {
                stackingSize = ScrollViewer._scrollLineDelta;
            }
  
            Debug.Assert(stackingSize > 0, "We should have returned a reasonable estimate for the stacking size");
  
            return stackingSize;
        }
 
  
        private MeasureData CreateChildMeasureData(MeasureData measureData, Size layoutSlotSize, Size stackDesiredSize, bool isHorizontal)
        {
            Invariant.Assert(IsPixelBased && measureData != null, "We can only use MeasureData when pixel-based");
            Rect viewport = measureData.Viewport;
  
            //
            // Adjust viewport offset for the child
            //
            if (isHorizontal)
            {
                viewport.X -= stackDesiredSize.Width;
            }
            else
            {
                viewport.Y -= stackDesiredSize.Height;
            }
 
            return new MeasureData(layoutSlotSize, viewport);
        }
  
        /// <summary>
        /// Inserts a new container in the visual tree
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        private void InsertNewContainer(int childIndex, UIElement container)
        {
            InsertContainer(childIndex, container, false);
        }
  
        /// <summary>
        /// Inserts a recycled container in the visual tree
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        /// <returns></returns>
        private bool InsertRecycledContainer(int childIndex, UIElement container)
        {
            return InsertContainer(childIndex, container, true);
        }
  
 
        /// <summary>
        /// Inserts a container into the Children collection.  The container is either new or recycled.
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        /// <param name="isRecycled">
        private bool InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            Debug.Assert(container != null, "Null container was generated");
 
            bool visualOrderChanged = false;
            UIElementCollection children = InternalChildren;
 
            //
            // Find the index in the Children collection where we hope to insert the container.
            // This is done by looking up the index of the container BEFORE the one we hope to insert.
            //
            // We have to do it this way because there could be recycled containers between the container we're looking for and the one before it.
            // By finding the index before the place we want to insert and adding one, we ensure that we'll insert the new container in the
            // proper location.
            //
            // In recycling mode childIndex is the index in the _realizedChildren list, not the index in the
            // Children collection.  We have to convert the index; we'll call the index in the Children collection
            // the visualTreeIndex.
            //
  
            int visualTreeIndex = 0;
 
            if (childIndex > 0)
            {
                visualTreeIndex = ChildIndexFromRealizedIndex(childIndex - 1);
                visualTreeIndex++;
            }
 
  
            if (isRecycled && visualTreeIndex < children.Count && children[visualTreeIndex] == container)
            {
                // Don't insert if a recycled container is in the proper place already
            }
            else
            {
                if (visualTreeIndex < children.Count)
                {
                    int insertIndex = visualTreeIndex;
                    if (isRecycled && container.InternalVisualParent != null)
                    {
                        // If the container is recycled we have to remove it from its place in the visual tree and
                        // insert it in the proper location.   For perf we'll use an internal Move API that moves
                        // the first parameter to right before the second one.
                        Debug.Assert(children[visualTreeIndex] != null, "MoveVisualChild interprets a null destination as 'move to end'");
                        children.MoveVisualChild(container, children[visualTreeIndex]);
                        visualOrderChanged = true;
                    }
                    else
                    {
                        VirtualizingPanel.InsertInternalChild(children, insertIndex, container);
                    }
                }
                else
                {
                    if (isRecycled && container.InternalVisualParent != null)
                    {
                        // Recycled container is still in the tree; move it to the end
                        children.MoveVisualChild(container, null);
                        visualOrderChanged = true;
                    }
                    else
                    {
                        VirtualizingPanel.AddInternalChild(children, container);
                    }
                }
            }
 
            //
            // Keep realizedChildren in [....] w/ the visual tree.
            //
            if (IsVirtualizing && InRecyclingMode)
            {
                _realizedChildren.Insert(childIndex, container);
            }
 
            Generator.PrepareItemContainer(container);
 
            return visualOrderChanged;
        }
  
  
 
  
        private void EnsureCleanupOperation(bool delay)
        {
            if (delay)
            {
                bool noPendingOperations = true;
                if (_cleanupOperation != null)
                {
                    noPendingOperations = _cleanupOperation.Abort();
                    if (noPendingOperations)
                    {
                        _cleanupOperation = null;
                    }
                }
                if (noPendingOperations && (_cleanupDelay == null))
                {
                    _cleanupDelay = new DispatcherTimer();
                    _cleanupDelay.Tick += new EventHandler(OnDelayCleanup);
                    _cleanupDelay.Interval = TimeSpan.FromMilliseconds(500.0);
                    _cleanupDelay.Start();
                }
            }
            else
            {
                if ((_cleanupOperation == null) && (_cleanupDelay == null))
                {
                    _cleanupOperation = Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(OnCleanUp), null);
                }
            }
        }
 
        private bool PreviousChildIsGenerated(int childIndex)
        {
            GeneratorPosition position = new GeneratorPosition(childIndex, 0);
            position = Generator.GeneratorPositionFromIndex(Generator.IndexFromGeneratorPosition(position) - 1);
            return (position.Offset == 0 && position.Index >= 0);
        }
 
 
        /// <summary>
        /// Takes a container returned from Generator.GenerateNext() and places it in the visual tree if necessary.
        /// Takes into account whether the container is new, recycled, or already realized.
        /// </summary>
        /// <param name="childIndex">
        /// <param name="child">
        /// <param name="newlyRealized">
        private bool AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
        {
            bool visualOrderChanged = false;
  
            if (!newlyRealized)
            {
                //
                // Container is either realized or recycled.  If it's realized do nothing; it already exists in the visual
                // tree in the proper place.
                //
 
                if (InRecyclingMode)
                {
                    // Note there's no check for IsVirtualizing here.  If the user has just flipped off virtualization it's possible that
                    // the Generator will still return some recycled containers until its list runs out.
  
                    IList children = RealizedChildren;
  
                    if (childIndex >= children.Count || !(children[childIndex] == child))
                    {
                        Debug.Assert(!children.Contains(child), "we incorrectly identified a recycled container");
  
                        //
                        // We have a recycled container (if it was a realized container it would have been returned in the
                        // proper location).  Note also that recycled containers are NOT in the _realizedChildren list.
                        //
  
                        visualOrderChanged = InsertRecycledContainer(childIndex, child);
                    }
                    else
                    {
                        // previously realized child.
                    }
                }
                else
                {
                    // Not recycling; realized container
                    Debug.Assert(child == InternalChildren[childIndex], "Wrong child was generated");
                }
            }
            else
            {
                InsertNewContainer(childIndex, child);
            }
  
            return visualOrderChanged;
        }
 
        private UIElement GeneratePreviousChild(int childIndex, Size layoutSlotSize)
        {
            int newIndex = Generator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0)) - 1;
            if (newIndex >= 0)
            {
                UIElement child;
                bool visualOrderChanged = false;
                IItemContainerGenerator generator = Generator;
 
                int newGeneratedIndex;
                GeneratorPosition newStartPos = IndexToGeneratorPositionForStart(newIndex, out newGeneratedIndex);
                using (generator.StartAt(newStartPos, GeneratorDirection.Forward, true))
                {
                    bool newlyRealized;
                    child = generator.GenerateNext(out newlyRealized) as UIElement;
                    Debug.Assert(child != null, "Null child was generated");
 
                    AddContainerFromGenerator(childIndex, child, newlyRealized);
  
                    if (childIndex <= _firstVisibleChildIndex)
                    {
                        _firstVisibleChildIndex++;
                    }
  
                    child.Measure(layoutSlotSize);
                }
 
                if (visualOrderChanged)
                {
                    Debug.Assert(IsVirtualizing && InRecyclingMode, "We should only modify the visual order when in recycling mode");
                    InvalidateZState();
                }
  
                return child;
            }
 
            return null;
        }
  
  
        private void OnItemsRemove(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }
 
        private void OnItemsReplace(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }
 
        private void OnItemsMove(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.OldPosition, args.ItemCount, args.ItemUICount);
        }
  
        private void RemoveChildRange(GeneratorPosition position, int itemCount, int itemUICount)
        {
            if (IsItemsHost)
            {
                UIElementCollection children = InternalChildren;
                int pos = position.Index;
                if (position.Offset > 0)
                {
                    // An item is being removed after the one at the index
                    pos++;
                }
  
                if (pos < children.Count)
                {
                    int uiCount = itemUICount;
                    Debug.Assert((itemCount == itemUICount) || (itemUICount == 0), "Both ItemUICount and ItemCount should be equal or ItemUICount should be 0.");
                    if (uiCount > 0)
                    {
                        VirtualizingPanel.RemoveInternalChildRange(children, pos, uiCount);
  
                        if (IsVirtualizing && InRecyclingMode)
                        {
                            _realizedChildren.RemoveRange(pos, uiCount);
                        }
                    }
                }
            }
        }
  
  
        private void AdjustCacheWindow(int firstViewport, int itemCount)
        {
            //
            // Adjust the container cache window such that the viewport is always contained inside.
            //
  
            // firstViewport is the index of the first container in the viewport, not counting the before trail.
            // _visibleCount is the total number of items we generated. It already contains the _afterTrail.
  
            // First and last containers that we must keep in view; index is into the data item collection
            int firstContainer = firstViewport > 0 ? firstViewport - _beforeTrail : firstViewport;
            int lastContainer = firstViewport + _visibleCount - 1;   // beforeTrail is not included in _visibleCount
 
            // clamp last container
            if (lastContainer >= itemCount)
            {
                lastContainer = itemCount - 1;
            }
 
            int cacheEnd = CacheEnd;
 
            if (firstContainer < _cacheStart)
            {
                // shift the cache start up
                _cacheStart = firstContainer;
            }
            else if (lastContainer > cacheEnd)
            {
                // shift the cache start down
                _cacheStart += (lastContainer - cacheEnd);
            }
 
  
            // In some cases cacheEnd can be past the end of the list of items.  This is perfectly fine.
            Debug.Assert(_cacheStart <= firstContainer && (CacheEnd >= firstContainer + _visibleCount - 1 || CacheEnd >= itemCount - 1), "The container cache window is out of place");
        }
 
        private bool IsOutsideCacheWindow(int itemIndex)
        {
 
            return (itemIndex < _cacheStart || itemIndex > CacheEnd);
        }
 
  
        /// <summary>
        /// Immediately cleans up any containers that have gone offscreen.  Called by MeasureOverride.
        /// When recycling this runs before generating and measuring children; otherwise it runs after.
        /// </summary>
        private void CleanupContainers(int firstViewport, ItemsControl itemsControl)
        {
            Debug.Assert(IsVirtualizing, "Can't clean up containers if not virtualizing");
            Debug.Assert(InRecyclingMode || IsPixelBased,
                "For backwards compat the standard virtualizing mode has its own cleanup algorithm");
            Debug.Assert(itemsControl != null, "We can't cleanup if we aren't the itemshost");
 
            //
            // It removes items outside of the container cache window (a logical 'window' at
            // least as large as the viewport).
            //
            // firstViewport is the index of first data item that will be in the viewport
            // at the end of Measure.  This is effectively the scroll offset.
            //
            // _visibleStart is index of the first data item that was previously at the top of the viewport
            // At the end of a Measure pass _visibleStart == firstViewport.
            //
            // _visibleCount is the number of data items that were previously visible in the viewport.
 
            int cleanupRangeStart = -1;
            int cleanupCount = 0;
            int itemIndex = -1;              // data item index used to compare with the cache window position.
            int lastItemIndex;
            IList children = RealizedChildren;
            int focusedChild = -1, previousFocusable = -1, nextFocusable = -1;  // child indices for the focused item and before and after focus trail items
  
            bool performCleanup = false;
            UIElement child;
 
            if (children.Count == 0)
            {
                return; // nothing to do
            }
 
            AdjustCacheWindow(firstViewport, itemsControl.Items.Count);
 
            if (IsKeyboardFocusWithin && !IsPixelBased)
            {
                // If we're not in a hieararchy we can find the focus trail locally; for hierarchies it has already been
                // precalculated.
                FindFocusedChild(out focusedChild, out previousFocusable, out nextFocusable);
            }
 
            //
            // Iterate over all realized children and recycle the ones that are eligible.  Items NOT eligible for recycling
            // have one or more of the following properties
            //
            //  - inside the cache window
            //  - the item is its own container
            //  - has keyboard focus
            //  - is the first focusable item before or after the focused item
            //  - the CleanupVirtualizedItem event was canceled
            //
 
            for (int childIndex = 0; childIndex < children.Count; childIndex++)
            {
                child = (UIElement)children[childIndex];
                lastItemIndex = itemIndex;
                itemIndex = GetGeneratedIndex(childIndex);
  
                if (itemIndex - lastItemIndex != 1)
                {
                    // There's a generated gap between the current item and the last.  Clean up the last range of items.
                    performCleanup = true;
                }
  
                if (performCleanup)
                {
                    if (cleanupRangeStart >= 0 && cleanupCount > 0)
                    {
                        //
                        // We've hit a non-virtualizable container or a non-contiguous section.
                        //
 
                        CleanupRange(children, Generator, cleanupRangeStart, cleanupCount);
 
                        // CleanupRange just modified the _realizedChildren list.  Adjust the childIndex.
                        childIndex -= cleanupCount;
                        focusedChild -= cleanupCount;
                        previousFocusable -= cleanupCount;
                        nextFocusable -= cleanupCount;
 
                        cleanupCount = 0;
                        cleanupRangeStart = -1;
                    }
  
                    performCleanup = false;
                }
  
 
                if (IsOutsideCacheWindow(itemIndex) &&
                    !((IGeneratorHost)itemsControl).IsItemItsOwnContainer(itemsControl.Items[itemIndex]) &&
                    childIndex != focusedChild &&
                    childIndex != previousFocusable &&
                    childIndex != nextFocusable &&
                    !IsInFocusTrail(child) &&                   // logically the same computation as the three above; used when in a treeview.
                    child != _bringIntoViewContainer &&         // the container we're going to bring into view must not be recycled
                    NotifyCleanupItem(child, itemsControl))
                {
                    //
                    // The container is eligible to be virtualized
                    //
                    if (cleanupRangeStart == -1)
                    {
                        cleanupRangeStart = childIndex;
                    }
  
                    cleanupCount++;
 
                    //
                    // Save off the child's desired size if we're doing pixel-based virtualization.
                    // We need to save off the size when doing hierarchical (i.e. TreeView) virtualization, since containers will vary
                    // greatly in size. This is required both to compute the index of the first visible item in the viewport and to Arrange
                    // children in their proper locations.
                    //
                    if (IsPixelBased)
                    {
                        itemsControl.StoreItemValue(itemsControl.Items[itemIndex], child.DesiredSize, _desiredSizeStorageIndex);
                    }
                }
                else
                {
                    // Non-recyclable container;
                    performCleanup = true;
                }
            }
 
            if (cleanupRangeStart >= 0 && cleanupCount > 0)
            {
                CleanupRange(children, Generator, cleanupRangeStart, cleanupCount);
            }
        }
 
  
        private void EnsureRealizedChildren()
        {
            Debug.Assert(InRecyclingMode, "This method only applies to recycling mode");
            if (_realizedChildren == null)
            {
                UIElementCollection children = InternalChildren;
  
                _realizedChildren = new List<uielement>(children.Count);
  
                for (int i = 0; i < children.Count; i++)
                {
                    _realizedChildren.Add(children[i]);
                }
            }
        }
  
 
        [Conditional("DEBUG")]
        private void debug_VerifyRealizedChildren()
        {
            // Debug method that ensures the _realizedChildren list matches the realized containers in the Generator.
            Debug.Assert(IsVirtualizing && InRecyclingMode, "Realized children only exist when recycling");
            Debug.Assert(_realizedChildren != null, "Realized children must exist to verify it");
            System.Windows.Controls.ItemContainerGenerator generator = Generator as System.Windows.Controls.ItemContainerGenerator;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (generator != null && itemsControl != null && itemsControl.IsGrouping == false)
            {
 
                foreach (UIElement child in InternalChildren)
                {
                    int dataIndex = generator.IndexFromContainer(child);
  
                    if (dataIndex == -1)
                    {
                        // Child is not in the generator's realized container list (i.e. it's a recycled container): ensure it's NOT in _realizedChildren.
                        Debug.Assert(!_realizedChildren.Contains(child), "_realizedChildren should not contain recycled containers");
                    }
                    else
                    {
                        // Child is a realized container; ensure it's in _realizedChildren at the proper place.
                        GeneratorPosition position = Generator.GeneratorPositionFromIndex(dataIndex);
                        Debug.Assert(_realizedChildren[position.Index] == child, "_realizedChildren is corrupt!");
                    }
                }
            }
        }
 
        [Conditional("DEBUG")]
        private void debug_AssertRealizedChildrenEqualVisualChildren()
        {
            if (IsVirtualizing && InRecyclingMode)
            {
                UIElementCollection children = InternalChildren;
                Debug.Assert(_realizedChildren.Count == children.Count, "Realized and visual children must match");
 
                for (int i = 0; i < children.Count; i++)
                {
                    Debug.Assert(_realizedChildren[i] == children[i], "Realized and visual children must match");
                }
            }
        }
  
        /// <summary>
        /// Takes an index from the realized list and returns the corresponding index in the Children collection
        /// </summary>
        /// <param name="realizedChildIndex">
        /// <returns></returns>
        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            //
            // If we're not recycling containers then we're not using a realizedChild index and no translation is necessary
            //
            if (IsVirtualizing && InRecyclingMode)
            {
  
                if (realizedChildIndex < _realizedChildren.Count)
                {
  
                    UIElement child = _realizedChildren[realizedChildIndex];
                    UIElementCollection children = InternalChildren;
 
                    for (int i = realizedChildIndex; i < children.Count; i++)
                    {
                        if (children[i] == child)
                        {
                            return i;
                        }
                    }
  
                    Debug.Assert(false, "We should have found a child");
                }
            }
  
            return realizedChildIndex;
        }
  
        /// <summary>
        /// Recycled containers still in the Children collection at the end of Measure should be disconnected
        /// from the visual tree.  Otherwise they're still visible to things like Arrange, keyboard navigation, etc.
        /// </summary>
        private void DisconnectRecycledContainers()
        {
            int realizedIndex = 0;
            UIElement visualChild;
            UIElement realizedChild = _realizedChildren.Count > 0 ? _realizedChildren[0] : null;
            UIElementCollection children = InternalChildren;
  
            for (int i = 0; i < children.Count; i++)
            {
                visualChild = children[i];
  
                if (visualChild == realizedChild)
                {
                    realizedIndex++;
 
                    if (realizedIndex < _realizedChildren.Count)
                    {
 
                        realizedChild = _realizedChildren[realizedIndex];
                    }
                    else
                    {
                        realizedChild = null;
                    }
                }
                else
                {
                    // The visual child is a recycled container
                    children.RemoveNoVerify(visualChild);
                    i--;
                }
            }
 
            debug_VerifyRealizedChildren();
            debug_AssertRealizedChildrenEqualVisualChildren();
        }
 
        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            IItemContainerGenerator generator = Generator;
            GeneratorPosition position = (generator != null) ? generator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
 
            // determine the position in the children collection for the first
            // generated container.  This assumes that generator.StartAt will be called
            // with direction=Forward and  allowStartAtRealizedItem=true.
            childIndex = (position.Offset == 0) ? position.Index : position.Index + 1;
  
            return position;
        }
  
 
        #region Delayed Cleanup Methods
 
        //
        // Delayed Cleanup is used when the VirtualizationMode is standard (not recycling) and the panel is scrolling and item-based
        // It chooses to defer virtualizing items until there are enough available.  It then cleans them using a background priority dispatcher
        // work item
        //
  
        private void OnDelayCleanup(object sender, EventArgs e)
        {
            Debug.Assert(_cleanupDelay != null);
 
            bool needsMoreCleanup = false;
  
            try
            {
                needsMoreCleanup = CleanUp();
            }
            finally
            {
                // Cleanup the timer if more cleanup is unnecessary
                if (!needsMoreCleanup)
                {
                    _cleanupDelay.Stop();
                    _cleanupDelay = null;
                }
            }
        }
 
        private object OnCleanUp(object args)
        {
            Debug.Assert(_cleanupOperation != null);
 
            bool needsMoreCleanup = false;
  
            try
            {
                needsMoreCleanup = CleanUp();
            }
            finally
            {
                // Keeping this non-null until here in case cleaning up causes re-entrancy
                _cleanupOperation = null;
            }
 
            if (needsMoreCleanup)
            {
                EnsureCleanupOperation(true /* delay */);
            }
  
            return null;
        }
  
        private bool CleanUp()
        {
            Debug.Assert(!InRecyclingMode, "This method only applies to standard virtualization");
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (!IsVirtualizing || !IsItemsHost)
            {
                // Virtualization is turned off or we aren't hosting children; no need to cleanup.
                return false;
            }
  
            int startMilliseconds = Environment.TickCount;
            bool needsMoreCleanup = false;
            UIElementCollection children = InternalChildren;
            int minDesiredGenerated = MinDesiredGenerated;
            int maxDesiredGenerated = MaxDesiredGenerated;
            int pageSize = maxDesiredGenerated - minDesiredGenerated;
            int extraChildren = children.Count - pageSize;
 
            if (extraChildren > (pageSize * 2))
            {
                if ((Mouse.LeftButton == MouseButtonState.Pressed) &&
                    (extraChildren < 1000))
                {
                    // An optimization for when we are dragging the mouse.
                    needsMoreCleanup = true;
                }
                else
                {
                    bool trailingFocus = IsKeyboardFocusWithin;
                    bool keepForwardTrail = false;
                    int focusIndex = -1;
                    IItemContainerGenerator generator = Generator;
 
                    int cleanupRangeStart = 0;
                    int cleanupCount = 0;
                    int lastGeneratedIndex = -1;
                    int counterAdjust;
 
                    for (int i = 0; i < children.Count; i++)
                    {
                        // It is possible for TickCount to wrap around about every 30 days.
                        // If that were to occur, then this particular cleanup may not be interrupted.
                        // That is OK since the worst that can happen is that there is more of a stutter than normal.
                        int totalMilliseconds = Environment.TickCount - startMilliseconds;
                        if ((totalMilliseconds > 50) && (cleanupCount > 0))
                        {
                            // Cleanup has been working for 50ms already and the user might start
                            // noticing a lag. Stop cleaning up and release the thread for other work.
                            // Cleanup will continue later.
                            // Don't break out until after at least one item has been found to cleanup.
                            // Otherwise, we might end up in an infinite loop.
                            needsMoreCleanup = true;
                            break;
                        }
  
                        int childIndex = i;
                        if (trailingFocus)
                        {
                            // Focus lies somewhere within the panel, but it has not been found yet.
                            UIElement child = children[i];
                            if (child.IsKeyboardFocusWithin)
                            {
                                // Focus has been found, we can now re-virtualize items before the focus.
                                trailingFocus = false;
                                keepForwardTrail = true;
                                focusIndex = i;
                                if (i > 0)
                                {
                                    // Go through the trailing items and find a focusable item to keep.
                                    int trailIndex = i - 1;
                                    int end = Math.Max(0, i - FocusTrail);
                                    for (; trailIndex >= end; trailIndex--)
                                    {
                                        child = children[trailIndex];
                                        if (Keyboard.IsFocusable(child))
                                        {
                                            trailIndex--;
                                            break;
                                        }
                                    }
 
                                    // The rest of the trailing items can be re-virtualized.
                                    for (childIndex = end; childIndex <= trailIndex; childIndex++)
                                    {
                                        ManageCleanup(
                                            children,
                                            itemsControl,
                                            generator,
                                            childIndex,
                                            minDesiredGenerated,
                                            maxDesiredGenerated,
                                            ref childIndex,
                                            ref cleanupRangeStart,
                                            ref cleanupCount,
                                            ref lastGeneratedIndex,
                                            out counterAdjust);
                                        if (counterAdjust > 0)
                                        {
                                            i -= counterAdjust;
                                            trailIndex -= counterAdjust;
                                        }
                                    }
 
                                    if (cleanupCount > 0)
                                    {
                                        // Cleanup the last batch for the focused item
                                        CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                                        i -= cleanupCount;
                                        cleanupCount = 0;
                                    }
                                    cleanupRangeStart = i + 1;
 
                                    // At this point, we are caught up and should go to the next item
                                    continue;
                                }
                            }
                            else if (i >= FocusTrail)
                            {
                                childIndex = i - FocusTrail;
                            }
                            else
                            {
                                continue;
                            }
                        }
 
                        if (keepForwardTrail)
                        {
                            // Find a focusable item after the focused item to keep
                            if (childIndex <= (focusIndex + FocusTrail))
                            {
                                UIElement child = children[childIndex];
                                if (Keyboard.IsFocusable(child))
                                {
                                    // A focusable item was found, all items after this one can be re-virtualized
                                    keepForwardTrail = false;
                                    cleanupRangeStart = childIndex + 1;
                                    cleanupCount = 0;
                                }
                                continue;
                            }
                            else
                            {
                                keepForwardTrail = false;
                            }
                        }
 
                        ManageCleanup(
                            children,
                            itemsControl,
                            generator,
                            childIndex,
                            minDesiredGenerated,
                            maxDesiredGenerated,
                            ref i,
                            ref cleanupRangeStart,
                            ref cleanupCount,
                            ref lastGeneratedIndex,
                            out counterAdjust);
                    }
  
                    if (cleanupCount > 0)
                    {
                        // Cleanup the final batch
                        CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                    }
                }
            }
  
            return needsMoreCleanup;
        }
  
        private void ManageCleanup(
            UIElementCollection children,
            ItemsControl itemsControl,
            IItemContainerGenerator generator,
            int childIndex,
            int minDesiredGenerated,
            int maxDesiredGenerated,
            ref int counter,
            ref int cleanupRangeStart,
            ref int cleanupCount,
            ref int lastGeneratedIndex,
            out int counterAdjust)
        {
            counterAdjust = 0;
            bool performCleanup = false;
            bool countThisChild = false;
            int generatedIndex = GetGeneratedIndex(childIndex);
  
            if (OutsideMinMax(generatedIndex, minDesiredGenerated, maxDesiredGenerated) &&
                NotifyCleanupItem(childIndex, children, itemsControl))
            {
                // The item can be re-virtualized.
                if ((generatedIndex - lastGeneratedIndex) == 1)
                {
                    // Add another to the current batch.
                    cleanupCount++;
                }
                else
                {
                    // There was a gap in generated items. Cleanup any from the previous batch.
                    performCleanup = countThisChild = true;
                }
            }
            else
            {
                // The item cannot be re-virtualized. Cleanup any from the previous batch.
                performCleanup = true;
            }
 
            if (performCleanup)
            {
                // Cleanup a batch of items
                if (cleanupCount > 0)
                {
                    CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                    counterAdjust = cleanupCount;
                    counter -= counterAdjust;
                    childIndex -= counterAdjust;
                    cleanupCount = 0;
                }
  
                if (countThisChild)
                {
                    // The current child was not included in the batch and should be saved for later
                    cleanupRangeStart = childIndex;
                    cleanupCount = 1;
                }
                else
                {
                    // The next child will start the next batch.
                    cleanupRangeStart = childIndex + 1;
                }
            }
            lastGeneratedIndex = generatedIndex;
        }
 
        private bool NotifyCleanupItem(int childIndex, UIElementCollection children, ItemsControl itemsControl)
        {
            return NotifyCleanupItem(children[childIndex], itemsControl);
        }
 
        private bool NotifyCleanupItem(UIElement child, ItemsControl itemsControl)
        {
            CleanUpVirtualizedItemEventArgs e = new CleanUpVirtualizedItemEventArgs(itemsControl.ItemContainerGenerator.ItemFromContainer(child), child);
            e.Source = this;
            OnCleanUpVirtualizedItem(e);
 
            return !e.Cancel;
        }
 
        private void CleanupRange(IList children, IItemContainerGenerator generator, int startIndex, int count)
        {
            if (InRecyclingMode)
            {
                Debug.Assert(startIndex >= 0 && count > 0);
                Debug.Assert(children == _realizedChildren, "the given child list must be the _realizedChildren list when recycling");
                ((IRecyclingItemContainerGenerator)generator).Recycle(new GeneratorPosition(startIndex, 0), count);
  
                // The call to Recycle has caused the ItemContainerGenerator to remove some items
                // from its list of realized items; we adjust _realizedChildren to match.
                _realizedChildren.RemoveRange(startIndex, count);
            }
            else
            {
                // Remove the desired range of children
                VirtualizingPanel.RemoveInternalChildRange((UIElementCollection)children, startIndex, count);
                generator.Remove(new GeneratorPosition(startIndex, 0), count);
            }
  
            AdjustFirstVisibleChildIndex(startIndex, count);
        }
 
        #endregion
 
        /// <summary>
        /// Called after 'count' items were removed or recycled from the Generator.  _firstVisibleChildIndex is the
        /// index of the first visible container.  This index isn't exactly the child position in the UIElement collection;
        /// it's actually the index of the realized container inside the generator.  Since we've just removed some realized
        /// containers from the generator (by calling Remove or Recycle), we have to adjust the first visible child index.
        /// </summary>
        /// <param name="startIndex">index of the first removed item
        /// <param name="count">number of items removed
        private void AdjustFirstVisibleChildIndex(int startIndex, int count)
        {
  
            // Update the index of the first visible generated child
            if (startIndex < _firstVisibleChildIndex)
            {
                int endIndex = startIndex + count - 1;
                if (endIndex < _firstVisibleChildIndex)
                {
                    // The first visible index is after the items that were removed
                    _firstVisibleChildIndex -= count;
                }
                else
                {
                    // The first visible index was within the items that were removed
                    _firstVisibleChildIndex = startIndex;
                }
            }
        }
  
        private static bool OutsideMinMax(int i, int min, int max)
        {
            return ((i < min) || (i > max));
        }
 
        private void EnsureTopCapGenerated(Size layoutSlotSize)
        {
            // Ensure that a focusable item is generated above the first visible item
            // so that keyboard navigation works.
  
            IList children;
  
            _beforeTrail = 0;
            if (_visibleStart > 0)
            {
                children = RealizedChildren;
                int childIndex = _firstVisibleChildIndex;
  
                UIElement child;
 
                // At most, we will search FocusTrail number of items for a focusable item
                for (; _beforeTrail < FocusTrail; _beforeTrail++)
                {
                    if (PreviousChildIsGenerated(childIndex))
                    {
                        // The previous child is already generated, check its focusability
                        childIndex--;
                        child = (UIElement)children[childIndex];
                    }
                    else
                    {
                        // Generate the previous child
                        child = GeneratePreviousChild(childIndex, layoutSlotSize);
                    }
 
                    if ((child == null) || Keyboard.IsFocusable(child))
                    {
                        // Either a focusable item was found, or no child was generated
                        _beforeTrail++;
                        break;
                    }
                }
            }
        }
  
  
        /// <summary>
        /// Returns the MeasureData we'll be using for computations in MeasureOverride.  This updates the viewport offset
        /// based on the one set in the MeasureData property prior to the call to MeasureOverride.
        /// </summary>
        /// <param name="constraint">
        /// <param name="itemsControl">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private MeasureData AdjustViewportOffset(MeasureData givenMeasureData, ItemsControl itemsControl, bool isHorizontal)
        {
            // Note that a panel should not modify its own MeasureData -- it needs to be treated exactly as if it was a variable
            // passed into MeasureOverride.  That's why we make a copy of MeasureData in this method and return that.
 
            Rect viewport;
            MeasureData newMeasureData = null;
            IProvideStackingSize stackingSize;
            double offset = 0d;
            Debug.Assert(MeasureData == null || IsPixelBased, "If a panel has measure data then it must be pixel based");
            Debug.Assert(!IsScrolling && IsPixelBased, "This only applies to internal panels");
  
            //
            // This panel isn't a scroll owner but some panel above it is.  It will be able to use the viewport data
            // to virtualize.
            //
 
            if (givenMeasureData != null)
            {
                viewport = givenMeasureData.Viewport;
                stackingSize = itemsControl as IProvideStackingSize;
 
                Debug.Assert(givenMeasureData.HasViewport, "MeasureData is only set on objects when we want to pass down viewport information.");
 
                //
                // We need to offset the viewport to take into account the delta between the top of the items control
                // and this panel (i.e. the header).  Ask for the header, and, if not available, use the estimated container size.
  
                if (stackingSize != null)
                {
                    offset = stackingSize.HeaderSize(isHorizontal);
 
                    if (offset <= 0d || DoubleUtil.IsNaN(offset))
                    {
                        offset = ContainerStackingSizeEstimate(stackingSize, isHorizontal);
                    }
                }
 
                if (isHorizontal)
                {
                    viewport.X -= offset;
                }
                else
                {
                    // adjust viewport for the header of the TreeViewItem containing this as an ItemsPanel.
                    viewport.Y -= offset;
                }
  
                newMeasureData = new MeasureData(givenMeasureData.AvailableSize, viewport);
            }
 
  
            return newMeasureData;
        }
  
        /// <summary>
        /// Sets up IsVirtualizing, VirtualizationMode, and IsPixelBased
        ///
        /// IsVirtualizing is true if turned on via the items control and if the panel has a viewport.
        /// VSP has a viewport if it's either the scrolling panel or it was given MeasureData.
        ///
        /// IsPixelBased is true if the panel is virtualizing and (for backwards compat) is the ItemsHost for a TreeView or TreeViewItem.
        /// VSP can only make use of, create, and propagate down MeasureData if it is pixel-based, since the viewport is in pixels.
        /// </summary>
        /// <param name="itemsControl">
        private void SetVirtualizationState(ItemsControl itemsControl, bool hasMeasureData)
        {
            VirtualizationMode mode = (itemsControl != null) ? GetVirtualizationMode(itemsControl) : VirtualizationMode.Standard;
 
            if (itemsControl != null)
            {
                // Set IsVirtualizing.  This panel can only virtualize if IsVirtualizing is set on its ItemsControl and it has viewport data.
                // It has viewport data if it's either the scroll host or was given viewport information by measureData.
 
                if (GetIsVirtualizing(itemsControl) && (IsScrolling || hasMeasureData))
                {
                    IsVirtualizing = true;
                }
            }
            else
            {
                IsVirtualizing = false;
            }
  
 
            //
            // Set up info on first measure
            //
            if (HasMeasured)
            {
                VirtualizationMode oldMode = VirtualizationMode;
 
                if (oldMode != mode)
                {
                    throw new InvalidOperationException(SR.Get(SRID.CantSwitchVirtualizationModePostMeasure));
                }
            }
            else
            {
                HasMeasured = true;
 
                if (IsVirtualizing && (itemsControl is TreeView || itemsControl is TreeViewItem))
                {
                    IsPixelBased = true;
                }
  
                VirtualizationMode = mode;
            }
        }
 
        private int MinDesiredGenerated
        {
            get
            {
                return Math.Max(0, _visibleStart - _beforeTrail);
            }
        }
  
        private int MaxDesiredGenerated
        {
            get
            {
                return Math.Min(ItemCount, _visibleStart + _visibleCount + _afterTrail);
            }
        }
  
        private int ItemCount
        {
            get
            {
                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                return (itemsControl != null) ? itemsControl.Items.Count : 0;
            }
        }
  
        #endregion
 
  
        private void EnsureScrollData()
        {
            if (_scrollData == null) { _scrollData = new ScrollData(); }
        }
 
        private static void ResetScrolling(VirtualizingStackPanel element)
        {
            element.InvalidateMeasure();
  
            // Clear scrolling data.  Because of thrash (being disconnected & reconnected, &c...), we may
            if (element.IsScrolling)
            {
                element._scrollData.ClearLayout();
            }
        }
  
        // OnScrollChange is an override called whenever the IScrollInfo exposed scrolling state changes on this element.
        // At the time this method is called, scrolling state is in its new, valid state.
        private void OnScrollChange()
        {
            if (ScrollOwner != null) { ScrollOwner.InvalidateScrollInfo(); }
        }
 
        private void SetAndVerifyScrollingData(Size viewport, Size extent, Vector offset)
        {
            Debug.Assert(IsScrolling);
  
            if (IsPixelBased)
            {
                // _scrollData is in pixels and thus operations like LineDown can push the offset too far.
                // The behavior here is effectively the same as ScrollContentPresenter.VerifyScrollData
                offset.X = ScrollContentPresenter.CoerceOffset(offset.X, extent.Width, viewport.Width);
                offset.Y = ScrollContentPresenter.CoerceOffset(offset.Y, extent.Height, viewport.Height);
            }
 
            // Detect changes to the viewport, extent, and offset
            bool viewportChanged = !DoubleUtil.AreClose(viewport, _scrollData._viewport);
            bool extentChanged = !DoubleUtil.AreClose(extent, _scrollData._extent);
            bool offsetChanged = !DoubleUtil.AreClose(offset, _scrollData._computedOffset);
  
            // Update data and fire scroll change notifications
            _scrollData._offset = offset;
            if (viewportChanged || extentChanged || offsetChanged)
            {
                Vector oldViewportOffset = _scrollData._computedOffset;
                Size oldViewportSize = _scrollData._viewport;
 
                _scrollData._viewport = viewport;
                _scrollData._extent = extent;
                _scrollData._computedOffset = offset;
  
                // Report changes to the viewport
                if (viewportChanged)
                {
                    OnViewportSizeChanged(oldViewportSize, viewport);
                }
 
                // Report changes to the offset
                if (offsetChanged)
                {
                    OnViewportOffsetChanged(oldViewportOffset, offset);
                }
  
                OnScrollChange();
            }
        }
  
        /// <summary>
        ///     Allows subclasses to be notified of changes to the viewport size data.
        /// </summary>
        /// <param name="oldViewportSize">The old value of the size.
        /// <param name="newViewportSize">The new value of the size.
        protected virtual void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize)
        {
        }
  
        /// <summary>
        ///     Allows subclasses to be notified of changes to the viewport offset data.
        /// </summary>
        /// <param name="oldViewportOffset">The old value of the offset.
        /// <param name="newViewportOffset">The new value of the offset.
        protected virtual void OnViewportOffsetChanged(Vector oldViewportOffset, Vector newViewportOffset)
        {
        }
  
        // Translates a logical (child index) offset to a physical (1/96") when scrolling.
        // If virtualizing, it makes the assumption that the logicalOffset is always the first in the visual collection
        //   and thus returns 0.
        // If not virtualizing, it assumes that children are Measure clean; should only be called after running Measure.
        private double ComputePhysicalFromLogicalOffset(double logicalOffset, bool fHorizontal)
        {
            double physicalOffset = 0.0;
 
            IList children = RealizedChildren;
 
            Debug.Assert(logicalOffset == 0 || (logicalOffset > 0 && logicalOffset < children.Count));
  
            for (int i = 0; i < logicalOffset; i++)
            {
                UIElement child = (UIElement)children[i];
                physicalOffset -= (fHorizontal)
                    ? child.DesiredSize.Width
                    : child.DesiredSize.Height;
            }
  
            return physicalOffset;
        }
  
        private int FindChildIndexThatParentsVisual(Visual v)
        {
            DependencyObject child = v;
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != this)
            {
                child = parent;
                parent = VisualTreeHelper.GetParent(child);
            }
 
            IList children = RealizedChildren;
 
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == child)
                {
                    return GetGeneratedIndex(i);
                }
            }
 
            return -1;
        }
 
        // This is very similar to the work that ScrollContentPresenter does for MakeVisible.  Simply adjust by a
        // pixel offset.
        private void MakeVisiblePhysicalHelper(Rect r, ref Vector newOffset, ref Rect newRect, bool isHorizontal)
        {
            double viewportOffset;
            double viewportSize;
            double targetRectOffset;
            double targetRectSize;
            double minPhysicalOffset;
  
            if (isHorizontal)
            {
                viewportOffset = _scrollData._computedOffset.X;
                viewportSize = ViewportWidth;
                targetRectOffset = r.X;
                targetRectSize = r.Width;
            }
            else
            {
                viewportOffset = _scrollData._computedOffset.Y;
                viewportSize = ViewportHeight;
                targetRectOffset = r.Y;
                targetRectSize = r.Height;
            }
 
            targetRectOffset += viewportOffset;
            minPhysicalOffset = ScrollContentPresenter.ComputeScrollOffsetWithMinimalScroll(
                viewportOffset, viewportOffset + viewportSize, targetRectOffset, targetRectOffset + targetRectSize);
  
            // Compute the visible rectangle of the child relative to the viewport.
            double left = Math.Max(targetRectOffset, minPhysicalOffset);
            targetRectSize = Math.Max(Math.Min(targetRectSize + targetRectOffset, minPhysicalOffset + viewportSize) - left, 0);
            targetRectOffset = left;
            targetRectOffset -= viewportOffset;
  
            if (isHorizontal)
            {
                newOffset.X = minPhysicalOffset;
                newRect.X = targetRectOffset;
                newRect.Width = targetRectSize;
            }
            else
            {
                newOffset.Y = minPhysicalOffset;
                newRect.Y = targetRectOffset;
                newRect.Height = targetRectSize;
            }
        }
  
        private void MakeVisibleLogicalHelper(int childIndex, Rect r, ref Vector newOffset, ref Rect newRect)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstChildInView;
            int newFirstChild;
            int viewportSize;
            double childOffsetWithinViewport = r.Y;
 
            if (fHorizontal)
            {
                firstChildInView = (int)_scrollData._computedOffset.X;
                viewportSize = (int)_scrollData._viewport.Width;
            }
            else
            {
                firstChildInView = (int)_scrollData._computedOffset.Y;
                viewportSize = (int)_scrollData._viewport.Height;
            }
 
            newFirstChild = firstChildInView;
 
            // If the target child is before the current viewport, move the viewport to put the child at the top.
            if (childIndex < firstChildInView)
            {
                childOffsetWithinViewport = 0;
                newFirstChild = childIndex;
            }
            // If the target child is after the current viewport, move the viewport to put the child at the bottom.
            else if (childIndex > firstChildInView + viewportSize - 1)
            {
                newFirstChild = childIndex - viewportSize + 1;
                double pixelSize = fHorizontal ? ActualWidth : ActualHeight;
                childOffsetWithinViewport = pixelSize * (1.0 - (1.0 / viewportSize));
            }
 
            if (fHorizontal)
            {
                newOffset.X = newFirstChild;
                newRect.X = childOffsetWithinViewport;
                newRect.Width = r.Width;
            }
            else
            {
                newOffset.Y = newFirstChild;
                newRect.Y = childOffsetWithinViewport;
                newRect.Height = r.Height;
            }
        }
  
        // Converts an index into the item collection as a double into an int
        static private int CoerceIndexToInteger(double index, int numberOfItems)
        {
            int newIndex;
  
            if (Double.IsNegativeInfinity(index))
            {
                newIndex = 0;
            }
            else if (Double.IsPositiveInfinity(index))
            {
                newIndex = numberOfItems - 1;
            }
            else
            {
                newIndex = (int)index;
                newIndex = Math.Max(Math.Min(numberOfItems - 1, newIndex), 0);
            }
 
            return newIndex;
        }
 
        private int GetGeneratedIndex(int childIndex)
        {
            return Generator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));
        }
  
 
        //
        // Focus Helpers
        //
  
        #region Focus Helpers
 
        //
        // Methods to keep track of focus.
        //
        // Dealing with Focus while virtualizing a list is easy: don't throw away the focused item and the next and previous
        // focusable items.  When in a TreeView it's much harder; Measure (and thus the cleanup code) for any VSP in the hierarchy
        // can run at any time. The only performant way for a panel to know that one of its children may be the next or previous focusable
        // item is for it to be marked.  We do this every time focus changes within the hierarchy.
        //
 
        private WeakReference[] EnsureFocusTrail()
        {
            WeakReference[] focusTrail = FocusTrailField.GetValue(this);
  
            if (focusTrail == null)
            {
                focusTrail = new WeakReference[2];
                FocusTrailField.SetValue(this, focusTrail);
            }
 
            return focusTrail;
        }
  
  
        /// <summary>
        /// Finds the focused child along with the previous and next focusable children.  Used only when recycling containers;
        /// the standard mode has a different cleanup algorithm
        /// </summary>
        /// <param name="focusedChild">
        /// <param name="previousFocusable">
        /// <param name="nextFocusable">
        private void FindFocusedChild(out int focusedChild, out int previousFocusable, out int nextFocusable)
        {
            Debug.Assert(InRecyclingMode, "This method is only valid for the recycling mode");
            Debug.Assert(IsKeyboardFocusWithin, "we should only search for a focusable child if we have focus");
            focusedChild = previousFocusable = nextFocusable = -1;
            UIElement child;
            bool foundFocusedChild = false;
  
            for (int i = 0; i < _realizedChildren.Count; i++)
            {
                child = _realizedChildren[i];
 
                if (!foundFocusedChild && child.IsKeyboardFocusWithin)
                {
                    focusedChild = i;
                    foundFocusedChild = true;
  
                    // Go through the trailing items.
                    // Go through the trailing items and find a focusable item to keep.
                    int trailIndex = i - 1;
                    int end = Math.Max(0, i - FocusTrail);
                    for (; trailIndex >= end; trailIndex--)
                    {
                        child = _realizedChildren[trailIndex];
                        if (Keyboard.IsFocusable(child))
                        {
                            previousFocusable = trailIndex;
                            break;
                        }
                    }
                }
                else if (foundFocusedChild)
                {
                    if (i <= focusedChild + FocusTrail)
                    {
                        if (Keyboard.IsFocusable(child))
                        {
                            nextFocusable = i;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
  
 
        /// <summary>
        /// Called when the focused item has changed.  Used to set a special DP on the next and previous focusable items.
        /// Only used when virtualizing in a hieararchy (i.e. TreeView virtualization).
        /// </summary>
        /// <param name="e">
        private void FocusChanged(KeyboardFocusChangedEventArgs e)
        {
  
            if (IsVirtualizing && IsScrolling && IsPixelBased)
            {
                // IsScrolling ensures that only the top-level panel tracks focus.
                // The IsPixelBased condition here needs explanation.  It's used here to mean 'Is this panel in a hierarchy?'
                // The assert below is just a reminder to modify this code if the meaning changes.
                Debug.Assert(ItemsControl.GetItemsOwner(this) is TreeView);
  
                // This code is TreeViewItem-specific, since it has its own focus logic and we can't override UIElement.PredictFocus
                TreeViewItem focusedElement = Keyboard.FocusedElement as TreeViewItem;
                WeakReference[] focusTrail = EnsureFocusTrail();
 
 
                //
                // Clear the old focus trail items
                //
  
                for (int i = 0; i < 2; i++)
                {
                    DependencyObject trailItem = (DependencyObject)(focusTrail[i] != null ? focusTrail[i].Target : null);
 
                    if (trailItem != null)
                    {
                        FocusTrailItemField.ClearValue(trailItem);
                    }
                }
 
  
                //
                // Set the new focus trail items
                //
                if (IsKeyboardFocusWithin)
                {
                    DependencyObject previous = null;
                    DependencyObject next = null;
 
                    if (focusedElement != null)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            previous = focusedElement.InternalPredictFocus(FocusNavigationDirection.Left);
                            next = focusedElement.InternalPredictFocus(FocusNavigationDirection.Right);
                        }
                        else
                        {
                            previous = focusedElement.InternalPredictFocus(FocusNavigationDirection.Up);
                            next = focusedElement.InternalPredictFocus(FocusNavigationDirection.Down);
                        }
                    }
  
                    if (previous != null)
                    {
                        FocusTrailItemField.SetValue(previous, true);
                        focusTrail[0] = new WeakReference(previous);
                    }
 
                    if (next != null)
                    {
                        FocusTrailItemField.SetValue(next, true);
                        focusTrail[1] = new WeakReference(next);
                    }
                }
                else
                {
                    // Focus has left the tree
                    FocusTrailField.SetValue(this, null);
                }
            }
        }
  
  
        /// <summary>
        /// Checks the precomputed focus trail.  Valid only if we're in a hierararchy.
        /// </summary>
        /// <param name="container">
        /// <returns></returns>
        private bool IsInFocusTrail(UIElement container)
        {
            if (IsPixelBased)
            {
                return FocusTrailItemField.GetValue(container) || container.IsKeyboardFocusWithin;
            }
            else
            {
                return false;
            }
        }
  
  
 
        #endregion
 
        //------------------------------------------------------------
        // Avalon Property Callbacks/Overrides
        //-----------------------------------------------------------
        #region Avalon Property Callbacks/Overrides
  
        /// <summary>
        /// <see cref="PropertyMetadata.PropertyChangedCallback">
        /// </see></summary>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Since Orientation is so essential to logical scrolling/virtualization, we synchronously check if
            // the new value is different and clear all scrolling data if so.
            ResetScrolling(d as VirtualizingStackPanel);
        }
  
        #endregion
  
        #endregion Private Methods
 
        //-----------------------------------------------------
        //
        //  Private Properties
        //
        //-----------------------------------------------------
 
        #region Private Properties
 
        /// <summary>
        /// Index of the last item in the cache window
        /// </summary>
        private int CacheEnd
        {
            get
            {
                // Note we don't have the _afterTrail here:  _afterTrail is already contained inside of _visibleCount.
                int cacheCount = _beforeTrail + _visibleCount + ContainerCacheSize;
 
                if (cacheCount > 0)
                {
                    return _cacheStart + cacheCount - 1;
                }
                else
                {
                    return 0;
                }
            }
        }
  
        /// <summary>
        /// True after the first MeasureOverride call. We can't use UIElement.NeverMeasured because it's set to true by the first call to MeasureOverride.
        /// Stored in a bool field on Panel.
        /// </summary>
        private bool HasMeasured
        {
            get
            {
                return VSP_HasMeasured;
            }
            set
            {
                VSP_HasMeasured = value;
            }
        }
 
        private bool InRecyclingMode
        {
            get
            {
                return _virtualizationMode == VirtualizationMode.Recycling;
            }
        }
 
 
        internal bool IsScrolling
        {
            get { return (_scrollData != null) && (_scrollData._scrollOwner != null); }
        }
  
 
        /// <summary>
        /// Specifies if this panel uses item-based or pixel-based computations in Measure and Arrange.
        ///
        /// Differences between the two:
        ///
        /// When pixel-based mode VSP behaves the same to the layout system virtualized as not; its desired size is the sum
        /// of all its children and it arranges children such that the ones in view appear in the right place.
        /// In this mode VSP is also able to make use of the viewport passed down in MeasureData to virtualize chidren.  When
        /// it's the scrolling panel it computes the offset and extent in pixels rather than logical units.
        ///
        /// When in item mode VSP's desired size grows and shrinks depending on which containers are virtualized and it arranges
        /// all children one on top the the other.
        /// In this mode VSP cannot use the viewport from MeasureData to virtualize; it can only virtualize if it is the scrolling panel
        /// (IsScrolling == true).  Thus its looseness with desired size isn't much of an issue since it owns the extent.
        /// </summary>
        /// <remarks>
        /// This should be private, except that one Debug.Assert in TreeView requires it.
        /// </remarks>
        internal bool IsPixelBased
        {
            get
            {
                // For backwards compat we don't use pixel mode unless we're virtualzing a TreeView or TreeViewItem.  This should
                // be changed if we decide to later publicly expose the pixel-based viewport.
                Debug.Assert(VSP_IsPixelBased == false || IsVirtualizing && (ItemsControl.GetItemsOwner(this) is TreeView || ItemsControl.GetItemsOwner(this) is TreeViewItem));
  
                return VSP_IsPixelBased;
            }
            set
            {
                VSP_IsPixelBased = value;
            }
        }
  
  
        private bool IsVirtualizing
        {
            get
            {
                return VSP_IsVirtualizing;
            }
            set
            {
                // We must be the ItemsHost to turn on Virtualization.
                bool isVirtualizing = IsItemsHost && value;
  
                if (isVirtualizing == false)
                {
                    _realizedChildren = null;
                }
 
                VSP_IsVirtualizing = value;
            }
        }
  
 
        /// <summary>
        /// Returns the list of childen that have been realized by the Generator.
        /// We must use this method whenever we interact with the Generator's index.
        /// In recycling mode the Children collection also contains recycled containers and thus does
        /// not map to the Generator's list.
        /// </summary>
        private IList RealizedChildren
        {
            get
            {
                if (IsVirtualizing && InRecyclingMode)
                {
                    EnsureRealizedChildren();
                    return _realizedChildren;
                }
                else
                {
                    return InternalChildren;
                }
            }
        }
 
  
        private VirtualizationMode VirtualizationMode
        {
            get
            {
                return _virtualizationMode;
            }
            set
            {
                _virtualizationMode = value;
            }
        }
  
 
        #endregion Private Properties
 
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //-----------------------------------------------------
  
        #region Private Fields
 
        // Scrolling and virtualization data.  Only used when this is the scrolling panel (IsScrolling is true).
        // When VSP is in pixel mode _scrollData is in units of pixels.  Otherwise the units are logical.
        private ScrollData _scrollData;
  
        // Virtualization state
        private VirtualizationMode _virtualizationMode;
        private int _visibleStart;                  // index of of the first visible data item
        private int _visibleCount;                  // count of the number of data items visible in the viewport
        private int _cacheStart;                    // index of the first data item in the container cache.  This is always <= _visibleStart
 
        // UIElement collection index of the first visible child container.  This is NOT the data item index. If the first visible container
        // is the 3rd child in the visual tree and contains data item 312, _firstVisibleChildIndex will be 2, while _visibleStart is 312.
        // This is useful because could be several live containers in the collection offscreen (maybe we cleaned up lazily, they couldn't be virtualized, etc).
        // This actually maps directly to realized containers inside the Generator.  It's the index of the first visible realized container.
        // Note that when RecyclingMode is active this is the index into the _realizedChildren collection, not the Children collection.
        private int _firstVisibleChildIndex;
 
        // Used by the Recycling mode to maintain the list of actual realized children (a realized child is one that the ItemContainerGenerator has
        // generated).  We need a mapping between children in the UIElementCollection and realized containers in the generator.  In standard virtualization
        // mode these lists are identical; in recycling mode they are not. When a container is recycled the Generator removes it from its realized list, but
        // for perf reasons the panel keeps these containers in its UIElement collection.  This list is the actual realized children -- i.e. the InternalChildren
        // list minus all recycled containers.
        private List<uielement> _realizedChildren;
 
        // Cleanup
        private DispatcherOperation _cleanupOperation;
        private DispatcherTimer _cleanupDelay;
        private int _beforeTrail = 0;
        private int _afterTrail = 0;
        private const int FocusTrail = 5; // The maximum number of items off the edge we will generate to get a focused item (so that keyboard navigation can work)
        private DependencyObject _bringIntoViewContainer;  // pointer to the container we're about to bring into view; it can't be recycled even if it's offscreen.
  
        // ContainerCacheSize specifies how many items we cache past the viewport boundaries.  Until we expose an API to allow users to tweak this
        // the safest thing is to leave it at 0.
        private const int ContainerCacheSize = 0;
 
        // Global index used by ItemValueStorage to store the DesiredSize of a UIElement when it is a virtualized container.
        // Used by TreeView and TreeViewItem to remember the size of TreeViewItems when they get virtualized away.
        private static int _desiredSizeStorageIndex;
  
        // Holds the 'focus trail': the previous or next focusable item, neither of which can be virtualized.
        // Used only when virtualizing in a hierarchy (i.e. TreeView virtualization).
        private static UncommonField<weakreference[]> FocusTrailField = new UncommonField<weakreference[]>(null);
        private static UncommonField<bool> FocusTrailItemField = new UncommonField<bool>(false);
 
        #endregion Private Fields
  
 
        //------------------------------------------------------
        //
        //  Private Structures / Classes
        //
        //------------------------------------------------------
 
        #region Private Structures Classes
  
        //-----------------------------------------------------------
        // ScrollData class
        //------------------------------------------------------------
        #region ScrollData
  
        // Helper class to hold scrolling data.
        // This class exists to reduce working set when VirtualizingStackPanel is used outside a scrolling situation.
        // Standard "extra pointer always for less data sometimes" cache savings model:
        //      !Scroll [1xReference]
        //      Scroll  [1xReference] + [6xDouble + 1xReference]
        private class ScrollData
        {
            // Clears layout generated data.
            // Does not clear scrollOwner, because unless resetting due to a scrollOwner change, we won't get reattached.
            internal void ClearLayout()
            {
                _offset = new Vector();
                _viewport = _extent = _maxDesiredSize = new Size();
            }
  
            // For Stack/Flow, the two dimensions of properties are in different units:
            // 1. The "logically scrolling" dimension uses items as units.
            // 2. The other dimension physically scrolls.  Units are in Avalon pixels (1/96").
            internal bool _allowHorizontal;
            internal bool _allowVertical;
 
            // Scroll offset of content.  Positive corresponds to a visually upward offset.  Set by methods like LineUp, PageDown, etc.
            internal Vector _offset;
  
            // Computed offset based on _offset set by the IScrollInfo methods.  Set at the end of a successful Measure pass.
            // This is the offset used by Arrange and exposed externally.  Thus an offset set by PageDown via IScrollInfo isn't
            // reflected publicly (e.g. via the VerticalOffset property) until a Measure pass.
            internal Vector _computedOffset = new Vector(0,0);
            internal Size _viewport;            // ViewportSize is in {pixels x items} (or vice-versa).
            internal Size _extent;              // Extent is the total number of children (logical dimension) or physical size
            internal ScrollViewer _scrollOwner; // ScrollViewer to which we're attached.
 
            internal Size _maxDesiredSize;      // Hold onto the maximum desired size to avoid re-laying out the parent ScrollViewer.
        }
 
        #endregion ScrollData
 
 
        /// <summary>
        /// Allows pixel-based virtualization to ask an ItemsControl for the size of its header (if available)
        /// and a size estimate for its containers.  This is used for TreeView virtualization.
        ///
        /// </summary>
        internal interface IProvideStackingSize
        {
            double HeaderSize(bool isHorizontal);
            double EstimatedContainerSize(bool isHorizontal);
        }
  
        #endregion Private Structures Classes
    }
}
 
  
 
// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------
 
//#define Profiling
  
using MS.Internal;
using MS.Internal.Controls;
using MS.Utility;
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
  
namespace System.Windows.Controls
{
    /// <summary>
    /// VirtualizingStackPanel is used to arrange children into single line.
    /// </summary>
    public class VirtualizingStackPanel : VirtualizingPanel, IScrollInfo
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------
 
        #region Constructors
 
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VirtualizingStackPanel()
        {
        }
 
        static VirtualizingStackPanel()
        {
            lock (DependencyProperty.Synchronized)
            {
                _desiredSizeStorageIndex = DependencyProperty.GetUniqueGlobalIndex(null, null);
                DependencyProperty.RegisteredPropertyList.Add();
            }
        }
 
        #endregion Constructors
 
        //--------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------
  
        #region Public Methods
  
        //------------------------------------------------------------
        //  IScrollInfo Methods
        //------------------------------------------------------------
        #region IScrollInfo Methods
 
        /// <summary>
        ///     Scroll content by one line to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one line to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one line to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        ///     Scroll content by one line to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "line" means.
        /// </summary>
        public virtual void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }
 
        /// <summary>
        ///     Scroll content by one page to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }
 
        /// <summary>
        ///     Scroll content by one page to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }
 
        /// <summary>
        ///     Scroll content by one page to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of what "page" means.
        /// </summary>
        public virtual void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }
  
        /// <summary>
        ///     Scroll content by one page to the top.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - SystemParameters.WheelScrollLines * ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        ///     Scroll content by one page to the bottom.
        ///     Subclases can override this method and call SetVerticalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + SystemParameters.WheelScrollLines * ((Orientation == Orientation.Vertical && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the left.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - 3.0 * ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
  
        /// <summary>
        ///     Scroll content by one page to the right.
        ///     Subclases can override this method and call SetHorizontalOffset to change
        ///     the behavior of the mouse wheel increment.
        /// </summary>
        public virtual void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + 3.0 * ((Orientation == Orientation.Horizontal && !IsPixelBased) ? 1.0 : ScrollViewer._scrollLineDelta));
        }
 
        /// <summary>
        /// Set the HorizontalOffset to the passed value.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            EnsureScrollData();
  
            double scrollX = ScrollContentPresenter.ValidateInputOffset(offset, "HorizontalOffset");
            if (!DoubleUtil.AreClose(scrollX, _scrollData._offset.X))
            {
                Vector oldViewportOffset = _scrollData._offset;
 
                // Store the new offset
                _scrollData._offset.X = scrollX;
 
                // Report the change in offset
                OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);
 
                InvalidateMeasure();
            }
        }
 
        /// <summary>
        /// Set the VerticalOffset to the passed value.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            EnsureScrollData();
 
            double scrollY = ScrollContentPresenter.ValidateInputOffset(offset, "VerticalOffset");
            if (!DoubleUtil.AreClose(scrollY, _scrollData._offset.Y))
            {
                Vector oldViewportOffset = _scrollData._offset;
  
                // Store the new offset
                _scrollData._offset.Y = scrollY;
  
                // Report the change in offset
                OnViewportOffsetChanged(oldViewportOffset, _scrollData._offset);
 
                InvalidateMeasure();
            }
        }
  
        /// <summary>
        /// VirtualizingStackPanel implementation of <seealso cref="IScrollInfo.MakeVisible">.
        /// </seealso></summary>
        // The goal is to change offsets to bring the child into view, and return a rectangle in our space to make visible.
        // The rectangle we return is in the physical dimension the input target rect transformed into our pace.
        // In the logical dimension, it is our immediate child's rect.
        // Note: This code presently assumes we/children are layout clean.  See work item 22269 for more detail.
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            Vector newOffset = new Vector();
            Rect newRect = new Rect();
            Rect originalRect = rectangle;
            bool isHorizontal = (Orientation == Orientation.Horizontal);
 
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (    rectangle.IsEmpty
                || visual == null
                || visual == (Visual)this
                ||  !this.IsAncestorOf(visual))
            {
                return Rect.Empty;
            }
  
#pragma warning disable 1634, 1691
#pragma warning disable 56506
            // Compute the child's rect relative to (0,0) in our coordinate space.
            // This is a false positive by PreSharp. visual cannot be null because of the 'if' check above
            GeneralTransform childTransform = visual.TransformToAncestor(this);
#pragma warning restore 56506
#pragma warning restore 1634, 1691
            rectangle = childTransform.TransformBounds(rectangle);
  
            // We can't do any work unless we're scrolling.
            if (!IsScrolling)
            {
                return rectangle;
            }
 
            // Make ourselves visible in the non-stacking direction
            MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, !isHorizontal);
  
            if (IsPixelBased)
            {
                MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect, isHorizontal);
            }
            else
            {
                // Bring our child containing the visual into view.
                // For non-pixel based scrolling the offset is in logical units in the stacking direction
                // and physical units in the other. Hence the logical helper call here.
                int childIndex = FindChildIndexThatParentsVisual(visual);
                MakeVisibleLogicalHelper(childIndex, rectangle, ref newOffset, ref newRect);
            }
 
            // We have computed the scrolling offsets; validate and scroll to them.
            newOffset.X = ScrollContentPresenter.CoerceOffset(newOffset.X, _scrollData._extent.Width, _scrollData._viewport.Width);
            newOffset.Y = ScrollContentPresenter.CoerceOffset(newOffset.Y, _scrollData._extent.Height, _scrollData._viewport.Height);
 
            if (!DoubleUtil.AreClose(newOffset, _scrollData._offset))
            {
                Vector oldOffset = _scrollData._offset;
                _scrollData._offset = newOffset;
 
                OnViewportOffsetChanged(oldOffset, newOffset);
 
                InvalidateMeasure();
                OnScrollChange();
                if (ScrollOwner != null)
                {
                    // When layout gets updated it may happen that visual is obscured by a ScrollBar
                    // We call MakeVisible again to make sure element is visible in this case
                    ScrollOwner.MakeVisible(visual, originalRect);
                }
 
                _bringIntoViewContainer = null;
            }
  
            // Return the rectangle
            return newRect;
        }
 
        /// <summary>
        /// Generates the item at the specified index and calls BringIntoView on it.
        /// </summary>
        /// <param name="index">Specify the item index that should become visible
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if index is out of range
        /// </exception>
        protected internal override void BringIndexIntoView(int index)
        {
            if (index < 0 || index >= ItemCount)
                throw new ArgumentOutOfRangeException("index");
 
            IItemContainerGenerator generator = Generator;
            int childIndex;
            bool visualOrderChanged = false;
            GeneratorPosition position = IndexToGeneratorPositionForStart(index, out childIndex);
            using (generator.StartAt(position, GeneratorDirection.Forward, true))
            {
                bool newlyRealized;
                UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                if (child != null)
                {
  
                    visualOrderChanged = AddContainerFromGenerator(childIndex, child, newlyRealized);
  
                    if (visualOrderChanged)
                    {
                        Debug.Assert(IsVirtualizing && InRecyclingMode, "We should only modify the visual order when in recycling mode");
                        InvalidateZState();
                    }
 
                    FrameworkElement element = child as FrameworkElement;
                    if (element != null)
                    {
                        element.BringIntoView();
                        _bringIntoViewContainer = element;
                    }
                }
            }
        }
  
        #endregion
  
        #endregion
 
        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //--------------------------------------------------------------------
 
        #region Public Properties
  
        /// <summary>
        /// Specifies dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
  
        /// <summary>
        /// This property is always true because this panel has vertical or horizontal orientation
        /// </summary>
        protected internal override bool HasLogicalOrientation
        {
            get { return true; }
        }
  
        /// <summary>
        ///     Orientation of the panel if its layout is in one dimension.
        /// Otherwise HasLogicalOrientation is false and LogicalOrientation should be ignored
        /// </summary>
        protected internal override Orientation LogicalOrientation
        {
            get { return this.Orientation; }
        }
  
        /// <summary>
        /// DependencyProperty for <see cref="Orientation"> property.
        /// </see></summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(Orientation.Vertical,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(OnOrientationChanged)),
                new ValidateValueCallback(ScrollBar.IsValidOrientation));
  
        /// <summary>
        ///     Attached property for use on the ItemsControl that is the host for the items being
        ///     presented by this panel. Use this property to turn virtualization on/off.
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(true));
  
        /// <summary>
        ///     Retrieves the value for <see cref="IsVirtualizingProperty">.
        /// </see></summary>
        /// <param name="o">The object on which to query the value.
        /// <returns>True if virtualizing, false otherwise.</returns>
        public static bool GetIsVirtualizing(DependencyObject o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
  
            return (bool)o.GetValue(IsVirtualizingProperty);
        }
 
        /// <summary>
        ///     Sets the value for <see cref="IsVirtualizingProperty">.
        /// </see></summary>
        /// <param name="element">The element on which to set the value.
        /// <param name="value">True if virtualizing, false otherwise.
        public static void SetIsVirtualizing(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
  
            element.SetValue(IsVirtualizingProperty, value);
        }
  
 
 
        /// <summary>
        ///     Attached property for use on the ItemsControl that is the host for the items being
        ///     presented by this panel. Use this property to modify the virtualization mode.
        ///
        ///     Note that this property can only be set before the panel has been initialized
        /// </summary>
        public static readonly DependencyProperty VirtualizationModeProperty =
            DependencyProperty.RegisterAttached("VirtualizationMode", typeof(VirtualizationMode), typeof(VirtualizingStackPanel),
                new FrameworkPropertyMetadata(VirtualizationMode.Standard));
 
        /// <summary>
        ///     Retrieves the value for <see cref="VirtualizationModeProperty">.
        /// </see></summary>
        /// <param name="o">The object on which to query the value.
        /// <returns>The current virtualization mode.</returns>
        public static VirtualizationMode GetVirtualizationMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
  
            return (VirtualizationMode)element.GetValue(VirtualizationModeProperty);
        }
  
        /// <summary>
        ///     Sets the value for <see cref="VirtualizationModeProperty">.
        /// </see></summary>
        /// <param name="element">The element on which to set the value.
        /// <param name="value">The desired virtualization mode.
        public static void SetVirtualizationMode(DependencyObject element, VirtualizationMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
 
            element.SetValue(VirtualizationModeProperty, value);
        }
  
  
        //-----------------------------------------------------------
        //  IScrollInfo Properties
        //-----------------------------------------------------------
        #region IScrollInfo Properties
 
        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing its child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanHorizontallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowHorizontal;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowHorizontal != value)
                {
                    _scrollData._allowHorizontal = value;
                    InvalidateMeasure();
                }
            }
        }
 
        /// <summary>
        /// VirtualizingStackPanel reacts to this property by changing its child measurement algorithm.
        /// If scrolling in a dimension, infinite space is allowed the child; otherwise, available size is preserved.
        /// </summary>
        [DefaultValue(false)]
        public bool CanVerticallyScroll
        {
            get
            {
                if (_scrollData == null) { return false; }
                return _scrollData._allowVertical;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._allowVertical != value)
                {
                    _scrollData._allowVertical = value;
                    InvalidateMeasure();
                }
            }
        }
  
        /// <summary>
        /// ExtentWidth contains the horizontal size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Width;
            }
        }
  
        /// <summary>
        /// ExtentHeight contains the vertical size of the scrolled content element in 1/96"
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._extent.Height;
            }
        }
 
        /// <summary>
        /// ViewportWidth contains the horizontal size of content's visible range in 1/96"
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Width;
            }
        }
  
        /// <summary>
        /// ViewportHeight contains the vertical size of content's visible range in 1/96"
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._viewport.Height;
            }
        }
  
        /// <summary>
        /// HorizontalOffset is the horizontal offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double HorizontalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.X;
            }
        }
 
        /// <summary>
        /// VerticalOffset is the vertical offset of the scrolled content in 1/96".
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double VerticalOffset
        {
            get
            {
                if (_scrollData == null) { return 0.0; }
                return _scrollData._computedOffset.Y;
            }
        }
  
        /// <summary>
        /// ScrollOwner is the container that controls any scrollbars, headers, etc... that are dependant
        /// on this IScrollInfo's properties.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollViewer ScrollOwner
        {
            get
            {
                EnsureScrollData();
                return _scrollData._scrollOwner;
            }
            set
            {
                EnsureScrollData();
                if (value != _scrollData._scrollOwner)
                {
                    ResetScrolling(this);
                    _scrollData._scrollOwner = value;
                }
            }
        }
 
        #endregion IScrollInfo Properties
  
        #endregion Public Properties
  
        //-------------------------------------------------------------------
        //
        //  Public Events
        //
        //--------------------------------------------------------------------
 
  
        #region Public Events
  
        /// <summary>
        ///     Called on the ItemsControl that owns this panel when an item is being re-virtualized.
        /// </summary>
        public static readonly RoutedEvent CleanUpVirtualizedItemEvent = EventManager.RegisterRoutedEvent("CleanUpVirtualizedItemEvent", RoutingStrategy.Direct, typeof(CleanUpVirtualizedItemEventHandler), typeof(VirtualizingStackPanel));
 
 
        /// <summary>
        ///     Adds a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event
        /// <param name="handler">Event Handler to be added
        public static void AddCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        {
            FrameworkElement.AddHandler(element, CleanUpVirtualizedItemEvent, handler);
        }
  
        /// <summary>
        ///     Removes a handler for the CleanUpVirtualizedItem attached event
        /// </summary>
        /// <param name="element">DependencyObject that listens to this event
        /// <param name="handler">Event Handler to be removed
        public static void RemoveCleanUpVirtualizedItemHandler(DependencyObject element, CleanUpVirtualizedItemEventHandler handler)
        {
            FrameworkElement.RemoveHandler(element, CleanUpVirtualizedItemEvent, handler);
        }
 
        /// <summary>
        ///     Called when an item is being re-virtualized.
        /// </summary>
        protected virtual void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (itemsControl != null)
            {
                itemsControl.RaiseEvent(e);
            }
        }
  
        #endregion
 
        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //--------------------------------------------------------------------
 
        #region Protected Methods
 
        /// <summary>
        /// General VirtualizingStackPanel layout behavior is to grow unbounded in the "stacking" direction (Size To Content).
        /// Children in this dimension are encouraged to be as large as they like.  In the other dimension,
        /// VirtualizingStackPanel will assume the maximum size of its children.
        /// </summary>
        /// <remarks>
        /// When scrolling, VirtualizingStackPanel will not grow in layout size but effectively add the children on a z-plane which
        /// will probably be clipped by some parent (typically a ScrollContentPresenter) to Stack's size.
        /// </remarks>
        /// <param name="constraint">Constraint
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if Profiling
            if (Panel.IsAboutToGenerateContent(this))
                return MeasureOverrideProfileStub(constraint);
            else
                return RealMeasureOverride(constraint);
        }
 
        // this is a handy place to start/stop profiling
        private Size MeasureOverrideProfileStub(Size constraint)
        {
            return RealMeasureOverride(constraint);
        }
  
        private Size RealMeasureOverride(Size constraint)
        {
#endif
            bool etwTracingEnabled = IsScrolling && EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal);
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.StartEvent, "VirtualizingStackPanel :MeasureOverride");
            }
  
            Debug.Assert(MeasureData == null || constraint == MeasureData.AvailableSize, "MeasureData needs to be passed down in [....] with size");
 
            MeasureData measureData = MeasureData;
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstViewport;                              // First child index in the viewport.
            double firstItemOffset;                         // Offset of the top of the first child relative to the top of the viewport.
            double virtualizedItemsSize = 0d;               // Amount that virtualized children contribute to the desired size in the stacking direction
            int lastViewport = -1;                          // Last child index in the viewport.  -1 indicates we have not yet iterated through the last child.
            double logicalVisibleSpace, childLogicalSize;
            Rect originalViewport = Rect.Empty;             // Only used if this is the scrolling panel.  Saves off the given viewport for scroll computations.
  
            // Collect information from the ItemsControl, if there is one.
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = (itemsControl != null) ? itemsControl.Items.Count : 0;
            SetVirtualizationState(itemsControl, /* hasMeasureData = */ measureData != null && measureData.HasViewport);
  
            IList children = RealizedChildren;  // yes, this is weird, but this property ensures the Generator is properly initialized.
            IItemContainerGenerator generator = Generator;
 
            // Adjust the viewport
            if (IsPixelBased)
            {
                if (IsScrolling)
                {
  
                    // We're the top level scrolling panel.  Set the viewport and extend it to add a focus trail
                    originalViewport = new Rect(_scrollData._offset.X, _scrollData._offset.Y, constraint.Width, constraint.Height);
                    measureData = new MeasureData(constraint, originalViewport);
  
                    // The way we have a focus trail when pixel-based is to artificially extend the viewport.  All calculations are done
                    // with this 'artificial' viewport with the exception of the scroll offset, extent, etc.
                    measureData = AddFocusTrail(measureData, fHorizontal);
                    Debug.Assert(!object.ReferenceEquals(originalViewport, measureData.Viewport), "original viewport should not have a focus trail");
                }
                else
                {
                    measureData = AdjustViewportOffset(measureData, itemsControl, fHorizontal);
                    Debug.Assert(!object.ReferenceEquals(MeasureData, measureData), "The value set in the MeasureData property should not be modified");
                }
            }
  
 
            //
            // Initialize child sizing and iterator data
            // Allow children as much size as they want along the stack.
            //
            if (fHorizontal)
            {
                layoutSlotSize.Width = Double.PositiveInfinity;
                if (IsScrolling && CanVerticallyScroll) { layoutSlotSize.Height = Double.PositiveInfinity; }
                logicalVisibleSpace = constraint.Width;
            }
            else
            {
                layoutSlotSize.Height = Double.PositiveInfinity;
                if (IsScrolling && CanHorizontallyScroll) { layoutSlotSize.Width = Double.PositiveInfinity; }
                logicalVisibleSpace = constraint.Height;
            }
  
            // Compute index of first item in the viewport
            firstViewport = ComputeIndexOfFirstVisibleItem(measureData, itemsControl, fHorizontal, out firstItemOffset);
 
            if (IsPixelBased)
            {
                // Acount for the size of items that won't be generated
                Debug.Assert(stackDesiredSize.Width == 0 && stackDesiredSize.Height == 0, "stack desired size must be 0 for virtualizedItemsSize to work");
                stackDesiredSize = ExtendDesiredSize(itemsControl, stackDesiredSize, firstViewport, /*before = */ true, fHorizontal);
  
                virtualizedItemsSize = fHorizontal ? stackDesiredSize.Width : stackDesiredSize.Height;
            }
 
 
            debug_AssertRealizedChildrenEqualVisualChildren();
  
            // If recycling clean up before generating children.
            if (IsVirtualizing && InRecyclingMode)
            {
                CleanupContainers(firstViewport, itemsControl);
                debug_VerifyRealizedChildren();
            }
 
            //
            // Figure out the position of the first visible item
            //
            GeneratorPosition startPos = IndexToGeneratorPositionForStart(IsVirtualizing ? firstViewport : 0, out _firstVisibleChildIndex);
            int childIndex = _firstVisibleChildIndex;
 
            //
            // Main loop: generate and measure all children (or all visible children if virtualizing).
            //
            bool ranOutOfItems = true;
            bool visualOrderChanged = false;
            _visibleCount = 0;
            if (itemCount > 0)
            {
                _afterTrail = 0;
                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    for (int i = IsVirtualizing ? firstViewport : 0, count = itemCount; i < count; ++i)
                    {
                        // Get next child.
                        bool newlyRealized;
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
  
                        if (child == null)
                        {
                            Debug.Assert(!newlyRealized, "The generator realized a null value.");
 
                            // We reached the end of the items (because of a group)
                            break;
                        }
  
                        visualOrderChanged |= AddContainerFromGenerator(childIndex, child, newlyRealized);
 
                        childIndex++;
                        _visibleCount++;
 
                        if (IsPixelBased)
                        {
                            // Pass along MeasureData so it continues down the tree.
                            child.MeasureData = CreateChildMeasureData(measureData, layoutSlotSize, stackDesiredSize, fHorizontal);
                        }
 
                        Size childDesiredSize = child.DesiredSize;
                        child.Measure(layoutSlotSize);
 
                        if (childDesiredSize != child.DesiredSize)
                        {
                            childDesiredSize = child.DesiredSize;
  
                            // Reset the _maxDesiredSize cache if child DesiredSize changes
                            if (_scrollData != null)
                                _scrollData._maxDesiredSize = new Size();
                        }
 
 
                        // Accumulate child size.
                        if (fHorizontal)
                        {
                            stackDesiredSize.Width += childDesiredSize.Width;
                            stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                            childLogicalSize = childDesiredSize.Width;
                        }
                        else
                        {
                            stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                            stackDesiredSize.Height += childDesiredSize.Height;
                            childLogicalSize = childDesiredSize.Height;
                        }
 
  
 
                        // Adjust remaining viewport space if we are scrolling and within the viewport region.
                        // While scrolling (not virtualizing), we always measure children before and after the viewport.
                        if (IsScrolling && lastViewport == -1 && i >= firstViewport)
                        {
                            logicalVisibleSpace -= childLogicalSize;
                            if (DoubleUtil.LessThanOrClose(logicalVisibleSpace, 0.0))
                            {
                                lastViewport = i;
                            }
                        }
 
  
                        // When under a viewport, virtualizing and at or beyond the first element, stop creating elements when out of space.
                        if (IsVirtualizing && (i >= firstViewport))
                        {
                            double viewportSize;
                            double totalGenerated;
 
                            //
                            // Decide if the end of the item is outside the viewport.
                            //
                            // StackDesiredSize, with some adjustment, is a measure of exactly how much viewport space we have used.
                            //
                            // StackDesiredSize is the sum of all generated children (starting with the first visible item).  The first
                            // visible item doesn't always start at the top of the viewport, so we have to adjust by the firstItemoffset.
                            //
                            // When pixel-based we add the sum of all virtualized children to the stackDesiredSize; this has to be removed as well.
                            //
                            Debug.Assert(IsPixelBased || virtualizedItemsSize == 0d);
  
                            if (fHorizontal)
                            {
                                viewportSize = IsPixelBased ? measureData.Viewport.Width : constraint.Width;
                                totalGenerated = stackDesiredSize.Width - virtualizedItemsSize + firstItemOffset;
  
                            }
                            else
                            {
                                viewportSize = IsPixelBased ? measureData.Viewport.Height : constraint.Height;
                                totalGenerated = stackDesiredSize.Height - virtualizedItemsSize + firstItemOffset;
                            }
  
 
                            if (totalGenerated > viewportSize)
                            {
                                // The end of this child is outside the viewport.  Check if we want to generate some more.
 
                                if (IsPixelBased)
                                {
                                    // For pixel-based virtualization (specifically TreeView virtualization) we deal with
                                    // the after trail later, since it has to function hierarchically.
                                    break;
                                }
                                else
                                {
                                    // We want to keep a focusable item after the end so that keyboard navigation
                                    // can work, but we want to limit that to FocusTrail number of items
                                    // in case all the items are not focusable.
                                    if (_afterTrail > 0 && ( _afterTrail >= FocusTrail || Keyboard.IsFocusable(child)))
                                    {
                                        // Either we passed the limit or the child was focusable
                                        ranOutOfItems = false;
                                        break;
                                    }
 
                                    _afterTrail++;
                                    // Loop around and generate another item
                                }
                            }
                        }
                    }
                }
            }
 
#if DEBUG
            if (IsVirtualizing && InRecyclingMode)
            {
                debug_VerifyRealizedChildren();
            }
#endif
 
            _visibleStart = firstViewport;
 
  
            if (IsPixelBased)
            {
                // Acount for the size of items that won't be generated
                stackDesiredSize = ExtendDesiredSize(itemsControl, stackDesiredSize, firstViewport + _visibleCount, /*before = */ false, fHorizontal);
            }
 
 
            //
            // Adjust the scroll offset, extent, etc.
            //
            if (IsScrolling)
            {
                if (IsPixelBased)
                {
 
                    Vector offset = new Vector(originalViewport.Location.X, originalViewport.Location.Y);
                    SetAndVerifyScrollingData(originalViewport.Size, stackDesiredSize, offset);
                }
                else
                {
                    // Compute the extent before we fill remaining space and modify the stack desired size
                    Size extent = ComputeLogicalExtent(stackDesiredSize, itemCount, fHorizontal);
  
                    if (ranOutOfItems)
                    {
                        // If we or children have resized, it's possible that we can now display more content.
                        // This is true if we started at a nonzero offeset and still have space remaining.
                        // In this case, we loop back through previous children until we run out of space.
                        FillRemainingSpace(ref firstViewport, ref logicalVisibleSpace, ref stackDesiredSize, layoutSlotSize, fHorizontal);
                    }
 
                    // Create the Before focus trail
                    // NOTE: the call here (under IsScrolling) implicitly assumes that only a scrolling panel can virtualize and thus requires
                    // a focus trail.  That's not true for hierarchical (pixel-based) virtualization, but it handles the focus trail differently anyway.
                    EnsureTopCapGenerated(layoutSlotSize);
  
                    // Compute Scrolling data such as extent, viewport, and offset.
                    stackDesiredSize = UpdateLogicalScrollData(stackDesiredSize, constraint, logicalVisibleSpace,
                                                               extent, firstViewport, lastViewport, itemCount, fHorizontal);
                }
            }
 
            //
            // Cleanup items no longer in the viewport
            //
            if (IsVirtualizing && !InRecyclingMode)
            {
                if (IsPixelBased)
                {
                    // Immediate cleanup
                    CleanupContainers(firstViewport, itemsControl);
                }
                else
                {
                    // Less aggressive backwards-compat background cleanup operation
                    EnsureCleanupOperation(false /* delay */);
                }
            }
  
 
            if (IsVirtualizing && InRecyclingMode)
            {
                DisconnectRecycledContainers();
 
                if (visualOrderChanged)
                {
                    // We moved some containers in the visual tree without firing changed events.  ZOrder is now invalid.
                    InvalidateZState();
                }
            }
 
  
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.EndEvent, "VirtualizingStackPanel :MeasureOverride");
            }
  
            debug_AssertRealizedChildrenEqualVisualChildren();
 
            return stackDesiredSize;
        }
 
  
  
        /// <summary>
        /// Content arrangement.
        /// </summary>
        /// <param name="arrangeSize">Arrange size
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            Rect rcChild = new Rect(arrangeSize);
  
            IList children;
            double previousChildSize = 0.0;
            ItemsControl itemsControl = null;
            bool childrenAreContainers = true;
 
            bool etwTracingEnabled = IsScrolling && EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal);
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.StartEvent, "VirtualizingStackPanel :ArrangeOverride");
            }
  
            //
            // Compute scroll offset and seed it into rcChild.
            //
            if (IsScrolling)
            {
                if (fHorizontal)
                {
                    double offsetX = _scrollData._computedOffset.X;
                    rcChild.X = IsPixelBased ? -offsetX : ComputePhysicalFromLogicalOffset(IsVirtualizing ? _firstVisibleChildIndex : offsetX, true);
                    rcChild.Y = -1.0 * _scrollData._computedOffset.Y;
                }
                else
                {
                    double offsetY = _scrollData._computedOffset.Y;
                    rcChild.X = -1.0 * _scrollData._computedOffset.X;
                    rcChild.Y = IsPixelBased ? -offsetY : ComputePhysicalFromLogicalOffset(IsVirtualizing ? _firstVisibleChildIndex : offsetY, false);
                }
            }
 
            //
            // Arrange and Position Children.
            //
            // If we're virtualizing and pixel-based we loop through the entire items collection (the policy is to arrange items exactly where they
            // should appear regardless of the virtualization state of siblings).  This is required to properly virtualize hiearchically.
            // Otherwise we loop through the children collection (when virtualizing in items mode VSP arranges children in a simple stack order).
            //
  
            if (IsPixelBased && IsVirtualizing)
            {
                // This is a pixel-based internal panel.  It must behave externally exactly the way a non-virtualizing panel does in Arrange.
                // Specifically, it arranges its children in the 'proper' place, regardless of whether or not their siblings are virtualized.
 
                itemsControl = ItemsControl.GetItemsOwner(this);
                children = itemsControl.Items;
                childrenAreContainers = false;
            }
            else
            {
                debug_AssertRealizedChildrenEqualVisualChildren();  // RealizedChildren only differs from InternalChildren inside of Measure when container recycling is on.
                children = RealizedChildren;
            }
  
  
            for (int i = 0; i < children.Count; ++i)
            {
                UIElement container = null;
                Size childSize;
 
                if (childrenAreContainers)
                {
                    // we are looping through the actual containers; the visual children of this panel.
                    container = (UIElement)children[i];
                    childSize = container.DesiredSize;
                }
                else
                {
                    // We are looping through items and may or may not have a container for each given item.
                    childSize = ContainerSizeForItem(itemsControl, children[i], i, out container);
                }
  
  
                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = childSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, childSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = childSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(arrangeSize.Width, childSize.Width);
                }
  
                if (container != null)
                {
                    container.Arrange(rcChild);
                }
            }
 
 
            if (etwTracingEnabled)
            {
                EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.GENERICSTRINGGUID), MS.Utility.EventType.EndEvent, "VirtualizingStackPanel :ArrangeOverride");
            }
  
            return arrangeSize;
        }
 
 
        /// <summary>
        ///     Called when the Items collection associated with the containing ItemsControl changes.
        /// </summary>
        /// <param name="sender">sender
        /// <param name="args">Event arguments
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
 
            bool resetMaximumDesiredSize = false;
  
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    OnItemsRemove(args);
                    resetMaximumDesiredSize = true;
                    break;
 
                case NotifyCollectionChangedAction.Replace:
                    OnItemsReplace(args);
                    resetMaximumDesiredSize = true;
                    break;
  
                case NotifyCollectionChangedAction.Move:
                    OnItemsMove(args);
                    break;
 
                case NotifyCollectionChangedAction.Reset:
                    resetMaximumDesiredSize = true;
                    break;
            }
  
            if (resetMaximumDesiredSize && IsScrolling)
            {
                // The items changed such that the maximum size may no longer be valid.
                // The next layout pass will update this value.
                _scrollData._maxDesiredSize = new Size();
            }
 
        }
  
        /// <summary>
        ///     Called when the UI collection of children is cleared by the base Panel class.
        /// </summary>
        protected override void OnClearChildren()
        {
            base.OnClearChildren();
            _realizedChildren = null;
            _visibleStart = _firstVisibleChildIndex = _visibleCount = 0;
        }
 
        // Override of OnGotKeyboardFocus.  Called when focus moves to any child or subchild of this VSP
        // Used by TreeView virtualization to keep track of the focused item.
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            FocusChanged(e);
        }
  
        // Override of OnLostKeyboardFocus.  Called when focus moves away from this VSP.
        // Used by TreeView virtualization to keep track of the focused item.
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            FocusChanged(e);
        }
  
  
 
        #endregion Protected Methods
 
        #region Internal Methods
 
        // Tells the Generator to clear out all containers for this ItemsControl.  This is called by the ItemValueStorage
        // service when the ItemsControl this panel is a host for is about to be thrown away.  This allows the VSP to save
        // off any properties it is interested in and results in a call to ClearContainerForItem on the ItemsControl, allowing
        // the Item Container Storage to do so as well.
 
        // Note: A possible perf improvement may be to make 'fast' RemoveAll on the Generator that simply calls ClearContainerForItem
        // for us without walking through its data structures to actually clean out items.
        internal void ClearAllContainers(ItemsControl itemsControl)
        {
            Debug.Assert(IsVirtualizing,
                         "We should only clear containers for ItemsControls that are virtualizing");
  
            Debug.Assert(itemsControl == ItemsControl.GetItemsOwner(this),
                        "We can only clear containers that this panel is a host for");
  
            IItemContainerGenerator generator = Generator;
 
            if (IsPixelBased)
            {
                IList children = RealizedChildren;
                UIElement child;
  
                for (int i = 0; i < children.Count; i++)
                {
                    child = (UIElement)children[i];
                    itemsControl.StoreItemValue(((ItemContainerGenerator)generator).ItemFromContainer(child), child.DesiredSize, _desiredSizeStorageIndex);
                }
  
            }
  
            if (generator != null)
            {
                generator.RemoveAll();
            }
        }
 
        #endregion
 
        //------------------------------------------------------
        //
        //  Private Methods
        //
        //-----------------------------------------------------
 
        #region Private Methods
  
 
        //
        // MeasureOverride Helpers
        //
  
        #region MeasureOverride Helpers
 
        /// <summary>
        /// Extends the viewport of the given MeasureData to give a focus trail.  Returns by how much it extended the viewport.
        /// </summary>
        /// <param name="childData">
        /// <returns></returns>
        private MeasureData AddFocusTrail(MeasureData measureData, bool isHorizontal)
        {
            //
            // Create the before / after focus trail for interior panels that use MeasureData's viewport to virtualize.
            // We expand the viewport so that roughly two extra items are generated at the top and the bottom.
            //
            // For the before / after focus trail good values are
            //  padding = header height * 4;
            //
            // To make page up / down work without rewriting TreeView's algorithm we actually extend the viewport one extra page
            // top and bottm.
            //
            Debug.Assert(IsScrolling, "The scrolling panel is the only one that should extend the viewport");
            Invariant.Assert(IsPixelBased, "If we're sending down a viewport to the children we should be doing pixel-based computations");
  
            double page = isHorizontal ? ViewportWidth : ViewportHeight;
            Rect viewport = measureData.Viewport;
  
            if (isHorizontal)
            {
                viewport.Width += page * 2;
                viewport.X -= page;
            }
            else
            {
                viewport.Height += page * 2;
                viewport.Y -= page;
            }
  
            measureData.Viewport = viewport;
            return measureData;
        }
  
        #region Scroll Computation Helpers
  
        /// <summary>
        /// Returns the extent in logical units in the stacking direction.
        /// </summary>
        /// <param name="stackDesiredSize">
        /// <param name="itemCount">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private Size ComputeLogicalExtent(Size stackDesiredSize, int itemCount, bool isHorizontal)
        {
            bool accumulateExtent = false;
            Size extent = new Size();
  
            if (ScrollOwner != null)
            {
                accumulateExtent = ScrollOwner.InChildInvalidateMeasure;
                ScrollOwner.InChildInvalidateMeasure = false;
            }
  
            if (isHorizontal)
            {
                extent.Width = itemCount;
                extent.Height = accumulateExtent ? Math.Max(stackDesiredSize.Height, _scrollData._extent.Height) : stackDesiredSize.Height;
            }
            else
            {
                extent.Width = accumulateExtent ? Math.Max(stackDesiredSize.Width, _scrollData._extent.Width) : stackDesiredSize.Width;
                extent.Height = itemCount;
            }
 
            return extent;
        }
 
 
  
        /// <summary>
        /// Called when we ran out of children before filling up the viewport.
        /// </summary>
        private void FillRemainingSpace(ref int firstViewport, ref double logicalVisibleSpace, ref Size stackDesiredSize, Size layoutSlotSize, bool isHorizontal)
        {
            Debug.Assert(IsScrolling, "Only the scrolling panel can fill remaining space");
            Debug.Assert(!IsPixelBased, "This is a logical operation");
 
            double projectedLogicalVisibleSpace;
            Size childDesiredSize;
            IList children = RealizedChildren;
            int childIndex = IsVirtualizing ? _firstVisibleChildIndex : firstViewport;
 
            while (childIndex > 0)
            {
                if (!PreviousChildIsGenerated(childIndex))
                {
                    GeneratePreviousChild(childIndex, layoutSlotSize);
                    childIndex++; // We just inserted a child, so increment the index
                }
                else if (childIndex <= _firstVisibleChildIndex)
                {
                    ((UIElement)children[childIndex - 1]).Measure(layoutSlotSize);
                }
 
                projectedLogicalVisibleSpace = logicalVisibleSpace;
  
                childDesiredSize = ((UIElement)children[childIndex - 1]).DesiredSize;
  
                if (isHorizontal)
                {
                    projectedLogicalVisibleSpace -= childDesiredSize.Width;
                }
                else
                {
                    projectedLogicalVisibleSpace -= childDesiredSize.Height;
                }
  
                // If we have run out of room, break.
                if (DoubleUtil.LessThan(projectedLogicalVisibleSpace, 0.0)) { break; }
  
                // Account for the child in the panel's desired size
                if (isHorizontal)
                {
                    stackDesiredSize.Width += childDesiredSize.Width;
                    stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                }
                else
                {
                    stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                    stackDesiredSize.Height += childDesiredSize.Height;
                }
 
                // Adjust viewport
                childIndex--;
                logicalVisibleSpace = projectedLogicalVisibleSpace;
                _visibleCount++;
            }
            if ((childIndex < _firstVisibleChildIndex) || !IsVirtualizing)
            {
                _firstVisibleChildIndex = childIndex;
            }
  
            _visibleStart = firstViewport = (IsItemsHost && children.Count != 0) ? GetGeneratedIndex(_firstVisibleChildIndex) : 0;
        }
  
 
        /// <summary>
        /// Updates ScrollData's offset, extent, and viewport in logical units.
        /// </summary>
        /// <param name="stackDesiredSize">
        /// <param name="constraint">
        /// <param name="logicalVisibleSpace">
        /// <param name="extent">
        /// <param name="firstViewport">
        /// <param name="lastViewport">
        /// <param name="itemCount">
        /// <param name="fHorizontal">
        /// <returns></returns>
        private Size UpdateLogicalScrollData(Size stackDesiredSize, Size constraint, double logicalVisibleSpace, Size extent,
                                             int firstViewport, int lastViewport, int itemCount, bool fHorizontal)
        {
            Debug.Assert(IsScrolling && !IsPixelBased, "this computes logical scroll data");
  
            Size viewport = constraint;
            Vector offset = _scrollData._offset;
 
            // If we have not yet set the last child in the viewport, set it to the last child.
            if (lastViewport == -1) { lastViewport = itemCount - 1; }
  
            int logicalExtent = itemCount;
            int logicalViewport = lastViewport - firstViewport;
  
            //
            // Compute the logical viewport size.
            //
 
            // We are conservative when estimating a viewport, not including the last element in case it is only partially visible.
            // We want to count it if it is fully visible (>= 0 space remaining) or the only element in the viewport.
            if (logicalViewport == 0 || DoubleUtil.GreaterThanOrClose(logicalVisibleSpace, 0.0)) { logicalViewport++; }
  
            if (fHorizontal)
            {
                viewport.Width = logicalViewport;
                offset.X = firstViewport;
                offset.Y = Math.Max(0, Math.Min(offset.Y, extent.Height - viewport.Height));
 
                // In case last item is visible because we scroll all the way to the right and scrolling is on
                // we want desired size not to be smaller than constraint to avoid another relayout
                if (logicalExtent > logicalViewport && !Double.IsPositiveInfinity(constraint.Width))
                {
                    stackDesiredSize.Width = constraint.Width;
                }
            }
            else
            {
                viewport.Height = logicalViewport;
                offset.Y = firstViewport;
                offset.X = Math.Max(0, Math.Min(offset.X, extent.Width - viewport.Width));
  
                // In case last item is visible because we scroll all the way to the bottom and scrolling is on
                // we want desired size not to be smaller than constraint to avoid another relayout
                if (logicalExtent > logicalViewport && !Double.IsPositiveInfinity(constraint.Height))
                {
                    stackDesiredSize.Height = constraint.Height;
                }
            }
  
  
 
            // Since we can offset and clip our content, we never need to be larger than the parent suggestion.
            // If we returned the full size of the content, we would always be so big we didn't need to scroll.  :)
            stackDesiredSize.Width = Math.Min(stackDesiredSize.Width, constraint.Width);
            stackDesiredSize.Height = Math.Min(stackDesiredSize.Height, constraint.Height);
  
            // When scrolling, the maximum horizontal or vertical size of items can cause the desired size of the
            // panel to change, which can cause the owning ScrollViewer re-layout as well when it is not necessary.
            // We will thus remember the maximum desired size and always return that. The actual arrangement and
            // clipping still be calculated from actual scroll data values.
            // The maximum desired size is reset when the items change.
            _scrollData._maxDesiredSize.Width = Math.Max(stackDesiredSize.Width, _scrollData._maxDesiredSize.Width);
            _scrollData._maxDesiredSize.Height = Math.Max(stackDesiredSize.Height, _scrollData._maxDesiredSize.Height);
            stackDesiredSize = _scrollData._maxDesiredSize;
  
            // Verify Scroll Info, invalidate ScrollOwner if necessary.
            SetAndVerifyScrollingData(viewport, extent, offset);
  
            return stackDesiredSize;
        }
 
        #endregion
 
  
        /// <summary>
        /// DesiredSize is normally computed by summing up the size of all items we've generated.  Pixel-based virtualization uses a 'full' desired size.
        /// This extends the given desired size beyond the visible items.  It will extend it by the items before or after the set of generated items.
        /// The given pivotIndex is the index of either the first or last item generated.
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="stackDesiredSize">
        /// <param name="pivotIndex">
        /// <param name="before">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private Size ExtendDesiredSize(ItemsControl itemsControl, Size stackDesiredSize, int pivotIndex, bool before, bool isHorizontal)
        {
            Debug.Assert(IsPixelBased, "MeasureOverride should have already computed desiredSize if non-virtualizing or items-based");
 
            //
            // If we're virtualizing the sum of all generated containers is not the true desired size since not all containers were generated.
            // In the old items-based mode it didn't matter because only the scrolling panel could virtualize and scrollviewer doesn't *really*
            // care about desired size.
            //
            // In pixel-based mode we need to compute the same desired size as if we weren't virtualizing.
            //
            // Note: there are faster ways to do this than loop through items, but the cost isn't significant and the other possible implementations are nasty.
            //
 
            Size containerSize;
            ItemCollection items = itemsControl.Items;
 
            for (int i = (before ? 0 : pivotIndex); i < (before ? pivotIndex : items.Count); i++)
            {
                containerSize = ContainerSizeForItem(itemsControl, items[i], i);
  
                if (isHorizontal)
                {
                    stackDesiredSize.Width += containerSize.Width;
                }
                else
                {
                    stackDesiredSize.Height += containerSize.Height;
                }
            }
 
            return stackDesiredSize;
        }
  
 
        //
        // Returns the index of the first item visible (even partially) in the viewport.
        //
        private int ComputeIndexOfFirstVisibleItem(MeasureData measureData, ItemsControl itemsControl, bool isHorizontal, out double firstItemOffset)
        {
            firstItemOffset = 0d;   // offset of the top of the first visible child from the top of the viewport.  The child always
                                    // starts before the top of the viewport so this is always negative.
  
            if (itemsControl != null)
            {
                ItemCollection items = itemsControl.Items;
                int itemsCount = items.Count;
  
                if (!IsPixelBased)
                {
                    //
                    // Classic case that shipped with V1
                    //
                    // If the panel is implementing IScrollInfo then _scrollData keeps track of the
                    // current offset, extent, etc in logical units
                    //
                    if (IsScrolling)
                    {
                        return CoerceIndexToInteger(isHorizontal ? _scrollData._offset.X : _scrollData._offset.Y, itemsCount);
                    }
                }
                else
                {
                    Size containerSize;
                    double totalSpan = 0.0;      // total height or width in the stacking direction
                    double containerSpan = 0.0;
                    double viewportOffset = isHorizontal ? measureData.Viewport.X : measureData.Viewport.Y;
 
                    for (int i = 0; i < itemsCount; i++)
                    {
                        containerSize = ContainerSizeForItem(itemsControl, items[i], i);
                        containerSpan = isHorizontal ? containerSize.Width : containerSize.Height;
                        totalSpan += containerSpan;
 
                        if (totalSpan > viewportOffset)
                        {
                            // This is the first item that starts before the viewportOffset but ends after it; i is thus the index
                            // to the first item in the viewport.
                            firstItemOffset = totalSpan - containerSpan - viewportOffset;
                            return i;
                        }
                    }
                }
            }
 
            return 0;
        }
  
 
        private Size ContainerSizeForItem(ItemsControl itemsControl, object item, int index)
        {
            UIElement temp;
            return ContainerSizeForItem(itemsControl, item, index, out temp);
        }
 
        /// <summary>
        /// Returns the size of the container for a given item.  The size can come from the container, a lookup, or a guess depending
        /// on the virtualization state of the item.
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="item">
        /// <param name="index">
        /// <param name="container">returns the container for the item; null if the container wasn't found
        /// <returns></returns>
        private Size ContainerSizeForItem(ItemsControl itemsControl, object item, int index, out UIElement container)
        {
            Size containerSize;
            container = index >= 0 ? ((ItemContainerGenerator)Generator).ContainerFromIndex(index) as UIElement : null;
  
            if (container != null)
            {
                containerSize = container.DesiredSize;
            }
            else
            {
                // It's virtualized; grab the height off the item if available.
                object value = itemsControl.ReadItemValue(item, _desiredSizeStorageIndex);
                if (value != null)
                {
                    containerSize = (Size)value;
                }
                else
                {
                    //
                    // No stored container height; simply guess.
                    //
                    containerSize = new Size();
 
  
                    if (Orientation == Orientation.Horizontal)
                    {
                        containerSize.Width = ContainerStackingSizeEstimate(itemsControl, /*isHorizontal = */ true);
                        containerSize.Height = DesiredSize.Height;
                    }
                    else
                    {
                        containerSize.Height = ContainerStackingSizeEstimate(itemsControl, /*isHorizontal = */ false);
                        containerSize.Width = DesiredSize.Width;
                    }
                }
            }
  
            Debug.Assert(!containerSize.IsEmpty, "We can't estimate an empty size");
            return containerSize;
        }
 
  
        private double ContainerStackingSizeEstimate(ItemsControl itemsControl, bool isHorizontal)
        {
            return ContainerStackingSizeEstimate(itemsControl as IProvideStackingSize, isHorizontal);
        }
 
        /// <summary>
        /// Estimates a container size in the stacking direction for the given ItemsControl
        /// </summary>
        /// <param name="itemsControl">
        /// <param name="IsHorizontal">
        /// <returns></returns>
        private double ContainerStackingSizeEstimate(IProvideStackingSize estimate, bool isHorizontal)
        {
            double stackingSize = 0d;
  
            if (estimate != null)
            {
                stackingSize = estimate.EstimatedContainerSize(isHorizontal);
            }
 
            if (stackingSize <= 0d || DoubleUtil.IsNaN(stackingSize))
            {
                stackingSize = ScrollViewer._scrollLineDelta;
            }
  
            Debug.Assert(stackingSize > 0, "We should have returned a reasonable estimate for the stacking size");
  
            return stackingSize;
        }
 
  
        private MeasureData CreateChildMeasureData(MeasureData measureData, Size layoutSlotSize, Size stackDesiredSize, bool isHorizontal)
        {
            Invariant.Assert(IsPixelBased && measureData != null, "We can only use MeasureData when pixel-based");
            Rect viewport = measureData.Viewport;
  
            //
            // Adjust viewport offset for the child
            //
            if (isHorizontal)
            {
                viewport.X -= stackDesiredSize.Width;
            }
            else
            {
                viewport.Y -= stackDesiredSize.Height;
            }
 
            return new MeasureData(layoutSlotSize, viewport);
        }
  
        /// <summary>
        /// Inserts a new container in the visual tree
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        private void InsertNewContainer(int childIndex, UIElement container)
        {
            InsertContainer(childIndex, container, false);
        }
  
        /// <summary>
        /// Inserts a recycled container in the visual tree
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        /// <returns></returns>
        private bool InsertRecycledContainer(int childIndex, UIElement container)
        {
            return InsertContainer(childIndex, container, true);
        }
  
 
        /// <summary>
        /// Inserts a container into the Children collection.  The container is either new or recycled.
        /// </summary>
        /// <param name="childIndex">
        /// <param name="container">
        /// <param name="isRecycled">
        private bool InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            Debug.Assert(container != null, "Null container was generated");
 
            bool visualOrderChanged = false;
            UIElementCollection children = InternalChildren;
 
            //
            // Find the index in the Children collection where we hope to insert the container.
            // This is done by looking up the index of the container BEFORE the one we hope to insert.
            //
            // We have to do it this way because there could be recycled containers between the container we're looking for and the one before it.
            // By finding the index before the place we want to insert and adding one, we ensure that we'll insert the new container in the
            // proper location.
            //
            // In recycling mode childIndex is the index in the _realizedChildren list, not the index in the
            // Children collection.  We have to convert the index; we'll call the index in the Children collection
            // the visualTreeIndex.
            //
  
            int visualTreeIndex = 0;
 
            if (childIndex > 0)
            {
                visualTreeIndex = ChildIndexFromRealizedIndex(childIndex - 1);
                visualTreeIndex++;
            }
 
  
            if (isRecycled && visualTreeIndex < children.Count && children[visualTreeIndex] == container)
            {
                // Don't insert if a recycled container is in the proper place already
            }
            else
            {
                if (visualTreeIndex < children.Count)
                {
                    int insertIndex = visualTreeIndex;
                    if (isRecycled && container.InternalVisualParent != null)
                    {
                        // If the container is recycled we have to remove it from its place in the visual tree and
                        // insert it in the proper location.   For perf we'll use an internal Move API that moves
                        // the first parameter to right before the second one.
                        Debug.Assert(children[visualTreeIndex] != null, "MoveVisualChild interprets a null destination as 'move to end'");
                        children.MoveVisualChild(container, children[visualTreeIndex]);
                        visualOrderChanged = true;
                    }
                    else
                    {
                        VirtualizingPanel.InsertInternalChild(children, insertIndex, container);
                    }
                }
                else
                {
                    if (isRecycled && container.InternalVisualParent != null)
                    {
                        // Recycled container is still in the tree; move it to the end
                        children.MoveVisualChild(container, null);
                        visualOrderChanged = true;
                    }
                    else
                    {
                        VirtualizingPanel.AddInternalChild(children, container);
                    }
                }
            }
 
            //
            // Keep realizedChildren in [....] w/ the visual tree.
            //
            if (IsVirtualizing && InRecyclingMode)
            {
                _realizedChildren.Insert(childIndex, container);
            }
 
            Generator.PrepareItemContainer(container);
 
            return visualOrderChanged;
        }
  
  
 
  
        private void EnsureCleanupOperation(bool delay)
        {
            if (delay)
            {
                bool noPendingOperations = true;
                if (_cleanupOperation != null)
                {
                    noPendingOperations = _cleanupOperation.Abort();
                    if (noPendingOperations)
                    {
                        _cleanupOperation = null;
                    }
                }
                if (noPendingOperations && (_cleanupDelay == null))
                {
                    _cleanupDelay = new DispatcherTimer();
                    _cleanupDelay.Tick += new EventHandler(OnDelayCleanup);
                    _cleanupDelay.Interval = TimeSpan.FromMilliseconds(500.0);
                    _cleanupDelay.Start();
                }
            }
            else
            {
                if ((_cleanupOperation == null) && (_cleanupDelay == null))
                {
                    _cleanupOperation = Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(OnCleanUp), null);
                }
            }
        }
 
        private bool PreviousChildIsGenerated(int childIndex)
        {
            GeneratorPosition position = new GeneratorPosition(childIndex, 0);
            position = Generator.GeneratorPositionFromIndex(Generator.IndexFromGeneratorPosition(position) - 1);
            return (position.Offset == 0 && position.Index >= 0);
        }
 
 
        /// <summary>
        /// Takes a container returned from Generator.GenerateNext() and places it in the visual tree if necessary.
        /// Takes into account whether the container is new, recycled, or already realized.
        /// </summary>
        /// <param name="childIndex">
        /// <param name="child">
        /// <param name="newlyRealized">
        private bool AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
        {
            bool visualOrderChanged = false;
  
            if (!newlyRealized)
            {
                //
                // Container is either realized or recycled.  If it's realized do nothing; it already exists in the visual
                // tree in the proper place.
                //
 
                if (InRecyclingMode)
                {
                    // Note there's no check for IsVirtualizing here.  If the user has just flipped off virtualization it's possible that
                    // the Generator will still return some recycled containers until its list runs out.
  
                    IList children = RealizedChildren;
  
                    if (childIndex >= children.Count || !(children[childIndex] == child))
                    {
                        Debug.Assert(!children.Contains(child), "we incorrectly identified a recycled container");
  
                        //
                        // We have a recycled container (if it was a realized container it would have been returned in the
                        // proper location).  Note also that recycled containers are NOT in the _realizedChildren list.
                        //
  
                        visualOrderChanged = InsertRecycledContainer(childIndex, child);
                    }
                    else
                    {
                        // previously realized child.
                    }
                }
                else
                {
                    // Not recycling; realized container
                    Debug.Assert(child == InternalChildren[childIndex], "Wrong child was generated");
                }
            }
            else
            {
                InsertNewContainer(childIndex, child);
            }
  
            return visualOrderChanged;
        }
 
        private UIElement GeneratePreviousChild(int childIndex, Size layoutSlotSize)
        {
            int newIndex = Generator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0)) - 1;
            if (newIndex >= 0)
            {
                UIElement child;
                bool visualOrderChanged = false;
                IItemContainerGenerator generator = Generator;
 
                int newGeneratedIndex;
                GeneratorPosition newStartPos = IndexToGeneratorPositionForStart(newIndex, out newGeneratedIndex);
                using (generator.StartAt(newStartPos, GeneratorDirection.Forward, true))
                {
                    bool newlyRealized;
                    child = generator.GenerateNext(out newlyRealized) as UIElement;
                    Debug.Assert(child != null, "Null child was generated");
 
                    AddContainerFromGenerator(childIndex, child, newlyRealized);
  
                    if (childIndex <= _firstVisibleChildIndex)
                    {
                        _firstVisibleChildIndex++;
                    }
  
                    child.Measure(layoutSlotSize);
                }
 
                if (visualOrderChanged)
                {
                    Debug.Assert(IsVirtualizing && InRecyclingMode, "We should only modify the visual order when in recycling mode");
                    InvalidateZState();
                }
  
                return child;
            }
 
            return null;
        }
  
  
        private void OnItemsRemove(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }
 
        private void OnItemsReplace(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
        }
 
        private void OnItemsMove(ItemsChangedEventArgs args)
        {
            RemoveChildRange(args.OldPosition, args.ItemCount, args.ItemUICount);
        }
  
        private void RemoveChildRange(GeneratorPosition position, int itemCount, int itemUICount)
        {
            if (IsItemsHost)
            {
                UIElementCollection children = InternalChildren;
                int pos = position.Index;
                if (position.Offset > 0)
                {
                    // An item is being removed after the one at the index
                    pos++;
                }
  
                if (pos < children.Count)
                {
                    int uiCount = itemUICount;
                    Debug.Assert((itemCount == itemUICount) || (itemUICount == 0), "Both ItemUICount and ItemCount should be equal or ItemUICount should be 0.");
                    if (uiCount > 0)
                    {
                        VirtualizingPanel.RemoveInternalChildRange(children, pos, uiCount);
  
                        if (IsVirtualizing && InRecyclingMode)
                        {
                            _realizedChildren.RemoveRange(pos, uiCount);
                        }
                    }
                }
            }
        }
  
  
        private void AdjustCacheWindow(int firstViewport, int itemCount)
        {
            //
            // Adjust the container cache window such that the viewport is always contained inside.
            //
  
            // firstViewport is the index of the first container in the viewport, not counting the before trail.
            // _visibleCount is the total number of items we generated. It already contains the _afterTrail.
  
            // First and last containers that we must keep in view; index is into the data item collection
            int firstContainer = firstViewport > 0 ? firstViewport - _beforeTrail : firstViewport;
            int lastContainer = firstViewport + _visibleCount - 1;   // beforeTrail is not included in _visibleCount
 
            // clamp last container
            if (lastContainer >= itemCount)
            {
                lastContainer = itemCount - 1;
            }
 
            int cacheEnd = CacheEnd;
 
            if (firstContainer < _cacheStart)
            {
                // shift the cache start up
                _cacheStart = firstContainer;
            }
            else if (lastContainer > cacheEnd)
            {
                // shift the cache start down
                _cacheStart += (lastContainer - cacheEnd);
            }
 
  
            // In some cases cacheEnd can be past the end of the list of items.  This is perfectly fine.
            Debug.Assert(_cacheStart <= firstContainer && (CacheEnd >= firstContainer + _visibleCount - 1 || CacheEnd >= itemCount - 1), "The container cache window is out of place");
        }
 
        private bool IsOutsideCacheWindow(int itemIndex)
        {
 
            return (itemIndex < _cacheStart || itemIndex > CacheEnd);
        }
 
  
        /// <summary>
        /// Immediately cleans up any containers that have gone offscreen.  Called by MeasureOverride.
        /// When recycling this runs before generating and measuring children; otherwise it runs after.
        /// </summary>
        private void CleanupContainers(int firstViewport, ItemsControl itemsControl)
        {
            Debug.Assert(IsVirtualizing, "Can't clean up containers if not virtualizing");
            Debug.Assert(InRecyclingMode || IsPixelBased,
                "For backwards compat the standard virtualizing mode has its own cleanup algorithm");
            Debug.Assert(itemsControl != null, "We can't cleanup if we aren't the itemshost");
 
            //
            // It removes items outside of the container cache window (a logical 'window' at
            // least as large as the viewport).
            //
            // firstViewport is the index of first data item that will be in the viewport
            // at the end of Measure.  This is effectively the scroll offset.
            //
            // _visibleStart is index of the first data item that was previously at the top of the viewport
            // At the end of a Measure pass _visibleStart == firstViewport.
            //
            // _visibleCount is the number of data items that were previously visible in the viewport.
 
            int cleanupRangeStart = -1;
            int cleanupCount = 0;
            int itemIndex = -1;              // data item index used to compare with the cache window position.
            int lastItemIndex;
            IList children = RealizedChildren;
            int focusedChild = -1, previousFocusable = -1, nextFocusable = -1;  // child indices for the focused item and before and after focus trail items
  
            bool performCleanup = false;
            UIElement child;
 
            if (children.Count == 0)
            {
                return; // nothing to do
            }
 
            AdjustCacheWindow(firstViewport, itemsControl.Items.Count);
 
            if (IsKeyboardFocusWithin && !IsPixelBased)
            {
                // If we're not in a hieararchy we can find the focus trail locally; for hierarchies it has already been
                // precalculated.
                FindFocusedChild(out focusedChild, out previousFocusable, out nextFocusable);
            }
 
            //
            // Iterate over all realized children and recycle the ones that are eligible.  Items NOT eligible for recycling
            // have one or more of the following properties
            //
            //  - inside the cache window
            //  - the item is its own container
            //  - has keyboard focus
            //  - is the first focusable item before or after the focused item
            //  - the CleanupVirtualizedItem event was canceled
            //
 
            for (int childIndex = 0; childIndex < children.Count; childIndex++)
            {
                child = (UIElement)children[childIndex];
                lastItemIndex = itemIndex;
                itemIndex = GetGeneratedIndex(childIndex);
  
                if (itemIndex - lastItemIndex != 1)
                {
                    // There's a generated gap between the current item and the last.  Clean up the last range of items.
                    performCleanup = true;
                }
  
                if (performCleanup)
                {
                    if (cleanupRangeStart >= 0 && cleanupCount > 0)
                    {
                        //
                        // We've hit a non-virtualizable container or a non-contiguous section.
                        //
 
                        CleanupRange(children, Generator, cleanupRangeStart, cleanupCount);
 
                        // CleanupRange just modified the _realizedChildren list.  Adjust the childIndex.
                        childIndex -= cleanupCount;
                        focusedChild -= cleanupCount;
                        previousFocusable -= cleanupCount;
                        nextFocusable -= cleanupCount;
 
                        cleanupCount = 0;
                        cleanupRangeStart = -1;
                    }
  
                    performCleanup = false;
                }
  
 
                if (IsOutsideCacheWindow(itemIndex) &&
                    !((IGeneratorHost)itemsControl).IsItemItsOwnContainer(itemsControl.Items[itemIndex]) &&
                    childIndex != focusedChild &&
                    childIndex != previousFocusable &&
                    childIndex != nextFocusable &&
                    !IsInFocusTrail(child) &&                   // logically the same computation as the three above; used when in a treeview.
                    child != _bringIntoViewContainer &&         // the container we're going to bring into view must not be recycled
                    NotifyCleanupItem(child, itemsControl))
                {
                    //
                    // The container is eligible to be virtualized
                    //
                    if (cleanupRangeStart == -1)
                    {
                        cleanupRangeStart = childIndex;
                    }
  
                    cleanupCount++;
 
                    //
                    // Save off the child's desired size if we're doing pixel-based virtualization.
                    // We need to save off the size when doing hierarchical (i.e. TreeView) virtualization, since containers will vary
                    // greatly in size. This is required both to compute the index of the first visible item in the viewport and to Arrange
                    // children in their proper locations.
                    //
                    if (IsPixelBased)
                    {
                        itemsControl.StoreItemValue(itemsControl.Items[itemIndex], child.DesiredSize, _desiredSizeStorageIndex);
                    }
                }
                else
                {
                    // Non-recyclable container;
                    performCleanup = true;
                }
            }
 
            if (cleanupRangeStart >= 0 && cleanupCount > 0)
            {
                CleanupRange(children, Generator, cleanupRangeStart, cleanupCount);
            }
        }
 
  
        private void EnsureRealizedChildren()
        {
            Debug.Assert(InRecyclingMode, "This method only applies to recycling mode");
            if (_realizedChildren == null)
            {
                UIElementCollection children = InternalChildren;
  
                _realizedChildren = new List<uielement>(children.Count);
  
                for (int i = 0; i < children.Count; i++)
                {
                    _realizedChildren.Add(children[i]);
                }
            }
        }
  
 
        [Conditional("DEBUG")]
        private void debug_VerifyRealizedChildren()
        {
            // Debug method that ensures the _realizedChildren list matches the realized containers in the Generator.
            Debug.Assert(IsVirtualizing && InRecyclingMode, "Realized children only exist when recycling");
            Debug.Assert(_realizedChildren != null, "Realized children must exist to verify it");
            System.Windows.Controls.ItemContainerGenerator generator = Generator as System.Windows.Controls.ItemContainerGenerator;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (generator != null && itemsControl != null && itemsControl.IsGrouping == false)
            {
 
                foreach (UIElement child in InternalChildren)
                {
                    int dataIndex = generator.IndexFromContainer(child);
  
                    if (dataIndex == -1)
                    {
                        // Child is not in the generator's realized container list (i.e. it's a recycled container): ensure it's NOT in _realizedChildren.
                        Debug.Assert(!_realizedChildren.Contains(child), "_realizedChildren should not contain recycled containers");
                    }
                    else
                    {
                        // Child is a realized container; ensure it's in _realizedChildren at the proper place.
                        GeneratorPosition position = Generator.GeneratorPositionFromIndex(dataIndex);
                        Debug.Assert(_realizedChildren[position.Index] == child, "_realizedChildren is corrupt!");
                    }
                }
            }
        }
 
        [Conditional("DEBUG")]
        private void debug_AssertRealizedChildrenEqualVisualChildren()
        {
            if (IsVirtualizing && InRecyclingMode)
            {
                UIElementCollection children = InternalChildren;
                Debug.Assert(_realizedChildren.Count == children.Count, "Realized and visual children must match");
 
                for (int i = 0; i < children.Count; i++)
                {
                    Debug.Assert(_realizedChildren[i] == children[i], "Realized and visual children must match");
                }
            }
        }
  
        /// <summary>
        /// Takes an index from the realized list and returns the corresponding index in the Children collection
        /// </summary>
        /// <param name="realizedChildIndex">
        /// <returns></returns>
        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            //
            // If we're not recycling containers then we're not using a realizedChild index and no translation is necessary
            //
            if (IsVirtualizing && InRecyclingMode)
            {
  
                if (realizedChildIndex < _realizedChildren.Count)
                {
  
                    UIElement child = _realizedChildren[realizedChildIndex];
                    UIElementCollection children = InternalChildren;
 
                    for (int i = realizedChildIndex; i < children.Count; i++)
                    {
                        if (children[i] == child)
                        {
                            return i;
                        }
                    }
  
                    Debug.Assert(false, "We should have found a child");
                }
            }
  
            return realizedChildIndex;
        }
  
        /// <summary>
        /// Recycled containers still in the Children collection at the end of Measure should be disconnected
        /// from the visual tree.  Otherwise they're still visible to things like Arrange, keyboard navigation, etc.
        /// </summary>
        private void DisconnectRecycledContainers()
        {
            int realizedIndex = 0;
            UIElement visualChild;
            UIElement realizedChild = _realizedChildren.Count > 0 ? _realizedChildren[0] : null;
            UIElementCollection children = InternalChildren;
  
            for (int i = 0; i < children.Count; i++)
            {
                visualChild = children[i];
  
                if (visualChild == realizedChild)
                {
                    realizedIndex++;
 
                    if (realizedIndex < _realizedChildren.Count)
                    {
 
                        realizedChild = _realizedChildren[realizedIndex];
                    }
                    else
                    {
                        realizedChild = null;
                    }
                }
                else
                {
                    // The visual child is a recycled container
                    children.RemoveNoVerify(visualChild);
                    i--;
                }
            }
 
            debug_VerifyRealizedChildren();
            debug_AssertRealizedChildrenEqualVisualChildren();
        }
 
        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            IItemContainerGenerator generator = Generator;
            GeneratorPosition position = (generator != null) ? generator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
 
            // determine the position in the children collection for the first
            // generated container.  This assumes that generator.StartAt will be called
            // with direction=Forward and  allowStartAtRealizedItem=true.
            childIndex = (position.Offset == 0) ? position.Index : position.Index + 1;
  
            return position;
        }
  
 
        #region Delayed Cleanup Methods
 
        //
        // Delayed Cleanup is used when the VirtualizationMode is standard (not recycling) and the panel is scrolling and item-based
        // It chooses to defer virtualizing items until there are enough available.  It then cleans them using a background priority dispatcher
        // work item
        //
  
        private void OnDelayCleanup(object sender, EventArgs e)
        {
            Debug.Assert(_cleanupDelay != null);
 
            bool needsMoreCleanup = false;
  
            try
            {
                needsMoreCleanup = CleanUp();
            }
            finally
            {
                // Cleanup the timer if more cleanup is unnecessary
                if (!needsMoreCleanup)
                {
                    _cleanupDelay.Stop();
                    _cleanupDelay = null;
                }
            }
        }
 
        private object OnCleanUp(object args)
        {
            Debug.Assert(_cleanupOperation != null);
 
            bool needsMoreCleanup = false;
  
            try
            {
                needsMoreCleanup = CleanUp();
            }
            finally
            {
                // Keeping this non-null until here in case cleaning up causes re-entrancy
                _cleanupOperation = null;
            }
 
            if (needsMoreCleanup)
            {
                EnsureCleanupOperation(true /* delay */);
            }
  
            return null;
        }
  
        private bool CleanUp()
        {
            Debug.Assert(!InRecyclingMode, "This method only applies to standard virtualization");
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
 
            if (!IsVirtualizing || !IsItemsHost)
            {
                // Virtualization is turned off or we aren't hosting children; no need to cleanup.
                return false;
            }
  
            int startMilliseconds = Environment.TickCount;
            bool needsMoreCleanup = false;
            UIElementCollection children = InternalChildren;
            int minDesiredGenerated = MinDesiredGenerated;
            int maxDesiredGenerated = MaxDesiredGenerated;
            int pageSize = maxDesiredGenerated - minDesiredGenerated;
            int extraChildren = children.Count - pageSize;
 
            if (extraChildren > (pageSize * 2))
            {
                if ((Mouse.LeftButton == MouseButtonState.Pressed) &&
                    (extraChildren < 1000))
                {
                    // An optimization for when we are dragging the mouse.
                    needsMoreCleanup = true;
                }
                else
                {
                    bool trailingFocus = IsKeyboardFocusWithin;
                    bool keepForwardTrail = false;
                    int focusIndex = -1;
                    IItemContainerGenerator generator = Generator;
 
                    int cleanupRangeStart = 0;
                    int cleanupCount = 0;
                    int lastGeneratedIndex = -1;
                    int counterAdjust;
 
                    for (int i = 0; i < children.Count; i++)
                    {
                        // It is possible for TickCount to wrap around about every 30 days.
                        // If that were to occur, then this particular cleanup may not be interrupted.
                        // That is OK since the worst that can happen is that there is more of a stutter than normal.
                        int totalMilliseconds = Environment.TickCount - startMilliseconds;
                        if ((totalMilliseconds > 50) && (cleanupCount > 0))
                        {
                            // Cleanup has been working for 50ms already and the user might start
                            // noticing a lag. Stop cleaning up and release the thread for other work.
                            // Cleanup will continue later.
                            // Don't break out until after at least one item has been found to cleanup.
                            // Otherwise, we might end up in an infinite loop.
                            needsMoreCleanup = true;
                            break;
                        }
  
                        int childIndex = i;
                        if (trailingFocus)
                        {
                            // Focus lies somewhere within the panel, but it has not been found yet.
                            UIElement child = children[i];
                            if (child.IsKeyboardFocusWithin)
                            {
                                // Focus has been found, we can now re-virtualize items before the focus.
                                trailingFocus = false;
                                keepForwardTrail = true;
                                focusIndex = i;
                                if (i > 0)
                                {
                                    // Go through the trailing items and find a focusable item to keep.
                                    int trailIndex = i - 1;
                                    int end = Math.Max(0, i - FocusTrail);
                                    for (; trailIndex >= end; trailIndex--)
                                    {
                                        child = children[trailIndex];
                                        if (Keyboard.IsFocusable(child))
                                        {
                                            trailIndex--;
                                            break;
                                        }
                                    }
 
                                    // The rest of the trailing items can be re-virtualized.
                                    for (childIndex = end; childIndex <= trailIndex; childIndex++)
                                    {
                                        ManageCleanup(
                                            children,
                                            itemsControl,
                                            generator,
                                            childIndex,
                                            minDesiredGenerated,
                                            maxDesiredGenerated,
                                            ref childIndex,
                                            ref cleanupRangeStart,
                                            ref cleanupCount,
                                            ref lastGeneratedIndex,
                                            out counterAdjust);
                                        if (counterAdjust > 0)
                                        {
                                            i -= counterAdjust;
                                            trailIndex -= counterAdjust;
                                        }
                                    }
 
                                    if (cleanupCount > 0)
                                    {
                                        // Cleanup the last batch for the focused item
                                        CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                                        i -= cleanupCount;
                                        cleanupCount = 0;
                                    }
                                    cleanupRangeStart = i + 1;
 
                                    // At this point, we are caught up and should go to the next item
                                    continue;
                                }
                            }
                            else if (i >= FocusTrail)
                            {
                                childIndex = i - FocusTrail;
                            }
                            else
                            {
                                continue;
                            }
                        }
 
                        if (keepForwardTrail)
                        {
                            // Find a focusable item after the focused item to keep
                            if (childIndex <= (focusIndex + FocusTrail))
                            {
                                UIElement child = children[childIndex];
                                if (Keyboard.IsFocusable(child))
                                {
                                    // A focusable item was found, all items after this one can be re-virtualized
                                    keepForwardTrail = false;
                                    cleanupRangeStart = childIndex + 1;
                                    cleanupCount = 0;
                                }
                                continue;
                            }
                            else
                            {
                                keepForwardTrail = false;
                            }
                        }
 
                        ManageCleanup(
                            children,
                            itemsControl,
                            generator,
                            childIndex,
                            minDesiredGenerated,
                            maxDesiredGenerated,
                            ref i,
                            ref cleanupRangeStart,
                            ref cleanupCount,
                            ref lastGeneratedIndex,
                            out counterAdjust);
                    }
  
                    if (cleanupCount > 0)
                    {
                        // Cleanup the final batch
                        CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                    }
                }
            }
  
            return needsMoreCleanup;
        }
  
        private void ManageCleanup(
            UIElementCollection children,
            ItemsControl itemsControl,
            IItemContainerGenerator generator,
            int childIndex,
            int minDesiredGenerated,
            int maxDesiredGenerated,
            ref int counter,
            ref int cleanupRangeStart,
            ref int cleanupCount,
            ref int lastGeneratedIndex,
            out int counterAdjust)
        {
            counterAdjust = 0;
            bool performCleanup = false;
            bool countThisChild = false;
            int generatedIndex = GetGeneratedIndex(childIndex);
  
            if (OutsideMinMax(generatedIndex, minDesiredGenerated, maxDesiredGenerated) &&
                NotifyCleanupItem(childIndex, children, itemsControl))
            {
                // The item can be re-virtualized.
                if ((generatedIndex - lastGeneratedIndex) == 1)
                {
                    // Add another to the current batch.
                    cleanupCount++;
                }
                else
                {
                    // There was a gap in generated items. Cleanup any from the previous batch.
                    performCleanup = countThisChild = true;
                }
            }
            else
            {
                // The item cannot be re-virtualized. Cleanup any from the previous batch.
                performCleanup = true;
            }
 
            if (performCleanup)
            {
                // Cleanup a batch of items
                if (cleanupCount > 0)
                {
                    CleanupRange(children, generator, cleanupRangeStart, cleanupCount);
                    counterAdjust = cleanupCount;
                    counter -= counterAdjust;
                    childIndex -= counterAdjust;
                    cleanupCount = 0;
                }
  
                if (countThisChild)
                {
                    // The current child was not included in the batch and should be saved for later
                    cleanupRangeStart = childIndex;
                    cleanupCount = 1;
                }
                else
                {
                    // The next child will start the next batch.
                    cleanupRangeStart = childIndex + 1;
                }
            }
            lastGeneratedIndex = generatedIndex;
        }
 
        private bool NotifyCleanupItem(int childIndex, UIElementCollection children, ItemsControl itemsControl)
        {
            return NotifyCleanupItem(children[childIndex], itemsControl);
        }
 
        private bool NotifyCleanupItem(UIElement child, ItemsControl itemsControl)
        {
            CleanUpVirtualizedItemEventArgs e = new CleanUpVirtualizedItemEventArgs(itemsControl.ItemContainerGenerator.ItemFromContainer(child), child);
            e.Source = this;
            OnCleanUpVirtualizedItem(e);
 
            return !e.Cancel;
        }
 
        private void CleanupRange(IList children, IItemContainerGenerator generator, int startIndex, int count)
        {
            if (InRecyclingMode)
            {
                Debug.Assert(startIndex >= 0 && count > 0);
                Debug.Assert(children == _realizedChildren, "the given child list must be the _realizedChildren list when recycling");
                ((IRecyclingItemContainerGenerator)generator).Recycle(new GeneratorPosition(startIndex, 0), count);
  
                // The call to Recycle has caused the ItemContainerGenerator to remove some items
                // from its list of realized items; we adjust _realizedChildren to match.
                _realizedChildren.RemoveRange(startIndex, count);
            }
            else
            {
                // Remove the desired range of children
                VirtualizingPanel.RemoveInternalChildRange((UIElementCollection)children, startIndex, count);
                generator.Remove(new GeneratorPosition(startIndex, 0), count);
            }
  
            AdjustFirstVisibleChildIndex(startIndex, count);
        }
 
        #endregion
 
        /// <summary>
        /// Called after 'count' items were removed or recycled from the Generator.  _firstVisibleChildIndex is the
        /// index of the first visible container.  This index isn't exactly the child position in the UIElement collection;
        /// it's actually the index of the realized container inside the generator.  Since we've just removed some realized
        /// containers from the generator (by calling Remove or Recycle), we have to adjust the first visible child index.
        /// </summary>
        /// <param name="startIndex">index of the first removed item
        /// <param name="count">number of items removed
        private void AdjustFirstVisibleChildIndex(int startIndex, int count)
        {
  
            // Update the index of the first visible generated child
            if (startIndex < _firstVisibleChildIndex)
            {
                int endIndex = startIndex + count - 1;
                if (endIndex < _firstVisibleChildIndex)
                {
                    // The first visible index is after the items that were removed
                    _firstVisibleChildIndex -= count;
                }
                else
                {
                    // The first visible index was within the items that were removed
                    _firstVisibleChildIndex = startIndex;
                }
            }
        }
  
        private static bool OutsideMinMax(int i, int min, int max)
        {
            return ((i < min) || (i > max));
        }
 
        private void EnsureTopCapGenerated(Size layoutSlotSize)
        {
            // Ensure that a focusable item is generated above the first visible item
            // so that keyboard navigation works.
  
            IList children;
  
            _beforeTrail = 0;
            if (_visibleStart > 0)
            {
                children = RealizedChildren;
                int childIndex = _firstVisibleChildIndex;
  
                UIElement child;
 
                // At most, we will search FocusTrail number of items for a focusable item
                for (; _beforeTrail < FocusTrail; _beforeTrail++)
                {
                    if (PreviousChildIsGenerated(childIndex))
                    {
                        // The previous child is already generated, check its focusability
                        childIndex--;
                        child = (UIElement)children[childIndex];
                    }
                    else
                    {
                        // Generate the previous child
                        child = GeneratePreviousChild(childIndex, layoutSlotSize);
                    }
 
                    if ((child == null) || Keyboard.IsFocusable(child))
                    {
                        // Either a focusable item was found, or no child was generated
                        _beforeTrail++;
                        break;
                    }
                }
            }
        }
  
  
        /// <summary>
        /// Returns the MeasureData we'll be using for computations in MeasureOverride.  This updates the viewport offset
        /// based on the one set in the MeasureData property prior to the call to MeasureOverride.
        /// </summary>
        /// <param name="constraint">
        /// <param name="itemsControl">
        /// <param name="isHorizontal">
        /// <returns></returns>
        private MeasureData AdjustViewportOffset(MeasureData givenMeasureData, ItemsControl itemsControl, bool isHorizontal)
        {
            // Note that a panel should not modify its own MeasureData -- it needs to be treated exactly as if it was a variable
            // passed into MeasureOverride.  That's why we make a copy of MeasureData in this method and return that.
 
            Rect viewport;
            MeasureData newMeasureData = null;
            IProvideStackingSize stackingSize;
            double offset = 0d;
            Debug.Assert(MeasureData == null || IsPixelBased, "If a panel has measure data then it must be pixel based");
            Debug.Assert(!IsScrolling && IsPixelBased, "This only applies to internal panels");
  
            //
            // This panel isn't a scroll owner but some panel above it is.  It will be able to use the viewport data
            // to virtualize.
            //
 
            if (givenMeasureData != null)
            {
                viewport = givenMeasureData.Viewport;
                stackingSize = itemsControl as IProvideStackingSize;
 
                Debug.Assert(givenMeasureData.HasViewport, "MeasureData is only set on objects when we want to pass down viewport information.");
 
                //
                // We need to offset the viewport to take into account the delta between the top of the items control
                // and this panel (i.e. the header).  Ask for the header, and, if not available, use the estimated container size.
  
                if (stackingSize != null)
                {
                    offset = stackingSize.HeaderSize(isHorizontal);
 
                    if (offset <= 0d || DoubleUtil.IsNaN(offset))
                    {
                        offset = ContainerStackingSizeEstimate(stackingSize, isHorizontal);
                    }
                }
 
                if (isHorizontal)
                {
                    viewport.X -= offset;
                }
                else
                {
                    // adjust viewport for the header of the TreeViewItem containing this as an ItemsPanel.
                    viewport.Y -= offset;
                }
  
                newMeasureData = new MeasureData(givenMeasureData.AvailableSize, viewport);
            }
 
  
            return newMeasureData;
        }
  
        /// <summary>
        /// Sets up IsVirtualizing, VirtualizationMode, and IsPixelBased
        ///
        /// IsVirtualizing is true if turned on via the items control and if the panel has a viewport.
        /// VSP has a viewport if it's either the scrolling panel or it was given MeasureData.
        ///
        /// IsPixelBased is true if the panel is virtualizing and (for backwards compat) is the ItemsHost for a TreeView or TreeViewItem.
        /// VSP can only make use of, create, and propagate down MeasureData if it is pixel-based, since the viewport is in pixels.
        /// </summary>
        /// <param name="itemsControl">
        private void SetVirtualizationState(ItemsControl itemsControl, bool hasMeasureData)
        {
            VirtualizationMode mode = (itemsControl != null) ? GetVirtualizationMode(itemsControl) : VirtualizationMode.Standard;
 
            if (itemsControl != null)
            {
                // Set IsVirtualizing.  This panel can only virtualize if IsVirtualizing is set on its ItemsControl and it has viewport data.
                // It has viewport data if it's either the scroll host or was given viewport information by measureData.
 
                if (GetIsVirtualizing(itemsControl) && (IsScrolling || hasMeasureData))
                {
                    IsVirtualizing = true;
                }
            }
            else
            {
                IsVirtualizing = false;
            }
  
 
            //
            // Set up info on first measure
            //
            if (HasMeasured)
            {
                VirtualizationMode oldMode = VirtualizationMode;
 
                if (oldMode != mode)
                {
                    throw new InvalidOperationException(SR.Get(SRID.CantSwitchVirtualizationModePostMeasure));
                }
            }
            else
            {
                HasMeasured = true;
 
                if (IsVirtualizing && (itemsControl is TreeView || itemsControl is TreeViewItem))
                {
                    IsPixelBased = true;
                }
  
                VirtualizationMode = mode;
            }
        }
 
        private int MinDesiredGenerated
        {
            get
            {
                return Math.Max(0, _visibleStart - _beforeTrail);
            }
        }
  
        private int MaxDesiredGenerated
        {
            get
            {
                return Math.Min(ItemCount, _visibleStart + _visibleCount + _afterTrail);
            }
        }
  
        private int ItemCount
        {
            get
            {
                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                return (itemsControl != null) ? itemsControl.Items.Count : 0;
            }
        }
  
        #endregion
 
  
        private void EnsureScrollData()
        {
            if (_scrollData == null) { _scrollData = new ScrollData(); }
        }
 
        private static void ResetScrolling(VirtualizingStackPanel element)
        {
            element.InvalidateMeasure();
  
            // Clear scrolling data.  Because of thrash (being disconnected & reconnected, &c...), we may
            if (element.IsScrolling)
            {
                element._scrollData.ClearLayout();
            }
        }
  
        // OnScrollChange is an override called whenever the IScrollInfo exposed scrolling state changes on this element.
        // At the time this method is called, scrolling state is in its new, valid state.
        private void OnScrollChange()
        {
            if (ScrollOwner != null) { ScrollOwner.InvalidateScrollInfo(); }
        }
 
        private void SetAndVerifyScrollingData(Size viewport, Size extent, Vector offset)
        {
            Debug.Assert(IsScrolling);
  
            if (IsPixelBased)
            {
                // _scrollData is in pixels and thus operations like LineDown can push the offset too far.
                // The behavior here is effectively the same as ScrollContentPresenter.VerifyScrollData
                offset.X = ScrollContentPresenter.CoerceOffset(offset.X, extent.Width, viewport.Width);
                offset.Y = ScrollContentPresenter.CoerceOffset(offset.Y, extent.Height, viewport.Height);
            }
 
            // Detect changes to the viewport, extent, and offset
            bool viewportChanged = !DoubleUtil.AreClose(viewport, _scrollData._viewport);
            bool extentChanged = !DoubleUtil.AreClose(extent, _scrollData._extent);
            bool offsetChanged = !DoubleUtil.AreClose(offset, _scrollData._computedOffset);
  
            // Update data and fire scroll change notifications
            _scrollData._offset = offset;
            if (viewportChanged || extentChanged || offsetChanged)
            {
                Vector oldViewportOffset = _scrollData._computedOffset;
                Size oldViewportSize = _scrollData._viewport;
 
                _scrollData._viewport = viewport;
                _scrollData._extent = extent;
                _scrollData._computedOffset = offset;
  
                // Report changes to the viewport
                if (viewportChanged)
                {
                    OnViewportSizeChanged(oldViewportSize, viewport);
                }
 
                // Report changes to the offset
                if (offsetChanged)
                {
                    OnViewportOffsetChanged(oldViewportOffset, offset);
                }
  
                OnScrollChange();
            }
        }
  
        /// <summary>
        ///     Allows subclasses to be notified of changes to the viewport size data.
        /// </summary>
        /// <param name="oldViewportSize">The old value of the size.
        /// <param name="newViewportSize">The new value of the size.
        protected virtual void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize)
        {
        }
  
        /// <summary>
        ///     Allows subclasses to be notified of changes to the viewport offset data.
        /// </summary>
        /// <param name="oldViewportOffset">The old value of the offset.
        /// <param name="newViewportOffset">The new value of the offset.
        protected virtual void OnViewportOffsetChanged(Vector oldViewportOffset, Vector newViewportOffset)
        {
        }
  
        // Translates a logical (child index) offset to a physical (1/96") when scrolling.
        // If virtualizing, it makes the assumption that the logicalOffset is always the first in the visual collection
        //   and thus returns 0.
        // If not virtualizing, it assumes that children are Measure clean; should only be called after running Measure.
        private double ComputePhysicalFromLogicalOffset(double logicalOffset, bool fHorizontal)
        {
            double physicalOffset = 0.0;
 
            IList children = RealizedChildren;
 
            Debug.Assert(logicalOffset == 0 || (logicalOffset > 0 && logicalOffset < children.Count));
  
            for (int i = 0; i < logicalOffset; i++)
            {
                UIElement child = (UIElement)children[i];
                physicalOffset -= (fHorizontal)
                    ? child.DesiredSize.Width
                    : child.DesiredSize.Height;
            }
  
            return physicalOffset;
        }
  
        private int FindChildIndexThatParentsVisual(Visual v)
        {
            DependencyObject child = v;
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != this)
            {
                child = parent;
                parent = VisualTreeHelper.GetParent(child);
            }
 
            IList children = RealizedChildren;
 
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == child)
                {
                    return GetGeneratedIndex(i);
                }
            }
 
            return -1;
        }
 
        // This is very similar to the work that ScrollContentPresenter does for MakeVisible.  Simply adjust by a
        // pixel offset.
        private void MakeVisiblePhysicalHelper(Rect r, ref Vector newOffset, ref Rect newRect, bool isHorizontal)
        {
            double viewportOffset;
            double viewportSize;
            double targetRectOffset;
            double targetRectSize;
            double minPhysicalOffset;
  
            if (isHorizontal)
            {
                viewportOffset = _scrollData._computedOffset.X;
                viewportSize = ViewportWidth;
                targetRectOffset = r.X;
                targetRectSize = r.Width;
            }
            else
            {
                viewportOffset = _scrollData._computedOffset.Y;
                viewportSize = ViewportHeight;
                targetRectOffset = r.Y;
                targetRectSize = r.Height;
            }
 
            targetRectOffset += viewportOffset;
            minPhysicalOffset = ScrollContentPresenter.ComputeScrollOffsetWithMinimalScroll(
                viewportOffset, viewportOffset + viewportSize, targetRectOffset, targetRectOffset + targetRectSize);
  
            // Compute the visible rectangle of the child relative to the viewport.
            double left = Math.Max(targetRectOffset, minPhysicalOffset);
            targetRectSize = Math.Max(Math.Min(targetRectSize + targetRectOffset, minPhysicalOffset + viewportSize) - left, 0);
            targetRectOffset = left;
            targetRectOffset -= viewportOffset;
  
            if (isHorizontal)
            {
                newOffset.X = minPhysicalOffset;
                newRect.X = targetRectOffset;
                newRect.Width = targetRectSize;
            }
            else
            {
                newOffset.Y = minPhysicalOffset;
                newRect.Y = targetRectOffset;
                newRect.Height = targetRectSize;
            }
        }
  
        private void MakeVisibleLogicalHelper(int childIndex, Rect r, ref Vector newOffset, ref Rect newRect)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            int firstChildInView;
            int newFirstChild;
            int viewportSize;
            double childOffsetWithinViewport = r.Y;
 
            if (fHorizontal)
            {
                firstChildInView = (int)_scrollData._computedOffset.X;
                viewportSize = (int)_scrollData._viewport.Width;
            }
            else
            {
                firstChildInView = (int)_scrollData._computedOffset.Y;
                viewportSize = (int)_scrollData._viewport.Height;
            }
 
            newFirstChild = firstChildInView;
 
            // If the target child is before the current viewport, move the viewport to put the child at the top.
            if (childIndex < firstChildInView)
            {
                childOffsetWithinViewport = 0;
                newFirstChild = childIndex;
            }
            // If the target child is after the current viewport, move the viewport to put the child at the bottom.
            else if (childIndex > firstChildInView + viewportSize - 1)
            {
                newFirstChild = childIndex - viewportSize + 1;
                double pixelSize = fHorizontal ? ActualWidth : ActualHeight;
                childOffsetWithinViewport = pixelSize * (1.0 - (1.0 / viewportSize));
            }
 
            if (fHorizontal)
            {
                newOffset.X = newFirstChild;
                newRect.X = childOffsetWithinViewport;
                newRect.Width = r.Width;
            }
            else
            {
                newOffset.Y = newFirstChild;
                newRect.Y = childOffsetWithinViewport;
                newRect.Height = r.Height;
            }
        }
  
        // Converts an index into the item collection as a double into an int
        static private int CoerceIndexToInteger(double index, int numberOfItems)
        {
            int newIndex;
  
            if (Double.IsNegativeInfinity(index))
            {
                newIndex = 0;
            }
            else if (Double.IsPositiveInfinity(index))
            {
                newIndex = numberOfItems - 1;
            }
            else
            {
                newIndex = (int)index;
                newIndex = Math.Max(Math.Min(numberOfItems - 1, newIndex), 0);
            }
 
            return newIndex;
        }
 
        private int GetGeneratedIndex(int childIndex)
        {
            return Generator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));
        }
  
 
        //
        // Focus Helpers
        //
  
        #region Focus Helpers
 
        //
        // Methods to keep track of focus.
        //
        // Dealing with Focus while virtualizing a list is easy: don't throw away the focused item and the next and previous
        // focusable items.  When in a TreeView it's much harder; Measure (and thus the cleanup code) for any VSP in the hierarchy
        // can run at any time. The only performant way for a panel to know that one of its children may be the next or previous focusable
        // item is for it to be marked.  We do this every time focus changes within the hierarchy.
        //
 
        private WeakReference[] EnsureFocusTrail()
        {
            WeakReference[] focusTrail = FocusTrailField.GetValue(this);
  
            if (focusTrail == null)
            {
                focusTrail = new WeakReference[2];
                FocusTrailField.SetValue(this, focusTrail);
            }
 
            return focusTrail;
        }
  
  
        /// <summary>
        /// Finds the focused child along with the previous and next focusable children.  Used only when recycling containers;
        /// the standard mode has a different cleanup algorithm
        /// </summary>
        /// <param name="focusedChild">
        /// <param name="previousFocusable">
        /// <param name="nextFocusable">
        private void FindFocusedChild(out int focusedChild, out int previousFocusable, out int nextFocusable)
        {
            Debug.Assert(InRecyclingMode, "This method is only valid for the recycling mode");
            Debug.Assert(IsKeyboardFocusWithin, "we should only search for a focusable child if we have focus");
            focusedChild = previousFocusable = nextFocusable = -1;
            UIElement child;
            bool foundFocusedChild = false;
  
            for (int i = 0; i < _realizedChildren.Count; i++)
            {
                child = _realizedChildren[i];
 
                if (!foundFocusedChild && child.IsKeyboardFocusWithin)
                {
                    focusedChild = i;
                    foundFocusedChild = true;
  
                    // Go through the trailing items.
                    // Go through the trailing items and find a focusable item to keep.
                    int trailIndex = i - 1;
                    int end = Math.Max(0, i - FocusTrail);
                    for (; trailIndex >= end; trailIndex--)
                    {
                        child = _realizedChildren[trailIndex];
                        if (Keyboard.IsFocusable(child))
                        {
                            previousFocusable = trailIndex;
                            break;
                        }
                    }
                }
                else if (foundFocusedChild)
                {
                    if (i <= focusedChild + FocusTrail)
                    {
                        if (Keyboard.IsFocusable(child))
                        {
                            nextFocusable = i;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
  
 
        /// <summary>
        /// Called when the focused item has changed.  Used to set a special DP on the next and previous focusable items.
        /// Only used when virtualizing in a hieararchy (i.e. TreeView virtualization).
        /// </summary>
        /// <param name="e">
        private void FocusChanged(KeyboardFocusChangedEventArgs e)
        {
  
            if (IsVirtualizing && IsScrolling && IsPixelBased)
            {
                // IsScrolling ensures that only the top-level panel tracks focus.
                // The IsPixelBased condition here needs explanation.  It's used here to mean 'Is this panel in a hierarchy?'
                // The assert below is just a reminder to modify this code if the meaning changes.
                Debug.Assert(ItemsControl.GetItemsOwner(this) is TreeView);
  
                // This code is TreeViewItem-specific, since it has its own focus logic and we can't override UIElement.PredictFocus
                TreeViewItem focusedElement = Keyboard.FocusedElement as TreeViewItem;
                WeakReference[] focusTrail = EnsureFocusTrail();
 
 
                //
                // Clear the old focus trail items
                //
  
                for (int i = 0; i < 2; i++)
                {
                    DependencyObject trailItem = (DependencyObject)(focusTrail[i] != null ? focusTrail[i].Target : null);
 
                    if (trailItem != null)
                    {
                        FocusTrailItemField.ClearValue(trailItem);
                    }
                }
 
  
                //
                // Set the new focus trail items
                //
                if (IsKeyboardFocusWithin)
                {
                    DependencyObject previous = null;
                    DependencyObject next = null;
 
                    if (focusedElement != null)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            previous = focusedElement.InternalPredictFocus(FocusNavigationDirection.Left);
                            next = focusedElement.InternalPredictFocus(FocusNavigationDirection.Right);
                        }
                        else
                        {
                            previous = focusedElement.InternalPredictFocus(FocusNavigationDirection.Up);
                            next = focusedElement.InternalPredictFocus(FocusNavigationDirection.Down);
                        }
                    }
  
                    if (previous != null)
                    {
                        FocusTrailItemField.SetValue(previous, true);
                        focusTrail[0] = new WeakReference(previous);
                    }
 
                    if (next != null)
                    {
                        FocusTrailItemField.SetValue(next, true);
                        focusTrail[1] = new WeakReference(next);
                    }
                }
                else
                {
                    // Focus has left the tree
                    FocusTrailField.SetValue(this, null);
                }
            }
        }
  
  
        /// <summary>
        /// Checks the precomputed focus trail.  Valid only if we're in a hierararchy.
        /// </summary>
        /// <param name="container">
        /// <returns></returns>
        private bool IsInFocusTrail(UIElement container)
        {
            if (IsPixelBased)
            {
                return FocusTrailItemField.GetValue(container) || container.IsKeyboardFocusWithin;
            }
            else
            {
                return false;
            }
        }
  
  
 
        #endregion
 
        //------------------------------------------------------------
        // Avalon Property Callbacks/Overrides
        //-----------------------------------------------------------
        #region Avalon Property Callbacks/Overrides
  
        /// <summary>
        /// <see cref="PropertyMetadata.PropertyChangedCallback">
        /// </see></summary>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Since Orientation is so essential to logical scrolling/virtualization, we synchronously check if
            // the new value is different and clear all scrolling data if so.
            ResetScrolling(d as VirtualizingStackPanel);
        }
  
        #endregion
  
        #endregion Private Methods
 
        //-----------------------------------------------------
        //
        //  Private Properties
        //
        //-----------------------------------------------------
 
        #region Private Properties
 
        /// <summary>
        /// Index of the last item in the cache window
        /// </summary>
        private int CacheEnd
        {
            get
            {
                // Note we don't have the _afterTrail here:  _afterTrail is already contained inside of _visibleCount.
                int cacheCount = _beforeTrail + _visibleCount + ContainerCacheSize;
 
                if (cacheCount > 0)
                {
                    return _cacheStart + cacheCount - 1;
                }
                else
                {
                    return 0;
                }
            }
        }
  
        /// <summary>
        /// True after the first MeasureOverride call. We can't use UIElement.NeverMeasured because it's set to true by the first call to MeasureOverride.
        /// Stored in a bool field on Panel.
        /// </summary>
        private bool HasMeasured
        {
            get
            {
                return VSP_HasMeasured;
            }
            set
            {
                VSP_HasMeasured = value;
            }
        }
 
        private bool InRecyclingMode
        {
            get
            {
                return _virtualizationMode == VirtualizationMode.Recycling;
            }
        }
 
 
        internal bool IsScrolling
        {
            get { return (_scrollData != null) && (_scrollData._scrollOwner != null); }
        }
  
 
        /// <summary>
        /// Specifies if this panel uses item-based or pixel-based computations in Measure and Arrange.
        ///
        /// Differences between the two:
        ///
        /// When pixel-based mode VSP behaves the same to the layout system virtualized as not; its desired size is the sum
        /// of all its children and it arranges children such that the ones in view appear in the right place.
        /// In this mode VSP is also able to make use of the viewport passed down in MeasureData to virtualize chidren.  When
        /// it's the scrolling panel it computes the offset and extent in pixels rather than logical units.
        ///
        /// When in item mode VSP's desired size grows and shrinks depending on which containers are virtualized and it arranges
        /// all children one on top the the other.
        /// In this mode VSP cannot use the viewport from MeasureData to virtualize; it can only virtualize if it is the scrolling panel
        /// (IsScrolling == true).  Thus its looseness with desired size isn't much of an issue since it owns the extent.
        /// </summary>
        /// <remarks>
        /// This should be private, except that one Debug.Assert in TreeView requires it.
        /// </remarks>
        internal bool IsPixelBased
        {
            get
            {
                // For backwards compat we don't use pixel mode unless we're virtualzing a TreeView or TreeViewItem.  This should
                // be changed if we decide to later publicly expose the pixel-based viewport.
                Debug.Assert(VSP_IsPixelBased == false || IsVirtualizing && (ItemsControl.GetItemsOwner(this) is TreeView || ItemsControl.GetItemsOwner(this) is TreeViewItem));
  
                return VSP_IsPixelBased;
            }
            set
            {
                VSP_IsPixelBased = value;
            }
        }
  
  
        private bool IsVirtualizing
        {
            get
            {
                return VSP_IsVirtualizing;
            }
            set
            {
                // We must be the ItemsHost to turn on Virtualization.
                bool isVirtualizing = IsItemsHost && value;
  
                if (isVirtualizing == false)
                {
                    _realizedChildren = null;
                }
 
                VSP_IsVirtualizing = value;
            }
        }
  
 
        /// <summary>
        /// Returns the list of childen that have been realized by the Generator.
        /// We must use this method whenever we interact with the Generator's index.
        /// In recycling mode the Children collection also contains recycled containers and thus does
        /// not map to the Generator's list.
        /// </summary>
        private IList RealizedChildren
        {
            get
            {
                if (IsVirtualizing && InRecyclingMode)
                {
                    EnsureRealizedChildren();
                    return _realizedChildren;
                }
                else
                {
                    return InternalChildren;
                }
            }
        }
 
  
        private VirtualizationMode VirtualizationMode
        {
            get
            {
                return _virtualizationMode;
            }
            set
            {
                _virtualizationMode = value;
            }
        }
  
 
        #endregion Private Properties
 
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //-----------------------------------------------------
  
        #region Private Fields
 
        // Scrolling and virtualization data.  Only used when this is the scrolling panel (IsScrolling is true).
        // When VSP is in pixel mode _scrollData is in units of pixels.  Otherwise the units are logical.
        private ScrollData _scrollData;
  
        // Virtualization state
        private VirtualizationMode _virtualizationMode;
        private int _visibleStart;                  // index of of the first visible data item
        private int _visibleCount;                  // count of the number of data items visible in the viewport
        private int _cacheStart;                    // index of the first data item in the container cache.  This is always <= _visibleStart
 
        // UIElement collection index of the first visible child container.  This is NOT the data item index. If the first visible container
        // is the 3rd child in the visual tree and contains data item 312, _firstVisibleChildIndex will be 2, while _visibleStart is 312.
        // This is useful because could be several live containers in the collection offscreen (maybe we cleaned up lazily, they couldn't be virtualized, etc).
        // This actually maps directly to realized containers inside the Generator.  It's the index of the first visible realized container.
        // Note that when RecyclingMode is active this is the index into the _realizedChildren collection, not the Children collection.
        private int _firstVisibleChildIndex;
 
        // Used by the Recycling mode to maintain the list of actual realized children (a realized child is one that the ItemContainerGenerator has
        // generated).  We need a mapping between children in the UIElementCollection and realized containers in the generator.  In standard virtualization
        // mode these lists are identical; in recycling mode they are not. When a container is recycled the Generator removes it from its realized list, but
        // for perf reasons the panel keeps these containers in its UIElement collection.  This list is the actual realized children -- i.e. the InternalChildren
        // list minus all recycled containers.
        private List<uielement> _realizedChildren;
 
        // Cleanup
        private DispatcherOperation _cleanupOperation;
        private DispatcherTimer _cleanupDelay;
        private int _beforeTrail = 0;
        private int _afterTrail = 0;
        private const int FocusTrail = 5; // The maximum number of items off the edge we will generate to get a focused item (so that keyboard navigation can work)
        private DependencyObject _bringIntoViewContainer;  // pointer to the container we're about to bring into view; it can't be recycled even if it's offscreen.
  
        // ContainerCacheSize specifies how many items we cache past the viewport boundaries.  Until we expose an API to allow users to tweak this
        // the safest thing is to leave it at 0.
        private const int ContainerCacheSize = 0;
 
        // Global index used by ItemValueStorage to store the DesiredSize of a UIElement when it is a virtualized container.
        // Used by TreeView and TreeViewItem to remember the size of TreeViewItems when they get virtualized away.
        private static int _desiredSizeStorageIndex;
  
        // Holds the 'focus trail': the previous or next focusable item, neither of which can be virtualized.
        // Used only when virtualizing in a hierarchy (i.e. TreeView virtualization).
        private static UncommonField<weakreference[]> FocusTrailField = new UncommonField<weakreference[]>(null);
        private static UncommonField<bool> FocusTrailItemField = new UncommonField<bool>(false);
 
        #endregion Private Fields
  
 
        //------------------------------------------------------
        //
        //  Private Structures / Classes
        //
        //------------------------------------------------------
 
        #region Private Structures Classes
  
        //-----------------------------------------------------------
        // ScrollData class
        //------------------------------------------------------------
        #region ScrollData
  
        // Helper class to hold scrolling data.
        // This class exists to reduce working set when VirtualizingStackPanel is used outside a scrolling situation.
        // Standard "extra pointer always for less data sometimes" cache savings model:
        //      !Scroll [1xReference]
        //      Scroll  [1xReference] + [6xDouble + 1xReference]
        private class ScrollData
        {
            // Clears layout generated data.
            // Does not clear scrollOwner, because unless resetting due to a scrollOwner change, we won't get reattached.
            internal void ClearLayout()
            {
                _offset = new Vector();
                _viewport = _extent = _maxDesiredSize = new Size();
            }
  
            // For Stack/Flow, the two dimensions of properties are in different units:
            // 1. The "logically scrolling" dimension uses items as units.
            // 2. The other dimension physically scrolls.  Units are in Avalon pixels (1/96").
            internal bool _allowHorizontal;
            internal bool _allowVertical;
 
            // Scroll offset of content.  Positive corresponds to a visually upward offset.  Set by methods like LineUp, PageDown, etc.
            internal Vector _offset;
  
            // Computed offset based on _offset set by the IScrollInfo methods.  Set at the end of a successful Measure pass.
            // This is the offset used by Arrange and exposed externally.  Thus an offset set by PageDown via IScrollInfo isn't
            // reflected publicly (e.g. via the VerticalOffset property) until a Measure pass.
            internal Vector _computedOffset = new Vector(0,0);
            internal Size _viewport;            // ViewportSize is in {pixels x items} (or vice-versa).
            internal Size _extent;              // Extent is the total number of children (logical dimension) or physical size
            internal ScrollViewer _scrollOwner; // ScrollViewer to which we're attached.
 
            internal Size _maxDesiredSize;      // Hold onto the maximum desired size to avoid re-laying out the parent ScrollViewer.
        }
 
        #endregion ScrollData
 
 
        /// <summary>
        /// Allows pixel-based virtualization to ask an ItemsControl for the size of its header (if available)
        /// and a size estimate for its containers.  This is used for TreeView virtualization.
        ///
        /// </summary>
        internal interface IProvideStackingSize
        {
            double HeaderSize(bool isHorizontal);
            double EstimatedContainerSize(bool isHorizontal);
        }
  
        #endregion Private Structures Classes
    }
}
 
  
 
// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.