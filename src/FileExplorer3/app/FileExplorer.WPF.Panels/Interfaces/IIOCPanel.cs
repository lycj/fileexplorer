using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FileExplorer.WPF
{
    public interface IIOCPanel : IScrollInfo
    {
        IPanelScrollHelper Scroll { get; set; }
        IItemGeneratorHelper Generator { get; set; }
        IPanelLayoutHelper Layout { get; set; }

        Orientation Orientation { get; set; }
        uint SmallChanges { get; set; }
    }
}
