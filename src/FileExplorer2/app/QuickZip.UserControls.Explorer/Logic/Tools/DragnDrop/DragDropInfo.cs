using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    public class DragDropInfo<T>
    {
        #region Constructor

        public DragDropInfo(ISupportDrag<T> dragSource)
        {
            SupportedEffects = dragSource.SupportedDragDropEffects;
            SelectedItems = new List<DragDropItemInfo<T>>();
            HandledInternally = false;

            if (SupportedEffects != DragDropEffects.None)
            {
                T[] selectedItems = dragSource.SelectedItems;
                if (selectedItems != null && selectedItems.Length > 0)
                    foreach (T selItem in selectedItems)
                        SelectedItems.Add(dragSource.GetItemInfo(selItem));
            }
        }

        #endregion

        #region Public Properties

        public DragDropEffects SupportedEffects { get; private set; }
        public List<DragDropItemInfo<T>> SelectedItems { get; private set; }
        
        /// <summary>
        /// When true, ISupportDrag.PrepareDrop() is NOT called.
        /// </summary>
        public bool HandledInternally { get; set; }

        #endregion
    }

}
