using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public static partial class EntryModelIconExtractors
    {
        public static IModelIconExtractor<IEntryModel> FromUrl(Func<IEntryModel, Uri> uriFunc, Func<HttpClient> clientFunc = null)
        {
            return new GetUriIcon(uriFunc, clientFunc);
        }

        public static IModelIconExtractor<IEntryModel> FromUrl(Uri uri, Func<HttpClient> clientFunc = null)
        {
            return FromUrl(em => uri, clientFunc);
        }
    }

    public class GetUriIcon : IModelIconExtractor<IEntryModel>
    {
        private Func<IEntryModel, Uri> _uriFunc;
        private System.Threading.CancellationToken CancellationToken;
        private Func<HttpClient> _clientFunc;
        public GetUriIcon(Func<IEntryModel, Uri> uriFunc, Func<HttpClient> clientFunc = null)
        {
            _uriFunc = uriFunc;
            _clientFunc = clientFunc ?? (() => new HttpClient());
        }


        public async Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            var response = await _clientFunc().GetAsync(_uriFunc(model).AbsoluteUri, ct);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var output = await response.Content.ReadAsByteArrayAsync();

                BitmapImage retIcon = new BitmapImage();
                return new MemoryStream(output).ToArray();
            }
            return await new GetDefaultIcon().GetIconBytesForModelAsync(model, ct);
        }
    }

  
    
}
