using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        public static IScriptCommand GetFileName(string pathVariable = "{Path}", string destinationVariable = "{Destination}", 
            IScriptCommand nextCommand = null)
        {
            return new GetFilePart()
            {
                FilePartType = FilePartType.FileName,
                PathKey = pathVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
        public static IScriptCommand GetDirectoryName(string pathVariable = "{Path}", string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return new GetFilePart()
            {
                FilePartType = FilePartType.DirectoryName,
                PathKey = pathVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public enum FilePartType {  FileName, DirectoryName }

    public class GetFilePart : ScriptCommandBase
    {
        public string PathKey { get; set; }
        public string DestinationKey { get; set; }
        public FilePartType FilePartType { get; set; }

        public GetFilePart()
            : base("GetFilePart")
        {
            PathKey = "{Path}";
            DestinationKey = "{Destination}";
            FilePartType = FilePartType.FileName;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            string path = pm.ReplaceVariableInsideBracketed(PathKey);
            switch (FilePartType)
            {
                case Script.FilePartType.FileName: pm.SetValue(DestinationKey, PathFE.GetFileName(path)); break;
                case Script.FilePartType.DirectoryName: pm.SetValue(DestinationKey, PathFE.GetDirectoryName(path)); break;
                default: throw new NotSupportedException("FilePartType=" +  FilePartType.ToString()); 
            }

            return NextCommand;
        }
    }
}
