using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }
        bool IsSelecting { get; set; }
    }
}
