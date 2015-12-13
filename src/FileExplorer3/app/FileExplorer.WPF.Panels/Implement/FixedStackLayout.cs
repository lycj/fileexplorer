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
    public class FixedStackLayout : IPanelLayoutHelper
    {
        #region fields

        protected IOCPanel _panel;
        protected IItemGeneratorHelper _generator;
        protected ConcurrentDictionary<int, Size> _desiredSizeDic;

        #endregion

        #region constructors

        public FixedStackLayout(IOCPanel panel, IItemGeneratorHelper generator)
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
            return Extent;// availableSize;
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

        private Size calculateExtent(Size availableSize, int itemCount)
        {
            if (_panel.Orientation == Orientation.Horizontal)
                return new Size(itemCount * _panel.ChildSize.Width, _panel.Scroll.ViewPort.Height);
            else return new Size(_panel.Scroll.ViewPort.Width, _panel.ChildSize.Height * itemCount);
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="startIdx">The item index of the first visible item</param>
        /// <param name="endIdx">The item index of the last visible item</param>
        private void getVisibleRange(out int startIdx, out int endIdx)
        {
            Point offset = _panel.Scroll.Offset;
            Size viewport = _panel.Scroll.ViewPort;


            if (_panel.Orientation == Orientation.Horizontal)
            {
                startIdx = (int)Math.Floor(offset.X / _panel.ChildSize.Width);
                endIdx = (int)Math.Ceiling((offset.X + viewport.Width) / _panel.ChildSize.Width) - 1;
            }
            else
            {
                startIdx = (int)Math.Floor(offset.Y / _panel.ChildSize.Height);
                endIdx = (int)Math.Ceiling((offset.Y + viewport.Height) / _panel.ChildSize.Height) - 1;
            }

        }

        private Rect getChildRect(int itemIndex, Size finalSize)
        {
            Size viewport = _panel.Scroll.ViewPort;

            if (_panel.Orientation == Orientation.Horizontal)
            {
                double lineLeft = itemIndex * _panel.ChildSize.Width;
                double lineWidth = _panel.ChildSize.Width;
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
                double lineTop = itemIndex * _panel.ChildSize.Height;
                double lineHeight = _panel.ChildSize.Height;
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
