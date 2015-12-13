using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Tools;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.Defines;
using Caliburn.Micro;
using FileExplorer.IO;
using FileExplorer.Models;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        /// <summary>
        /// Transfer srcModel to destDirModel using IDiskProfile.DiskIO.GetTransferCommand, which allow to use custom implementation.
        /// </summary>
        /// <param name="srcModel"></param>
        /// <param name="destDirModel"></param>
        /// <param name="removeOriginal"></param>
        /// <returns></returns>
        [Obsolete("Use CoreScriptCommands.DiskTransfer")]
        public static IScriptCommand Transfer(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal = false, 
            bool allowCustomImplementation = true, IScriptCommand nextCommand = null)
        {
            IScriptCommand retCommand = allowCustomImplementation ? 
                (destDirModel.Profile as IDiskProfile).DiskIO.GetTransferCommand(srcModel, destDirModel, removeOriginal) :
                new FileTransferScriptCommand(srcModel, destDirModel, removeOriginal);

            return nextCommand == null ? retCommand : ScriptCommands.RunInSequence(retCommand, nextCommand);
        }

        [Obsolete("Use CoreScriptCommands.DiskTransferChild")]
        public static IScriptCommand TransferChild(IEntryModel srcModel, IEntryModel destDirModel, 
            Func<IEntryModel, bool> filterFunc = null, bool recrusive = false, IScriptCommand nextCommand = null)
        {
            return WPFScriptCommands.List(srcModel, filterFunc, null, recrusive, ems =>
                         WPFScriptCommands.ReportProgress(TransferProgress.IncrementTotalEntries(ems.Length),
                               ScriptCommands.ForEach(ems, em =>
                                     ScriptCommands.RunInSequence(
                                            IOScriptCommands.Transfer(em, destDirModel),
                                            WPFScriptCommands.ReportProgress(TransferProgress.IncrementProcessedEntries())), 
                                                  nextCommand)));
        }

        [Obsolete("Use ScriptCommands.DiskDelete")]
        public static IScriptCommand DeleteFromParameter = new DeleteFileBasedEntryCommand(WPFScriptCommands.GetEntryModelFromParameter);

        [Obsolete("Use ScriptCommands.DiskDelete")]
        public static IScriptCommand Delete(params IEntryModel[] deleteModels)
        {
            return new DeleteFileBasedEntryCommand(pd => deleteModels);
        }

    }



    /// <summary>
    /// These Commands require IDIskProfile.
    /// </summary>    
    public class OpenWithScriptCommand : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryModel[]> _srcModelFunc;
        OpenWithInfo _info;

        /// <summary>
        /// Launch a file (e.g. txt) using the OpenWithInfo, if null then default method will be used, Require Parameter (IEntryModel[])
        /// </summary>
        /// <param name="info"></param>
        public OpenWithScriptCommand(OpenWithInfo info = null, Func<ParameterDic, IEntryModel[]> srcModelFunc = null)
            : base("OpenWith")
        {
            _info = info;
            _srcModelFunc = srcModelFunc ?? WPFScriptCommands.GetEntryModelFromParameter;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return base.CanExecute(pm);
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var parameter = _srcModelFunc(pm);
            //Last line can obtain from FileList.Selection, not current directory. This just temporary fix.
            if (parameter != null && parameter.Count() == 0)
                parameter = new[] { pm.GetValue<IEntryModel>("{FileList.CurrentDirectory}") };

            if (parameter != null && parameter.Count() == 1)
            {
                bool _isFolder = parameter[0].IsDirectory;
                IDiskProfile profile = parameter[0].Profile as IDiskProfile;
                if (profile == null)
                    return ResultCommand.Error(new NotSupportedException("IDiskProfile"));

                if (profile.DiskIO.Mapper is NullDiskPatheMapper)
                    return ResultCommand.Error(new NotSupportedException());

                string appliedFileName = await profile.DiskIO.WriteToCacheAsync(parameter[0], pm.CancellationToken);

                if (_isFolder || appliedFileName.StartsWith("::{"))
                {
                    if (appliedFileName.StartsWith("::{") || Directory.Exists(appliedFileName))
                        try { Process.Start(appliedFileName); }
                        catch (Exception ex) { return ResultCommand.Error(ex); }
                }
                else
                {

                    ProcessStartInfo psi = null;
                    if (File.Exists(appliedFileName))
                        if (_info != null)
                        {
                            if (!_info.Equals(OpenWithInfo.OpenAs))
                                psi = new ProcessStartInfo(OpenWithInfo.GetExecutablePath(_info.OpenCommand), appliedFileName);
                            else
                            {
                                //http://bytes.com/topic/c-sharp/answers/826842-call-windows-open-dialog
                                psi = new ProcessStartInfo("Rundll32.exe");
                                psi.Arguments = String.Format(" shell32.dll, OpenAs_RunDLL {0}", appliedFileName);
                            }
                        }
                        else psi = new ProcessStartInfo(appliedFileName);

                    if (psi != null)
                        try { Process.Start(psi); }
                        catch (Exception ex) { return ResultCommand.Error(ex); }

                }

                return ResultCommand.OK;
            }
            else return ResultCommand.Error(new Exception("Wrong Parameter type or more than one item."));
        }

    }


    [Obsolete("Use CoreScriptCommands.DiskDelete")]
    public class DeleteFileBasedEntryCommand : ScriptCommandBase
    {
        [Obsolete]
        public static DeleteFileBasedEntryCommand FromParameter =
            new DeleteFileBasedEntryCommand(WPFScriptCommands.GetEntryModelFromParameter);

        private Func<ParameterDic, IEntryModel[]> _srcModelFunc;

        public DeleteFileBasedEntryCommand(Func<ParameterDic, IEntryModel[]> srcModelFunc)
            : base("Delete")
        {
            _srcModelFunc = srcModelFunc;
        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            CancellationToken ct = pm.CancellationToken;
            var progress = pm.ContainsKey("Progress") ? pm["Progress"] as IProgress<TransferProgress> : NullTransferProgress.Instance;
            var _srcModels = _srcModelFunc(pm);

            if (_srcModels != null)
            {
                progress.Report(TransferProgress.IncrementTotalEntries(_srcModels.Count()));
                Task[] tasks = _srcModels.Select(m =>
                    (m.Profile as IDiskProfile).DiskIO.DeleteAsync(m, ct)
                    .ContinueWith(
                        tsk => progress.Report(TransferProgress.IncrementProcessedEntries()))
                    ).ToArray();
                await Task.WhenAll(tasks);

                return new RunInSequenceScriptCommand(
                    _srcModels.Select(m => new NotifyChangedCommand(m.Profile, m.FullPath, ChangeType.Deleted))
                    .ToArray());
            }

            return ResultCommand.NoError;
        }
    }




    #region FileTransferScriptCommand and it's subcommands

    [Obsolete("Use IOScriptCommands.DiskTransfer")]
    public class StreamFileTransferCommand : ScriptCommandBase
    {        
        private IEntryModel _srcModel;
        private IEntryModel _destDirModel;
        private bool _removeOriginal;
        private IProgress<TransferProgress> _progress;

        public StreamFileTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal, IProgress<TransferProgress> progress)
            : base("StreamFileTransfer")
        {
            if (srcModel.Profile is IDiskProfile && destDirModel.Profile is IDiskProfile)
            {
                _progress = progress;
                _srcModel = srcModel;
                _destDirModel = destDirModel;
                _removeOriginal = removeOriginal;
            }
            else throw new ArgumentException("Transfer work with IDiskProfile only.");
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var srcProfile = _srcModel.Profile as IDiskProfile;
            var destProfile = _destDirModel.Profile as IDiskProfile;
            string destName = _srcModel.GetName();
            string srcFullName = _srcModel.FullPath;
            string destFullName = _destDirModel.Combine(destName);

            ChangeType ct = ChangeType.Created;
            if (destFullName.StartsWith("::"))
                return ResultCommand.Error(new ArgumentException("Transfer does not work with shell folders."));
            if (File.Exists(destFullName))
            {
                File.Delete(destFullName);
                ct = ChangeType.Changed;
            }

            using (var srcStream = await srcProfile.DiskIO.OpenStreamAsync(_srcModel.FullPath,
                FileExplorer.Defines.FileAccess.Read, pm.CancellationToken))
            using (var destStream = await destProfile.DiskIO.OpenStreamAsync(destFullName,
                FileExplorer.Defines.FileAccess.Write, pm.CancellationToken))
            {
                _progress.Report(TransferProgress.From(srcFullName, destFullName));
                await StreamUtils.CopyStreamAsync(srcStream, destStream, false, false, false,
                    p => _progress.Report(new TransferProgress() { CurrentProgressPercent = p })).ConfigureAwait(false);
            }
            if (_removeOriginal)
                await srcProfile.DiskIO.DeleteAsync(_srcModel, pm.CancellationToken).ConfigureAwait(false);

            _progress.Report(TransferProgress.IncrementProcessedEntries());

            return new NotifyChangedCommand(_destDirModel.Profile, destFullName,
                _srcModel.Profile, srcFullName, ct);
        }

    }

    [Obsolete("Use IOScriptCommands.DiskTransferChild.")]
    public class CopyDirectoryTransferCommand : ScriptCommandBase
    {
        private IEntryModel _srcModel;
        private IEntryModel _destDirModel;
        private bool _removeOriginal;
        private IProgress<TransferProgress> _progress;
        public CopyDirectoryTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal, IProgress<TransferProgress> progress)
            : base("CopyDirectoryTransfer")
        {
            if (srcModel.Profile is IDiskProfile && destDirModel.Profile is IDiskProfile)
            {
                _progress = progress;
                _srcModel = srcModel;
                _destDirModel = destDirModel;
                _removeOriginal = removeOriginal;
            }
            else throw new ArgumentException("Transfer work with IDiskProfile only.");
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var destProfile = _destDirModel.Profile as IDiskProfile;

            var destMapping = (_destDirModel.Profile as IDiskProfile).DiskIO.Mapper[_destDirModel];
            var srcMapping = (_srcModel.Profile as IDiskProfile).DiskIO.Mapper[_srcModel];
            string destName = PathFE.GetFileName(srcMapping.IOPath);
            string destFullName = destProfile.Path.Combine(_destDirModel.FullPath, destName); //PathFE.Combine(destMapping.IOPath, destName);

            IEntryModel destModel = await _destDirModel.Profile.ParseAsync(destFullName);

            if (destModel == null)
            {
                destModel = await destProfile.DiskIO.CreateAsync(destFullName, true, pm.CancellationToken);

                destModel = (await _destDirModel.Profile.ListAsync(_destDirModel, CancellationToken.None, em =>
                        em.FullPath.Equals(destFullName,
                        StringComparison.CurrentCultureIgnoreCase), true)).FirstOrDefault();
                _destDirModel.Profile.Events.PublishOnUIThread(new EntryChangedEvent(ChangeType.Created, destFullName));
            }

            if (destModel == null)
                return ResultCommand.Error(new Exception("Cannot construct destination " + destFullName));
            else
            {
                _progress.Report(TransferProgress.From(_srcModel.FullPath, destFullName));
                _progress.Report(TransferProgress.IncrementProcessedEntries()); //dest directory created
            }
            var srcSubModels = (await _srcModel.Profile.ListAsync(_srcModel, CancellationToken.None)).ToList();

            _progress.Report(TransferProgress.IncrementTotalEntries(srcSubModels.Count())); //source entries

            var resultCommands = srcSubModels.Select(m =>
                (IScriptCommand)new FileTransferScriptCommand(m, destModel, _removeOriginal)).ToList();
            resultCommands.Add(new NotifyChangedCommand(_destDirModel.Profile, destFullName, ChangeType.Created));

            //if (_removeOriginal)
            //    resultCommands.Add(new DeleteEntryCommand(_srcModel));

            return new RunInSequenceScriptCommand(resultCommands.ToArray());
        }
    }

   
    [Obsolete("Use IOScriptCommands.DiskTransfer instead")]
    public class FileTransferScriptCommand : ScriptCommandBase
    {
        private IEntryModel _srcModel;
        private IEntryModel _destDirModel;
        private bool _removeOriginal;

        public FileTransferScriptCommand(IEntryModel srcModel, IEntryModel destDirModel,
            bool removeOriginal = false)
            : base(removeOriginal ? "Move" : "Copy")
        {
            _srcModel = srcModel;
            _destDirModel = destDirModel;
            _removeOriginal = removeOriginal;

            if (!(srcModel.Profile is IDiskProfile) || !(destDirModel.Profile is IDiskProfile))
                throw new NotSupportedException();
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


                if (!srcMapping.IsVirtual && !destMapping.IsVirtual && _removeOriginal)
                {
                    //Disk based transfer
                    progress.Report(TransferProgress.From(_srcModel.FullPath, destFullName));
                    if (_srcModel.IsDirectory)
                    {
                        if (Directory.Exists(destFullName))
                            Directory.Delete(destFullName, true);
                        Directory.Move(srcMapping.IOPath, destFullName); //Move directly.
                        progress.Report(TransferProgress.IncrementProcessedEntries());
                    }
                    else
                    {
                        if (File.Exists(destFullName))
                            File.Delete(destFullName);
                        File.Move(srcMapping.IOPath, destFullName);
                    }
                    progress.Report(TransferProgress.IncrementProcessedEntries());
                    return new NotifyChangedCommand(_destDirModel.Profile, destFullName, _srcModel.Profile,
                        _srcModel.FullPath, ChangeType.Moved);
                }
                else
                {
                    if (_srcModel.IsDirectory)
                        return new CopyDirectoryTransferCommand(_srcModel, _destDirModel, _removeOriginal, progress);
                    else
                        return new StreamFileTransferCommand(_srcModel, _destDirModel, _removeOriginal, progress);
                }



                //switch (_transferMode)
                //{
                //    case DragDropEffects.Move:
                //      
                //       

                //    case DragDropEffects.Copy:
                //        Directory.CreateDirectory(destFullName);
                //        _destDirModel.Profile.Events.Publish(new EntryChangedEvent(destFullName, ChangeType.Created));

                //        var destModel = (await _destDirModel.Profile.ListAsync(_destDirModel, em =>
                //                em.FullPath.Equals(destFullName,
                //                StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault();
                //        var srcSubModels = (await _srcModel.Profile.ListAsync(_srcModel)).ToList();

                //        var resultCommands = srcSubModels.Select(m => 
                //            (IScriptCommand)new FileTransferScriptCommand(m, destModel, _transferMode)).ToList();
                //        resultCommands.Insert(0, new NotifyChangedCommand(_destDirModel.Profile, destFullName, ChangeType.Created));

                //        return new RunInSequenceScriptCommand(resultCommands.ToArray());
                //    default:
                //        throw new NotImplementedException();
                //}

                //}
                //else
                //{

                //}

                return ResultCommand.NoError;
            }
            catch (Exception ex)
            {
                return ResultCommand.Error(ex);
            }
        }
    }

    #endregion



}
