using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{

    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Run another script command (IScriptCommand) defined in ParameterDic.
        /// </summary>
        /// <param name="commandVariable"></param>
        /// <param name="throwIfError"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Run(string commandVariable, bool throwIfError = false, IScriptCommand nextCommand = null)
        {
            return new RunScriptCommand()
            {
                CommandKey = commandVariable,
                ThrowIfError = throwIfError,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Serializable, Run another script command (IScriptCommand) defined in ParameterDic
    /// </summary>
    public class RunScriptCommand : ScriptCommandBase
    {
        /// <summary>
        /// Point to script Command (IScriptCommand) to run.
        /// </summary>
        public string CommandKey { get; set; }

        public bool ThrowIfError { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<RunCommands>();

        public RunScriptCommand()
            : base("RunScriptCommand")
        {
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IScriptCommand command = pm.GetValue<IScriptCommand>(CommandKey);
            if (command == null && ThrowIfError)
                return ResultCommand.Error(new ArgumentNullException(CommandKey));
            command = command ?? ResultCommand.NoError;

            logger.Info("Running " + CommandKey);
            return ScriptCommands.RunQueue(NextCommand, command);
        }

    }
}
