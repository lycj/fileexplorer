using FileExplorer.IO;
using FileExplorer.IO.Compress;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models.SevenZipSharp
{
    public class SzsDiskIOHelper : DiskIOHelperBase
    {
        public SzsDiskIOHelper(SzsProfile profile)
            : base(profile)
        {
            this.Mapper = new FileBasedDiskPathMapper(m =>
                {
                    ISzsItemModel model = m as ISzsItemModel;
                    return model.Profile.Path.Combine(model.Root.FullPath, model.RelativePath);
                });
        }

        public override async Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            SevenZipWrapper wrapper = (Profile as SzsProfile).Wrapper;
            string destPath =  Profile.Path.Combine(Profile.Path.GetDirectoryName(entryModel.FullPath), newName);

            SzsProfile profile = Profile as SzsProfile;
            ISzsItemModel szsEntryModel = entryModel as ISzsItemModel;
            string type = profile.Path.GetExtension(szsEntryModel.Root.Name);

            using (var releaser = await profile.WorkingLock.LockAsync())
            using (var stream = await profile.DiskIO.OpenStreamAsync(szsEntryModel.Root, Defines.FileAccess.ReadWrite, ct))            
                wrapper.Modify(type, stream, (entryModel as ISzsItemModel).RelativePath, newName,
                    entryModel.IsDirectory && !(entryModel is SzsRootModel));

            lock (profile.VirtualModels)
            {
                if (profile.VirtualModels.Contains(szsEntryModel))
                    profile.VirtualModels.Remove(szsEntryModel);
            }                

            Profile.NotifyEntryChanges(this, destPath, Defines.ChangeType.Moved, entryModel.FullPath);

            return await Profile.ParseAsync(destPath);
        }

        public override async Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            SzsProfile profile = Profile as SzsProfile;
            ISzsItemModel szsEntryModel = entryModel as ISzsItemModel;

            if (szsEntryModel is SzsRootModel)
            {
                IEntryModel rootFile = (szsEntryModel as SzsRootModel).ReferencedFile;
                await (rootFile.Profile as IDiskProfile)
                    .DiskIO.DeleteAsync(rootFile, ct);
                return;
            }

            using (var releaser = await profile.WorkingLock.LockAsync())
            using (var stream = await profile.DiskIO.OpenStreamAsync(szsEntryModel.Root, Defines.FileAccess.ReadWrite, ct))
            {
                string type = profile.Path.GetExtension(szsEntryModel.Root.Name);

                profile.Wrapper.Delete(type, stream, szsEntryModel.RelativePath + (szsEntryModel.IsDirectory ? "\\*" : ""));

                lock (profile.VirtualModels)
                    if (profile.VirtualModels.Contains(szsEntryModel))
                        profile.VirtualModels.Remove(szsEntryModel);
            }
        }

        public override async Task<Stream> OpenStreamAsync(IEntryModel entryModel,
            FileExplorer.Defines.FileAccess access, CancellationToken ct)
        {
            //SevenZipWrapper wrapper = (Profile as SzsProfile).Wrapper;
            ISzsItemModel entryItemModel = entryModel as ISzsItemModel;
            //IEntryModel rootReferenceModel = itemModel.Root.ReferencedFile;
            //return new CompressMemoryStream(wrapper, rootReferenceModel, itemModel.RelativePath, access, ct);
            //To-DO: save to Profile.DiskIO.Mapper[itemModel].IOPath


            if (entryItemModel.Root.Equals(entryItemModel))
            {
                IEntryModel referencedFile = entryItemModel.Root.ReferencedFile;
                return await (referencedFile.Profile as IDiskProfile).DiskIO.OpenStreamAsync(referencedFile, access, ct);
            }

            switch (access)
            {
                case FileExplorer.Defines.FileAccess.Read: return await SzsFileStream.OpenReadAsync(entryModel, ct);
                case FileExplorer.Defines.FileAccess.Write: return SzsFileStream.OpenWrite(entryModel);
                case FileExplorer.Defines.FileAccess.ReadWrite: return await SzsFileStream.OpenReadWriteAsync(entryModel, ct);
            }
            throw new NotSupportedException();
        }

        public override Script.IScriptCommand GetTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal)
        {
            return new SzsBatchTransferScriptCommand(srcModel, destDirModel, removeOriginal);                
        }

        public override IScriptCommand GetTransferCommand(string sourceKey, string destinationDirKey, string destinationKey, bool removeOriginal, IScriptCommand nextCommand)
        {
            return IOScriptCommands.SzsDiskTransfer(sourceKey, destinationDirKey, removeOriginal, nextCommand);
        }

        public override async Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            string parentPath = Profile.Path.GetDirectoryName(fullPath);
            string name = Profile.Path.GetFileName(fullPath);
            ISzsItemModel parentDir = await Profile.ParseAsync(parentPath) as ISzsItemModel;
            if (parentDir == null)
                throw new Exception(String.Format("Parent dir {0} not exists.", parentPath));
            string relativePath = Profile.Path.Combine(parentDir.RelativePath, name);
            ISzsItemModel retEntryModel = new SzsChildModel(parentDir.Root, relativePath, isDirectory);

            (Profile as SzsProfile).VirtualModels.Add(retEntryModel);
            return retEntryModel;

        }

    }
}
