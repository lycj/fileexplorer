using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Utils;
using FileExplorer.Scripting;

namespace FileExplorer.Utils
{
    public static class ParameterDicUtils
    {
        /// <summary>
        /// Parse a variable string (e.g. {Directory.Temp} and remove the quote (e.g. Directory.Temp)).
        /// </summary>
        /// <param name="variableKey"></param>
        /// <returns></returns>
        public static string GetVariable(string variableKey)
        {
            if (!(variableKey.StartsWith("{") && variableKey.EndsWith("}")))
                throw new ArgumentException(variableKey + " is not a valid variable.");
            return variableKey.TrimStart('{').TrimEnd('}');
        }

        /// <summary>
        /// Combine a variable key (e.g. {Directory} and additional sub variable key (e.g. Temp) 
        /// to a new variable key ({Directory.Temp}))
        /// </summary>
        /// <param name="variableKey"></param>
        /// <param name="combineStr"></param>
        /// <param name="checkVariablekey"></param>
        /// <returns></returns>
        public static string CombineVariable(string variableKey, string combineStr, bool checkVariablekey = true)
        {
            if (checkVariablekey)
                variableKey = variableKey.Replace(".", "");
            return "{" + GetVariable(variableKey) + combineStr + "}";
        }

        /// <summary>
        /// Combine a variable key (e.g. {Directory} and additional sub variable key (e.g. Temp) 
        /// to a new variable key ({Directory.Temp}))
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableKey"></param>
        /// <param name="expression"></param>
        /// <param name="checkVariablekey"></param>
        /// <returns></returns>
        public static string CombineVariable<C, T>(string variableKey, Expression<Func<C, T>> expression, bool checkVariablekey = true)
        {
            string combineStr = "." + ExpressionUtils.GetPropertyName(expression);
            return CombineVariable(variableKey, combineStr, checkVariablekey);
        }

        public static IParameterDic CombineDictionary(params IParameterDic[] dics)
        {
            return new ParameterDic(dics);
        }

        public static string[] VariableSplit(string variableKey, int skipCount)
        {
            string variable = GetVariable(variableKey);
            string[] variableSplit = variable.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (skipCount > 0)
                return variableSplit.Take(skipCount).ToArray();
            else if (skipCount < 0)
                return variableSplit.TakeLast(skipCount).ToArray();
            return variableSplit;
        }

        public static string RandomVariable(string prefix = "")
        {
            return "{" + prefix + new Random().Next().ToString() + "}";
        }
    }
}
