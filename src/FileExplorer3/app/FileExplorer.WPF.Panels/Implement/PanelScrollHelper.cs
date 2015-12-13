using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer.WPF
{
    public class PanelScrollHelper : IPanelScrollHelper
    {
        #region fields

        private IOCPanel _panel;
        private Size _extent, _viewport;
        private TranslateTransform _trans;
        private Point _offset;
        private object _lockObj = new object();

        #endregion

        #region constructors

        public PanelScrollHelper(IOCPanel panel, ScrollViewer scrollOwner)
        {
            _panel = panel;
            _panel.RenderTransform = _trans = new TranslateTransform(); // For use in the IScrollInfo implementation
            _extent = Size.Empty;
            _viewport = Size.Empty;
            _owner = scrollOwner;
            _offset = new Point(0, 0);

        }

        #endregion

        #region events

        #endregion

        #region properties

        public EventHandler OnViewPortChanged = (o, e) => { };
        private ScrollViewer _owner;


        public Size ViewPort { get { return _viewport; } }
        public Size Extent { get { return _extent; } }
        public Point Offset { get { return _offset; } }

        #endregion

        #region methods

        /// <summary>
        /// See Ben Constable's series of posts at http://blogs.msdn.com/bencon/
        /// </summary>
        /// <param name="availableSize"></param>
        public void UpdateScrollInfo(Size availableSize)
        {                        
            // Update viewport
            if (availableSize != _viewport)
            {
                _viewport = availableSize;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
                _panel.Layout.ResetLayout();
            }
            
            // Update extent
            if (_extent != _panel.Layout.Extent)
            {
                _extent = _panel.Layout.Extent;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }

        }


        public void UpdateOffsetX(OffsetType type, double value)
        {
            double newOffset =
                type == OffsetType.Fixed ? value : _offset.X + value;

            if (_panel.CanHorizontallyScroll)
            {
                if (newOffset < 0 || _viewport.Width >= _extent.Width)
                {
                    newOffset = 0;
                }
                else
                {
                    if (newOffset + _viewport.Width >= _extent.Width)
                    {
                        newOffset = _extent.Width - _viewport.Width;
                    }
                }

                _offset.X = newOffset;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
                _trans.X = -newOffset;
                // Force us to realize the correct children
                _panel.InvalidateMeasure();
            }
        }

        public void UpdateOffsetY(OffsetType type, double value)
        {
            double newOffset =
                type == OffsetType.Fixed ? value : _offset.Y + value;

            if (newOffset != _offset.Y && _panel.CanVerticallyScroll)
            {
                if (newOffset < 0 || _viewport.Height >= _extent.Height)
                {
                    newOffset = 0;
                }
                else
                {
                    if (newOffset + _viewport.Height >= _extent.Height)
                    {
                        newOffset = _extent.Height - _viewport.Height;
                    }
                }

                _offset.Y = newOffset;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
                _trans.Y = -newOffset;
                // Force us to realize the correct children
                _panel.InvalidateMeasure();
            }

        }

        public void UpdateOffset(OffsetType type, double value)
        {
            if (_panel.Layout.DefaultScrollOrientation == Orientation.Horizontal)
                UpdateOffsetX(type, value);
            else UpdateOffsetY(type, value);

        }

        #endregion


    }
}
