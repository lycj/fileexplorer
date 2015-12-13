using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable, Set parameter of current ParameterDic to Explorer.CommandManager's, so when it run a script it can use that parameter
        /// </summary>
        /// <param name="explorerVariable"></param>
        /// <param name="variableKeys"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerAssignScriptParameters(string explorerVariable = "{Explorer}", 
            string variableKeys = "", IScriptCommand nextCommand = null)
        {
            return new ExplorerAssignScriptParameters()
            {
                ExplorerKey = explorerVariable,
                VariableKeys = variableKeys,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Set parameter of current ParameterDic to Explorer.CommandManager's, so when it run a script it can use that parameter
    /// </summary>
    public class ExplorerAssignScriptParameters : ScriptCommandBase
    {
        /// <summary>
        /// Key point to Explorer, Default = "{Explorer}"
        /// </summary>        
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Comma separated variables key to assign.
        /// </summary>
        public string VariableKeys { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerAssignScriptParameters>();

        public ExplorerAssignScriptParameters()
            : base("ExplorerAssignScriptParameters")
        {
            ExplorerKey = "{Explorer}";
            VariableKeys = "";
        }

        private void addStartupParameters(ICommandManager cm, ParameterDic parameters)
        {
            cm.ParameterDicConverter.AddAdditionalParameters(parameters);
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IExplorerViewModel evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);
            if (evm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));

            ParameterDic startupParameters = new ParameterDic();
            foreach (var key in VariableKeys.Split(','))
            {
                var val = pm.GetValue(key);
                startupParameters.SetValue(key, val);
            }

            addStartupParameters(evm.Commands, startupParameters);
            addStartupParameters(evm.FileList.Commands, startupParameters);
            addStartupParameters(evm.DirectoryTree.Commands, startupParameters);
            addStartupParameters(evm.Navigation.Commands, startupParameters);
            addStartupParameters(evm.Breadcrumb.Commands, startupParameters);            

            return NextCommand;
        }
    }
}
