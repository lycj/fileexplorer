using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {
        /// <summary>
        /// Serializable, For DiskProfile only, parse a path, if not exists create it, store to destVariable (default Entry), then call next command.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="isFolder"></param>
        /// <param name="destVariable"></param>
        /// <param name="nameGenerationMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskParseOrCreatePath(string profileVariable = "{Profile}", string pathVariable = "{Path}", bool isFolder = false, string destVariable = "{Entry}",
            IScriptCommand nextCommand = null)
        {
            return ParsePath(profileVariable, pathVariable, destVariable, nextCommand, CoreScriptCommands.DiskCreatePath(profileVariable, pathVariable, isFolder, destVariable,
                NameGenerationMode.NoRename, nextCommand));
        }

        /// <summary>
        /// Serializable, For DiskProfile only, parse a file path, if not exists create it, store to destVariable (default Entry), then call next command.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="destVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskParseOrCreateFile(string profileVariable = "{Profile}", string pathVariable = "{Path}", string destVariable = "{Entry}",
           IScriptCommand nextCommand = null)
        {
            return DiskParseOrCreatePath(profileVariable, pathVariable, false, destVariable, nextCommand);
        }

        /// <summary>
        /// Serializable, For DiskProfile only, parse a folder path, if not exists create it, store to destVariable (default Entry), then call next command.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="destVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskParseOrCreateFolder(string profileVariable = "{Profile}", string pathVariable = "{Path}", string destVariable = "{Entry}",
           IScriptCommand nextCommand = null)
        {
            return DiskParseOrCreatePath(profileVariable, pathVariable, true, destVariable, nextCommand);
        }
    }
}
