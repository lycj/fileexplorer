using FileExplorer.Defines;
using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public interface IDragDropHandler 
    {
        ISupportDrag GetDragHelper(IEnumerable<IEntryModel> entries);
        ISupportDrop GetDropHelper(IEntryModel destEm);
    }
}
