using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{
    public interface IScriptRunner
    {
        void Run(Queue<IScriptCommand> cmds, IParameterDic initialParameters);
        Task RunAsync(Queue<IScriptCommand> cmds, IParameterDic initialParameters);
    }

}
