using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Script;
using FileExplorer.Models;
using System.ComponentModel.Composition;


namespace FileExplorer.WPF.ViewModels
{

    public enum FilePickerMode { Open, Save }

    public interface IEntryModelInfo
    {
        string FileName { get; }
        IProfile Profile { get; }
    }

    [Export(typeof(IExplorerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FilePickerViewModel : ExplorerViewModel, IEntryModelInfo
    {
        #region Constructor

        private void init(FilePickerMode mode = FilePickerMode.Open, string filterStr = "All files (*)|*")
        {
            FilterStr = filterStr;
            WindowTitleMask = Enum.GetName(typeof(FilePickerMode), mode);
            _mode = mode;
            _tryCloseCommand = new SimpleScriptCommand("TryClose", pd => { TryClose(true); return ResultCommand.NoError; });

            FileList.Commands.CommandDictionary.Open = FileExplorer.Script.FileList.IfSelection(evm => evm.Count() == 1,
                   FileExplorer.Script.FileList.IfSelection(evm => evm[0].EntryModel.IsDirectory,
                       OpenSelectedDirectory.FromFileList,  //Selected directory
                       new SimpleScriptCommand("", (pd) =>
                       {
                           switch (mode)
                           {
                               case FilePickerMode.Open: Open(); break;
                               case FilePickerMode.Save: Save(); break;
                           }

                           return ResultCommand.NoError;
                       })),   //Selected non-directory
                   ResultCommand.NoError //Selected more than one item.                   
                   );

            FileList.Selection.SelectionChanged += (o1, e1) =>
            {
                var firstDir =
                    FileList.Selection.SelectedItems.FirstOrDefault(evm => evm.EntryModel.IsDirectory);
                if (firstDir != null)
                {
                    //setFileName(firstDir.EntryModel.Label, false);
                }
                else
                {
                    string newFileName = String.Join(",",
                        FileList.Selection.SelectedItems.Where(evm => !evm.EntryModel.IsDirectory)
                        .Select(evm => evm.EntryModel.GetName()));

                    if (!String.IsNullOrEmpty(newFileName))
                        switch (mode)
                        {
                            case FilePickerMode.Save: setFileName(newFileName,
                                 !newFileName.Contains(',')); break;
                            default: setFileName(newFileName); break;
                        }


                }
            };

            FileList.EnableDrag = false;
            FileList.EnableDrop = false;
            FileList.EnableMultiSelect = false;
        }

        [ImportingConstructor]
        public FilePickerViewModel(IWindowManager windowManager, IEventAggregator events)
            : base(windowManager, events)
        {
            init();
        }

        public FilePickerViewModel(IExplorerInitializer initializer, string filterStr,
          FilePickerMode mode = FilePickerMode.Open)
            : base(initializer)
        {
            init(mode, filterStr);
        }

        public FilePickerViewModel(IEventAggregator events, IWindowManager windowManager, string filterStr,
            FilePickerMode mode = FilePickerMode.Open, params IEntryModel[] rootModels)
            : this(new ExplorerInitializer(windowManager, events, rootModels), filterStr, mode)
        {

        }

        #endregion

        #region Methods



        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            UserControl uc = view as UserControl;
            uc.Loaded += (o, e) =>
                {
                    if (Filters.AllNonBindable.Count() > 0)
                        SelectedFilter = Filters.AllNonBindable.First().Filter;                    
                };
        }

        //public void Save()
        //{
        //    string fullPath = FileList.CurrentDirectory.Profile.Path.Combine(FileList.CurrentDirectory.FullPath, FileName);
        //    var foundItem = FileList.ProcessedEntries.EntriesHelper.AllNonBindable.Select(evm => evm.EntryModel)
        //            .FirstOrDefault(em =>
        //            em.FullPath.Equals(fullPath, StringComparison.CurrentCultureIgnoreCase));
        //    if (foundItem != null)
        //    {
        //        string name = foundItem.Profile.Path.GetFileName(foundItem.FullPath);
        //        new IfOkCancel(_windowManager, () => "Overwrite", String.Format("Overwrite {0}?", name), 
        //    }

        //}

        public async Task Save()
        {
            var pm = FileList.Commands.ParameterDicConverter.Convert(new ParameterDic());
            Profile = FileList.CurrentDirectory.Profile;

            //Update FileName in case user does not enter full path name.
            IScriptCommand updateFileName =
                new SimpleScriptCommand("updateFileName", pd =>
                {
                    FileName = Profile.Path.Combine(FileList.CurrentDirectory.FullPath, FileName);
                    return ResultCommand.NoError;
                });

            //Update SelectedFiles property (if it's exists.")
            Func<IEntryModel, IScriptCommand> setSelectedFiles =
                m => new SimpleScriptCommand("SetSelectedFiles", pd =>
                {
                    SelectedFiles = new[] { m }; FileName = m.FullPath; return ResultCommand.NoError;
                });

            //Query whether to overwrite and if so, setSelectedFiles, otherwise throw a user cancel exception and no window close.
            Func<IEntryModel, IScriptCommand> queryOverwrite =
                m => new IfOkCancel(_windowManager,
                        pd => "Overwrite",
                        pd => "Overwrite " + Profile.Path.GetFileName(FileName),
                        setSelectedFiles(m), ResultCommand.Error(new Exception("User cancel")) /* Cancel if user press cancel. */);

            await ScriptRunner.RunScriptAsync(pm,
                ScriptCommands.RunInSequence(
                     Script.FileList.Lookup(
                               m => m.FullPath.Equals(FileName) || m.Profile.Path.GetFileName(m.FullPath).Equals(FileName),
                               queryOverwrite,
                               FileList.CurrentDirectory.Profile.Parse(FileName, queryOverwrite,
                               updateFileName))),
                    _tryCloseCommand
                );
        }

        public async Task Open()
        {
            List<IEntryModel> selectedFiles = new List<IEntryModel>();
            Profile = FileList.CurrentDirectory.Profile;
            Func<IEntryModel, IScriptCommand> addToSelectedFiles =
                m => new SimpleScriptCommand("AddToSelectedFiles", pd => { selectedFiles.Add(m); return ResultCommand.NoError; });
            var pm = FileList.Commands.ParameterDicConverter.Convert(new ParameterDic());
            await ScriptRunner.RunScriptAsync(pm,
            ScriptCommands.RunInSequence(
                ScriptCommands.ForEach(
                    FileName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                    f => Script.FileList.Lookup(
                            m => m.FullPath.Equals(f) || m.Profile.Path.GetFileName(m.FullPath).Equals(f),
                            addToSelectedFiles,
                            FileList.CurrentDirectory.Profile.Parse(f, addToSelectedFiles,
                            ResultCommand.Error(new System.IO.FileNotFoundException(f + " is not found.")))))),
                new SimpleScriptCommand("AssignSelectedFiles", pd => { SelectedFiles = selectedFiles.ToArray(); return ResultCommand.NoError; }),
                _tryCloseCommand
             );
        }

        public void Cancel()
        {
            TryClose(false);
        }



        private void setFileName(string value, bool? canOpen = null)
        {
            _selectedFileName = value;
            NotifyOfPropertyChange(() => FileName);
            if (canOpen.HasValue)
                CanOpen = canOpen.Value;
            else CanOpen = !String.IsNullOrEmpty(value);
        }

        #endregion

        #region Data

        protected IScriptCommand _tryCloseCommand;
        private static ColumnFilter _directoryOnlyFilter =
            ColumnFilter.CreateNew<IEntryModel>("DirectoryOnly", "IsDirectory", evm => evm.IsDirectory);
        IEntryModel[] _selectedFiles;
        bool _canOpen = false, _canSave = false;

        private string _selectedFileName = "";
        private FilePickerMode _mode;

        #endregion

        #region Public Properties

        public FilePickerMode PickerMode { get { return _mode; } set { _mode = value; } }

        public string FileName { get { return _selectedFileName; } set { setFileName(value); } }
        public IProfile Profile { get; private set; }

        public bool IsOpenEnabled { get { return _mode == FilePickerMode.Open; } }
        public bool IsSaveEnabled { get { return _mode == FilePickerMode.Save; } }

        public bool CanOpen
        {
            get { return _canOpen; }
            set
            {
                _canOpen = value;
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanOpen);
            }
        }
        public bool CanSave { get { return CanOpen; } set { CanOpen = value; } }
        public IEntryModel[] SelectedFiles
        {
            get { return _selectedFiles; }
            set { _selectedFiles = value; NotifyOfPropertyChange(() => SelectedFiles); }
        }

        #endregion

    }
}
