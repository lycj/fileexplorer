using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Models;
using FileExplorer.Models.SevenZipSharp;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        /// <summary>
        /// Serializable, transfer source entry (file or directory) to destination directory.
        /// For transfer to ISzsItemModel only, if unsure destination use DiskTransfer with allowCustom on.
        /// </summary>
        /// <param name="srcEntryVariable">Entry to transfer(IEntryModel)</param>
        /// <param name="destDirectoryVariable">Destination of transfer (Directory IEntryModel)</param>
        /// <param name="removeOriginal"></param>
        /// <param name="allowCustomImplementation"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand SzsDiskTransfer(string srcEntryVariable = "{Source}", string destDirectoryVariable = "{Destination}", bool removeOriginal = false,
           IScriptCommand nextCommand = null)
        {
            return new SzsDiskTransfer()
            {
                SourceEntryKey = srcEntryVariable,
                DestinationDirectoryEntryKey = destDirectoryVariable,
                RemoveOriginal = removeOriginal,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Not Serializable, transfer source entry to destentry.
        /// For transfer to ISzsItemModel only, if unsure destination use DiskTransfer with allowCustom on.
        /// </summary>
        /// <param name="srcModel"></param>
        /// <param name="destDirModel"></param>
        /// <param name="removeOriginal"></param>
        /// <param name="allowCustomImplementation"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand SzsDiskTransfer(IEntryModel[] srcModels, ISzsItemModel destDirModel, bool removeOriginal = false,
            IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign("{SourceDiskTransferEntry}", srcModels, false,
                ScriptCommands.Assign("{DestinationDiskTransferEntry}", destDirModel, false,
                SzsDiskTransfer("{SourceDiskTransferEntry}", "{DestinationDiskTransferEntry}",
                removeOriginal, nextCommand)));
        }

    }


    public class SzsDiskTransfer : ScriptCommandBase
    {
        /// <summary>
        /// IEntryModel or IEntryModel[] to transfer to destination, default = "{Source}"
        /// </summary>
        public string SourceEntryKey { get; set; }

        /// <summary>
        /// Destination directory IEntryModel (ISzsItemModel), default = "{Destination}" 
        /// </summary>
        public string DestinationDirectoryEntryKey { get; set; }

        /// <summary>
        /// Whether remove original (e.g. Move) after transfer, default = false
        /// </summary>
        public bool RemoveOriginal { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SzsDiskTransfer>();

        public SzsDiskTransfer()
            : base("SzsDiskTransfer")
        {
            SourceEntryKey = "{Source}";
            DestinationDirectoryEntryKey = "{Destination}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            Dictionary<string, Stream> compressDic = new Dictionary<string, Stream>();
            try
            {
                IEntryModel[] srcEntries = await pm.GetValueAsEntryModelArrayAsync(SourceEntryKey);
                ISzsItemModel destEntry = await pm.GetValueAsEntryModelAsync(DestinationDirectoryEntryKey, null) as ISzsItemModel;

                //If destination is not SzsRoot, use DiskTransfer instead.
                SzsProfile destProfile = destEntry.Profile as SzsProfile;
                if (destProfile == null)
                {
                    logger.Warn(String.Format("{0} isn't Szs based entry, DiskTransfer is used instead.", destEntry.Name));
                    return IOScriptCommands.DiskTransfer(SourceEntryKey, DestinationDirectoryEntryKey, null, RemoveOriginal, false, NextCommand);
                }
                if (!destEntry.IsDirectory) return ResultCommand.Error(new ArgumentException(DestinationDirectoryEntryKey + " is not a folder."));

                Func<IEntryModel, bool> fileAndArchiveOnly = em => !em.IsDirectory || (em is SzsRootModel);
                Func<IEntryModel, bool> lookupDirectoryNotArchiveFilter = em => em.IsDirectory && !(em is SzsRootModel);

                IProgress<TransferProgress> progress = pm.GetProgress();

                string archiveType = destProfile.Path.GetExtension((destEntry as ISzsItemModel).Root.Name);
                logger.Info(String.Format("Compressing {0} -> {1} using SzsDiskTransfer",
                    srcEntries.GetDescription(), destEntry.Name));

                await Task.Run(async () =>
                {

                    #region OpenStream of files
                    foreach (var srcEntry in srcEntries)
                    {
                        IDiskProfile srcProfile = srcEntry.Profile as IDiskProfile;
                        if (srcProfile == null)
                            break;


                        if (fileAndArchiveOnly(srcEntry))
                        {
                            logger.Debug(String.Format("Added to Dictionary : {0} -> {1}", srcEntry.FullPath, srcEntry.Name));
                            progress.Report(TransferProgress.SetMessage(ProgressType.Running, srcEntry.Name));
                            compressDic.Add(srcEntry.Name, await srcProfile.DiskIO
                                .OpenStreamAsync(srcEntry, Defines.FileAccess.Read, pm.CancellationToken));
                        }
                        else
                        {
                            IList<IEntryModel> srcSubEntries = await srcProfile.ListRecursiveAsync(srcEntry, pm.CancellationToken,
                                fileAndArchiveOnly, lookupDirectoryNotArchiveFilter, false);

                            foreach (var srcSubEntry in srcSubEntries)
                            {
                                string relativePath =
                                    destProfile.Path.Combine(
                                    destEntry.RelativePath,
                                    srcSubEntry.FullPath.Replace(srcEntry.Parent.FullPath, "").TrimStart('\\')
                                    );
                                logger.Debug(String.Format("Added to Dictionary : {0} -> {1}", srcSubEntry.FullPath, relativePath));
                                progress.Report(TransferProgress.SetMessage(ProgressType.Running, relativePath));
                                compressDic.Add(relativePath, await srcProfile.DiskIO
                                    .OpenStreamAsync(srcSubEntry, Defines.FileAccess.Read, pm.CancellationToken));
                            }
                        }

                    }
                    #endregion

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

                    progress.Report(TransferProgress.To(destEntry.Name));
                    using (await destProfile.WorkingLock.LockAsync())
                    using (var stream = await destProfile.DiskIO.OpenStreamAsync(destEntry, Defines.FileAccess.ReadWrite, pm.CancellationToken))
                        destProfile.Wrapper.CompressMultiple(archiveType, stream, compressDic, progress1);

                    logger.Info(String.Format("{0} items transfered", compressDic.Count()));
                    return CoreScriptCommands.NotifyEntryChanged( ChangeType.Changed, destEntry, NextCommand);
                });


                return NextCommand;
            }            
            finally
            {
                #region Dispose Streams
                if (compressDic != null)
                    foreach (var stream in compressDic.Values)
                        stream.Dispose();
                #endregion
            }
        }

    }
}
