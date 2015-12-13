using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.IO.Tools;
using System.IO;
//using QuickZip.IO.COFE.UserControls;
using QuickZip.IO.COFE;

namespace QuickZip.UserControls.MVVM
{
    public class RootModelBase : Cinch.ViewModelBase
    {
        //public event ProgressEventHandler OnProgress;
        private bool _disposed = false;

        public bool IsDisposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }
        

        protected override void OnDispose()
        {
            IsDisposed = true;
            base.OnDispose();
        }


        //internal void RaiseProgressEvent(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
        //{
        //    if (OnProgress != null)
        //        OnProgress(this, new ProgressEventArgs(id, text, work, workStatus, workResult));
        //}

        //protected FileSystemInfoEx GetEmbeddedItem(IFileSystemInfoExA fileInfoA)
        //{
        //    if (fileInfoA is IVirtualFileSystemInfoExA && !(fileInfoA as IVirtualFileSystemInfoExA).IsExpanded)
        //    {
        //        uint id = ProgressEventArgs.NewID();
        //        RaiseProgressEvent(id, "GetPath ->" + fileInfoA.Label, WorkType.Copy, WorkStatusType.wsRunning, WorkResultType.wrSuccess);
        //        try
        //        {
        //            FileSystemInfoEx retVal = COFETools.GetEmbeddedItem(fileInfoA);
        //            RaiseProgressEvent(id, "Completed", WorkType.Copy, WorkStatusType.wsCompleted, WorkResultType.wrSuccess);
        //            return COFETools.GetEmbeddedItem(fileInfoA);
        //        }
        //        catch (Exception ex)
        //        {
        //            RaiseProgressEvent(id, ex.Message, WorkType.Copy, WorkStatusType.wsCompleted, WorkResultType.wrFailed);
        //            return null;
        //        }
        //    }
        //    else return COFETools.GetEmbeddedItem(fileInfoA);
        //}
    }
}
