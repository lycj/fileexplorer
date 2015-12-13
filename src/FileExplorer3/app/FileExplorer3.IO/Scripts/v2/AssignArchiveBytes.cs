using FileExplorer;
using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.IO.Compress;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        public static IScriptCommand AssignArchiveBytes(string pathVariable, string destinationVariable, IScriptCommand nextCommand = null)
        {
            return new AssignArchiveBytes()
            {
                PathKey = pathVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DiskParseOrCreateArchive(string profileVariable = "{Profile}", string pathVariable = "{Path}", string destVariable = "{Entry}",
           IScriptCommand nextCommand = null)
        {
            return
                CoreScriptCommands.ParsePath(profileVariable, pathVariable, destVariable,
                    nextCommand, //FoundCommand                     
                    //NotFoundCommand
                    CoreScriptCommands.DiskCreatePath(profileVariable, pathVariable, false, destVariable, NameGenerationMode.NoRename,
                        IOScriptCommands.AssignArchiveBytes(pathVariable, "{ArchiveBytes}",
                            CoreScriptCommands.DiskOpenStream(destVariable, "{DestStream}", FileAccess.Write,
                                CoreScriptCommands.CopyStream("{ArchiveBytes}", "{DestStream}"), 
                                    CoreScriptCommands.ParsePath(profileVariable, pathVariable, destVariable, nextCommand)))));
                
        }
    }

    public class AssignArchiveBytes : ScriptCommandBase
    {
        public string PathKey { get; set; }

        public string DestinationKey { get; set; }

        public AssignArchiveBytes()
            : base("AssignArchiveBytes")
        {
            PathKey = "{Path}";
            DestinationKey = "{ArchiveBytes}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            string path = pm.ReplaceVariableInsideBracketed(PathKey);
            string ext = (path.Contains(".") ? PathFE.GetExtension(path) : path).TrimStart('.');

            pm.SetValue(DestinationKey, SevenZipWrapper.GetArchiveBytes(ext));
            return NextCommand;
        }

    }
}
