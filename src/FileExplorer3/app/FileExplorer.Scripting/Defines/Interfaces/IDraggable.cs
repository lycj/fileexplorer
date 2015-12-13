using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    /// <summary>
    /// Indicate an item is draggable.
    /// </summary>
    public interface IDraggable : IUIAware
    {        
        /// <summary>
        /// Whether the item is dragging, set by UIEventHub.
        /// </summary>
        bool IsDragging { get;  set; }
    }

}
