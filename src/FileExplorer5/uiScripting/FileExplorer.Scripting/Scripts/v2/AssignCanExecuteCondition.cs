using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Utils;

namespace FileExplorer.Scripting
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Specify the canExecute condition by execute a chain of commands.    
        /// </summary>
        /// <param name="conditionCommand">Run to check CanExecuteCondition, Default = ResultCommand.OK</param>
        /// <param name="nextCommand">Run after CanExecuteCondition.</param>
        /// <returns></returns>
        public static IScriptCommand AssignCanExecuteCondition(
            IScriptCommand conditionCommand = null, IScriptCommand nextCommand = null)
        {
            conditionCommand = conditionCommand ?? ResultCommand.OK;
            return new AssignCanExecuteCondition()
            {
                ConditionCommand = (ScriptCommandBase)conditionCommand,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Specify the canExecute condition by execute a chain of commands.    
    /// </summary>
    public class AssignCanExecuteCondition : ScriptCommandBase
    {
        /// <summary>
        /// Invoking CanExecute() will run the ConditionCommands, and only if 
        /// ParameterDic.IsHandled and no ParameterDic.Error = null will return true.
        /// Default = "ResultCommand.OK"
        /// </summary>
        public ScriptCommandBase ConditionCommand { get; set; }


        public AssignCanExecuteCondition()
            : base("AssignCanExecuteCondition")
        {
            ConditionCommand = ResultCommand.OK;
        }

        public override async Task<IScriptCommand> ExecuteAsync(IParameterDic pm)
        {
            if (CanExecute(pm))
                return NextCommand;
            else return ResultCommand.Error(pm.Error() ?? new ArgumentException("pm"));
        }

        public override bool CanExecute(IParameterDic pm)
        {
            IParameterDic pm2 = pm.Clone();
            ScriptRunner.RunScriptAsync(pm2, ConditionCommand);
            return pm2.IsHandled() && pm2.Error() == null;
        }
    }
}
