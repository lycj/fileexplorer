using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public interface IScriptRunner
    {
        void Run(Queue<IScriptCommand> cmds, ParameterDic initialParameters);
        Task RunAsync(Queue<IScriptCommand> cmds, ParameterDic initialParameters);
    }

}
