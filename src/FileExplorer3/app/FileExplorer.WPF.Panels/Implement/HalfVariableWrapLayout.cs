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
    public class HalfVariableWrapLayout : IPanelLayoutHelper
    {
        #region fields

        private ConcurrentDictionary<int, double> _lineSizeDictionary;
        protected IOCPanel _panel;
        protected IItemGeneratorHelper _generator;

        #endregion

        #region constructors

        public HalfVariableWrapLayout(IOCPanel panel, IItemGeneratorHelper generator)
        {
            _panel = panel;
            _generator = generator;
            _lineSizeDictionary = new ConcurrentDictionary<int, double>();
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

        public void ResetLayout(int idx = -1)
        {
            if (idx == -1)
                _lineSizeDictionary.Clear();
            else
            {
                if (_panel.Orientation == Orientation.Horizontal)
                {
                    int childPerRow = calculateChildrenPerRow(new Size(_panel.ActualWidth, _panel.ActualHeight));

                    int row = idx / childPerRow;
                    double value;
                    _lineSizeDictionary.TryRemove(row, out value);
                }
                else
                {
                    int childPerCol = calculateChildrenPerCol(new Size(_panel.ActualWidth, _panel.ActualHeight));

                    int column = idx / childPerCol;
                    double value;
                    _lineSizeDictionary.TryRemove(column, out value);
                }
            }
        }


        public Size Measure(Size availableSize)
        {
            int startIdx, endIdx;
            // Figure out range that's visible based on layout algorithm
            getVisibleRange(availableSize, out startIdx, out endIdx);

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

        #region Helpers

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="startIdx">The item index of the first visible item</param>
        /// <param name="endIdx">The item index of the last visible item</param>
        private void getVisibleRange(Size availableSize, out int startIdx, out int endIdx)
        {
            double topPos = 0;
            int itemCount = _panel.getItemCount();

            double viewPortSize = _panel.Scroll.ViewPort.Height;
            int itemPerLine = calculateChildrenPerRow(availableSize);
            double offset = _panel.Scroll.Offset.Y;

            if (_panel.Orientation == Orientation.Vertical)
            {
                itemPerLine = calculateChildrenPerCol(availableSize);
                viewPortSize = _panel.Scroll.ViewPort.Width;
                offset = _panel.Scroll.Offset.X;
            }
            int totalLine = (int)Math.Ceiling((double)itemCount / itemPerLine);


            startIdx = 0;
            endIdx = itemCount - 1;
            int startLine = 0;
            for (int lineIdx = 0; lineIdx < totalLine; lineIdx++)
            {
                if (!_lineSizeDictionary.ContainsKey(lineIdx))
                {
                    for (int lineItem = 0; lineItem < itemPerLine; lineItem++)
                    {
                        int idx = (lineIdx * itemPerLine) + lineItem;
                        double lineSize;
                        if (_panel.Orientation == Orientation.Horizontal)
                            lineSize = _generator.Measure(idx, new Size(_panel.ItemWidth, double.PositiveInfinity)).Height;
                        else
                            lineSize = _generator.Measure(idx, new Size(availableSize.Width, _panel.ItemHeight)).Width;                        
                        _lineSizeDictionary.AddOrUpdate(lineIdx, lineSize, (r, cur) => Math.Max(cur, lineSize));
                    }                    
                }
                if (topPos >= offset)
                {
                    startIdx = Math.Max(0, lineIdx * (itemPerLine - 1));
                    startLine = lineIdx;
                    break;
                }
                topPos += _lineSizeDictionary[lineIdx];
            }

            for (int lineIdx = startLine; lineIdx < totalLine; lineIdx++)
            {
                if (!_lineSizeDictionary.ContainsKey(lineIdx))
                {
                    for (int lineItem = 0; lineItem < itemPerLine; lineItem++)
                    {
                        int idx = (lineIdx * itemPerLine) + lineItem;
                        double lineSize;
                        if (_panel.Orientation == Orientation.Horizontal)
                            lineSize = _generator.Measure(idx, new Size(_panel.ItemWidth, availableSize.Height)).Height;
                        else
                            lineSize = _generator.Measure(idx, new Size(availableSize.Width, _panel.ItemHeight)).Width;

                        _lineSizeDictionary.AddOrUpdate(lineIdx, lineSize, (r, cur) => Math.Max(cur, lineSize));
                    }


                }
                if (topPos > viewPortSize + offset)
                {
                    endIdx = (lineIdx * itemPerLine) + itemPerLine - 1;
                    break;
                }

                topPos += _lineSizeDictionary[lineIdx];
            }


        }

        private double getLineTop(int lineIdx)
        {
            double curSize = 0;
            for (int l = 0; l < lineIdx; l++)
                curSize += getLineSize(l);
            return curSize;
        }

        private double getLineSize(int lineIdx)
        {
            if (_lineSizeDictionary.ContainsKey(lineIdx))
                return _lineSizeDictionary[lineIdx];
            else
            {
                if (_panel.Orientation == Orientation.Horizontal)
                    return _panel.ItemHeight;
                else return _panel.ItemWidth;
            }
        }


        private Rect getChildRect(int itemIndex, Size finalSize)
        {
            if (_panel.Orientation == Orientation.Horizontal)
            {
                int childPerRow = calculateChildrenPerRow(finalSize);

                int row = itemIndex / childPerRow;
                int column = itemIndex % childPerRow;


                return new Rect(column * _panel.ChildSize.Width, getLineTop(row),
                    _panel.ChildSize.Width, getLineSize(row));
            }
            else
            {
                int childPerCol = calculateChildrenPerCol(finalSize);

                int column = itemIndex / childPerCol;
                int row = itemIndex % childPerCol;

                return new Rect(getLineTop(column), row * _panel.ChildSize.Height,
                    getLineSize(column), _panel.ChildSize.Height);
            }
        }

        private Size calculateExtent(Size availableSize, int itemCount)
        {
            if (_panel.Orientation == Orientation.Horizontal)
            {
                int childPerRow = calculateChildrenPerRow(availableSize);
                int maxRowIdx = (int)Math.Ceiling((double)itemCount / childPerRow);
                return new Size(childPerRow * _panel.ChildSize.Width, getLineTop(maxRowIdx));
            }
            else
            {
                int childPerCol = calculateChildrenPerCol(availableSize);
                int maxColIdx = (int)Math.Ceiling((double)itemCount / childPerCol);
                return new Size(getLineTop(maxColIdx), childPerCol * _panel.ChildSize.Height);
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
