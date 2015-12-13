using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF
{
    public interface IPanelScrollHelper
    {
        Size Extent { get; }
        Size ViewPort { get; }
        Point Offset { get; }
        void UpdateOffsetX(OffsetType type, double value);
        void UpdateOffsetY(OffsetType type, double value);
        void UpdateOffset(OffsetType type, double value);
        void UpdateScrollInfo(Size availableSize);
    }
}
