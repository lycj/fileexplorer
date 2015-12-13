using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class ParameterDicConverterBase : IParameterDicConverter
    {
        private Func<ParameterDic, object[], object> _convertBackFunc;
        private Func<object, object[], ParameterDic> _convertFunc;
        private IParameterDicConverter _baseConverter;
        private ParameterDic _additionalParameters = new ParameterDic();        

        public ParameterDicConverterBase(Func<object, object[], ParameterDic> convertFunc,
            Func<ParameterDic, object[], object> convertBackFunc, IParameterDicConverter baseConverter = null)
        {
            _convertFunc = convertFunc;
            _convertBackFunc = convertBackFunc;
            _baseConverter = baseConverter;
        }

        public void AddAdditionalParameters(ParameterDic pd)
        {
            _additionalParameters = ParameterDic.Combine(_additionalParameters, pd);
        }

        public ParameterDic Convert(object parameter, params object[] additionalParameters)
        {
            var retVal = _convertFunc(parameter, additionalParameters);
            if (_baseConverter != null)
            {
                var baseRetVal = _baseConverter.Convert(parameter, additionalParameters);
                foreach (var v in baseRetVal.VariableNames)
                    retVal.SetValue(v, baseRetVal.GetValue(v), true);
                foreach (var v in _additionalParameters.VariableNames)
                    retVal.SetValue(v, _additionalParameters.GetValue(v), true);
            }
            return retVal;
        }

        //public object ConvertBack(ParameterDic pd, params object[] additionalParameters)
        //{
        //    object retVal = _convertBackFunc(pd, additionalParameters);

        //    if (retVal == null && _baseConverter != null)
        //        return _baseConverter.ConvertBack(pd, additionalParameters);

        //    return retVal;
        //}
    }
}
