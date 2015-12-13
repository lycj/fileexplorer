using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.BaseControls
{

    /// <summary>
    /// Allow one application (e.g. dragging) to handle events from an control.
    /// </summary>
    public interface IUIEventProcessor
    {
        int Priority { get; }
        string TargetName { get; set; }
        IScriptCommand OnEvent(RoutedEvent eventId);
        IEnumerable<RoutedEvent> ProcessEvents { get; }
    }
    
}
