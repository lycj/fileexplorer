using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.UserControls
{
    public interface IShowMessageBox : IDialogCommon
    {        
        string Message { get; set; }
    }
}
