using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.Script
{

    public static class ParameterDicConverters
    {
        static ParameterDicConverters()
        {
            ConvertParameterOnly = new ParameterDicConverter(UIParameterDicConvertRule.ConvertParameter);
            ConvertUIParameter = new ParameterDicConverter(UIParameterDicConvertRule.ConvertUIParameters);
            ConvertUIInputParameter = new ParameterDicConverter(UIParameterDicConvertRule.ConvertUIInputParameters);
        }

        public static IParameterDicConverter ConvertParameterOnly;
        public static IParameterDicConverter ConvertUIParameter;
        public static IParameterDicConverter ConvertUIInputParameter;

        #region Obsoluted
        //public static IParameterDicConverter ConvertParameterOnly =
        //    new ParameterDicConverterBase((p, p2) =>
        //            p is ParameterDic ? (p as ParameterDic) :
        //            new ParameterDic() { { "Parameter", p } },
        //        (pd, p2) => pd.GetValue<object>("{Parameter}", null));

        ///// <summary>
        ///// For CommandViewModel, Convert Sender, EventName, EventArgs to ParameterDic
        ///// </summary>
        //public static IParameterDicConverter ConvertUIParameter =
        //    new ParameterDicConverterBase((p, p2) =>
        //        {
        //            if (p2 == null || p2.Length < 3)
        //                return ConvertParameterOnly.Convert(p, p2);

        //            string eventName = p2[0] as string;
        //            object sender = p2[1];
        //            RoutedEventArgs eventArgs = p2[2] as RoutedEventArgs;

        //            return new ParameterDic()
        //            {
        //                { "Sender", sender }, 
        //                { "EventName", eventName }, 
        //                { "EventArgs",  eventArgs }
        //            };
        //        }, null, ParameterDicConverters.ConvertParameterOnly);


        ///// <summary>
        ///// For UIEventHub's UIInput, Convert Sender, Input, InputProcessors to ParameterDic
        ///// </summary>
        //public static IParameterDicConverter ConvertUIInputParameter =
        //    new ParameterDicConverterBase((p, p2) =>
        //    {
        //        if (p2 == null || p2.Length < 3)
        //            return ConvertParameterOnly.Convert(p, p2);

        //        string eventName = p2[0] as string;
        //        IUIInput input = p2[1] as IUIInput;
        //        UIInputManager inputProcManager = p2[2] as UIInputManager;

        //        return new ParameterDic()
        //        {
        //             { "Sender", input.Sender }, 
        //             { "EventName", eventName }, 
        //             { "EventArgs",  input.EventArgs },
        //             { "Input", input },
        //             { "InputProcessors", inputProcManager.Processors.ToList() },                    
        //        };
        //    }, null, ParameterDicConverters.ConvertParameterOnly);
        #endregion

        public static IParameterDicConverter ConvertVMParameter(params IParameterDicConvertRule[] rules)
        {
            return new ParameterDicConverter(ParameterDicConvertRule.Combine(UIParameterDicConvertRule.ConvertUIParameters, rules));
        }

        /// <summary>
        /// Convert ConvertUIParameter +  parameters specified in viewModelProperties
        /// </summary>
        /// <param name="viewModelProperties"></param>
        /// <returns></returns>
        public static IParameterDicConverter ConvertVMParameter(params Tuple<string, object>[] viewModelProperties)
        {
            return new ParameterDicConverterBase((p, p2) =>
            {
                var retVal = new ParameterDic();
                foreach (var pp in viewModelProperties)
                    retVal.Add(pp.Item1, pp.Item2);
                return retVal;
            }, null, ParameterDicConverters.ConvertUIParameter);
        }

        public static IParameterDicConverter FromParameterDic(ParameterDic pd, IParameterDicConverter baseConverter = null)
        {
            return new ParameterDicConverterBase((p, p2) =>
            {
                return pd;

            }, null, baseConverter);
        }
            
    }




}
