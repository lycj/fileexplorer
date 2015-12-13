using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Models
{
    public static class DropBoxFileStream
    {
        public static async Task<WebFileStream> OpenReadAsync(IEntryModel entryModel, CancellationToken ct)
        {
            var fileModel = entryModel as DropBoxItemModel;
            var profile = fileModel.Profile as DropBoxProfile;
            var contents = (await profile.GetClient().GetFileTask(fileModel.RemotePath)).RawBytes;
            return new WebFileStream(entryModel, contents, null);
        }


        public static async Task<WebFileStream> OpenReadWriteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            var fileModel = entryModel as DropBoxItemModel;
            var profile = fileModel.Profile as DropBoxProfile;
            var contents = (await profile.GetClient().GetFileTask(fileModel.RemotePath)).RawBytes;
            return new WebFileStream(entryModel, contents, (m, s) =>
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
            var profile = stream.Profile as DropBoxProfile;
            var fileModel = stream.EntryModel as DropBoxItemModel;
            string fileName = fileModel.Name;
            string filePath = profile.Path.GetDirectoryName(fileModel.RemotePath);
            stream.Seek(0, SeekOrigin.Begin);

            CancellationTokenSource cts = new CancellationTokenSource();
            var client = profile.GetClient();
            await client.UploadFileTask(filePath, fileName, stream.ToByteArray());
        }

    }

}
