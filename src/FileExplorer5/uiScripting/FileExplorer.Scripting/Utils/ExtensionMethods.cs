using FileExplorer.Defines;
using FileExplorer.Scripting;
using FileExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Utils
{
    public static partial class ExtensionMethods
    {
        
        /// <summary>
        /// Add specified values to the value assigned to the parameter dic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="variableKey"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Add<T>(this IParameterDic dic, string variableKey, params T[] values)
        {
            T retValue = dic.Get<T>(variableKey, default(T));
            foreach (T value in values)
                retValue = ExpressionUtils.Add<T>(retValue, value);
            dic.Set<T>(variableKey, retValue);
            return retValue;
        }


        /// <summary>
        /// Subtract specified values to the value assigned to the parameter dic.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="variableKey"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Subtract<T>(this IParameterDic dic, string variableKey, params T[] values)
        {
            T retValue = dic.Get<T>(variableKey, default(T));
            foreach (T value in values)
                retValue = ExpressionUtils.Subtract<T>(value, retValue);
            dic.Set<T>(variableKey, retValue);
            return retValue;
        }

        public static bool IsHandled(this IParameterDic dic)
        {
            return dic.Get<bool>(VariableKeys.IsHandled, false);
        }

        internal static void IsHandled(this IParameterDic dic, bool value)
        {
            dic.Set<bool>(VariableKeys.IsHandled, value);
        }

        public static Exception Error(this IParameterDic dic)
        {
            return dic.Get<Exception>(VariableKeys.Error, null);
        }

        public static void Error(this IParameterDic dic, Exception error)
        {
            dic.Set<Exception>(VariableKeys.Error, error);
        }

        public static IScriptRunner ScriptRunner(this IParameterDic dic)
        {
            return dic.Get<IScriptRunner>(VariableKeys.ScriptRunner, null);
        }

        public static void ScriptRunner(this IParameterDic dic, IScriptRunner runner)
        {
            dic.Set<IScriptRunner>(VariableKeys.ScriptRunner, runner);
        }

        public static IParameterDic Clone(this IParameterDic dic)
        {
            return AsyncUtils.RunSync(() => dic.CloneAsync());
        }


        private static string getProgressVariable<T>()
        {
            return ParameterDicUtils.CombineVariable(VariableKeys.Progress, 
                typeof(T).ToString());
        }


        public static IProgress<T> Progress<T>(this IParameterDic dic)
        {
            return dic.Get<IProgress<T>>(getProgressVariable<T>(), null) ??
                NullProgress<T>.Instance;
        }

        public static void Progress<T>(this IParameterDic dic, IProgress<T> progress)
        {
            dic.Set(getProgressVariable<T>(), progress);
        }

        public static string RandomVariable(this IParameterDic dic, string prefix = "")
        {
            var rnd = new Random();

            string nextVariable = prefix + rnd.Next().ToString();
            while (dic.List().Contains(nextVariable))
                nextVariable = prefix + rnd.Next().ToString();
            return "{" + nextVariable + "}";
        }
    }
}
