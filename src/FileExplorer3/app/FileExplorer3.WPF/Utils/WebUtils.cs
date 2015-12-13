using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public static class WebUtils
    {
        public static async Task<byte[]> DownloadAsync(Uri uri, CancellationToken ct)
        {
            byte[] bytes = null;
            using (WebClient webClient = new WebClient())
            {
                webClient.Proxy = null;  //avoids dynamic proxy discovery delay
                webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
                try
                {
                    ct.Register(webClient.CancelAsync);
                    bytes = await webClient.DownloadDataTaskAsync(uri);
                }
                catch(Exception ex) 
                {
                    Debug.WriteLine(ex);
                }
            }
            return bytes;
        }

        public static async Task<byte[]> DownloadToBytesAsync(string url, Func<HttpClient> clientFunc, CancellationToken ct)
        {
            var client = clientFunc();
            var response = await client.GetAsync(url, ct);
            if (response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return bytes;
            }
            else throw new Exception(String.Format("{0} when downloading {1}", response.StatusCode, url));
        }

        public static async Task DownloadToStreamAsync(string url, Stream stream, Func<HttpClient> clientFunc, CancellationToken ct)
        {
            var client = clientFunc();
            var response = await client.GetAsync(url, ct);
            if (response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            else throw new Exception(String.Format("{0} when downloading {1}", response.StatusCode, url));
        }

    }
}
