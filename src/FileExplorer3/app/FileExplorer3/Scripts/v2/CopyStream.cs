using MetroLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {
        /// <summary>
        /// Serializable, Copy contents from ParameterDic[sourceVariable] (Stream or Byte[]) to ParameterDic[destinationVariable] (Stream).
        /// </summary>
        /// <param name="sourceVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand CopyStream(string sourceVariable = "{Source}", string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return new CopyStream()
            {
                SourceKey = sourceVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Serializable, Copy contents from ParameterDic[SourceKey] (Stream or Byte[]) to ParameterDic[thisProperty] (Stream).
    /// </summary>
    public class CopyStream : ScriptCommandBase
    {
        /// <summary>
        /// Stream or ByteArray is read from  ParameterDic[thisProperty], default = "Source"
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// Stream to write, default = "Destination"
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<CopyStream>();

        public CopyStream()
        {
            SourceKey = "{Source}";
            DestinationKey = "{Destination}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            byte[] srcStreamAsByte = pm.GetValue<byte[]>(SourceKey);
            Stream srcStream = srcStreamAsByte != null ? new MemoryStream(srcStreamAsByte) :
                pm.GetValue<Stream>(SourceKey);
            Stream destStream = pm.GetValue<Stream>(DestinationKey);

            if (srcStream == null)
                return ResultCommand.Error(new ArgumentException(SourceKey + " not a stream."));
            if (destStream == null)
                return ResultCommand.Error(new ArgumentException(DestinationKey + " not a stream."));

            logger.Debug(String.Format("{0} -> {1}", SourceKey, DestinationKey));
            await srcStream.CopyToAsync(destStream);

            return NextCommand;
        }
    }
}
