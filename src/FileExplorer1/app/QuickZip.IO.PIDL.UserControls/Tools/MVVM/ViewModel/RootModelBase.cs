using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Tools;
using System.IO;
using QuickZip.IO.PIDL.UserControls;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class RootModelBase : Cinch.ViewModelBase
    {
        public event ProgressEventHandler OnProgress;

        internal void RaiseProgressEvent(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
        {
            if (OnProgress != null)
                OnProgress(this, new ProgressEventArgs(id, text, work, workStatus, workResult));
        }

    }
}
