using FileExplorer.Scripting;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq.Expressions;
using FileExplorer.Utils;

namespace FileExplorer.Scripting
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
        public static Assign Assign(string variable = "{Variable}", object value = null,
            IScriptCommand nextCommand = null)
        {
            return new Assign()
            {
                VariableKey = variable,
                Value = value,
                SkipIfExists = false,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Assign using an expression, variable name based on input of variable name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberExpression"></param>
        /// <param name="nextCommand"></param>
        /// <example>ScriptCommands.Assign(() => val1, ResultCommand.OK), pm));</example></example>
        /// <returns></returns>
        public static Assign Assign<T>(Expression<Func<T>> memberExpression,
            IScriptCommand nextCommand = null)
        {
            string variableName = "{" + LinqUtils.GetMemberName<T>(memberExpression) + "}";
            return Assign(variableName, memberExpression.Compile()(), nextCommand);
        }

        /// <summary>
        /// Assign using multiple expression, variable name based on input of variable name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nextCommand"></param>
        /// <param name="memberExpressions"></param>
        /// <example>ScriptCommands.AssignMulti(ResultCommand.OK, () => val1, () => val2), pm));</example>
        /// <returns></returns>
        public static IScriptCommand AssignMulti<T>(IScriptCommand nextCommand,
            params Expression<Func<T>>[] memberExpressions)
        {
            return ScriptCommands.RunSequence(nextCommand, memberExpressions.Select(
                (me) => Assign(me, null)).ToArray());
        }

        public static Assign AssignValueFunc<T>(string variable = "{Variable}", 
            Func<T> valueFunc = null,
            IScriptCommand nextCommand = null)
        {
            valueFunc = valueFunc ?? (() => default(T));
            return new Assign()
            {
                VariableKey = variable,
                ValueFunc = () => valueFunc(),
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand Assign(Dictionary<string, object> variableDic, IScriptCommand nextCommand = null)
        {
            IScriptCommand cmd = nextCommand;
            foreach (var k in variableDic.Keys)
                cmd = Assign(k, variableDic[k], cmd);
            return cmd;
        }

        public static IScriptCommand AssignValueFunc(Dictionary<string, Func<object>> variableDic,
            IScriptCommand nextCommand = null)
        {
            IScriptCommand cmd = nextCommand;
            foreach (var k in variableDic.Keys)
                cmd = AssignValueFunc(k, variableDic[k], cmd);
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

        private static ILog logger = LogManager.GetLogger<Assign>();

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

        public Assign IfExists(bool skip)
        {
            SkipIfExists = skip;
            return this;
        }

        public override IScriptCommand Execute(IParameterDic pm)
        {
            object value = Value;
            if (ValueFunc != null)
                value = ValueFunc();
            if (value is string)
            {
                string valueString = (string)value;
                if (valueString.StartsWith("{") && valueString.EndsWith("}"))
                    value = pm.Get(valueString);
            }

            if (pm.Set<Object>(VariableKey, value, SkipIfExists))
                logger.Debug(String.Format("{0} = {1}", VariableKey, value));
            // else logger.Debug(String.Format("Skipped {0}, already exists.", VariableKey));

            return NextCommand;
        }
    }

}
