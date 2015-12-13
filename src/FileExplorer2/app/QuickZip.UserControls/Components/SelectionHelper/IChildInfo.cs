using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickZip.UserControls
{
    public interface IChildInfo
    {        
        Rect GetChildRect(int itemIndex);
    }

}
