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
        /// Serializable, run commands in queue, sequences or in parallel (if async).
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="thenCommand"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public static IScriptCommand Run(RunMode mode = RunMode.Queue,
            IScriptCommand thenCommand = null, params IScriptCommand[] commands)
        {
            return new RunCommands()
            {
                Mode = mode,
                NextCommand = (ScriptCommandBase)thenCommand,
                ScriptCommands = commands.Cast<ScriptCommandBase>().ToArray()
            };
        }

        public static IScriptCommand RunQueue(IScriptCommand thenCommand = null, params IScriptCommand[] commands)
        {
            return Run(RunMode.Queue, thenCommand, commands);
        }

        public static IScriptCommand RunSequence(IScriptCommand thenCommand = null, params IScriptCommand[] commands)
        {
            return Run(RunMode.Sequence, thenCommand, commands);
        }

        public static IScriptCommand RunParallel(IScriptCommand thenCommand = null, params IScriptCommand[] commands)
        {
            return Run(RunMode.Parallel, thenCommand, commands);
        }
    }

    public enum RunMode
    {
        /// <summary>
        /// Run ScriptCommands one by one, when it returns a command, it's queued. (Default)
        /// </summary>
        Queue,
        /// <summary>
        /// Run all command together, ParameterDic are cloned per instance. (async only)
        /// </summary>
        Parallel,
        /// <summary>
        /// Run ScriptCommands one by one, when it returns a command, it run it next.
        /// </summary>
        Sequence
    }

    /// <summary>
    /// Serializable, run a number of ScriptCommand in a sequence.
    /// </summary>
    public class RunCommands : ScriptCommandBase
    {


        /// <summary>
        /// A list of ScriptCommands to run.
        /// </summary>
        public ScriptCommandBase[] ScriptCommands { get; set; }

        /// <summary>
        /// How to run commands, default = Queue
        /// </summary>
        public RunMode Mode { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<RunCommands>();



        public RunCommands()
            : base("RunCommands")
        {
            Mode = RunMode.Queue;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            switch (Mode)
            {
                case RunMode.Parallel:
                case RunMode.Queue:
                    ScriptRunner.RunScript(pm, ScriptCommands);
                    break;
                case RunMode.Sequence:
                    foreach (var cmd in ScriptCommands)
                    {
                        ScriptRunner.RunScript(pm, cmd);
                        if (pm.Error != null)
                            return ResultCommand.Error(pm.Error);
                    }
                    break;
                default:
                    return ResultCommand.Error(new NotSupportedException(Mode.ToString()));
            }

            if (pm.Error != null)
                return ResultCommand.Error(pm.Error);
            else return NextCommand;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return ScriptCommands.Length == 0 || ScriptCommands.First().CanExecute(pm);
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            switch (Mode)
            {
                case RunMode.Parallel:
                    await Task.WhenAll(ScriptCommands.Select(cmd => ScriptRunner.RunScriptAsync(pm.Clone(), cmd)));
                    break;
                case RunMode.Queue:
                    await ScriptRunner.RunScriptAsync(pm, ScriptCommands)
                        .ConfigureAwait(this.ContinueOnCaptureContext);
                    break;
                case RunMode.Sequence:
                    foreach (var cmd in ScriptCommands)
                    {
                        await ScriptRunner.RunScriptAsync(pm, cmd)    
                            .ConfigureAwait(this.ContinueOnCaptureContext);
                        if (pm.Error != null)
                            return ResultCommand.Error(pm.Error);
                    }
                    break;
                default:
                    return ResultCommand.Error(new NotSupportedException(Mode.ToString()));
            }

            if (pm.Error != null)
                return ResultCommand.Error(pm.Error);
            else return NextCommand;
        }

        public override bool ContinueOnCaptureContext
        {
            get
            {
                return ScriptCommands.Any(c => c.RequireCaptureContext());
            }
            set { }
        }
    }


}
