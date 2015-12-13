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
        public static IScriptCommand Delay(int delayTimeInMs = 1000, IScriptCommand nextCommand = null)
        {
            return new Delay()
            {
                DelayTime = delayTimeInMs,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class Delay : ScriptCommandBase
    {
        /// <summary>
        /// Time to delay (in ms)
        /// </summary>
        public int DelayTime { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<Delay>();

        public Delay()
            : base("Delay")
        {
            DelayTime = 1000;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            await Task.Delay(DelayTime);
            return NextCommand;
        }

    }
}
