using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;
using FileExplorer.Script;

namespace FileExplorer
{
    public static partial class ExtensionMethods
    {
        public static bool RequireCaptureContext(this IScriptCommand command)
        {
            return command.ContinueOnCaptureContext || (command.NextCommand != null && command.NextCommand.RequireCaptureContext());
        }

        public static void Run(this IScriptRunner scriptRunner, ParameterDic initialParameters, params IScriptCommand[] cmds)
        {
            scriptRunner.Run(new Queue<IScriptCommand>(cmds), initialParameters);
        }
        public static async Task RunAsync(this IScriptRunner scriptRunner, ParameterDic initialParameters, params IScriptCommand[] cmds)
        {
            await scriptRunner.RunAsync(new Queue<IScriptCommand>(cmds), initialParameters);
        }

        public static ParameterDic ConvertAndMerge(this IParameterDicConverter converter, ParameterDic pd, object parameter = null, params object[] additionalParameters)
        {
            var convertedPd = converter.Convert(parameter, additionalParameters);
            return ParameterDic.Combine(convertedPd, pd);
        }

    }
}
