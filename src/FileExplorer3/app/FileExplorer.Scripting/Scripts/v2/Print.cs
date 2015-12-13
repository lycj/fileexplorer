using FileExplorer.Script;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, print content of a variable to debug.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand PrintDebug(string variable, IScriptCommand nextCommand = null)
        {
            return new Print()
            {
                DestinationType = Print.PrintDestinationType.Debug,
                VariableKey = variable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, print content of a variable to logger.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand PrintLogger(LogLevel logLevel, string variable, IScriptCommand nextCommand = null)
        {
            return new Print()
            {
                DestinationType = Print.PrintDestinationType.Logger,
                VariableKey = variable,
                LogLevel = logLevel,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand PrintConsole(string variable, IScriptCommand nextCommand = null)
        {
            return new Print()
            {
                DestinationType = Print.PrintDestinationType.Console,
                VariableKey = variable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Serializable, print content of a variable to debug.
    /// </summary>
    public class Print : ScriptCommandBase
    {
        public static Action<string> PrintConsoleAction = msg => { throw new NotImplementedException("Print.PrintConsoleAction not assigned."); };

        [Flags]
        public enum PrintDestinationType { Logger = 1 << 0, Debug = 1 << 1, Console = 1 << 2 }

        /// <summary>
        /// Variable to print.
        /// </summary>
        public string VariableKey { get; set; }

        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                if (_indentLevel != value)
                {
                    _indentLevel = value; 
                    Indent = createIndent(value);
                }
            }
        }

        protected string Indent { get; private set; }

        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Where to print to.
        /// </summary>
        public PrintDestinationType DestinationType { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<Print>();
        private int _indentLevel = 0;

        public Print()
            : this("Print")
        {
            IndentLevel = 0;
        }

        protected Print(string commandKey)
            : base(commandKey)
        {
            DestinationType = PrintDestinationType.Logger;
        }

        private static string createIndent(int indentLevel)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indentLevel; i++)
                sb.Append("  ");
            return sb.ToString();
        }

        protected virtual void print(string text)
        {
            string printText = Indent + text;
            if (DestinationType.HasFlag(PrintDestinationType.Debug))
                Debug.WriteLine(printText);
            if (DestinationType.HasFlag(PrintDestinationType.Logger))
                logger.Log(LogLevel, printText);
            if (DestinationType.HasFlag(PrintDestinationType.Console))
                PrintConsoleAction(printText);
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            string variable = pm.ReplaceVariableInsideBracketed(VariableKey);
            print(variable);
            return NextCommand;
        }

    }

}
