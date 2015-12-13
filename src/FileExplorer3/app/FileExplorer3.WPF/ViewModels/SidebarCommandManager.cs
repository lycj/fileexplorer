using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    public class SidebarCommandManager: CommandManagerBase
    {
        #region Constructor

        public SidebarCommandManager(ISidebarViewModel svm, IEventAggregator events,
             params IExportCommandBindings[] additionalBindingExportSource)
            : base(additionalBindingExportSource)
        {
            _svm = svm;

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
            return ParameterDicConverters.ConvertVMParameter(new Tuple<string, object>("Sidebar", _svm));
        }

        protected override IEnumerable<string> getScriptCommands()
        {
            yield return "TogglePreviewer";
        }

        protected override void setupScriptCommands(dynamic commandDictionary)
        {            
            commandDictionary.TogglePreviewer = Sidebar.Toggle();
        }

        protected override IExportCommandBindings[] setupExportBindings()
        {
            List<IExportCommandBindings> exportBindingSource = new List<IExportCommandBindings>();
            exportBindingSource.Add(
              new ExportCommandBindings(
                  ScriptCommandBinding.FromScriptCommand(ExplorerCommands.TogglePreviewer, this, (ch) => ch.CommandDictionary.TogglePreviewer, ParameterDicConverter, ScriptBindingScope.Explorer)
              ));            
            return exportBindingSource.ToArray();
        }

        #endregion

        #region Data

        ISidebarViewModel _svm;
        
        #endregion

        #region Public Properties
        
        #endregion
    }
}
