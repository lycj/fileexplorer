using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{

    public interface ISupportDragHelper
    {
        ISupportDrag DragHelper { get; }
    }

    public interface ISupportDrag
    {        
        bool HasDraggables { get; }
        bool IsDraggingFrom { get; set; }
        IEnumerable<IDraggable> GetDraggables();
        DragDropEffectsEx QueryDrag(IEnumerable<IDraggable> draggables);
        void OnDragCompleted(IEnumerable<IDraggable> draggables, DragDropEffectsEx effect);
    }

    public class NullSupportDrag : ISupportDrag
    {
        public static NullSupportDrag Instance = new NullSupportDrag();
        public bool HasDraggables { get { return false; } }
        public bool IsDraggingFrom { get; set; }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return new List<IDraggable>();
        }

        public DragDropEffectsEx QueryDrag(IEnumerable<IDraggable> draggables)
        {
            return DragDropEffectsEx.None;
        }
      

        public void OnDragCompleted(IEnumerable<IDraggable> draggables, DragDropEffectsEx effect)
        {
           
        }
    }
}
