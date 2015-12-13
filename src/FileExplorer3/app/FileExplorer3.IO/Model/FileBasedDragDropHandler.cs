using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.IO;
using FileExplorer.WPF.BaseControls;
using FileExplorer.UIEventHub;
using FileExplorer.Defines;
using System.Windows.Data;

namespace FileExplorer.Models
{
    /// <summary>
    /// Create virtual items in preset path if not created already.
    /// </summary>
    public class HandleFileDropped : INotifyDropped
    {
        IEntryModel[] _models;
        public HandleFileDropped(IEntryModel[] models)
        {
            _models = models;
        }

        public void NotifyPrepareDrop(VirtualDataObject sender, string format)
        {
            FileDropDataObject dataObject = sender as FileDropDataObject;

            foreach (var m in _models)
                if (m.Profile is IDiskProfile)
                {
                    var mapping = (m.Profile as IDiskProfile).DiskIO.Mapper[m];
                    if (mapping != null && !mapping.IsCached)
                        AsyncUtils.RunSync(() => (m.Profile as IDiskProfile).DiskIO
                            .WriteToCacheAsync(m, CancellationToken.None)
                            );
                }

            //AsyncUtils.RunSync(() => Task.Run(async () =>
            //    {
            //        foreach (var m in _models)
            //            if (m.Profile is IDiskProfile)
            //            {
            //                var mapping = (m.Profile as IDiskProfile).DiskIO.Mapper[m];                              
            //                if (mapping != null && !mapping.IsCached)
            //                    await (m.Profile as IDiskProfile).DiskIO.WriteToCacheAsync(m, CancellationToken.None);
            //            }
            //    }));
        }
    }

    public class FileBasedDragDropHandler : IDragDropHandler
    {
        private static IProfile _fsiProfile; //For loading drag items.        
        public  DiskTransfer TransferCommand { get; set; }

        public ISupportDrag DragHelper { get; private set; }
        public ISupportDrop DropHelper { get; private set; }


        private IProfile _profile;
        private Func<IEntryModel, bool> _getIsVirtualFunc;
        private IValueConverter converter;
        private IValueConverter dataObjConverter;

        public FileBasedDragDropHandler(IProfile profile, Func<IEntryModel, bool> getIsVirtualFunc = null)
        {
            _profile = profile;
            _fsiProfile = profile is FileSystemInfoProfile ? (FileSystemInfoProfile)profile :
                new FileSystemInfoProfile(profile.Events);
            _getIsVirtualFunc = getIsVirtualFunc ?? (em => true);

            TransferCommand =
                new DiskTransfer();
               //new FileExplorer.Script.TransferCommand((effect, source, destDir) =>
               //    source.Profile is IDiskProfile ?                      
               //        IOScriptCommands.DiskTransfer(source, destDir, effect == DragDropEffectsEx.Move, true)                       
               //        : ResultCommand.Error(new NotSupportedException())
               //    );
            
            converter = LambdaValueConverter.ConvertUsingCast<IDraggable, IEntryModel>();
            dataObjConverter = new LambdaValueConverter<IEnumerable<IEntryModel>, IDataObject>(
             ems => getDataObject(ems), da => getEntryModels(da));   
        }

        public ISupportDrag GetDragHelper(IEnumerable<IEntryModel> entries)
        {
            return new LambdaShellDragHelper<IEntryModel>(converter, dataObjConverter, () => entries,
                ems => QueryDrag(ems), (ems, da, eff) => OnDragCompleted(ems, da, eff));
        }

        public ISupportDrop GetDropHelper(IEntryModel destEm)
        {
            return new LambdaShellDropHelper<IEntryModel>(converter, dataObjConverter,
               (ems, eff) => QueryDrop(ems, destEm, eff), (ems, da, eff) => OnDropCompleted(ems, da, destEm, eff));
        }

