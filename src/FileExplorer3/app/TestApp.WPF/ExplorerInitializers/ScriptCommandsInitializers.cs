using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.WPF.Models;
using FileExplorer.WPF;
using FileExplorer.Defines;
using FileExplorer.IO;

namespace TestApp
{
  

    public class ScriptCommandsInitializers : IViewModelInitializer<IExplorerViewModel>
    {
        public static IScriptCommand TransferCommand { get; private set; }

        public static void InitializeScriptCommands(IExplorerViewModel explorerModel, 
            IWindowManager windowManager, IEventAggregator events, params IProfile[] profiles)
        {
            var initilizer = AppViewModel.getInitializer(windowManager, events, explorerModel.RootModels.ToArray(),
               new ColumnInitializers(),
               new ScriptCommandsInitializers(windowManager, events),
               new ToolbarCommandsInitializers(windowManager));


            explorerModel.FileList.Commands.Commands.Open =
             FileList.IfSelection(evm => evm.Count() == 1,
                 FileList.IfSelection(evm => evm[0].EntryModel.IsDirectory,
                     FileList.OpenSelectedDirectory, //Selected directory                        
                     FileList.AssignSelectionToParameter(
                         new OpenWithScriptCommand(null))),  //Selected non-directory
                 ResultCommand.NoError //Selected more than one item, ignore.
                 );

            
            explorerModel.FileList.Commands.Commands.NewFolder =
                 FileList.Do(flvm => WPFScriptCommands.CreatePath(
                        flvm.CurrentDirectory, "NewFolder", true, true,
                     //FileList.Do(flvm => CoreScriptCommands.DiskCreateFolder(
                     //        flvm.CurrentDirectory, "NewFolder", "{DestinationFolder}", NameGenerationMode.Rename, 
                        m => FileList.Refresh(FileList.Select(fm => fm.Equals(m), ResultCommand.OK), true)));

            explorerModel.FileList.Commands.Commands.Delete =
                 FileList.IfSelection(evm => evm.Count() >= 1,
                    WPFScriptCommands.IfOkCancel(windowManager, pd => "Delete",
                        pd => String.Format("Delete {0} items?", (pd["FileList"] as IFileListViewModel).Selection.SelectedItems.Count),
                        WPFScriptCommands.ShowProgress(windowManager, "Delete",
                                    ScriptCommands.RunInSequence(
                                        FileList.AssignSelectionToParameter(
                                            IOScriptCommands.DeleteFromParameter),
                                        new HideProgress())),
                        ResultCommand.NoError),
                    NullScriptCommand.Instance);
            
            
            explorerModel.FileList.Commands.Commands.Copy =
                 FileList.IfSelection(evm => evm.Count() >= 1,
                   WPFScriptCommands.IfOkCancel(windowManager, pd => "Copy",
                        pd => String.Format("Copy {0} items?", (pd["FileList"] as IFileListViewModel).Selection.SelectedItems.Count),
                            ScriptCommands.RunInSequence(FileList.AssignSelectionToParameter(ClipboardCommands.Copy)),
                            ResultCommand.NoError),
                    NullScriptCommand.Instance);

            explorerModel.FileList.Commands.Commands.Cut =
                  FileList.IfSelection(evm => evm.Count() >= 1,
                   WPFScriptCommands.IfOkCancel(windowManager, pd => "Cut",
                        pd => String.Format("Cut {0} items?", (pd["FileList"] as IFileListViewModel).Selection.SelectedItems.Count),
                            ScriptCommands.RunInSequence(FileList.AssignSelectionToParameter(ClipboardCommands.Cut)),
                            ResultCommand.NoError),
                    NullScriptCommand.Instance);

            explorerModel.DirectoryTree.Commands.Commands.Delete =
                       WPFScriptCommands.IfOkCancel(windowManager, pd => "Delete",
                           pd => String.Format("Delete {0}?", ((pd["DirectoryTree"] as IDirectoryTreeViewModel).Selection.RootSelector.SelectedValue.Label)),
                                WPFScriptCommands.ShowProgress(windowManager, "Delete",
                                        ScriptCommands.RunInSequence(
                                            DirectoryTree.AssignSelectionToParameter(
                                                IOScriptCommands.DeleteFromParameter),
                                            new HideProgress())),
                           ResultCommand.NoError);


            //explorerModel.DirectoryTree.Commands.Commands.Map =
            //    UIScriptCommands.ExplorerShow

            if (profiles.Length > 0)

                explorerModel.DirectoryTree.Commands.CommandDictionary.Map =
                    Explorer.PickDirectory(initilizer, profiles,
                    dir => Explorer.BroadcastRootChanged(RootChangedEvent.Created(dir)), ResultCommand.NoError);



            //explorerModel.Commands.ScriptCommands.Transfer =
            //    TransferCommand =
            //    new TransferCommand((effect, source, destDir) =>
            //        source.Profile is IDiskProfile ?
            //            IOScriptCommands.Transfer(source, destDir, effect == DragDropEffects.Move)
            //            : ResultCommand.Error(new NotSupportedException())
            //        , _windowManager);
        }
       
        private IWindowManager _windowManager;
        private IEventAggregator _events;
        private IProfile[] _profiles;
        public ScriptCommandsInitializers(IWindowManager windowManager, IEventAggregator events, params IProfile[] profiles)
        {
            _windowManager = windowManager;
            _events = events;
            _profiles = profiles;
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            InitializeScriptCommands(explorerModel, _windowManager, _events, _profiles);
        }
    }

}


