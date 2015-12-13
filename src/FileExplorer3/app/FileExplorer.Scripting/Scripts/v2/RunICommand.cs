using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.Script
{

    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Run a WPF command (ICommand) defined in ParameterDic.
        /// </summary>
        /// <param name="commandVariable"></param>
        /// <param name="throwIfError"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand RunICommand(string commandVariable,
            string parameterVariable,
            bool throwIfError = false, IScriptCommand nextCommand = null)
        {
            return new RunICommand()
            {
                CommandKey = commandVariable,
                ParameterKey = parameterVariable,
                ThrowIfError = throwIfError,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand RunICommand(ICommand command,
            string parameterVariable,
            bool throwIfError = false, IScriptCommand nextCommand = null)
        {
            string commandVariable = "{" + string.Format("Command{0}", new Random().Next()) + "}";

            return ScriptCommands.Assign(commandVariable, command, false,
                ScriptCommands.RunICommand(commandVariable, parameterVariable, throwIfError, nextCommand));
        }
    }

    /// <summary>
    /// Serializable,  Run a WPF command (ICommand) defined in ParameterDic.
    /// </summary>
    public class RunICommand : ScriptCommandBase
    {
        /// <summary>
        /// Point to script Command (IScriptCommand) to run.
        /// </summary>
        public string CommandKey { get; set; }

        public string ParameterKey { get; set; }

        public bool ThrowIfError { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<RunICommand>();

        public RunICommand()
            : base("RunICommand")
        {
            CommandKey = "{Command}";
            ParameterKey = "{Parameter}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            ICommand command = pm.GetValue<ICommand>(CommandKey);
            object parameter = pm.GetValue<object>(ParameterKey);
            if (command == null && ThrowIfError)
                return ResultCommand.Error(new ArgumentNullException(CommandKey));

            if ((command == null || !command.CanExecute(parameter)) && ThrowIfError)
            {
                logger.Debug("Running " + CommandKey);
                return ResultCommand.Error(new ArgumentException("CanExecute = false."));
            }

            if (command != null)
                command.Execute(parameter);

            return NextCommand;
        }

    }
}
