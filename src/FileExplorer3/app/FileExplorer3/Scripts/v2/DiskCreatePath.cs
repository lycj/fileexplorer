using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;
using MetroLog;
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
        /// Serializable, For DiskProfile only, create a folder or file, requires Profile set to an IDiskProfile.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="isFolder"></param>
        /// <param name="destVariable"></param>
        /// <param name="nameGenerationMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCreatePath(string profileVariable = "{Profile}", string pathVariable = "{Path}", bool isFolder = false, string destVariable = "{Entry}",
            NameGenerationMode nameGenerationMode = NameGenerationMode.Rename, IScriptCommand nextCommand = null)
        {
            return new DiskCreatePath()
            {
                ProfileKey = profileVariable,
                PathKey = pathVariable,
                IsFolder = isFolder,
                DestinationKey = destVariable,
                NameGenerationMode = nameGenerationMode,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, For DiskProfile only, create a file, requires Profile set to an IDiskProfile.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="destVariable"></param>
        /// <param name="nameGenerationMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCreateFile(string profileVariable = "{Profile}", string pathVariable = "{Path}", string destVariable = "{Entry}",
            NameGenerationMode nameGenerationMode = NameGenerationMode.Rename, IScriptCommand nextCommand = null)
        {
            return DiskCreatePath(profileVariable, pathVariable, false, destVariable, nameGenerationMode, nextCommand);
        }

        /// <summary>
        /// Serializable, For DiskProfile only, create a folder, requires Profile set to an IDiskProfile.
        /// </summary>
        /// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="destVariable"></param>
        /// <param name="nameGenerationMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCreateFolder(string profileVariable = "{Profile}", string pathVariable = "{Path}", string destVariable = "{Entry}",
           NameGenerationMode nameGenerationMode = NameGenerationMode.Rename, IScriptCommand nextCommand = null)
        {
            return DiskCreatePath(profileVariable, pathVariable, true, destVariable, nameGenerationMode, nextCommand);
        }


        /// <summary>
        /// Not Serializable, create a folder by specified parameter.
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="folderName"></param>
        /// <param name="destVariable"></param>
        /// <param name="nameGenerationMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskCreateFolder(IEntryModel parentFolder, string folderName, string destVariable = "{Entry}",
           NameGenerationMode nameGenerationMode = NameGenerationMode.Rename, IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign("{DiskCreateFolder-Profile}", parentFolder, false,
                ScriptCommands.Assign("{DiskCreateFolder-Path}", parentFolder.Profile.Path.Combine(parentFolder.FullPath, folderName), false,
                DiskCreateFolder("{DiskCreateFolder-Profile}", "{DiskCreateFolder-Path}", destVariable, nameGenerationMode, nextCommand)));
        }

    }

    public class DiskCreatePath : ScriptCommandBase
    {
        /// <summary>
        /// Profile to create path IProfile, default = Profile,
        /// only IDiskProfile will be used.
        /// </summary>
        public string ProfileKey { get; set; }

        /// <summary>
        /// Required, path to create, default = {Path} or C:\Temp
        /// </summary>
        public string PathKey { get; set; }

        /// <summary>
        /// Whether to create folder or file.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Where to store after parse completed, default = Entry.
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// If filename already exists, how to generate a file.
        /// </summary>
        public NameGenerationMode NameGenerationMode { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DiskCreatePath>();

        public DiskCreatePath()
            : base("DiskCreatePath")
        { ProfileKey = "{Profile}"; DestinationKey = "{Entry}"; PathKey = "{Path}"; }


        public override bool CanExecute(ParameterDic pm)
        {
            string path = pm.ReplaceVariableInsideBracketed(PathKey);
            return !String.IsNullOrEmpty(path) && !path.StartsWith("::{"); //Cannot execute if GuidPath            
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            string path = pm.ReplaceVariableInsideBracketed(PathKey);
            if (path == null)
                return ResultCommand.Error(new ArgumentException("Path not specified."));

            IDiskProfile profile = pm.GetValue<IDiskProfile>(ProfileKey);
            if (profile == null)
                return ResultCommand.Error(new ArgumentException(ProfileKey + " is not assigned or not IDiskProfile."));

            string parentPath = profile.Path.GetDirectoryName(path);
            IFileNameGenerator fNameGenerator = FileNameGenerator.FromNameGenerationMode(NameGenerationMode,
                profile.Path.GetFileName(path));

            string fileName = fNameGenerator.Generate();
            while (fileName != null &&
                await profile.ParseAsync(profile.Path.Combine(parentPath, fileName)) != null)
                fileName = fNameGenerator.Generate();

            if (fileName == null)
                return ResultCommand.Error(new ArgumentException("Already exists."));

            string newEntryPath = profile.Path.Combine(parentPath, fileName);
            var createddModel = await profile.DiskIO.CreateAsync(newEntryPath, IsFolder, pm.CancellationToken);
            logger.Info(String.Format("{0} = {1} ({2})", DestinationKey, createddModel.FullPath, IsFolder ? "Folder" : "File"));
            pm.SetValue(DestinationKey, createddModel);

            return CoreScriptCommands.NotifyEntryChanged(ChangeType.Created, null, DestinationKey, NextCommand);
        }
    }
}
