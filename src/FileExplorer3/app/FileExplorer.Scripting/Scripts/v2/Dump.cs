using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FileExplorer.WPF.Utils;
using MetroLog;

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
        public static IScriptCommand DumpDebug(string variable, int indentLevel = 0,
            IScriptCommand nextCommand = null)
        {
            return new Dump()
            {                
                IndentLevel = indentLevel,
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
        public static IScriptCommand DumpLogger(LogLevel logLevel, string variable, int indentLevel = 0, 
            IScriptCommand nextCommand = null)
        {
            return new Dump()
            {
                DestinationType = Print.PrintDestinationType.Logger,
                IndentLevel = indentLevel,
                VariableKey = variable,
                LogLevel = logLevel,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DumpConsole(string variable, int indentLevel = 0, 
            IScriptCommand nextCommand = null)
        {
            return new Dump()
            {
                DestinationType = Print.PrintDestinationType.Console,
                IndentLevel = indentLevel,
                VariableKey = variable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class Dump : Print
    {
        public static string Format = "{0} = {1}";

        public Dump()
            : base("Dump")
        {            
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            object obj = pm.GetValue(VariableKey);
            
            if (obj != null)
            {
                print(obj.ToString());
                var typeInfo = obj is Array ? typeof(Array).GetTypeInfo() : obj.GetType().GetTypeInfo();
                foreach (var pi in typeInfo.EnumeratePropertyInfoRecursive())
                {
                    print(string.Format(Format, pi.Name, pi.GetValue(obj)));
                }
            }

            return base.Execute(pm);
        }
    }
}
