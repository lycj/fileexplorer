using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Defines
{
    public enum ProgressType { Waiting, Running, Error, Completed }

    public enum DragDropEffectsEx
    {
        // Summary:
        //     Scrolling is about to start or is currently occurring in the drop target.
        Scroll = -2147483648,
        //
        // Summary:
        //     The data is copied, removed from the drag source, and scrolled in the drop
        //     target.
        All = -2147483645,
        //
        // Summary:
        //     The drop target does not accept the data.
        None = 0,
        //
        // Summary:
        //     The data is copied to the drop target.
        Copy = 1,
        //
        // Summary:
        //     The data from the drag source is moved to the drop target.
        Move = 2,
        //
        // Summary:
        //     The data from the drag source is linked to the drop target.
        Link = 4
    }
}
