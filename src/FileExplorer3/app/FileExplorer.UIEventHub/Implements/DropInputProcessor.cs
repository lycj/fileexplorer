using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{

    public class DropInputProcessor : InputProcessorBase
    {
        public DropInputProcessor()
        {
            ProcessAllEvents = false;
            _processEvents.AddRange(new[] { 
                UIElement.DragEnterEvent,
                UIElement.DragLeaveEvent,

                UIElement.DragOverEvent,
                UIElement.DropEvent
            }
            );
        }

        public override void Update(ref IUIInput input)
        {
            input = new DragInput(input);
        }
    }
}
