using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF
{
    public class NullScrollHelper : IPanelScrollHelper
    {

        public NullScrollHelper(IOCPanel panel)
        {
            Extent = ViewPort = Size.Empty;
        }

        public Size Extent { get; set; }
        public Size ViewPort { get; set; }

        public void UpdateScrollInfo(Size availableSize)
        {
            Extent = ViewPort = availableSize;
        }

        public Point Offset
        {
            get { return new Point(); }
        }
        

        public void UpdateOffsetX(OffsetType type, double value)
        {
        }

        public void UpdateOffsetY(OffsetType type, double value)
        {
        }

        public void UpdateOffset(OffsetType type, double value)
        {
        }
    }
}
