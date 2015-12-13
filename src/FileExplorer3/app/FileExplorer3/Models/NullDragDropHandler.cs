using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Models;
using FileExplorer.Defines;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public class NullDragDropHandler : IDragDropHandler
    {
        private static ISupportDrag dragHelper = new NullSupportDrag();
        private static ISupportDrop dropHelper = new NullSupportDrop();
        public static IDragDropHandler Instance = new NullDragDropHandler();

        private NullDragDropHandler()
        {            
        }

        public ISupportDrag GetDragHelper(IEnumerable<IEntryModel> entries)
        {
            return dragHelper;
        }

        public ISupportDrop GetDropHelper(IEntryModel destEm)
        {
            return dropHelper;
        }
    }
}
