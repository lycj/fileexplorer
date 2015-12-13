using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{    

 

    public class AddVariableFromParameterDIc : IParameterDicConvertRule
    {
        private ParameterDic _pd;
        public AddVariableFromParameterDIc(ParameterDic pd)
        {
            _pd = pd;
        }

        public ParameterDic Convert(params object[] parameters)
        {
            return _pd;
        }
    }


}
