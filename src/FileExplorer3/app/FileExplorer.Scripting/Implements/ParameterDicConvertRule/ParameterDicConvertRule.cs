using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class ParameterDicConvertRule : IParameterDicConvertRule
    {
        public static IParameterDicConvertRule Combine(params IParameterDicConvertRule[] rules)
        {
            return new ParameterDicConvertRule(rules);
        }

        public static IParameterDicConvertRule Combine(IParameterDicConvertRule rule, params IParameterDicConvertRule[] rules)
        {
            var ruleList = rules.ToList();
            ruleList.Insert(0, rule);
            return new ParameterDicConvertRule(ruleList.ToArray());
        }

        private IParameterDicConvertRule[] _rules;

        private ParameterDicConvertRule(params IParameterDicConvertRule[] rules)
        {
            _rules = rules;
        }

        public ParameterDic Convert(params object[] parameters)
        {
            return ParameterDic.Combine(_rules.Select(r => r.Convert(parameters)).ToArray());
        }
    }
}
