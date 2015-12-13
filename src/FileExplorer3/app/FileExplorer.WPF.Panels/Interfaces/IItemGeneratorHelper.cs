using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF
{
    public interface IItemGeneratorHelper
    {
        Dictionary<int, UIElement> Generate(int startIdx, int endIdx);
        void CleanUp(int minDesiredGenerated, int maxDesiredGenerated);
        Size Measure(int idx, Size availableSize);
        void Arrange(int idx, Rect finalRect);
        int this[UIElement ele] { get; }
        UIElement this[int idx] { get; }
    }
}
