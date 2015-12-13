using FileExplorer.Defines;
using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public interface IUIInputProcessor
    {
        IEnumerable<RoutedEvent> ProcessEvents { get; }
        bool ProcessAllEvents { get; }
        void Update(ref IUIInput input);
    }







    
}
