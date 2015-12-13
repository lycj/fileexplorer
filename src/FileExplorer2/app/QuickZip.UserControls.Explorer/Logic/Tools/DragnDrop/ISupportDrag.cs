using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{


    public interface ISupportDrag<T>
    {
        /// <summary>
        /// Return supported drag drop effects.
        /// </summary>
        DragDropEffects SupportedDragDropEffects { get; }

        /// <summary>
        /// Return the selected items.
        /// </summary>
        T[] SelectedItems { get; }
        
        /// <summary>
        /// Return a local Filesystem based file or directory path of specified entry or entry model, 
        /// A temp file/directory is created It must be created only AFTER BeforeDrop is called.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        DragDropItemInfo<T> GetItemInfo(T item);

        /// <summary>
        /// Called before VirtualDataObject.DoDragDrop is called, last chance to cancel the drag.
        /// </summary>
        /// <param name="dropInfo"></param>
        bool BeforeDrag(DragDropInfo<T> dropInfo);

        /// <summary>
        /// PrepareDrop is called right before user drop the files to any target. (Called by VirtualDataObject)
        /// </summary>
        /// <param name="dropInfo"></param>
        void PrepareDrop(DragDropInfo<T> dropInfo);
    }
}
