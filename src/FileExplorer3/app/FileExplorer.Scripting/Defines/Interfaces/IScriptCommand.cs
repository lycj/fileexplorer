using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{

    public interface IScriptCommand
    {
        ScriptCommandBase NextCommand { get; }

        /// <summary>
        /// Call ExecuteAsync() calls ConfigureAwait(ContinueOnCaptureContext), if command is UI command, set to true.
        /// </summary>
        bool ContinueOnCaptureContext { get; }
        string CommandKey { get; }

        IScriptCommand Execute(ParameterDic pm);
        Task<IScriptCommand> ExecuteAsync(ParameterDic pm);

        bool CanExecute(ParameterDic pm);
    }

   
}
