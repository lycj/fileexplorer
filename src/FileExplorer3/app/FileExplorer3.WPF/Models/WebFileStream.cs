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
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{

    public class WebFileStream : MemoryStream
    {
        #region Constructor

        public WebFileStream(IEntryModel entryModel, 
            byte[] contents,
            Action<IEntryModel, WebFileStream> updateSourceFunc)
        {
            _entryModel = entryModel;
            _updateSourceFunc = updateSourceFunc;

            if (contents != null)
            {
                this.Write(contents, 0, contents.Length);
                this.Seek(0, SeekOrigin.Begin);
            }
        }

        public static async Task DownloadToStream(string url, Stream stream, Func<HttpClient> clientFunc, CancellationToken ct)
        {
            var client = clientFunc();
            var response = await client.GetAsync(url, ct);
            if (response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            else throw new Exception(String.Format("{0} when downloading {1}", response.StatusCode, url));
        }

       

        #endregion

        #region Methods

        


        public override void Close()
        {
            if (!_closed)
            {
                _closed = true;
                if (_updateSourceFunc != null)
                    _updateSourceFunc(_entryModel, this);
            }
            //base.Close();
        }

        #endregion

        #region Data

        private bool _closed = false;        
        private IEntryModel _entryModel;
        private Action<IEntryModel, WebFileStream> _updateSourceFunc;        

        #endregion

        #region Public Properties

        public IEntryModel EntryModel { get { return _entryModel; } }
        public IProfile Profile { get { return _entryModel.Profile; } }

        #endregion
    }
}
