using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.IO.Compress;
using FileExplorer.Models;
using FileExplorer.Models.SevenZipSharp;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        public static IScriptCommand CreateArchive(IEntryModel entryModel, string name, bool renameIfExists,
            Func<IEntryModel, IScriptCommand> thenFunc)
        {
            string type = entryModel.Profile.Path.GetExtension(name).ToLower();
            byte[] bytes = SevenZipWrapper.GetArchiveBytes(type);

            if (bytes == null)
                return ResultCommand.Error(new ArgumentException(type + " is not recognized type."));

            return WPFScriptCommands.CreatePath(entryModel, name, false, renameIfExists,
                em => WPFScriptCommands.WriteBytes(em, bytes, thenFunc));
        }

        public static IScriptCommand ParseOrCreateArchive(IDiskProfile profile, string path, Func<IEntryModel, IScriptCommand> thenFunc)
        {
            string type = profile.Path.GetExtension(path).ToLower();
            byte[] bytes = SevenZipWrapper.GetArchiveBytes(type);

            if (bytes == null)
                return ResultCommand.Error(new ArgumentException(type + " is not recognized type."));

            return WPFScriptCommands.ParseOrCreatePath(profile, path, false,
                em => WPFScriptCommands.WriteBytes(em, bytes, thenFunc));
        }
    }
    
    /// <summary>
    /// Uses SevenZipWrapper.CompressMultiple thus quicker then FileTransferScriptCommand which move one file at a time.    
    /// </summary>
    [Obsolete("SzsDiskTransfer")]
    public class SzsBatchTransferScriptCommand : ScriptCommandBase
    {
        private IEntryModel _srcModel;
        private IEntryModel _destDirModel;
        private bool _removeOriginal;

        /// <summary>
        /// DestDirModel must be ISzsItemModel
        /// </summary>
        /// <param name="srcModel"></param>
        /// <param name="destDirModel"></param>
        /// <param name="removeOriginal"></param>
        public SzsBatchTransferScriptCommand(IEntryModel srcModel, IEntryModel destDirModel,
            bool removeOriginal = false)
            : base(removeOriginal ? "Move" : "Copy")
        {
            _srcModel = srcModel;
            _destDirModel = destDirModel;
            _removeOriginal = removeOriginal;

            if (!(srcModel.Profile is IDiskProfile) || !(destDirModel.Profile is SzsProfile))
                throw new NotSupportedException();
        }



        private async Task<IScriptCommand> transferAsync(ParameterDic pm, IEntryModel[] ems,
            IProgress<TransferProgress> progress, IScriptCommand thenCommand)
        {
            Dictionary<string, Stream> compressDic = new Dictionary<string, Stream>();
            IDiskProfile srcProfile = _srcModel.Profile as IDiskProfile;
            string srcParentPath = srcProfile.Path.GetDirectoryName(_srcModel.FullPath);

            foreach (var em in ems)
            {
                string relativePath = em.FullPath.Replace(srcParentPath, "").TrimStart('\\');
                compressDic.Add(relativePath, await srcProfile.DiskIO.OpenStreamAsync(em, Defines.FileAccess.Read, pm.CancellationToken));
            }

            var destProfile = _destDirModel.Profile as SzsProfile;
            string archiveType = destProfile.Path.GetExtension(_destDirModel.Name);

            using (await destProfile.WorkingLock.LockAsync())
                await Task.Run(async () =>
                    {
                        Progress<Defines.ProgressEventArgs> progress1 = new Progress<Defines.ProgressEventArgs>(
                            (pea) =>
                            {                                
                                if (!String.IsNullOrEmpty(pea.Message))
                                    progress.Report(TransferProgress.SetMessage(Defines.ProgressType.Running, pea.Message));
                                if (!String.IsNullOrEmpty(pea.File))
                                    progress.Report(TransferProgress.From(pea.File));
                                if (pea.CurrentProgress != -1 && pea.TotalProgress != -1)
                                    progress.Report(TransferProgress.UpdateCurrentProgress((short)((float)pea.CurrentProgress / (float)pea.TotalProgress * 100.0)));
                            }
                            );

                        progress.Report(TransferProgress.To(_destDirModel.Name));
                        using (var stream = await destProfile.DiskIO.OpenStreamAsync(_destDirModel, Defines.FileAccess.ReadWrite, pm.CancellationToken))
                            destProfile.Wrapper.CompressMultiple(archiveType, stream, compressDic, progress1);
                    });

            return thenCommand;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            try
            {
                var srcProfile = _srcModel.Profile as IDiskProfile;
                var destProfile = _destDirModel.Profile as IDiskProfile;
                var progress = pm.ContainsKey("Progress") ? pm["Progress"] as IProgress<TransferProgress> : NullTransferProgress.Instance;

                var destMapping = (_destDirModel.Profile as IDiskProfile).DiskIO.Mapper[_destDirModel];
                var srcMapping = (_srcModel.Profile as IDiskProfile).DiskIO.Mapper[_srcModel];
                string destName = PathFE.GetFileName(srcMapping.IOPath);
                string destFullName = destProfile.Path.Combine(_destDirModel.FullPath, destName); //PathFE.Combine(destMapping.IOPath, destName);
                

                if (_srcModel.IsDirectory)
                {
                    Func<IEntryModel, bool> filter = em => !em.IsDirectory || (em is SzsRootModel);
                    Func<IEntryModel, bool> lookupFilter = em => em.IsDirectory && !(em is SzsRootModel);
                    return WPFScriptCommands.List(_srcModel, filter, lookupFilter, true, ems =>
                        new SimpleScriptCommandAsync("BatchTransfer", pd => transferAsync(pm, ems, progress,
                             new NotifyChangedCommand(_destDirModel.Profile, destFullName,
                                _srcModel.Profile, _srcModel.FullPath, Defines.ChangeType.Changed))));
                }
                else
                    return IOScriptCommands.DiskTransfer(_srcModel, _destDirModel, _removeOriginal);                                         

                return ResultCommand.NoError;
            }
            catch (Exception ex)
            {
                return ResultCommand.Error(ex);
            }
        }
    }
}
