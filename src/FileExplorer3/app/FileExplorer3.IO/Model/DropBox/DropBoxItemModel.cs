using FileExplorer.IO;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class DropBoxItemModel : DiskEntryModelBase
    {
        #region Constructors

        /// <summary>
        /// For root directory.
        /// </summary>
        /// <param name="profile"></param>
        public DropBoxItemModel(DropBoxProfile profile)
            : base(profile)
        {
            this.FullPath = this.Label = this.Name = profile.Alias;
            this.IsDirectory = true;
            this.RemotePath = "/";


        }

        /// <summary>
        /// For temporary file.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="name"></param>
        /// <param name="parentFullPath"></param>
        internal DropBoxItemModel(DropBoxProfile profile, string name,
            string parentFullPath)
            : base(profile)
        {
            base.FullPath = profile.Path.Combine(parentFullPath, name);
            base.Label = base.Name = name;
            this.RemotePath = profile.ConvertRemotePath(this.FullPath);
        }

        public DropBoxItemModel(DropBoxProfile profile, DropNet.Models.MetaData metadata,
            string parentFullPath = null)
            : base(profile)
        {
            //A list of GetPropertyValue - https://www.dropbox.com/developers/core/docs#metadata-details
            Metadata = metadata;
            if (parentFullPath == null)
            {
                base.FullPath = base.Label = base.Name = profile.Alias;
            }
            else
            {
                base.FullPath = profile.Path.Combine(parentFullPath, metadata.Name);
                base.Label = base.Name = metadata.Name;
            }
            this.RemotePath = profile.ConvertRemotePath(base.FullPath);
            this.IsDirectory = metadata.Is_Dir;

            this._isRenamable = true;
            if (!this.IsDirectory)
            {
                long size = 0;
                if (long.TryParse(metadata.Size, out size))
                    this.Size = size;
            }

            //this.CreationTimeUtc = DateTime.MinValue;
            this.LastUpdateTimeUtc = metadata.UTCDateModified;

            this.Description = IsDirectory ? "Directory" :
                System.IO.Tools.FileTypeInfoProvider
                .GetFileTypeInfo(profile.Path.GetExtension(Name)).FileType;

        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public string RemotePath { get; private set; }
        public DropNet.Models.MetaData Metadata { get; private set; }

        #endregion
    }
}
