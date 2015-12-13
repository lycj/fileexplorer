using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public class BreadcrumbCommandManager : CommandManagerBase
    {
        
        #region Constructor

        public BreadcrumbCommandManager(IBreadcrumbViewModel bvm, IEventAggregator events, 
            params IExportCommandBindings[] additionalBindingExportSource)
            : base(additionalBindingExportSource)
        {
            _bvm = bvm;
            InitCommandManager();
        }

        #endregion

        #region Methods
        protected override IParameterDicConverter setupParamDicConverter()
        {
            return ParameterDicConverters.ConvertVMParameter(new Tuple<string, object>("Breadcrumb", _bvm));
        }

        protected override IEnumerable<string> getScriptCommands()
        {
            yield return "ToggleBreadcrumb";
        }

        protected override void setupScriptCommands(dynamic commandDictionary)
        {
            commandDictionary.ToggleBreadcrumb = new SimpleScriptCommand("ToggleBreadcrumb",
                 pd =>
                 {
                     IBreadcrumbViewModel bread = pd["Breadcrumb"] as IBreadcrumbViewModel;
                     bread.EnableBreadcrumb = !bread.EnableBreadcrumb; return ResultCommand.NoError;
                 });
        }

        protected override IExportCommandBindings[] setupExportBindings()
        {
            List<IExportCommandBindings> exportBindingSource = new List<IExportCommandBindings>();            
            exportBindingSource.Add(
                new ExportCommandBindings(
                    ScriptCommandBinding.FromScriptCommand(ExplorerCommands.ToggleBreadcrumb, this, (ch) => ch.CommandDictionary.ToggleBreadcrumb, ParameterDicConverter, ScriptBindingScope.Explorer)
                ));

            return exportBindingSource.ToArray();            
        }

        #endregion

        #region Data

        private IBreadcrumbViewModel _bvm;

        #endregion

        #region Public Properties

        #endregion

    }
}
