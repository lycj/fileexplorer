using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF
{
    public interface IPanelLayoutHelper
    {
        Orientation DefaultScrollOrientation { get; }
        ChildInfo this[int idx] { get; }
        void ResetLayout(int idx = -1);
        Size Extent { get; }
        
        Size Measure(Size availableSize);
        Size Arrange(Size finalSize);
    }
}
