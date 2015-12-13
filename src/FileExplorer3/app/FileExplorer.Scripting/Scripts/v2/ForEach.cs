using FileExplorer.Defines;
using FileExplorer.Script;
using MetroLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        /// <summary>
        /// Serializable, given an array, iterate it with NextCommand, then run ThenCommand when all iteration is finished.
        /// </summary>
        /// <param name="itemsVariable"></param>
        /// <param name="currentItemVariable"></param>
        /// <param name="doCommand"></param>
        /// <param name="thenCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ForEach(string itemsVariable = "{Items}", string currentItemVariable = "{CurrentItem}",
            string breakVariable = "{Break}",
            IScriptCommand doCommand = null, IScriptCommand thenCommand = null)
        {
            return new ForEach()
            {
                ItemsKey = itemsVariable,
                CurrentItemKey = currentItemVariable,
                BreakKey = breakVariable,
                NextCommand = (ScriptCommandBase)doCommand,
                ThenCommand = (ScriptCommandBase)thenCommand
            };
        }

        public static IScriptCommand ForEach(string itemsVariable = "{Items}", string currentItemVariable = "{CurrentItem}",
            IScriptCommand doCommand = null, IScriptCommand thenCommand = null)
        {
            return ForEach(itemsVariable, currentItemVariable, "{Break}", doCommand, thenCommand);
        }

        /// <summary>
        /// Iterate an IEnumeration and return true if anyone's property equals to the specified variable.
        /// </summary>
        /// <example>
        /// IScriptCommand iterateCommand2 =
        ///    ScriptCommands.ForEachIfAnyValue<DateTime>("{Items}", "Day", ComparsionOperator.Equals, "Today.Day",
        ///	   ScriptCommands.PrintDebug("True"), ScriptCommands.PrintDebug("False"));
        /// </example>
        /// <param name="itemsVariable"></param>
        /// <param name="property"></param>
        /// <param name="op"></param>
        /// <param name="compareVariable"></param>
        /// <param name="nextCommand"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ForEachIfAnyValue(string itemsVariable = "{Items}", string property = null,
            ComparsionOperator op = ComparsionOperator.Equals, string compareVariable = "{Variable}",
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string currentItemVariable = ParameterDic.CombineVariable(itemsVariable.Replace(".", ""), "Current");
            string currentPropertyVariable = ParameterDic.CombineVariable(itemsVariable.Replace(".", ""), "Current" + 
                ((property == null) ? "" : "." + property));
            string resultVariable = ParameterDic.CombineVariable(itemsVariable.Replace(".", ""), "Result");
            
            return
                Assign(resultVariable, false, false,
                ForEach(itemsVariable, currentItemVariable, resultVariable,                 
                   IfValue(op, currentPropertyVariable, compareVariable,
                        Assign(resultVariable, true, false)),
                        IfTrue(resultVariable, nextCommand, otherwiseCommand)));
        }

        /// <summary>
        /// Iterate an IEnumeration and return true if anyone's property equals to the value.
        /// <example>
        /// IScriptCommand iterateCommand2 =
        ///      ScriptCommands.ForEachIfAnyValue<DateTime>("{Items}", null, ComparsionOperator.Equals, DateTime.Today,
		///	    ScriptCommands.PrintDebug("True"), ScriptCommands.PrintDebug("False"));
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsVariable"></param>
        /// <param name="property"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <param name="nextCommand"></param>
        /// <param name="otherwiseCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ForEachIfAnyValue<T>(string itemsVariable = "{Items}", string property = null,
            ComparsionOperator op = ComparsionOperator.Equals, T value = default(T),
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string compareVariable = ParameterDic.CombineVariable(itemsVariable.Replace(".","") , "Compare");
            return Assign(compareVariable, value, false, 
                ForEachIfAnyValue(itemsVariable, property, op, compareVariable, nextCommand, otherwiseCommand));
        }
    }

    /// <summary>
    /// Serializable, given an array, iterate it with NextCommand, then run ThenCommand when all iteration is finished.
    /// </summary>
    public class ForEach : ScriptCommandBase
    {
        /// <summary>
        /// Array of item to be iterated, support IEnumerable or Array, Default=Items
        /// </summary>
        public string ItemsKey { get; set; }
        /// <summary>
        /// When iterating item (e.g. i in foreach (var i in array)), the current item will be stored in this key.
        /// Default = CurrentItem
        /// </summary>
        public string CurrentItemKey { get; set; }

        /// <summary>
        /// If set to true, break the foreach command
        /// </summary>
        public string BreakKey { get; set; }

        /// <summary>
        /// Iteration command is run in NextCommand, when all iteration complete ThenCommand is run.
        /// </summary>
        public ScriptCommandBase ThenCommand { get; set; }

        /// <summary>
        /// Whether to report progress, default : true.
        /// </summary>
        public bool IsProgressEnabled { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ForEach>();

        public ForEach()
            : base("ForEach")
        {
            CurrentItemKey = "{CurrentItem}";
            ItemsKey = "{Items}";
            BreakKey = "{Break}";
            IsProgressEnabled = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IEnumerable e = pm.GetValue<IEnumerable>(ItemsKey);
            if (e == null)
                return ResultCommand.Error(new ArgumentException(ItemsKey));

            IProgress<TransferProgress> progress = NullTransferProgress.Instance;
            if (IsProgressEnabled)
            {
                List<object> list;
                e = list = e.Cast<object>().ToList();
                progress = pm.GetProgress();
                progress.Report(TransferProgress.IncrementTotalEntries(list.Count));
            }

            uint counter = 0;
            pm.SetValue<bool>(BreakKey, false);
            foreach (var item in e)
            {
                if (pm.GetValue<bool>(BreakKey))
                    break;

                counter++;
                pm.SetValue(CurrentItemKey, item);
                await ScriptRunner.RunScriptAsync(pm, NextCommand);
                progress.Report(TransferProgress.IncrementProcessedEntries());
                if (pm.Error != null)
                {
                    pm.SetValue<Object>(CurrentItemKey, null);
                    return ResultCommand.Error(pm.Error);
                }
            }
            logger.Info("Looped {0} items", counter);
            pm.SetValue<Object>(CurrentItemKey, null);

            return ThenCommand;
        }
    }
}
