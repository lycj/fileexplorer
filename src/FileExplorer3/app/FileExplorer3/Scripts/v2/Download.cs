using FileExplorer.Defines;
using MetroLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {
        /// <summary>
        /// Serializable, Download source Url (urlVariable) to "destinationVariable" property,
        /// 
        /// Variable in ParameterDic:
        /// Progress :  IProgress[TransferProgress] (Optional)
        /// HttpClientFunc : Func[HttpClient] (Optional)    
        /// </summary>
        /// <param name="urlVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Download(string urlVariable = "{Url}", string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return new Download()
            {
                UrlKey = urlVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Download a web stream to a file.
        /// </summary>
        /// <param name="urlVariable">Url to access</param>
        /// <param name="destinationFileVariable">Destination file name.</param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DownloadFile(string urlVariable = "{Url}",
            string destinationProfileVariable = "{Profile}", string destinationFileVariable = "{DestinationFile}",
            IScriptCommand nextCommand = null)
        {
            return CoreScriptCommands.Download(urlVariable, "{DownloadStream}",
               CoreScriptCommands.DiskParseOrCreateFile(destinationProfileVariable, destinationFileVariable, "{Destination}",
               CoreScriptCommands.DiskOpenStream("{Destination}", "{DestinationStream}", FileExplorer.Defines.FileAccess.Write,
               CoreScriptCommands.CopyStream("{DownloadStream}", "{DestinationStream}",
               ScriptCommands.Reset(nextCommand, "{DownloadStream}", "{Destination}")))));
        }
    }

    /// <summary>
    /// Serializable, Download source Url to "destinationVariable" property,
    /// 
    /// Variable in ParameterDic:
    /// Progress :  IProgress[TransferProgress] (Optional)
    /// HttpClientFunc : Func[HttpClient] (Optional)    
    /// </summary>
    public class Download : ScriptCommandBase
    {
        /// <summary>
        /// Func[HttpClient], which is used to download file, and dispose when completed, optional, default = "HttpClient"
        /// </summary>
        public string HttpClientKey { get; set; }
        /// <summary>
        /// Web url to download, required, default = "Url"
        /// </summary>
        public string UrlKey { get; set; }
        /// <summary>
        /// After downloaded, store in ParameterDic[thisProperty] as ByteArray, default = "Stream"
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<Download>();

        /// <summary>
        /// Serializable, Download source Url to "destinationVariable" property,
        /// </summary>
        public Download() :
            base("Download", "Progress")
        {
            UrlKey = "{Url}";
            DestinationKey = "{Stream}";
            HttpClientKey = "{HttpClient}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var pdv = pm.GetValue<IProgress<TransferProgress>>("{Progress}", NullTransferProgress.Instance);
            string url = pm.GetValue<string>(UrlKey);
            if (url == null)
                return ResultCommand.Error(new ArgumentException("Unspecified Url."));

            try
            {
                using (var httpClient =
                    pm.ContainsKey(HttpClientKey) && pm[HttpClientKey] is Func<HttpClient> ? ((Func<HttpClient>)pm[HttpClientKey])() :
                    new HttpClient())
                {
                    var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, pm.CancellationToken);
                    if (!response.IsSuccessStatusCode)
                        throw new WebException(String.Format("{0} when downloading {1}", response.StatusCode, url));

                    MemoryStream destStream = new MemoryStream();
                    logger.Info(String.Format("{0} = Stream of {1}", DestinationKey, url));
                    using (Stream srcStream = await response.Content.ReadAsStreamAsync())
                    {
                        pdv.Report(TransferProgress.From(url));
                        byte[] buffer = new byte[1024];
                        ulong totalBytesRead = 0;
                        ulong totalBytes = 0;
                        try { totalBytes = (ulong)srcStream.Length; }
                        catch (NotSupportedException) { }

                        int byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);
                        while (byteRead > 0)
                        {
                            await destStream.WriteAsync(buffer, 0, byteRead, pm.CancellationToken);
                            totalBytesRead = totalBytesRead + (uint)byteRead;
                            short percentCompleted = (short)((float)totalBytesRead / (float)totalBytes * 100.0f);
                            pdv.Report(TransferProgress.UpdateCurrentProgress(percentCompleted));

                            byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);

                        }
                        await destStream.FlushAsync();
                    }

                    pm.SetValue(DestinationKey, destStream.ToByteArray());
                    return NextCommand;
                }
            }
            catch (Exception ex)
            {
                return ResultCommand.Error(ex);
            }
        }

    }
}
