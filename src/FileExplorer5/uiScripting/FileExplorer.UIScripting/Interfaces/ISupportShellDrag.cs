using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{

   

    public interface ISupportShellDrag : ISupportDrag
    {
        IDataObject GetDataObject(IEnumerable<IDraggable> draggables);
        void OnDragCompleted(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffectsEx effect);
    }
    
}
