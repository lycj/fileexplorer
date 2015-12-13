using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{    

 

  
    public class ConvertSenderToVariable : IParameterDicConvertRule
    {
        public static IParameterDicConvertRule Instance = new ConvertSenderToVariable("{Sender}");

        private string _variableName;
        public ConvertSenderToVariable(string variableName = "{Sender}")
        {
            _variableName = variableName;
        }

        public ParameterDic Convert(object sender, object parameter, params object[] additionalParameters)
        {
            var retVal = new ParameterDic();
            retVal.SetValue(_variableName, sender);
            return retVal;
        }
    }


}
