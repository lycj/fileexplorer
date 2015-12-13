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
    public class VariableStackLayout : IPanelLayoutHelper
    {
        #region fields

        protected IOCPanel _panel;
        protected IItemGeneratorHelper _generator;
        protected ConcurrentDictionary<int, Size> _desiredSizeDic;        

        #endregion

        #region constructors

        public VariableStackLayout(IOCPanel panel, IItemGeneratorHelper generator)
        {
            _panel = panel;
            _generator = generator;
            _desiredSizeDic = new ConcurrentDictionary<int, Size>();
            Extent = Size.Empty;
        }

        #endregion

        #region events

        #endregion

        #region properties

        public Orientation DefaultScrollOrientation { get { return _panel.Orientation; } }
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
                _desiredSizeDic.Clear();
            else
            {
                Size value;
                _desiredSizeDic.TryRemove(idx, out value);
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

        private void getVisibleRange(Size availableSize, out int startIdx, out int endIdx)
        {
            double topPos = 0;
            int itemCount = _panel.getItemCount();
            double viewPortSize = _panel.Scroll.ViewPort.Width;
            double offset = _panel.Scroll.Offset.X;
            if (_panel.Orientation == Orientation.Vertical)
            {
                viewPortSize = _panel.Scroll.ViewPort.Height;
                offset = _panel.Scroll.Offset.Y;
            }

            startIdx = endIdx = itemCount - 1;
            for (int idx = 0; idx < itemCount; idx++)
            {
                double lineSize = getLineSize(idx, availableSize);
                if (topPos >= offset)
                {
                    startIdx = idx;
                    break;
                }
                topPos += lineSize;
            }

            for (int idx = startIdx; idx < itemCount; idx++)
            {
                double lineSize = getLineSize(idx, availableSize);
                if (topPos > viewPortSize + offset)
                {
                    endIdx = idx;
                    break;
                }
                topPos += lineSize;
            }
        }


        private Size calculateExtent(Size availableSize, int itemCount)
        {
            double curSize = getLineTop(itemCount);
            if (_panel.Orientation == Orientation.Horizontal)
                return new Size(curSize, _panel.Scroll.ViewPort.Height);
            else return new Size(_panel.Scroll.ViewPort.Width, curSize);
        }

        private double getLineTop(int itemIdx)
        {
            double curSize = 0;
            for (int idx = 0; idx < itemIdx; idx++)
                curSize += getLineSize(idx);
            return curSize;
        }

        private double getLineSize(int idx)
        {
            if (_desiredSizeDic.ContainsKey(idx))                
               return (_panel.Orientation == Orientation.Horizontal) ?
                _desiredSizeDic[idx].Width : _desiredSizeDic[idx].Height;

            if (_panel.Orientation == Orientation.Horizontal)
                return _panel.ChildSize.Width;
            else return _panel.ChildSize.Height;
        }

        private double getLineSize(int idx, Size availableSize)
        {
            if (_desiredSizeDic.ContainsKey(idx))
                return getLineSize(idx);

            _desiredSizeDic[idx] = _generator.Measure(idx, availableSize);
            return (_panel.Orientation == Orientation.Horizontal) ?
                _desiredSizeDic[idx].Width : _desiredSizeDic[idx].Height;
        }

        private Rect getChildRect(int itemIndex, Size finalSize)
        {
            Size viewport = _panel.Scroll.ViewPort;

            if (_panel.Orientation == Orientation.Horizontal)
            {
                double lineLeft = getLineTop(itemIndex);
                double lineWidth = getLineSize(itemIndex);
                Func<Size> getDesiredSize = () => _desiredSizeDic.ContainsKey(itemIndex) ? _desiredSizeDic[itemIndex] :
                   _desiredSizeDic[itemIndex] = _generator.Measure(itemIndex, finalSize);
                switch (_panel.VerticalContentAlignment)
                {
                    case VerticalAlignment.Top: return new Rect(lineLeft, 0, getDesiredSize().Width, getDesiredSize().Height);
                    case VerticalAlignment.Bottom: return new Rect(lineLeft, viewport.Height - getDesiredSize().Height, getDesiredSize().Width, getDesiredSize().Height);
                    case VerticalAlignment.Center: return new Rect(lineLeft, (viewport.Height - getDesiredSize().Height) / 2, getDesiredSize().Width, getDesiredSize().Height);
                    default: return new Rect(lineLeft, 0, lineWidth, viewport.Height);
                }
            }
            else
            {
                double lineTop = getLineTop(itemIndex);
                double lineHeight = getLineSize(itemIndex);
                Func<Size> getDesiredSize = () => _desiredSizeDic.ContainsKey(itemIndex) ? _desiredSizeDic[itemIndex] :
                   _desiredSizeDic[itemIndex] = _generator.Measure(itemIndex, finalSize);
                switch (_panel.HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Left: return new Rect(0, lineTop, getDesiredSize().Width, getDesiredSize().Height);
                    case HorizontalAlignment.Right: return new Rect(viewport.Width - getDesiredSize().Width, lineTop, getDesiredSize().Width, getDesiredSize().Height);
                    case HorizontalAlignment.Center: return new Rect((viewport.Width - getDesiredSize().Width) / 2, lineTop, getDesiredSize().Width, getDesiredSize().Height);
                    default: return new Rect(0, lineTop, viewport.Width, lineHeight);
                }
            }
        }

        #endregion

        #endregion


    }
}

