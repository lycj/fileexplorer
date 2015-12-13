using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    /// <summary>
    /// Convert object parameter to ParameterDic
    /// </summary>
    public interface IParameterDicConverter
    {
        /// <summary>
        /// used by ExplorerAssignScriptParameters IScriptCommand to inject parameters to CommandManager.
        /// </summary>
        /// <param name="pd"></param>
        void AddAdditionalParameters(ParameterDic pd);
        ParameterDic Convert(object sender, params object[] additionalParameters);
        //object ConvertBack(ParameterDic pd, params object[] additionalParameters);
    }

    public interface IParameterDicConverter2
    {        
        ParameterDic Convert(params object[] parameters);        
    }
}
