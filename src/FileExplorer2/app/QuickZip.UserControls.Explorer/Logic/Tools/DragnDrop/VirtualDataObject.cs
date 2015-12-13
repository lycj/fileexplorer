using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.IO.Tools;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    public class VirtualDataObject<T> : System.Windows.Forms.DataObject
    {
        public static readonly string DATATYPE = "VirtualDataObject.DataType";
        public static readonly string DATAID = "VirtualDataObject.DataID";

        public VirtualDataObject(ISupportDrag<T> dragSource)
        {
            _dragSource = dragSource;
            _dropInfo = new DragDropInfo<T>(dragSource);
        }

        #region Methods

        /// <summary>
        /// Overrided GetData to implement dragloop monitoring.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public override object GetData(String format)
        {
            Object obj = base.GetData(format);
            if (format == System.Windows.Forms.DataFormats.FileDrop &&
                 !inDragLoop() && !_beforeDropCalled)
            {
                string s;
                try
                {
                    if (!_dropInfo.HandledInternally)
                        _dragSource.PrepareDrop(_dropInfo);
                    _beforeDropCalled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Drag and Drop failed.");
                    _beforeDropCalled = true;
                    return obj;
                }
                _beforeDropCalled = true;
            }
            return obj;
        }

        /// <summary>
        /// return false when if user droped the item, or canceled
        /// </summary>
        /// <returns></returns>
        private bool inDragLoop()
        {
            return (0 != (int)GetData(ShellClipboardFormats.CFSTR_INDRAGLOOP));
        }


        /// <summary>
        /// Perform a number of SetData
        /// </summary>
        public bool PrepareDataObject()
        {
            if (!ensureFileNameExists())
                return false;
            SetData(DataFormats.FileDrop, getFilenameList());
            SetData(DATATYPE, typeof(T).ToString());
            SetData(DATAID, _dataID);
            SetData(ShellClipboardFormats.CFSTR_PREFERREDDROPEFFECT, DragDropEffects.Copy);
            SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 1);
            return true;
        }

        /// <summary>
        /// Whether the specified IDataObject, which is from OnDragOver, is actually the same virtualdataObject
        /// </summary>
        /// <param name="incomingDataobject"></param>
        /// <returns></returns>
        public bool IsSameDataObject(IDataObject incomingDataobject)
        {
            if (incomingDataobject.GetDataPresent(DATATYPE))
                return _dataID == ((int)incomingDataobject.GetData(DATAID));
            return false;
        }


        /// <summary>
        /// Create a temp file
        /// </summary>
        /// <param name="fileName"></param>
        private static void createTemporaryFileName(String fileName)
        {
            if (!File.Exists(fileName))
            {
                System.IO.StreamWriter file;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                file = new System.IO.StreamWriter(fileName);
                file.Close();
            }
        }

        /// <summary>
        /// Create temp file/directory if not exists.
        /// </summary>
        private bool ensureFileNameExists()
        {
            foreach (var info in _dropInfo.SelectedItems)
                if (info.FileSystemPath == null)
                    return false;
                else
                    if (info.IsFolder)
                    {
                        if (!Directory.Exists(info.FileSystemPath))
                            Directory.CreateDirectory(info.FileSystemPath);
                    }
                    else
                    {
                        if (!File.Exists(info.FileSystemPath))
                            createTemporaryFileName(info.FileSystemPath);
                    }
            return true;
        }

        /// <summary>
        /// Create a list of filenames, may reside in temp directory and may not be even exists when returning.
        /// In that case, a temp file (0byte) or empty directory is created.
        /// </summary>
        /// <returns></returns>
        private string[] getFilenameList()
        {
            var flist = from info in _dropInfo.SelectedItems
                        select info.FileSystemPath;
            return flist.ToArray();
        }

        #endregion

        #region Data

        bool _beforeDropCalled = false;
        int _dataID = new Random(DateTime.Now.Millisecond).Next();
        ISupportDrag<T> _dragSource;
        DragDropInfo<T> _dropInfo;

        #endregion

        #region Public Properties

        public DragDropInfo<T> DropInfo { get { return _dropInfo; } private set { _dropInfo = value; } }

        #endregion
    }
}
