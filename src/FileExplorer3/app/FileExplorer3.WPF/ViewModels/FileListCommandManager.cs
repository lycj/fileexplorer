using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.BaseControls;
using System.Windows.Input;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    public class FileListCommandManager : CommandManagerBase
    {
        #region Constructor        

        public FileListCommandManager(IFileListViewModel flvm, IWindowManager windowManager, IEventAggregator events, 
            params IExportCommandBindings[] additionalBindingExportSource)
            : base(additionalBindingExportSource)
        {
            _flvm = flvm;            
           
            IEntryModel _currentDirectoryModel = null;
            InitCommandManager();
            ToolbarCommands = new ToolbarCommandsHelper(events, ParameterDicConverter,
                message => { _currentDirectoryModel = message.NewModel; return new IEntryModel[] { _currentDirectoryModel }; },
                message => message.SelectedModels.Count() == 0 && _currentDirectoryModel != null ? new IEntryModel[] { _currentDirectoryModel } : message.SelectedModels.ToArray())
                {
                    ExtraCommandProviders = new[] {                         
                        new StaticCommandProvider(new SelectGroupCommand(flvm), 
                            new ViewModeCommand(flvm),                             
                            new SeparatorCommandModel(),
                            new CommandModel(ExplorerCommands.NewFolder) { IsVisibleOnToolbar = true, 
                                HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE188)                        
                            },
                            new DirectoryCommandModel(
                                new CommandModel(ExplorerCommands.NewFolder) { Header = Strings.strFolder, IsVisibleOnMenu = true }) 
                                { IsVisibleOnMenu = true, Header = Strings.strNew, IsEnabled = true},
                            new ToggleVisibilityCommand(flvm.Sidebar, ExplorerCommands.TogglePreviewer)  
                            ) 
                    }
                };
           

        }

        #endregion

        #region Methods        

        protected override IParameterDicConverter setupParamDicConverter()
        {
            return ParameterDicConverters.ConvertVMParameter(new Tuple<string, object>("FileList", _flvm));
        }
                                   
        protected override IEnumerable<string> getScriptCommands()
        {
            yield return "Open";
            yield return "Delete";
            yield return "NewFolder";
            yield return "Refresh";
            yield return "ToggleRename";
            yield return "Copy";
            yield return "Cut";
            yield return "Paste";
            yield return "OpenTab";
            yield return "NewWindow";
            yield return "ZoomIn";
            yield return "ZoomOut";
        }

        protected override void setupScriptCommands(dynamic commandDictionary)
        {           

            commandDictionary.Refresh = new SimpleScriptCommand("Refresh", (pd) =>
            {
                pd.AsVMParameterDic().FileList.ProcessedEntries.EntriesHelper.LoadAsync(UpdateMode.Update, true);
                return ResultCommand.OK;
            });

            commandDictionary.ToggleRename = FileList.IfSelection(evm => evm.Count() == 1 && evm[0].IsRenamable,
                FileList.ToggleRename, NullScriptCommand.Instance);

            commandDictionary.OpenTab = NullScriptCommand.Instance;
            commandDictionary.NewWindow = NullScriptCommand.Instance;

            commandDictionary.ZoomIn = FileList.Zoom(ZoomMode.ZoomIn);
            commandDictionary.ZoomOut = FileList.Zoom(ZoomMode.ZoomOut);
        }

        protected override IExportCommandBindings[] setupExportBindings()
        {
            List<IExportCommandBindings> exportBindingSource = new List<IExportCommandBindings>();
            exportBindingSource.Add(
                new ExportCommandBindings(
                    ScriptCommandBinding.FromScriptCommand(ApplicationCommands.Open, this, (ch) => ch.CommandDictionary.Open, ParameterDicConverter, ScriptBindingScope.Local),
                    ScriptCommandBinding.FromScriptCommand(ExplorerCommands.NewFolder, this, (ch) => ch.CommandDictionary.NewFolder, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ExplorerCommands.Refresh, this, (ch) => ch.CommandDictionary.Refresh, ParameterDicConverter, ScriptBindingScope.Explorer),
                ScriptCommandBinding.FromScriptCommand(ApplicationCommands.Delete, this, (ch) => ch.CommandDictionary.Delete, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ExplorerCommands.Rename, this, (ch) => ch.CommandDictionary.ToggleRename, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ApplicationCommands.Cut, this, (ch) => ch.CommandDictionary.Cut, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ApplicationCommands.Copy, this, (ch) => ch.CommandDictionary.Copy, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ApplicationCommands.Paste, this, (ch) => ch.CommandDictionary.Paste, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ExplorerCommands.NewWindow, this, (ch) => ch.CommandDictionary.NewWindow, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(ExplorerCommands.OpenTab, this, (ch) => ch.CommandDictionary.OpenTab, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(NavigationCommands.IncreaseZoom, this, (ch) => ch.CommandDictionary.ZoomIn, ParameterDicConverter, ScriptBindingScope.Local),
                ScriptCommandBinding.FromScriptCommand(NavigationCommands.DecreaseZoom, this, (ch) => ch.CommandDictionary.ZoomOut, ParameterDicConverter, ScriptBindingScope.Local),
                new ScriptCommandBinding(ExplorerCommands.ToggleCheckBox, p => true, p => ToggleCheckBox(), ParameterDicConverter, ScriptBindingScope.Explorer),
                new ScriptCommandBinding(ExplorerCommands.ToggleViewMode, p => true, p => ToggleViewMode(), ParameterDicConverter, ScriptBindingScope.Explorer)
                ));

            return exportBindingSource.ToArray();
        }

        public void ToggleViewMode()
        {            
            var viewModeWoSeparator = ViewModeCommand.ViewModes.Where(vm => vm.IndexOf(",-1") == -1).ToArray();

            int curIdx = ViewModeCommand.findViewMode(viewModeWoSeparator, _flvm.Parameters.ItemSize);
            int nextIdx = curIdx + 1;
            if (nextIdx >= viewModeWoSeparator.Count()) nextIdx = 0;

            string viewMode; int step; int itemHeight;
            ViewModeCommand.parseViewMode(viewModeWoSeparator[nextIdx], out viewMode, out step, out itemHeight);
            ViewModeCommand vmc = this.ToolbarCommands.CommandModels.AllNonBindable.First(c => c.CommandModel is ViewModeCommand)
                .CommandModel as ViewModeCommand;
            vmc.SliderValue = step;
        }

        public void ToggleCheckBox()
        {
            _flvm.IsCheckBoxVisible = !_flvm.IsCheckBoxVisible;
        }

        #endregion

        #region Data

        private IFileListViewModel _flvm;

        #endregion

        #region Public Properties


        #endregion




    }
}
