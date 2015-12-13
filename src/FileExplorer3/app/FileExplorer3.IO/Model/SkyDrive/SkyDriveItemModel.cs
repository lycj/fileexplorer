using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.IO;

namespace FileExplorer.Models
{
    public class SkyDriveItemModel : DiskEntryModelBase
    {
        #region Constructor

        private SkyDriveItemModel(SkyDriveProfile profile)
            : base(profile)
        {            
            
        }


        internal void init(SkyDriveProfile profile, string path)
        {            
            FullPath = path;
            this.Label = this.Name = profile.Path.GetFileName(path);            
            //this._parentFunc = new Lazy<IEntryModel>(() =>
            //    AsyncUtils.RunSync(() => profile.ParseAsync(PathFE.GetDirectoryNameR(path))));
        }

        internal void init(SkyDriveProfile profile, string path, dynamic d)
        {            
            init(profile, path);
            Metadata = d;
            UniqueId = d.id;
            this.IsDirectory = d.type == "folder" || d.type == "album";
            if (!IsDirectory)
                this.Size = d.size != null ? d.size : 0;
            this.CreationTimeUtc = d.created_time != null ? DateTime.Parse(d.created_time) : this.CreationTimeUtc;
            this.LastUpdateTimeUtc = d.updated_tie != null ? DateTime.Parse(d.updated_time) : this.LastUpdateTimeUtc;

            this._isRenamable = true;
            this.Type = d.type; //photo, album or folder
            this.Description = d.description;
            this.ImageUrl = d.picture;
            this.SourceUrl = d.source;

        }
        /// <summary>
        /// Generate a temporary model for uploading.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="path"></param>
        /// <param name="isDirectory"></param>
        public SkyDriveItemModel(SkyDriveProfile profile, string path, bool isDirectory)
            : this(profile)
        {
            init(profile, path);
            this.IsDirectory = isDirectory;
            //SourceUrl = "Unknown";
        }

        public SkyDriveItemModel(SkyDriveProfile profile, object data, string parentFullPath = null)
            : this(profile)
        {
            dynamic d = data as dynamic;
            string path = parentFullPath == null ? profile.Alias :  parentFullPath  + "/" + d.name;
            init(profile, path, d);
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public dynamic Metadata { get; private set; }
        public string Description { get; private set; }
        public string UniqueId { get; private set; }
        public string Type { get; private set; }
        public string ImageUrl { get; protected set; }

        /// <summary>
        /// Url for downloading a file.
        /// </summary>
        public string SourceUrl { get; protected set; }

        #endregion
    }

   
}
