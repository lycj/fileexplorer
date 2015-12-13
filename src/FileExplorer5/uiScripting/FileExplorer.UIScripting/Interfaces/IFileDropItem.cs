using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{


    public interface IFileDropItem
    {
        bool IsVirtual { get; }
        bool IsFolder { get; }
        string FileSystemPath { get; }
    }

    public class FileDrop : IFileDropItem
    {
        #region Constructor

        public FileDrop(string path, bool isFolder)
        {
            FileSystemPath = path;
            IsFolder = isFolder;
        }

        #endregion

        #region Methods


        #endregion

        #region Data

        #endregion

        #region Public Properties

        public virtual bool IsVirtual { get { return false; } }
        public bool IsFolder { get; set; }
        public string FileSystemPath { get; set; }

        #endregion


    }


    public class VirtualFileDrop : FileDrop
    {
        #region Constructor

        public VirtualFileDrop(string name, bool isFolder)
            : base(Path.Combine(FileDropDataObject.DragNDropDirectory, name), isFolder)
        {
            if (IsFolder)
            {
                if (!Directory.Exists(FileSystemPath))
                    Directory.CreateDirectory(FileSystemPath);
            }
            else
            {
                if (!File.Exists(FileSystemPath))
                    createTemporaryFileName(FileSystemPath);
            }
        }

        #endregion

        #region Methods

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



        #endregion

        #region Data

        #endregion

        #region Public Properties

        public override bool IsVirtual { get { return true; } }

        #endregion

    }

}
