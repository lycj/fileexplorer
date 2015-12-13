using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using FileExplorer.Defines;
using System.Diagnostics;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;

namespace FileExplorer.WPF.BaseControls
{
    public interface INotifyDropped
    {
        void NotifyPrepareDrop(VirtualDataObject sender, string format);
    }


    public class VirtualDataObject : NonsealedDataObject //System.Windows.DataObject is sealed.
    {

        #region Constructor

        public VirtualDataObject(INotifyDropped notifyDropped)
        {
            _notifyDropped = notifyDropped;
            IsVirtual = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overrided GetData to implement dragloop monitoring.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public override object GetData(String format, bool autoConvert)
        {
            Object obj = base.GetData(format, autoConvert);
            if (IsVirtual && !inDragLoop() && !_notifyDropCalled && format.Equals(DataFormats.FileDrop))
            {
                string s;
                try
                {
                    if (_notifyDropped != null)
                        _notifyDropped.NotifyPrepareDrop(this, format);
                    _notifyDropCalled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Drag and Drop failed.");
                    _notifyDropCalled = true;
                }
            }
            return obj;
        }

        /// <summary>
        /// return false when if user droped the item, or canceled
        /// </summary>
        /// <returns></returns>
        private bool inDragLoop()
        {
            return (0 != (int)base.GetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, true));
        }

        #endregion

        #region Data

        bool _notifyDropCalled = false;
        INotifyDropped _notifyDropped;

        #endregion

        #region Public Properties

        public bool IsVirtual { get; protected set; }

        #endregion

    }


    public class FileDropDataObject : VirtualDataObject
    {
        #region Constructor

        public static string DragNDropDirectory { get; set; }

        static FileDropDataObject()
        {
            DragNDropDirectory = Path.Combine(Path.GetTempPath(), "DragnDrop");
        }

        public FileDropDataObject(INotifyDropped notifyDropped)
            : base(notifyDropped)
        {
        }

        #endregion

        #region Methods

        public void SetFileDropData(IFileDropItem[] fileDrops, DragDropEffectsEx effects = DragDropEffectsEx.Copy)
        {
            _fileDrops = fileDrops;

            string[] _fileNames = (from fd in _fileDrops select fd.FileSystemPath).ToArray();

            SetData(DataFormats.FileDrop, _fileNames);
            SetData(ShellClipboardFormats.CFSTR_PREFERREDDROPEFFECT, effects);
            IsVirtual = fileDrops.Any(fd => fd.IsVirtual);
            if (IsVirtual)
                SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 1);
        }


        #endregion

        #region Data

        IFileDropItem[] _fileDrops;

        #endregion

        #region Public Properties

        public IFileDropItem[] FileDrops { get { return _fileDrops; } }

        #endregion


    }
}
