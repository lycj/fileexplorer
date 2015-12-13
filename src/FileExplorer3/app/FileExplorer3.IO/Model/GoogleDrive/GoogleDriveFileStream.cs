using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using Google.Apis.Drive.v2.Data;
using FileExplorer.WPF.Models;

namespace FileExplorer.Models
{
    public static class GoogleDriveFileStream
    {
        public static async Task<WebFileStream> OpenReadAsync(IEntryModel entryModel, Func<HttpClient> httpClientFunc, CancellationToken ct)
        {
            var contents = await WebUtils.DownloadToBytesAsync((entryModel as GoogleDriveItemModel).SourceUrl, httpClientFunc, ct);
            return new WebFileStream(entryModel, contents, null);
        }


        public static async Task<WebFileStream> OpenReadWriteAsync(IEntryModel entryModel, Func<HttpClient> httpClientFunc, CancellationToken ct)
        {
            var contents = await WebUtils.DownloadToBytesAsync((entryModel as GoogleDriveItemModel).SourceUrl, httpClientFunc, ct);
            return new WebFileStream(entryModel, contents, (m, s) =>
            {
                AsyncUtils.RunSync(() => updateSourceAsync(s));
            });
        }

        public static WebFileStream OpenWrite(IEntryModel entryModel, Func<HttpClient> httpClientFunc)
        {
            return new WebFileStream(entryModel, null, (m, s) =>
            {
                AsyncUtils.RunSync(() => updateSourceAsync(s));
            });
        }


        private static async Task updateSourceAsync(WebFileStream stream)
        {
            var gProfile = stream.Profile as GoogleDriveProfile;
            var gModel = stream.EntryModel as GoogleDriveItemModel;
           
            CancellationTokenSource cts = new CancellationTokenSource();
            //Create New..

            if (String.IsNullOrEmpty(gModel.UniqueId))
            {

                var newFile = new Google.Apis.Drive.v2.Data.File()
                   {
                       Title = gModel.Name,
                       Description = gModel.Name,
                       MimeType = ""
                   };
                newFile.Parents = new List<ParentReference>();
                newFile.Parents.Add(new ParentReference() { Id = (gModel.Parent as GoogleDriveItemModel).Metadata.Id });
                stream.Seek(0, SeekOrigin.Begin);
                var request = gProfile.DriveService.Files.Insert(newFile, stream, "");
                await request.UploadAsync(cts.Token);
            }
            else
            {
                gProfile.DriveService.Files.Update(gModel.Metadata, gModel.UniqueId, stream, "");
            }

            stream.Seek(0, SeekOrigin.Begin);
        }

    }

}
