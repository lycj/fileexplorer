using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FileExplorer.WPF.Utils;
using System.Linq.Expressions;
using FileExplorer.Utils;
using Common.Logging;

namespace FileExplorer.Scripting
{


    public static partial class ScriptCommands
    {
        #region Add
        /// <summary>
        /// Serializable, Add variables (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand addCommand = ScriptCommands.Add("{Left}", "{Right}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", 10 }, 
        ///     { "Right", new int[] {1, 3, 5 } }, 
        /// addCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Add(string value1Variable = "{Value1}",
            string value2Variable = "{Value2}",
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,
                Value2Key = value2Variable,
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.Add,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand AddValue<T>(string value1Variable = "{Value1}",
            T[] value2 = null,
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            value2 = value2 ?? new T[] {};
            string value2Variable = ParameterDicUtils.RandomVariable();
            return ScriptCommands.Assign(value2Variable, value2,  
                 Add(value1Variable, value2Variable, destinationVariable, nextCommand));                
        }

        public static IScriptCommand AddValue<T>(string value1Variable = "{Value1}",
            T value2 = default(T),
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return AddValue<T>(value1Variable, new[] { value2 }, destinationVariable, nextCommand);
        }
        #endregion 

        #region Subtract
        /// <summary>
        /// Serializable, Add variables (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand addCommand = ScriptCommands.Add("{Left}", "{Right}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", 10 }, 
        ///     { "Right", new int[] {1, 3, 5 } }, 
        /// addCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Subtract(string value1Variable = "{Value1}",
            string value2Variable = "{Value2}",
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,
                Value2Key = value2Variable,
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.Subtract,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand SubtractValue<T>(string value1Variable = "{Value1}",
            T[] value2 = null,
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            value2 = value2 ?? new T[] { };
            string value2Variable = ParameterDicUtils.RandomVariable();
            return ScriptCommands.Assign(value2Variable, value2, 
                 Subtract(value1Variable, value2Variable, destinationVariable, nextCommand));
        }

