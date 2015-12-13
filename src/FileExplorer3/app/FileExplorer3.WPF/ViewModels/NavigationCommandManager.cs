using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using System.Windows.Input;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    public class NavigationCommandManager : CommandManagerBase
    {
        #region Constructor

        public NavigationCommandManager(INavigationViewModel nvm, IEventAggregator events,
             params IExportCommandBindings[] additionalBindingExportSource)
            : base(additionalBindingExportSource)
        {
            _nvm = nvm;

            InitCommandManager();
             ToolbarCommands = new ToolbarCommandsHelper(events,
                null,
                null)
                {
                };

        }

        #endregion

        #region Methods
        protected override IParameterDicConverter setupParamDicConverter()
        {
            return ParameterDicConverters.ConvertVMParameter(new Tuple<string, object>("Navigation", _nvm));
        }



        protected override IEnumerable<string> getScriptCommands()
        {
            yield return "Back";
            yield return "Next";
            yield return "Up";
        }

        protected override void setupScriptCommands(dynamic commandDictionary)
        {
            commandDictionary.Back = new SimpleScriptCommand("GoBack", (pd) =>
            {
                pd.AsVMParameterDic().Navigation.GoBack();
                return ResultCommand.OK;
            }, pd => pd.AsVMParameterDic().Navigation.CanGoBack);

            commandDictionary.Next = new SimpleScriptCommand("GoNext", (pd) =>
            {
                pd.AsVMParameterDic().Navigation.GoNext();
                return ResultCommand.OK;
            }, pd => pd.AsVMParameterDic().Navigation.CanGoNext);

            commandDictionary.Up = new SimpleScriptCommand("GoUp", (pd) =>
            {
                pd.AsVMParameterDic().Navigation.GoUp();
                return ResultCommand.OK;
            }, pd => pd.AsVMParameterDic().Navigation.CanGoUp);
 

        }

        protected override IExportCommandBindings[] setupExportBindings()
        {
            List<IExportCommandBindings> exportBindingSource = new List<IExportCommandBindings>();            
            exportBindingSource.Add(
                new ExportCommandBindings(
                ScriptCommandBinding.FromScriptCommand(NavigationCommands.BrowseBack, this, (ch) => ch.CommandDictionary.Back, ParameterDicConverter, ScriptBindingScope.Explorer),
                ScriptCommandBinding.FromScriptCommand(NavigationCommands.BrowseForward, this, (ch) => ch.CommandDictionary.Next, ParameterDicConverter, ScriptBindingScope.Explorer),
                ScriptCommandBinding.FromScriptCommand(NavigationCommands.BrowseHome, this, (ch) => ch.CommandDictionary.Up, ParameterDicConverter, ScriptBindingScope.Explorer)
                ));

            return exportBindingSource.ToArray();
        }

        #endregion

        #region Data

        private INavigationViewModel _nvm;

        #endregion

        #region Public Properties

        #endregion
    }
}
