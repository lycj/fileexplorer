using FileExplorer.IO;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models.SevenZipSharp
{
    public static class SzsFileStream
    {
        public static async Task<WebFileStream> OpenReadAsync(IEntryModel entryModel, CancellationToken ct)
        {
            var profile = entryModel.Profile as SzsProfile;
            ISzsItemModel entryItemModel = entryModel as ISzsItemModel;
            IEntryModel rootModel = entryItemModel.Root.ReferencedFile;

            byte[] bytes = new byte[] { };
            using (Stream stream = await (rootModel.Profile as IDiskProfile).DiskIO.OpenStreamAsync(rootModel, Defines.FileAccess.Read, ct))
            {
                MemoryStream ms = new MemoryStream();
                if (profile.Wrapper.ExtractOne(stream, entryItemModel.RelativePath, null, ms))
                    bytes = ms.ToByteArray();
            }

            return new WebFileStream(entryModel, bytes, null);
        }


        public static async Task<WebFileStream> OpenReadWriteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            byte[] bytes = new byte[] { };
            var profile = entryModel.Profile as SzsProfile;
            ISzsItemModel entryItemModel = entryModel as ISzsItemModel;
            IEntryModel rootModel = entryItemModel.Root.ReferencedFile;

            using (Stream stream = await (rootModel.Profile as IDiskProfile).DiskIO.OpenStreamAsync(rootModel, Defines.FileAccess.Read, ct))
            {
                MemoryStream ms = new MemoryStream();
                if (profile.Wrapper.ExtractOne(stream, entryItemModel.RelativePath, null, ms))
                    bytes = ms.ToByteArray();
            }

            return new WebFileStream(entryModel, bytes, (m, s) =>
            {
                AsyncUtils.RunSync(() => updateSourceAsync(s));
            });
        }

        public static WebFileStream OpenWrite(IEntryModel entryModel)
        {
            return new WebFileStream(entryModel, null, (m, s) =>
            {
                AsyncUtils.RunSync(() => updateSourceAsync(s));
            });
        }


        private static async Task updateSourceAsync(WebFileStream stream)
        {
            //throw new Exception();

            var szsProfile = stream.Profile as SzsProfile;
            var szsModel = stream.EntryModel as ISzsItemModel;
            //var rootReferencedFile = szsModel.Root.ReferencedFile;
            string type = szsModel.Root.GetExtension();

            using (var releaser = await szsProfile.WorkingLock.LockAsync())
            using (Stream archiveStream = await (szsModel.Root.Profile as IDiskProfile).DiskIO.OpenStreamAsync(szsModel.Root,
                Defines.FileAccess.ReadWrite, CancellationToken.None))
            {
                szsProfile.Wrapper.CompressOne(type, archiveStream, szsModel.RelativePath, stream);
            }
        }

    }

}
