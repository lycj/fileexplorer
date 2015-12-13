using FileExplorer.IO;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class DropBoxDiskIOHelper : DiskIOHelperBase
    {

        #region Constructors

        public DropBoxDiskIOHelper(DropBoxProfile profile)
            : base(profile)
        {
            _profile = profile;

            this.Mapper = new FileBasedDiskPathMapper();
        }


        #endregion

        #region Methods

        public override async Task<Stream> OpenStreamAsync(IEntryModel entryModel, 
            FileExplorer.Defines.FileAccess access, CancellationToken ct)
        {
            switch (access)
            {
                case FileExplorer.Defines.FileAccess.Read: return await DropBoxFileStream.OpenReadAsync(entryModel, ct);
                case FileExplorer.Defines.FileAccess.Write: return DropBoxFileStream.OpenWrite(entryModel);
                case FileExplorer.Defines.FileAccess.ReadWrite: return await DropBoxFileStream.OpenReadWriteAsync(entryModel, ct);
            }
            throw new NotSupportedException();
        }

        public override async Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            _profile.checkLogin();
            string parentPath = _profile.Path.GetDirectoryName(fullPath);
            string parentRemotePath = _profile.ConvertRemotePath(parentPath);
            string name = _profile.Path.GetFileName(fullPath);
            string remotePath = _profile.Path.Combine(parentRemotePath, name);

            if (isDirectory)
            {

                DropBoxItemModel parentDir = await _profile.ParseAsync(parentPath)
                     as DropBoxItemModel;

                if (parentDir == null)
                    throw new DirectoryNotFoundException(parentPath);

                
                var addedFolder = await _profile.GetClient().CreateFolderTask(remotePath, m => { }, e => { });
                ct.ThrowIfCancellationRequested();
                return new DropBoxItemModel(_profile, addedFolder, parentDir.FullPath);



            }
            else return new DropBoxItemModel(_profile, name, parentRemotePath);
        }

        public override async Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            DropBoxItemModel entry = entryModel as DropBoxItemModel;
            var profile = entry.Profile as DropBoxProfile;

            await profile.GetClient().DeleteTask(entry.RemotePath);
        }

        public override async Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            DropBoxItemModel entry = entryModel as DropBoxItemModel;
            var profile = entry.Profile as DropBoxProfile;
            string newRemotePath = profile.Path.Combine(
                profile.Path.GetDirectoryName(entry.RemotePath), newName);
            string newPath = profile.Path.Combine(
                profile.Path.GetDirectoryName(entry.FullPath), newName);
            await profile.GetClient().MoveTask(entry.RemotePath, newRemotePath);
            return await profile.ParseAsync(newPath);
        }

        #endregion

        #region Data

        private DropBoxProfile _profile;

        #endregion

        #region Public Properties

        #endregion
    }
}
