using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    /// <summary>
    /// Raise event when write.
    /// </summary>
    public class HookableMemoryStream : MemoryStream
    {
        private bool _isModified = false;

        public EventHandler OnWriting = (o, e) => { };
        public EventHandler OnWrote = (o, e) => { };
        public EventHandler OnClosing = (o, e) => { };
        public EventHandler OnClosed = (o, e) => { };
        public EventHandler OnDisposing = (o, e) => { };
        public EventHandler OnDisposed = (o, e) => { };
        public bool IsModified { get { return _isModified; } set { _isModified = value; } }

        public override void Write(byte[] buffer, int offset, int count)
        {
            OnWriting(this, EventArgs.Empty);
            base.Write(buffer, offset, count);            
            _isModified = true;
            OnWrote(this, EventArgs.Empty);
        }

        public override void WriteByte(byte value)
        {
            OnWriting(this, EventArgs.Empty);
            base.WriteByte(value);
            _isModified = true;
            OnWrote(this, EventArgs.Empty);
        }

#if NETFX_CORE        
        public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            _isModified = true;
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void Flush()
        {
            OnClosing(this, EventArgs.Empty);
            base.Flush();
            _isModified = false;
            OnClosed(this, EventArgs.Empty);
        }
#else
        public override void EndWrite(IAsyncResult asyncResult)
        {            
            base.EndWrite(asyncResult);
            _isModified = true;            
        }

         public override void Close()
        {
            OnClosing(this, EventArgs.Empty);
            base.Close();
            _isModified = false;
            OnClosed(this, EventArgs.Empty);
        }
#endif
        protected override void Dispose(bool disposing)
        {
            OnDisposing(this, EventArgs.Empty);            
            base.Dispose(disposing);
            OnDisposed(this, EventArgs.Empty);
        }

       
    }


    //public class VirtualFileStream : MemoryStream
    //{
    //    public VirtualFileStream(IPropertyHost propertyHost, FileMode mode, FileAccess access)
    //        : base()
    //    {
            
    //        switch (access)
    //        {
    //            case FileAccess.Write :
    //                break;
    //            case FileAccess.Read :
    //            case FileAccess.ReadWrite : 
    //                using (var streamContainer =  propertyHost.Behaviors.Invoke<StreamContainer>(CofeStreamProperties.OpenReadAsync))
    //                    StreamUtils.CopyStream(streamContainer.Stream, this, false, true, false);                    
    //                break;                    
    //        }

    //        _file = propertyHost;
    //        _access = access;
    //        _mode = mode;
    //        _isModified = false;
    //    }

        

    //    #region Methods

    //     public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        base.Write(buffer, offset, count);
    //        _isModified = true;
    //    }

    //    public override void WriteByte(byte value)
    //    {
    //        base.WriteByte(value);
    //        _isModified = true;
    //    }

    //    public override void EndWrite(IAsyncResult asyncResult)
    //    {
    //        base.EndWrite(asyncResult);
    //        _isModified = true;
    //    }
        

    //    public virtual void CallUpdateProc()
    //    {
    //        if (_isModified)
    //        {
    //            using (var streamContainer = _file.Behaviors.Invoke<StreamContainer>(CofeStreamProperties.OpenWriteAsync))
    //                StreamUtils.CopyStream(streamContainer.Stream, this, false, false, false);
                
    //            _isModified = false;
    //        }
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
            
    //        base.Dispose(disposing);
    //    }

    //    public override void Close()
    //    {
    //        base.Close();
    //        CallUpdateProc();
    //    }

    //    #endregion

    //    #region Data

    //    private FileAccess _access;
    //    private FileMode _mode;
    //    private IPropertyHost _file;
    //    private bool _isModified;

    //    #endregion
        
    //}
}