        public static IScriptCommand SubtractValue<T>(string value1Variable = "{Value1}",
            T value2 = default(T),
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return SubtractValue<T>(value1Variable, new[] { value2 }, destinationVariable, nextCommand);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Serializable, Multiply variables (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand MultiplyCommand = ScriptCommands.Multiply("{Left}", "{Right}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", 10 }, 
        ///     { "Right", new int[] {1, 3, 5 } }, 
        /// MultiplyCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Multiply(string value1Variable = "{Value1}",
            string value2Variable = "{Value2}",
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,
                Value2Key = value2Variable,
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.Multiply,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand MultiplyValue<T>(string value1Variable = "{Value1}",
            T[] value2 = null,
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            value2 = value2 ?? new T[] { };
            string value2Variable = ParameterDicUtils.RandomVariable();
            return ScriptCommands.Assign(value2Variable, value2, 
                 Multiply(value1Variable, value2Variable, destinationVariable, nextCommand));
        }

        public static IScriptCommand MultiplyValue<T>(string value1Variable = "{Value1}",
            T value2 = default(T),
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return MultiplyValue<T>(value1Variable, new[] { value2 }, destinationVariable, nextCommand);
        }
        #endregion 

        #region Divide
        /// <summary>
        /// Serializable, Divide variables (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand DivideCommand = ScriptCommands.Divide("{Left}", "{Right}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", 10 }, 
        ///     { "Right", new int[] {1, 3, 5 } }, 
        /// DivideCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Divide(string value1Variable = "{Value1}",
            string value2Variable = "{Value2}",
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,
                Value2Key = value2Variable,
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.Divide,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DivideValue<T>(string value1Variable = "{Value1}",
            T[] value2 = null,
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            value2 = value2 ?? new T[] { };
            string value2Variable = ParameterDicUtils.RandomVariable();
            return ScriptCommands.Assign(value2Variable, value2, 
                 Divide(value1Variable, value2Variable, destinationVariable, nextCommand));
        }

        public static IScriptCommand DivideValue<T>(string value1Variable = "{Value1}",
            T value2 = default(T),
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return DivideValue<T>(value1Variable, new[] { value2 }, destinationVariable, nextCommand);
        }
        #endregion 

        #region Modulo
        /// <summary>
        /// Serializable, Mod variables (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand ModCommand = ScriptCommands.Mod("{Left}", "{Right}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", 10 }, 
        ///     { "Right", new int[] {1, 3, 5 } }, 
        /// ModCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Modulo(string value1Variable = "{Value1}",
            string value2Variable = "{Value2}",
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,
                Value2Key = value2Variable,
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.Modulo,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ModuloValue<T>(string value1Variable = "{Value1}",
            T[] value2 = null,
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            value2 = value2 ?? new T[] { };
            string value2Variable = ParameterDicUtils.RandomVariable();
            return ScriptCommands.Assign(value2Variable, value2, 
                 Modulo(value1Variable, value2Variable, destinationVariable, nextCommand));
        }

        public static IScriptCommand ModuloValue<T>(string value1Variable = "{Value1}",
            T value2 = default(T),
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return ModuloValue<T>(value1Variable, new[] { value2 }, destinationVariable, nextCommand);
        }
        #endregion 

        #region AbsoluteValue
        /// <summary>
        /// Serializable, store Absolute value of source variable (using Expression) to destination.
        /// </summary>
        /// <example>
        /// IScriptCommand AbsCommand = ScriptCommands.Absolute("{Left}", "{Result}", 
        ///     ScriptCommands.PrintDebug("{Result}"));
        ///
        /// await ScriptRunner.RunScriptAsync(new ParameterDic() { 
        ///     { "Left", -1 } }        
        /// AbsCommand);
        /// </example>
        /// <param name="value1Variable"></param>
        /// <param name="value2Variable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AbsoluteValue(string value1Variable = "{Value1}",            
            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new ArithmeticCommand()
            {
                Value1Key = value1Variable,                
                VariableKey = destinationVariable,
                OperatorType = ArithmeticOperatorType.AbsoluteValue,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
        #endregion
    }

    public enum ArithmeticOperatorType
    {
        Add, Subtract, Multiply, Divide, Modulo, AbsoluteValue
    }

    /// <summary>
    /// Use Expression to do arithmetic operations (Take Value1 and Value2) , and assign the result to VariableKey.
    /// </summary>
    public class ArithmeticCommand : Assign
    {

        /// <summary>
        /// Specify which converter to use, default = GetProperty
        /// </summary>
        public ArithmeticOperatorType OperatorType { get; set; }

        /// <summary>
        /// Point to the first value (the , default = {Value1}.
        /// </summary>
        public string Value1Key { get; set; }

        /// <summary>
        /// Point to the second value (the , default = {Value2}.
        /// Note: second value can be an array, in this case, the arithmetic operations will be done multiple times.
        /// </summary>
        public string Value2Key { get; set; }

        private static ILog logger = LogManager.GetLogger<ArithmeticCommand>();

        public ArithmeticCommand()
            : base("AssignArithmeticOperator")
        {
            OperatorType = ArithmeticOperatorType.Add;
            VariableKey = "{Result}";
            Value1Key = "{Value1}";
            Value2Key = "{Value2}";
            Value = null;
            SkipIfExists = false;
        }

        public override IScriptCommand Execute(IParameterDic pm)
        {
            Func<object, object> checkParameters = p =>
                       {
                           if (p is string)
                           {
                               string pString = p as string;
                               if (pString.StartsWith("{") && pString.EndsWith("}"))
                                   return pm.Get(pString);
                           }
                           return p;
                       };

            object firstValue = pm.Get(Value1Key);
            object secondValue = pm.Get(Value2Key);
            List<object> secondArrayList = secondValue is Array ? (secondValue as Array).Cast<object>().ToList() :
                new List<object>() { secondValue };

            object value = firstValue;
            string methodName = OperatorType.ToString();
            
         
            var mInfo = typeof(FileExplorer.Utils.ExpressionUtils)
                              .GetRuntimeMethods().First(m => m.Name == methodName)
                              .MakeGenericMethod(value.GetType());
            foreach (var addItem in secondArrayList.Select(p => checkParameters(p)).ToArray())
                switch (mInfo.GetParameters().Length)
                {
                    case 1: value = mInfo.Invoke(null, new object[] { value }); break;
                    case 2: value = mInfo.Invoke(null, new object[] { value, addItem }); break;
                    default: throw new NotSupportedException();
                }
            
            Value = value;

            return base.Execute(pm);
        }
        
    }


}
