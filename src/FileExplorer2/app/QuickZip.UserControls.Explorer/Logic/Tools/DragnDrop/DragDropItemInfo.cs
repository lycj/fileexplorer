using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    /// <summary>
    /// Represent an item, DragDropInfo may include 1 to n DragDropItemInfo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DragDropItemInfo<T>
    {
        public bool IsFolder { get; set; }
        public bool IsTemp { get; set; }
        public T EmbeddedItem { get; set; }
        public string FileSystemPath { get; set; }        
    }

   
}
