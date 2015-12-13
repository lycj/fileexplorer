using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, Assign a new ParameterDic to currrent ParameterDic.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignParameterDic(string variable = "{Variable}", 
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new AssignParameterDic()
            {
                VariableKey = variable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, Assign a new ParameterDic to currrent ParameterDic and store it in a global dictionary 
        /// so the same ParameterDic is returned the next time when called.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignGlobalParameterDic(string variable = "{ParameterDic}",
           bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new AssignGlobalParameterDic()
            {
                VariableKey = variable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }


    }

    public class AssignParameterDic : Assign
    {
        public override IScriptCommand Execute(ParameterDic pm)
        {
            Value = new ParameterDic();
            return base.Execute(pm);
        }
    }

    public class AssignGlobalParameterDic : Assign
    {
        public static Dictionary<string, ParameterDic> ParameterDicDictionary = new Dictionary<string, ParameterDic>();        

        public AssignGlobalParameterDic()
        {
            VariableKey = "Dic#" + new Random().Next().ToString();
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (!ParameterDicDictionary.ContainsKey(VariableKey))
                ParameterDicDictionary.Add(VariableKey, new ParameterDic());
            Value = ParameterDicDictionary[VariableKey];
            return base.Execute(pm);
        }
    }

  
}
