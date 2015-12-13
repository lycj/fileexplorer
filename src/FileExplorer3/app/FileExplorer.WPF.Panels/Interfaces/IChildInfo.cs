using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public interface IChildInfo
    {
        Rect GetChildRect(int itemIndex);
    }
}
