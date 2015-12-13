using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        public static IScriptCommand RunCommadLine(string executableVariable = "{Executable}",
            string workingDirectoryVariable = null, 
            string argumentVariable = "{Argument}",            
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal, IScriptCommand nextCommand = null)
        {
            return new RunCommadLine()
            {
                ExecutableKey = executableVariable,
                WorkingDirectoryKey = workingDirectoryVariable,
                ArgumentKey = argumentVariable,                
                WindowStyle = windowStyle,
                NextCommand = (ScriptCommandBase)nextCommand
            };            
        }

        public static IScriptCommand OpenCommandPrompt(string startupDirectoryVariable = "{StartupDirectory}", IScriptCommand nextCommand = null)
        {
            return RunCommadLine("cmd.exe", startupDirectoryVariable, null, ProcessWindowStyle.Normal, nextCommand);
        }
    }

    public class RunCommadLine : ScriptCommandBase
    {
        /// <summary>
        /// Full path Name of the Executable, default = "{Executable}".
        /// </summary>
        public string ExecutableKey { get; set; }
        /// <summary>
        /// Argument to use when starting command line, default = "{Argument}".
        /// </summary>
        public string ArgumentKey { get; set; }

        /// <summary>
        /// Working directory, default = null.
        /// </summary>
        public string WorkingDirectoryKey { get; set; }

        /// <summary>
        /// WindowStyle when starting command line, default = Normal.
        /// </summary>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Where to store exit code of the run process (int), Default = null (not wait).
        /// </summary>
        public string ResultCodeKey { get; set; }

        public RunCommadLine()
            : base("RunCommandLine")
        {
            ExecutableKey = "{Executable}";
            WorkingDirectoryKey = null;
            ArgumentKey = "{Argument}";
            WindowStyle = ProcessWindowStyle.Normal;
            ResultCodeKey = null;
        }


        public override IScriptCommand Execute(ParameterDic pm)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = WindowStyle;
            startInfo.FileName = pm.ReplaceVariableInsideBracketed(ExecutableKey);
            
            string workingDirectory = pm.ReplaceVariableInsideBracketed(WorkingDirectoryKey);
            if (!String.IsNullOrEmpty(workingDirectory))
                startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = pm.ReplaceVariableInsideBracketed(ArgumentKey);

            process.StartInfo = startInfo;

            if (ResultCodeKey != null)
                process.WaitForExit();
            process.Start();

            if (ResultCodeKey != null)
                pm.SetValue(ResultCodeKey, process.ExitCode);

            return NextCommand;
        }
    }
}

