using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{



    public class ConvertParameterToVariable : IParameterDicConvertRule
    {
       

        private string _variableName;
        private int _idx;
        public ConvertParameterToVariable(int idx = 0, string variableName = "{EventArgs}")
        {
            _variableName = variableName;
            _idx = idx;
        }

        public ParameterDic Convert(params object[] parameters)
        {
            var retVal = new ParameterDic();

            if (parameters.Length > _idx)
                retVal.SetValue(_variableName, parameters[_idx]);
            return retVal;
        }
    }


}
