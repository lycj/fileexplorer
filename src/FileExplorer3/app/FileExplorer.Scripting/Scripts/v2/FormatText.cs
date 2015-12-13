using FileExplorer;
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
        /// Serializable, Format a text from a string with variable(s).
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value">If equals to null, remove the variable. (or use ScriptCommands.Reset)</param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FormatText(string destinationVariable = "{Variable}", string value = "Variable1 is {Variable1}", 
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new FormatText()
            {
                VariableKey = destinationVariable,
                Value = value,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand FormatTextFunc(string destinationVariable = "{Variable}", Func<string> valueFunc = null, 
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            valueFunc = valueFunc ?? (() => null);
            return new FormatText()
            {
                VariableKey = destinationVariable,
                ValueFunc = valueFunc,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }


    }


    /// <summary>
    /// Serializable, Format a text from a string with variable(s).
    /// </summary>
    public class FormatText : Assign
    {


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FormatText>();

        public FormatText()
            : base("FormatText")
        {
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            object value = Value;
            if (ValueFunc != null)
                value = ValueFunc();

            if (!(value is string))
                value = value.ToString();
            else value = pm.ReplaceVariableInsideBracketed((string)value);

            return ScriptCommands.Assign(VariableKey, value, SkipIfExists, NextCommand);         
        }
    }

}
