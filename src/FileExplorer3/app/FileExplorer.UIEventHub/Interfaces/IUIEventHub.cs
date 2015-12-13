using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.BaseControls
{

    public interface IUIEventHub
    {
        UIElement Control { get; }        
        IList<UIEventProcessorBase> EventProcessors { get; }
        IList<IUIInputProcessor> InputProcessors { get; }
        bool IsEnabled { get; set; }
    }

}
