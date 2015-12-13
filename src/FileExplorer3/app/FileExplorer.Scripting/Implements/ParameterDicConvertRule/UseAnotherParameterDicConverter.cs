using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{    

 

    public class UseAnotherParameterDicConverter : IParameterDicConvertRule
    {
        private IParameterDicConverter[] _converters;        
        
        public UseAnotherParameterDicConverter(params IParameterDicConverter[] converters)
        {
            _converters = converters;
        }

        public ParameterDic Convert(params object[] parameters)
        {
            return ParameterDic.Combine(_converters.Select(c => c.Convert(parameters)).ToArray());
        }
    }


}
