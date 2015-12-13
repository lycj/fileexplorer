using FileExplorer.Defines;
using FileExplorer.Models;
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
        /// Serializable, Copy contents from file1 to file2
        /// </summary>
        /// <param name="sourceFileVariable">Filepath of source</param>
        /// <param name="destinationFileVariable">Filepath of destination</param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCopyFile(string sourceProfileVariable = "{SourceProfile}", string sourceFileVariable = "{SourceFile}",
            string destinationProfileVariable = "{DestinationProfile}", string destinationFileVariable = "{DestinationFile}",
            IScriptCommand nextCommand = null)
        {
            
            return CoreScriptCommands.ParsePath(sourceProfileVariable, sourceFileVariable, dcSourceVariable,
              CoreScriptCommands.DiskParseOrCreateFile(destinationProfileVariable, destinationFileVariable, dcDestVariable,
                CoreScriptCommands.DiskOpenStream(dcSourceVariable, "{SourceStream}", FileExplorer.Defines.FileAccess.Read,
                    CoreScriptCommands.DiskOpenStream(dcDestVariable, "{DestinationStream}", FileExplorer.Defines.FileAccess.Write,
                        CoreScriptCommands.CopyStream("{SourceStream}", "{DestinationStream}",
                        CoreScriptCommands.NotifyEntryChanged(ChangeType.Created, null, dcSourceVariable, null, dcDestVariable,
                            ScriptCommands.Reset(nextCommand, dcSourceVariable, dcDestVariable)))))),
              ResultCommand.Error(new FileNotFoundException(sourceFileVariable)));
        }

        /// <summary>
        /// Not serializable, Copy contents from srcFile to destFile.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="destFile"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCopyFile(IEntryModel srcFile, IEntryModel destFile, IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign("{DiskCopyFile-Source}", srcFile, false,
                   ScriptCommands.Assign("{DiskCopyFile-Dest}", destFile, false,
                    CoreScriptCommands.DiskOpenStream(dcSourceVariable, "{SourceStream}", FileExplorer.Defines.FileAccess.Read,
                   CoreScriptCommands.DiskOpenStream(dcDestVariable, "{DestinationStream}", FileExplorer.Defines.FileAccess.Write,
                       CoreScriptCommands.CopyStream("{CopyStream-Source}", "{CopyStream-Dest}",
                       CoreScriptCommands.NotifyEntryChanged(ChangeType.Created, null, dcSourceVariable, null, dcDestVariable,
                           ScriptCommands.Reset(nextCommand, dcSourceVariable, dcDestVariable)))))));
        }

        private static string dcSourceVariable = "{DiskCopyFile-Source}";
        private static string dcDestVariable = "{DiskCopyFile-Destination}";
    }
}
