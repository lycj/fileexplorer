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
        /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
        /// </summary>
        /// <param name="explorerVariable"></param>
        /// <param name="directoryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerDoAsync(string explorerVariable = "{Explorer}",
            Func<IExplorerViewModel, Task> doActionAsync = null,
            bool continueOnCaptureContext = false, 
            IScriptCommand nextCommand = null)
        {
            return new ExplorerDoAsync()
            {
                ExplorerKey = explorerVariable,
                DoActionAsync = doActionAsync ?? (e => Task.Delay(0)),
                NextCommand = (ScriptCommandBase)nextCommand, 
                ContinueOnCaptureContext = continueOnCaptureContext
            };
        }

        /// <summary>
        /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
        /// </summary>
        /// <param name="doActionAsync"></param>
        /// <param name="continueOnCaptureContext"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerDoAsync(
            Func<IExplorerViewModel, Task> doActionAsync = null,
            bool continueOnCaptureContext = false,
            IScriptCommand nextCommand = null)
        {
            return ExplorerDoAsync("{Explorer}", doActionAsync, continueOnCaptureContext,  nextCommand);
        }

        /// <summary>
        /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
        /// </summary>
        /// <param name="explorerVariable"></param>
        /// <param name="directoryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerDo(string explorerVariable = "{Explorer}",
            Action<IExplorerViewModel> doAction = null,            
            IScriptCommand nextCommand = null)
        {
            return new ExplorerDo()
            {
                ExplorerKey = explorerVariable,
                DoAction = doAction ?? (e => {}),
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
        /// </summary>
        /// <param name="doAction"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerDo(
            Action<IExplorerViewModel> doAction = null,            
            IScriptCommand nextCommand = null)
        {
            return ExplorerDo("{Explorer}", doAction, nextCommand);
        }
    }


    /// <summary>
    /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
    /// </summary>
    public class ExplorerDo : ScriptCommandBase
    {
        /// <summary>
        /// Key point to Explorer, Default = "{Explorer}"
        /// </summary>
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Action to do.
        /// </summary>        
        public Action<IExplorerViewModel> DoAction { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerDo>();

        public ExplorerDo()
            : base("ExplorerDo")
        {
            ExplorerKey = "{Explorer}";
            DoAction = e => { };            
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IExplorerViewModel evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);
            if (evm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));

            DoAction(evm);
            logger.Info("Completed");

            return NextCommand;
        }
    }

    /// <summary>
    /// Not Serializable, given explorer (IExplorerViewModel), do the specified code.
    /// </summary>
    public class ExplorerDoAsync : ScriptCommandBase
    {
        /// <summary>
        /// Key point to Explorer, Default = "{Explorer}"
        /// </summary>
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Action to do.
        /// </summary>        
        public Func<IExplorerViewModel, Task> DoActionAsync { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerDoAsync>();

        public ExplorerDoAsync()
            : base("ExplorerDoAsync")
        {
            ExplorerKey = "{Explorer}";
            DoActionAsync = e => Task.Delay(0);
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IExplorerViewModel evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);
            if (evm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));

            await DoActionAsync(evm);
            logger.Info("Completed");

            return NextCommand;
        }
    }
}
