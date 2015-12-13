using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.UIEventHub
{
    public class UIScriptCommandBase : ScriptCommandBase
    {
        /// <summary>
        /// Point to sender (UIElement) to received event, Default = "{Sender}"
        /// </summary>
        public string SenderKey { get; set; }

        /// <summary>
        /// Point to the RoutedEvent (RoutedEventArgs), Default = "{EventArgs}"
        /// Not to be confused with {Events}, which is IEventAggregator.
        /// </summary>
        public string RoutedEventArgsKey { get; set; }

        /// <summary>
        /// Point to input (IUIInput) received, Default = "{Input}".
        /// </summary>
        public string InputKey { get; set; }

        /// <summary>
        /// Point to a list of input processors (IList[IUIInputProcessor]), Default = "{InputProcessors}".
        /// </summary>
        public string InputProcessorsKey { get; set; }

        public UIScriptCommandBase(string commandKey)
            : base(commandKey)
        {
            ContinueOnCaptureContext = true;
            SenderKey = "{Sender}";
            RoutedEventArgsKey = "{EventArgs}";
            InputKey = "{Input}";
            InputProcessorsKey = "{InputProcessors}";
        }
    }



    public class UIScriptCommandBase<S, R> : UIScriptCommandBase
        where S : UIElement
        where R : RoutedEventArgs
    {
       public UIScriptCommandBase(string commandKey)
            : base(commandKey)
        {

        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            S sender = pm.GetValue<S>(SenderKey);
            IUIInput input = pm.GetValue<IUIInput>(InputKey);
            R evnt = pm.GetValue<R>(RoutedEventArgsKey);
            IList<IUIInputProcessor> inpProcs = pm.GetValue<IList<IUIInputProcessor>>(InputProcessorsKey);

            return executeInner(pm, sender, evnt, input, inpProcs);
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            S sender = pm.GetValue<S>(SenderKey);
            IUIInput input = pm.GetValue<IUIInput>(InputKey);
            R evnt = pm.GetValue<R>(RoutedEventArgsKey);
            IList<IUIInputProcessor> inpProcs = pm.GetValue<IList<IUIInputProcessor>>(InputProcessorsKey);

            return await executeInnerAsync(pm, sender, evnt, input, inpProcs);
        }

        protected virtual IScriptCommand executeInner(ParameterDic pm, S sender, R evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            return AsyncUtils.RunSync(() => executeInnerAsync(pm, sender, evnt, input, inpProcs));
        }

        protected virtual async Task<IScriptCommand> executeInnerAsync(ParameterDic pm, S sender, R evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            return executeInner(pm, sender, evnt, input, inpProcs);
        }

    }
}
