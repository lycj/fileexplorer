using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF
{
    public class FixedWrapLayout : IPanelLayoutHelper
    {
        #region fields

        protected IOCPanel _panel;
        protected IItemGeneratorHelper _generator;

        #endregion

        #region constructors

        public FixedWrapLayout(IOCPanel panel, IItemGeneratorHelper generator)
        {
            _panel = panel;
            _generator = generator;
            Extent = Size.Empty;
        }

        #endregion

        #region events

        #endregion

        #region properties

        public Orientation DefaultScrollOrientation
        {
            get
            {
                return _panel.Orientation == Orientation.Horizontal ?
                    Orientation.Vertical : Orientation.Horizontal;
            }
        }
        public Size Extent { get; set; }
        public ChildInfo this[int idx]
        {
            get
            {
                return new ChildInfo()
                {
                    DesiredSize = _panel.ChildSize,
                    ArrangedRect = getChildRect(idx, new Size(_panel.ActualWidth, _panel.ActualHeight))
                };
            }
        }

        #endregion

        #region methods

        public void ResetLayout(int idx)
        {
        }

        public Size Measure(Size availableSize)
        {
            // Figure out range that's visible based on layout algorithm
            int startIdx, endIdx;
            getVisibleRange(out startIdx, out endIdx);

            //Add and subtract CacheItemCount to StartIdx and EndIdx.
            _panel.UpdateCacheItemCount(ref startIdx, ref endIdx);

            //Generate new items and cleaup unused items.
            _panel.Generator.Generate(startIdx, endIdx);
            _panel.Generator.CleanUp(startIdx, endIdx);

            //Output extent so it can be accessed by PanelScrollHelper.
            Extent = calculateExtent(availableSize, _panel.getItemCount());

            //And return availableSize (take all available).
            return availableSize;
        }

        public Size Arrange(Size finalSize)
        {
            foreach (UIElement child in _panel.Children)
            {
                // Map the child offset to an item offset 
                // Note: generator location is different compared to item index.
                int itemIndex = _panel.Generator[child];

                //Calculate children rect
                Rect childRect = getChildRect(itemIndex, finalSize);

                //Call arrange.
                child.Arrange(childRect);
            }
            return finalSize;
        }

        #region Helper

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="startIdx">The item index of the first visible item</param>
        /// <param name="endIdx">The item index of the last visible item</param>
        private void getVisibleRange(out int startIdx, out int endIdx)
        {
            Point offset = _panel.Scroll.Offset;
            Size viewport = _panel.Scroll.ViewPort;
            Size extent = _panel.Scroll.Extent;

            if (_panel.Orientation == Orientation.Horizontal)
            {
                int childPerRow = calculateChildrenPerRow(extent);

                startIdx = (int)Math.Floor(offset.Y / _panel.ChildSize.Height) * childPerRow;
                endIdx = (int)Math.Ceiling((offset.Y + viewport.Height) / _panel.ChildSize.Height) * childPerRow - 1;
            }
            else
            {
                int childPerCol = calculateChildrenPerCol(extent);

                startIdx = (int)Math.Floor(offset.X / _panel.ChildSize.Width) * childPerCol;
                endIdx = (int)Math.Ceiling((offset.X + viewport.Width) / _panel.ChildSize.Width) * childPerCol - 1;               
            }

        }

        private Rect getChildRect(int itemIndex, Size finalSize)
        {
            if (_panel.Orientation == Orientation.Horizontal)
            {
                int childPerRow = calculateChildrenPerRow(finalSize);

                int row = itemIndex / childPerRow;
                int column = itemIndex % childPerRow;

                return new Rect(column * _panel.ChildSize.Width, row * _panel.ChildSize.Height, 
                    _panel.ChildSize.Width, _panel.ChildSize.Height);
            }
            else
            {
                int childPerCol = calculateChildrenPerCol(finalSize);

                int column = itemIndex / childPerCol;
                int row = itemIndex % childPerCol;

                return new Rect(column * _panel.ChildSize.Width, row * _panel.ChildSize.Height, 
                    _panel.ChildSize.Width, _panel.ChildSize.Height);
            }
        }

        private Size calculateExtent(Size availableSize, int itemCount)
        {
            if (_panel.Orientation == Orientation.Horizontal)
            {
                int childPerRow = calculateChildrenPerRow(availableSize);
                return new Size(childPerRow * _panel.ChildSize.Width,
                    _panel.ChildSize.Height * Math.Ceiling((double)itemCount / childPerRow));
            }
            else
            {
                int childPerCol = calculateChildrenPerCol(availableSize);
                return new Size(_panel.ChildSize.Width * Math.Ceiling((double)itemCount / childPerCol),
                    childPerCol * _panel.ChildSize.Height);
            }
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int calculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow;
            if (availableSize.Width == Double.PositiveInfinity)
                childrenPerRow = _panel.Children.Count;
            else
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / _panel.ChildSize.Width));
            return childrenPerRow;
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int calculateChildrenPerCol(Size availableSize)
        {
            // Figure out how many children fit on each row            
            if (availableSize.Height == Double.PositiveInfinity)
                return _panel.Children.Count;
            else
                return Math.Max(1, (int)Math.Floor(availableSize.Height / _panel.ChildSize.Height));
        }

        #endregion


        #endregion
    }
}
