using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{    
 
 
    public class AddVariableFromGetter<T> : IParameterDicConvertRule
    {
        private Func<object[], T> _getterFunc;
        private string _variableName;
          
        public AddVariableFromGetter(string variableName = "{Variable}", Func<object[], T> getterFunc = null)
        {
            _variableName = variableName;
            _getterFunc = getterFunc;
        }

        public ParameterDic Convert(params object[] parameters)
        {
            var retVal = new ParameterDic();
            if (_getterFunc != null)
                retVal.SetValue(_variableName, _getterFunc(parameters));
            return retVal;
        }
    }

}