        private IDataObject getDataObject(IEnumerable<IEntryModel> entries)
        {
            entries =
                entries.Select(m => m is IConvertedEntryModel ? (m as IConvertedEntryModel).OriginalEntryModel : m);

            //if (entries.Any(e => _getIsVirtualFunc(e)))
            //{
            var retVal = new FileDropDataObject(new HandleFileDropped(entries.ToArray()));
            retVal.SetFileDropData(entries
                .Where(m => m.Profile is IDiskProfile)
                .Select(m =>
                    _getIsVirtualFunc(m) ?
                    (IFileDropItem)new VirtualFileDrop((m.Profile as IDiskProfile).DiskIO.Mapper[m].IOPath, m.IsDirectory) :
                    new FileDrop((m.Profile as IDiskProfile).DiskIO.Mapper[m].IOPath, m.IsDirectory))
                .Where(fd => fd.FileSystemPath != null)
                .ToArray());

            return retVal;
            //}
            //else return new DataObject(DataFormats.FileDrop,
            //    entries.Select(e => (e.Profile as IDiskProfile).DiskIO.Mapper[e].IOPath).ToArray()
            //    );
        }


        private IEnumerable<IEntryModel> getEntryModels(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNameList = dataObject.GetData(DataFormats.FileDrop) as string[];
                foreach (var fn in fileNameList)
                {
                    IEntryModel vm = null;
                    try
                    {
                        if (Directory.Exists(fn))
                            vm = new FileSystemInfoModel(_fsiProfile, new DirectoryInfo(fn));
                        else if (File.Exists(fn))
                            vm = new FileSystemInfoModel(_fsiProfile, new FileInfo(fn));
                    }
                    catch
                    {
                        vm = null;
                    }

                    if (vm != null)
                        yield return vm;
                }
            }
        }

        public DragDropEffectsEx QueryDrag(IEnumerable<IEntryModel> entries)
        {
            foreach (var e in entries)
                if (e.Profile is IDiskProfile && (e.Profile as IDiskProfile).DiskIO.Mapper[e].IsVirtual)
                    return DragDropEffectsEx.Copy;
            return DragDropEffectsEx.Copy | DragDropEffectsEx.Move | DragDropEffectsEx.Link;
        }

        public void OnDragCompleted(IEnumerable<IEntryModel> draggables, IDataObject da, DragDropEffectsEx effect)
        {
            //if (effect == DragDropEffectsEx.Move)
            //    draggables.First().Profile.
        }


        public bool QueryCanDrop(IEntryModel destDir)
        {
            return destDir.IsDirectory;
        }

        public QueryDropEffects QueryDrop(IEnumerable<IEntryModel> entries, IEntryModel destDir, DragDropEffectsEx allowedEffects)
        {
            if (entries.Any(e => e.Equals(destDir) || e.Parent.Equals(destDir)))
                return QueryDropEffects.None;

            if (destDir.IsDirectory)
                return QueryDropEffects.CreateNew(DragDropEffectsEx.Copy | DragDropEffectsEx.Move, DragDropEffectsEx.Copy);
            else return QueryDropEffects.None;
        }


        public DragDropEffectsEx OnDropCompleted(IEnumerable<IEntryModel> entries, IDataObject da, IEntryModel destDir, DragDropEffectsEx allowedEffects)
        {
            string fileName = PathEx.GetFileName(destDir.FullPath);
            if (fileName == null) fileName = destDir.FullPath;

            if (entries.Count() == 0)
                return DragDropEffectsEx.None;
            if (entries.Any(e => e.Equals(destDir) || e.Parent.Equals(destDir)))
                return DragDropEffectsEx.None;
            


            //if (MessageBox.Show(
            //    String.Format("[OnDropCompleted] ({2}) {0} entries to {1}",
            //    entries.Count(), fileName, allowedEffects), "FileBasedDragDropHandler.OnDropCompleted", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DragDropEffectsEx effect = allowedEffects.HasFlag(DragDropEffectsEx.Copy) ? DragDropEffectsEx.Copy :
                    allowedEffects.HasFlag(DragDropEffectsEx.Move) ? DragDropEffectsEx.Move : DragDropEffectsEx.None;

                if (effect == DragDropEffectsEx.None)
                    return DragDropEffectsEx.None;
                
                ScriptRunner.RunScriptAsync(
                    WPFScriptCommands.ShowProgress(effect.ToString(), 
                    IOScriptCommands.DiskTransfer(entries.ToArray(), destDir, effect == DragDropEffectsEx.Move, true), true));
                return effect;
            };
            return DragDropEffectsEx.None;
        }

       
    }
}
