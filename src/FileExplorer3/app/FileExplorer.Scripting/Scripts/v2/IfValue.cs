using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Use Expression to compare two value in ParameterDic, and run different command based on the result.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="variable1"></param>
        /// <param name="variable2"></param>
        /// <param name="trueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand IfValue(ComparsionOperator op, string variable1 = "{Variable1}",
            string variable2 = "{Variable2}", IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return new IfValue()
            {
                Variable1Key = variable1,
                Variable2Key = variable2,
                Operator = op,
                NextCommand = (ScriptCommandBase)trueCommand,
                OtherwiseCommand = (ScriptCommandBase)otherwiseCommand,
            };
        }

        public static IScriptCommand IfValue<T>(ComparsionOperator op, string variable = "{variable}", T value = default(T), IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            string ifValueValueProperty = "{IfValue-Value}";
            return
                ScriptCommands.Assign(ifValueValueProperty, value, false,
                    IfValue(op, variable, ifValueValueProperty, trueCommand, otherwiseCommand));
        }

        /// <summary>
        /// Serializable, do switch...case code by using multiple IfValue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable"></param>
        /// <param name="caseLookup"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Switch<T>(string variable = "{variable}",
            Dictionary<T, IScriptCommand> caseLookup = null,
            IScriptCommand otherwiseCommand = null, IScriptCommand nextCommnad = null)
        {
            IScriptCommand cmd = null;
            foreach (var key in caseLookup.Keys)
                cmd =
                    cmd == null ? IfEquals<T>(variable, key, caseLookup[key], otherwiseCommand) :
                    IfEquals<T>(variable, key, caseLookup[key], cmd);

            return ScriptCommands.RunSequence(nextCommnad, cmd);
        }


        public static IScriptCommand IfEquals<T>(string variable = "{variable}", T value = default(T), IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            string ifEqualValueProperty = "{IfEquals-Value}";
            return
                ScriptCommands.Assign(ifEqualValueProperty, value, false,
                    IfValue(ComparsionOperator.Equals, variable, ifEqualValueProperty, trueCommand, otherwiseCommand));
        }

        public static IScriptCommand IfNotEquals<T>(string variable = "{variable}", T value = default(T), IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<T>(variable, value, otherwiseCommand, trueCommand);
        }

        public static IScriptCommand IfAssigned(string variable = "{variable}", IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<Object>(variable, null, otherwiseCommand, trueCommand);
        }

        public static IScriptCommand IfAssigned(string[] variables = null, IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            IScriptCommand cmd = trueCommand;
            if (variables != null)
                foreach (var v in variables)
                    cmd = IfAssigned(v, cmd, otherwiseCommand);

            return cmd;
        }

        public static IScriptCommand IfNotAssigned(string variable = "{variable}", IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<Object>(variable, null, trueCommand, otherwiseCommand);
        }

        public static IScriptCommand IfAssignedAndNotEmptyString(string variable = "{variable}", IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<Object>(variable, null, otherwiseCommand,
                IfEquals<string>(variable, "", otherwiseCommand,
                trueCommand));
        }

        public static IScriptCommand IfTrue(string variable = "{variable}", IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<bool>(variable, true, trueCommand, otherwiseCommand);
        }

        public static IScriptCommand IfFalse(string variable = "{variable}", IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            return IfEquals<bool>(variable, false, trueCommand, otherwiseCommand);
        }        

        /// <summary>
        /// Serializable, Use Reassign to obtain a property of a vaiable in ParameterDic and use IfEquals to compare, and run different command based on result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="trueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand IfPropertyEquals<T>(string variable = "{variable}", string propertyName = "Property",
            T value = default(T),
            IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            string variableProperty = ParameterDic.CombineVariable(variable, propertyName);
            string valueProperty = "{IfPropertyEquals-Value}";
            return AssignProperty(variable, propertyName, valueProperty,
                    IfEquals<T>(valueProperty, value, trueCommand, otherwiseCommand));
        }

        public static IScriptCommand IfPropertyIsTrue(string variable = "{variable}", string propertyName = "Property",
            IScriptCommand trueCommand = null,
            IScriptCommand otherwiseCommand = null)
        {
            return IfPropertyEquals<bool>(variable, propertyName, true, trueCommand, otherwiseCommand);
        }

        /// <summary>
        /// Serializable, Run IfValue comparsion based on the length of an array in ParameterDic.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="arrayVariable"></param>
        /// <param name="valueVariable"></param>
        /// <param name="trueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand IfArrayLength(ComparsionOperator op, string arrayVariable = "{array}",
            string valueVariable = "{value}",
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return
                ScriptCommands.IfAssigned(arrayVariable,
                    ScriptCommands.AssignValueConverter(ValueConverterType.GetProperty, "{GetPropertyConverter}",
                        ScriptCommands.Reassign(arrayVariable, "{GetPropertyConverter}", "{ArrayLength}", false,
                            ScriptCommands.PrintLogger(MetroLog.LogLevel.Debug, "Length of array is {ArrayLength}",
                            ScriptCommands.IfValue(op, "{ArrayLength}", valueVariable, trueCommand, otherwiseCommand))), "Length"),
                            otherwiseCommand);
        }

        public static IScriptCommand IfArrayLength(ComparsionOperator op = ComparsionOperator.GreaterThanOrEqual,
            string arrayVariable = "{array}", int value = 1,
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return
                ScriptCommands.Assign("{ArrayLengthValue}", value, false,
                  IfArrayLength(op, arrayVariable, "{ArrayLengthValue}", trueCommand, otherwiseCommand));
        }

        public static IScriptCommand IfStartWith(string stringVariable = "{string}", string startWithText = "_StartWith_",
            bool ignoreCase = true,
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfValue<string>(ignoreCase ? ComparsionOperator.StartWithIgnoreCase :  ComparsionOperator.StartWith, 
                stringVariable, startWithText, trueCommand, otherwiseCommand);
        }

        public static IScriptCommand IfEndsWith(string stringVariable = "{string}", string startWithText = "_EndsWith_",
           bool ignoreCase = true,
           IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfValue<string>(ignoreCase ? ComparsionOperator.EndsWithIgnoreCase :  ComparsionOperator.EndsWith, 
                stringVariable, startWithText, trueCommand, otherwiseCommand);
        }
    }

    public enum ComparsionOperator
    {
        Equals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        /// <summary>
        /// Convert both value to string (.ToString()) and compare if Variable1.ToString().StartWith(Variable2.ToString())
        /// </summary>
        StartWith,
        EndsWith,
        StartWithIgnoreCase,
        EndsWithIgnoreCase
    }
    public class IfValue : ScriptCommandBase
    {
        public ScriptCommandBase OtherwiseCommand { get; set; }

        public string Variable1Key { get; set; }

        public ComparsionOperator Operator { get; set; }

        public string Variable2Key { get; set; }

        public override bool ContinueOnCaptureContext
        {
            get
            {
                return base.ContinueOnCaptureContext
                    || (NextCommand != null && NextCommand.RequireCaptureContext())
                    || (OtherwiseCommand != null && OtherwiseCommand.RequireCaptureContext());
            }
            set { base.ContinueOnCaptureContext = value; }
        }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<IfValue>();

        private bool compare(ParameterDic pm)
        {

            try
            {
                object value1 = pm.GetValue<Object>(Variable1Key);
                object value2 = pm.GetValue<Object>(Variable2Key);

                if (value1 != null && value2 != null)
                {
                    var left = Expression.Constant(value1);
                    var right = Expression.Constant(value2);
                    Expression expression;

                    switch (Operator)
                    {
                        case ComparsionOperator.Equals: expression = Expression.Equal(left, right); break;
                        case ComparsionOperator.GreaterThan: expression = Expression.GreaterThan(left, right); break;
                        case ComparsionOperator.GreaterThanOrEqual: expression = Expression.GreaterThanOrEqual(left, right); break;
                        case ComparsionOperator.LessThan: expression = Expression.LessThan(left, right); break;
                        case ComparsionOperator.LessThanOrEqual: expression = Expression.LessThanOrEqual(left, right); break;
                        case ComparsionOperator.StartWith:
                        case ComparsionOperator.StartWithIgnoreCase:
                            return value1.ToString().StartsWith(value2.ToString(),
                                Operator == ComparsionOperator.StartWith ? StringComparison.CurrentCulture :
                                StringComparison.CurrentCultureIgnoreCase);

                        case ComparsionOperator.EndsWith:
                        case ComparsionOperator.EndsWithIgnoreCase:
                            return value1.ToString().EndsWith(value2.ToString(),
                                Operator == ComparsionOperator.EndsWith ? StringComparison.CurrentCulture :
                                StringComparison.CurrentCultureIgnoreCase);
                        default:
                            throw new NotSupportedException(Operator.ToString());
                    }

                    return Expression.Lambda<Func<bool>>(expression).Compile().Invoke();
                }
                else return (Operator == ComparsionOperator.Equals && value1 == null && value2 == null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            try
            {
                object value1 = pm.GetValue<Object>(Variable1Key);
                object value2 = pm.GetValue<Object>(Variable2Key);

                bool result = compare(pm);

                logger.Debug(String.Format("Executing {0} Command ({1})",
                     result, result ? NextCommand : OtherwiseCommand));

                return result ? NextCommand : OtherwiseCommand;
            }
            catch (Exception ex)
            {
                return OtherwiseCommand;
            }
        }
    }
}
