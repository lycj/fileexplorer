using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    public interface IDropTarget<T>
    {
        bool IsDropEnabled { get; }

        /// <summary>
        /// Return supported drag drop effects.
        /// </summary>
        DragDropEffects QueryDrop(DragDropEffects sourceEffects, DragDropInfo<T> dropInfo);
        DragDropEffects QueryDrop(DragDropEffects sourceEffects, string[] droppingFiles);

        DragDropEffects Drop(DragDropEffects sourceEffects, DragDropInfo<T> dropInfo);
        DragDropEffects Drop(DragDropEffects sourceEffects, string[] droppingFiles);

        void NotifyRefresh();
    }


    public interface ISupportDrop<T>
    {        
        IDropTarget<T> CurrentDropTarget { get; }        
    }
}
