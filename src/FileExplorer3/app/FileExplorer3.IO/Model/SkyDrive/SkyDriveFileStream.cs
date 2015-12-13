using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using Microsoft.Live;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public static class SkyDriveFileStream
    {
        public static async Task<WebFileStream> OpenReadAsync(IEntryModel entryModel, CancellationToken ct)
        {
            var contents = await WebUtils.DownloadToBytesAsync((entryModel as SkyDriveItemModel).SourceUrl, () => new HttpClient(), ct);
            return new WebFileStream(entryModel, contents, null);
        }


        public static async Task<WebFileStream> OpenReadWriteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            var contents = await WebUtils.DownloadToBytesAsync((entryModel as SkyDriveItemModel).SourceUrl, () => new HttpClient(), ct);
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
            var skyProfile = stream.Profile as SkyDriveProfile;
            var skyModel = stream.EntryModel as SkyDriveItemModel;

            await skyProfile.checkLoginAsync();

            CancellationTokenSource cts = new CancellationTokenSource();            
            var progressHandler = new Progress<LiveOperationProgress>(
                (progress) => { });


            LiveConnectClient liveClient = new LiveConnectClient(skyProfile.Session);
            LiveOperationResult result;

            stream.Seek(0, SeekOrigin.Begin);
            var uid = (skyModel.Parent as SkyDriveItemModel).UniqueId;

            result = await liveClient.UploadAsync(uid,
                skyModel.Name, stream, OverwriteOption.Overwrite, cts.Token, progressHandler);
            skyModel.init(skyProfile, skyModel.FullPath, result.Result);
        }

    }

}
