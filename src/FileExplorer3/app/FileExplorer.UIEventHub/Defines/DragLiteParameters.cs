using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public enum DragMode
    {
        None, //Not dragging.
        DoDragDrop, //By calling DoDragDrop(), default mode. 
        Lite //By setting DraggingDataObject
    }

    //public static class DragLiteParameters
    //{
    //    public static DragMode DragMode = DragMode.None;
    //    private static IDraggable[] _draggingItems = null;
    //    public static IDraggable[] DraggingItems
    //    {
    //        get { return _draggingItems; }
    //        set
    //        {
    //            if (_draggingItems != null)
    //                foreach (var d in _draggingItems) d.IsDragging = false;
    //            _draggingItems = value;
    //            if (_draggingItems != null)
    //                foreach (var d in _draggingItems) d.IsDragging = true;
    //        }
    //    }

    //    public static UIInputType DragInputType = UIInputType.None;
    //    public static ISupportDrag DragSource = null;
    //    public static DragDropEffectsEx Effects;
    //}

}
