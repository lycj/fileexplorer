using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{

    public class InputProcessorBase : IUIInputProcessor
    {
        protected List<RoutedEvent> _processEvents;
        public InputProcessorBase()
        {
            ProcessAllEvents = false;
            _processEvents = new List<RoutedEvent>();
        }

        public virtual void Update(ref IUIInput input)
        {
        }

        public bool ProcessAllEvents { get; protected set; }

        public IEnumerable<RoutedEvent> ProcessEvents { get { return _processEvents; } }

    }
}
