using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    /// <summary>
    /// Used by ParameterDicConverter to convert.
    /// </summary>
    public interface IParameterDicConvertRule
    {        
        ParameterDic Convert(params object[] parameters);
    }
}
