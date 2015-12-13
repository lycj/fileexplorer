using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class ParameterDicConverter : IParameterDicConverter, IParameterDicConverter2
    {
        #region fields

        private List<IParameterDicConvertRule> _rules = new List<IParameterDicConvertRule>();

        #endregion

        #region constructors

        public ParameterDicConverter(params IParameterDicConvertRule[] rules)
        {
            AddRules(rules);
        }

        #endregion

        #region events

        #endregion

        #region properties

        #endregion

        #region methods

        public void AddRules(params IParameterDicConvertRule[] rules)
        {
            _rules.AddRange(rules);
        }

        public void AddAdditionalParameters(ParameterDic pd)
        {
            AddRules(new AddVariableFromParameterDIc(pd));
        }

        public ParameterDic Convert(params object[] parameters)
        {
            return ParameterDic.Combine(_rules.Select(r => r.Convert(parameters)).ToArray());
        }

        public ParameterDic Convert(object sender, params object[] additionalParameters)
        {
            var parameters = new List<object>();
            parameters.Add(sender);
            parameters.AddRange(additionalParameters);
            return Convert(parameters.ToArray());
        }

        #endregion


      
    }
}
