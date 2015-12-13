using FileExplorer.Script;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Assign a variable to ParameterDic when running.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value">If equals to null, remove the variable. (or use ScriptCommands.Reset)</param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Assign(string variable = "{Variable}", object value = null,
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new Assign()
            {
                VariableKey = variable,
                Value = value,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand AssignValueFunc(string variable = "{Variable}", Func<object> valueFunc = null,
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            valueFunc = valueFunc ?? (() => null);
            return new Assign()
            {
                VariableKey = variable,
                ValueFunc = valueFunc,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand Assign(Dictionary<string, object> variableDic, bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            IScriptCommand cmd = nextCommand;
            foreach (var k in variableDic.Keys)
                cmd = Assign(k, variableDic[k], skipIfExists, cmd);
            return cmd;
        }

        public static IScriptCommand AssignValueFunc(Dictionary<string, Func<object>> variableDic, bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            IScriptCommand cmd = nextCommand;
            foreach (var k in variableDic.Keys)
                cmd = AssignValueFunc(k, variableDic[k], skipIfExists, cmd);
            return cmd;
        }

        /// <summary>
        /// Serializable, remove a variable from ParameterDic.
        /// </summary>
        /// <param name="nextCommand"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static IScriptCommand Reset(IScriptCommand nextCommand = null, params string[] variables)
        {
            return ScriptCommands.Run(RunMode.Parallel, nextCommand,
                variables.Select(v => ScriptCommands.Assign(v, null)).ToArray());
        }

        public static IScriptCommand Reset(string variable, IScriptCommand nextCommand)
        {
            return ScriptCommands.Assign(variable, null);
        }

    }


    /// <summary>
    /// Serializable, Assign a variable to ParameterDic when running.
    /// </summary>
    public class Assign : ScriptCommandBase
    {
        /// <summary>
        /// Variable name to set to, default = "Variable".
        /// </summary>
        public string VariableKey { get; set; }

        /// <summary>
        /// The actual value, default = null = remove.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Func of value, if this is set, Value is overrided by result of this func at runtime, Default = null.
        /// </summary>
        [XmlIgnore]
        public Func<object> ValueFunc { get; set; }

        /// <summary>
        /// Whether skip (or override) if key already in dictionary, default = false.
        /// </summary>
        public bool SkipIfExists { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<Assign>();

        public Assign()
            : base("Assign")
        {
            VariableKey = "{Variable}";
            Value = null;
            ValueFunc = null;
            SkipIfExists = false;
        }

        protected Assign(string commandKey)
            : base(commandKey)
        {
            VariableKey = "{Variable}";
            Value = null;
            ValueFunc = null;
            SkipIfExists = false;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            object value = Value;
            if (ValueFunc != null)
                value = ValueFunc();
            if (value is string)
            {
                string valueString = (string)value;
                if (valueString.StartsWith("{") && valueString.EndsWith("}"))
                    value = pm.GetValue(valueString);
            }

            if (pm.SetValue<Object>(VariableKey, value, SkipIfExists))
                logger.Debug(String.Format("{0} = {1}", VariableKey, value));
            // else logger.Debug(String.Format("Skipped {0}, already exists.", VariableKey));

            return NextCommand;
        }
    }

}
