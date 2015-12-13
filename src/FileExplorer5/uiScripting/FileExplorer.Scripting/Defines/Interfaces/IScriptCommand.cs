using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{

    public interface IScriptCommand
    {
        ScriptCommandBase NextCommand { get; }

        /// <summary>
        /// Call ExecuteAsync() calls ConfigureAwait(ContinueOnCaptureContext), if command is UI command, set to true.
        /// </summary>
        bool ContinueOnCaptureContext { get; }
        string CommandKey { get; }

        IScriptCommand Execute(IParameterDic pm);
        Task<IScriptCommand> ExecuteAsync(IParameterDic pm);

        bool CanExecute(IParameterDic pm);
    }

   
}
