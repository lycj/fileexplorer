using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Models;
using FileExplorer.IO;

namespace FileExplorer.Models
{
    public class FileSystemInfoModel : DiskEntryModelBase
    {
        #region Cosntructor

        public FileSystemInfoModel(IProfile profile, FileSystemInfo fsi)
            : base(profile)
        {
            this.Label = fsi.Name;
            this.Attributes = fsi.Attributes;
            this.FullPath = fsi.FullName;
            this.Name = fsi.Name;
            this.IsRenamable = true;
            this.IsDirectory = fsi is DirectoryInfo;
            if (fsi is FileInfo)
                Size = (fsi as FileInfo).Length;
            string parentPath = Path.GetDirectoryName(fsi.FullName);
            this._parentFunc =
                () =>
                {
                    return String.IsNullOrEmpty(parentPath) ? null :
                    new FileSystemInfoModel(profile,
                        (profile as FileSystemInfoProfile).createDirectoryInfo(parentPath));
                };
            this.Description = fsi.GetType().ToString();
            this.CreationTimeUtc = fsi.CreationTimeUtc;
            this.LastUpdateTimeUtc = fsi.LastWriteTimeUtc;
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public FileAttributes Attributes { get; protected set; }
        public long Size { get; protected set; }       

        #endregion
    }
}
