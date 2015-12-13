using FileExplorer.IO;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class SkyDriveDiskIOHelper : DiskIOHelperBase
    {


        #region Constructor

        public SkyDriveDiskIOHelper(SkyDriveProfile profile)
            : base(profile)
        {
            _profile = profile;
            this.Mapper = new FileBasedDiskPathMapper(m => (m as SkyDriveItemModel).SourceUrl);
        }

        #endregion

        #region Methods

        public override async Task<Stream> OpenStreamAsync(IEntryModel entryModel,
            FileExplorer.Defines.FileAccess access, CancellationToken ct)
        {
            switch (access)
            {
                case FileExplorer.Defines.FileAccess.Read: return await SkyDriveFileStream.OpenReadAsync(entryModel, ct);
                case FileExplorer.Defines.FileAccess.Write: return SkyDriveFileStream.OpenWrite(entryModel);
                case FileExplorer.Defines.FileAccess.ReadWrite: return await SkyDriveFileStream.OpenReadWriteAsync(entryModel, ct);
            }
            throw new NotSupportedException();
        }

        public override async Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            await _profile.checkLoginAsync();

            var fileData = new Dictionary<string, object>();
            fileData.Add("name", newName);
            LiveConnectClient liveClient = new LiveConnectClient(_profile.Session);
            LiveOperationResult result =
                await liveClient.PutAsync((entryModel as SkyDriveItemModel).UniqueId, fileData, ct);
            return new SkyDriveItemModel(_profile, result.Result, entryModel.Parent.FullPath);
        }

        public override async Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            await _profile.checkLoginAsync();
            LiveConnectClient liveClient = new LiveConnectClient(_profile.Session);
            LiveOperationResult result = await liveClient.DeleteAsync((entryModel as SkyDriveItemModel).UniqueId, ct);
        }

        public override async Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            if (isDirectory)
            {
                await _profile.checkLoginAsync();
                string parentPath = _profile.Path.GetDirectoryName(fullPath);
                string name = _profile.Path.GetFileName(fullPath);
                SkyDriveItemModel parentDir = await _profile.ParseAsync(parentPath)
                     as SkyDriveItemModel;

                if (parentDir == null)
                    throw new DirectoryNotFoundException(parentPath);

                var folderData = new Dictionary<string, object>();
                folderData.Add("name", name);
                LiveConnectClient liveClient = new LiveConnectClient(_profile.Session);
                ct.ThrowIfCancellationRequested();
                LiveOperationResult result = await liveClient.PostAsync(parentDir.UniqueId, folderData, ct);
                ct.ThrowIfCancellationRequested();
                return new SkyDriveItemModel(_profile, result.Result, parentDir.FullPath);



            }
            else return new SkyDriveItemModel(Profile as SkyDriveProfile, fullPath, false);
        }


        #endregion

        #region Data

        private SkyDriveProfile _profile;

        #endregion

        #region Public Properties

        #endregion
    }
}
