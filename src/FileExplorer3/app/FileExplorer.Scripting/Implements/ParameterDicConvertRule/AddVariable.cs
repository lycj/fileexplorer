using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{    
 
    public static class AddVariable 
    {
        public static IParameterDicConvertRule FromGetter<T>(string variableName = "{Variable}", Func<object[], T> getterFunc = null)
        {
            return new AddVariableFromGetter<T>(variableName, getterFunc);
        }

        public static IParameterDicConvertRule FromParameterDic(ParameterDic pd)
        {            
            return new AddVariableFromParameterDIc(pd);
        }

        public static IParameterDicConvertRule FromParameterPairs(params ParameterPair[] ppairs)
        {
            return FromParameterDic(new ParameterDic(ppairs));
        }
    }



}
