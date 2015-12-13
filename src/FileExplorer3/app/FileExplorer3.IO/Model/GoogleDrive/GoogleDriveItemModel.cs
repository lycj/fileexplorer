using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.IO;

namespace FileExplorer.Models
{
    public class GoogleDriveItemModel : DiskEntryModelBase
    {

        #region Constants



        #endregion

        #region Constructor

        public GoogleDriveItemModel(GoogleDriveProfile profile, string alias)
            : base(profile)
        {
            init(profile, alias);
            this.UniqueId = "root";
            this.IsDirectory = true;
        }


        internal void init(GoogleDriveProfile profile, string path)
        {
            FullPath = path;
            this.Label = this.Name = profile.Path.GetFileName(path);
        }


        internal void init(GoogleDriveProfile profile, string path, Google.Apis.Drive.v2.Data.File f)
        {
            init(profile, path);
            UniqueId = f.Id;
            this.Metadata = f;
            this.IsDirectory = f.MimeType.Equals(GoogleMimeTypeManager.FolderMimeType);
            this.Name = profile.Path.GetFileName(path);

            this.Size = f.FileSize.HasValue ? f.FileSize.Value : 0;
            this._isRenamable = true;

            this.Description = f.Description;
            this.ImageUrl = f.ThumbnailLink;
            this.SourceUrl = f.DownloadUrl;
            this.SourceExportUrls = f.ExportLinks;
            
            this.Type = profile.MimeTypeManager.GetExportableMimeTypes(f.MimeType).FirstOrDefault();
            if (!this.IsDirectory && String.IsNullOrEmpty(Profile.Path.GetExtension(this.Name)))
            {
                string extension = profile.MimeTypeManager.GetExtension(this.Type);
                if (!String.IsNullOrEmpty(extension))
                {
                    this.FullPath += extension;
                    this.Label = this._name += extension;
                    this.SourceUrl = f.ExportLinks[this.Type];
                }
                else
                {
                    extension = profile.Path.GetExtension(f.OriginalFilename);
                    if (!String.IsNullOrEmpty(extension))
                    {
                        this.FullPath += extension;
                        this.Label = this._name += extension;
                    }
                }
            }

            
            this.Size = f.FileSize.HasValue ? f.FileSize.Value : this.Size;
            this.CreationTimeUtc = f.CreatedDate.HasValue ? f.CreatedDate.Value.ToUniversalTime() : this.CreationTimeUtc;
            this.LastUpdateTimeUtc = f.LastViewedByMeDate.HasValue ? f.LastViewedByMeDate.Value.ToUniversalTime() : this.LastUpdateTimeUtc;

            if (!this.IsDirectory && System.IO.Path.GetExtension(this.Name) == "" && this.Type != null)
            {
                string extension = ShellUtils.MIMEType2Extension(this.Type);
                if (!String.IsNullOrEmpty(extension))
                {
                    this.Name += extension;
                }
            }



        }

        /// <summary>
        /// Generate a temporary model for uploading.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="path"></param>
        /// <param name="isDirectory"></param>
        public GoogleDriveItemModel(GoogleDriveProfile profile, string path, bool isDirectory)
            : base(profile)
        {
            init(profile, path);
            this.IsDirectory = isDirectory;
            //SourceUrl = "Unknown";
        }

        public GoogleDriveItemModel(GoogleDriveProfile profile, Google.Apis.Drive.v2.Data.File file, string parentFullPath = null)
            : base(profile)
        {
            //string ext = file.OriginalFilename == null ? "" : profile.Path.GetExtension(file.OriginalFilename);
            string name = file.Title;
            //if (!String.IsNullOrEmpty(ext) &&                                  //OriginalFileName have ext  
            //    !file.MimeType.Equals(GoogleMimeTypeManager.FolderMimeType) && //Not folder
            //    String.IsNullOrEmpty(profile.Path.GetExtension(name)))         //Title does not have ext
            //    name += ext;
            string path = parentFullPath == null ? profile.Alias : parentFullPath + "/" + name;
            init(profile, path, file);
        }


        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        //public string Description { get; private set; }
        public string UniqueId { get; private set; }
        public string Type { get; private set; }
        public string ImageUrl { get; protected set; }
        //public long Size { get; protected set; }

        public Google.Apis.Drive.v2.Data.File Metadata { get; protected set; }

        /// <summary>
        /// Url for downloading a file.
        /// </summary>
        public string SourceUrl { get; protected set; }

        public IDictionary<string, string> SourceExportUrls { get; protected set; }

        #endregion
    }


}
