using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Set Control.Commands(CommandManager).Commands(DynamicDictionary[IScriptCommand])
        /// </summary>
        /// <param name="controlVariable"></param>
        /// <param name="target"></param>
        /// <param name="valueVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand SetScriptCommand(string controlVariable = "{FileList}", string target = "Open",
            string valueVariable = "{Value}", IScriptCommand nextCommand = null)
        {
            return new SetScriptCommand()
            {
                ControlKey = controlVariable,
                Target = target,
                ValueKey = valueVariable, 
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Set Control.Commands(CommandManager).Commands(DynamicDictionary[IScriptCommand])
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static IScriptCommand SetScriptCommand(string controlVariable = "{FileList}", string target = "Open",
            IScriptCommand valueCommand = null, IScriptCommand nextCommand = null)
        {
            string valueVarable = ParameterDic.CombineVariable(controlVariable, "Value");
            return ScriptCommands.Assign(valueVarable, valueCommand, false, 
                SetScriptCommand(controlVariable, target, valueVarable, nextCommand));
        }
    }

    /// <summary>  
    /// Set Control.Commands(CommandManager).Commands(DynamicDictionary[IScriptCommand])
    /// </summary>
    public class SetScriptCommand : ScriptCommandBase
    {
        /// <summary>
        /// Set by inherited class, the view model which support ISupportCommandManager
        /// </summary>
        public string ControlKey { get; set; }

        /// <summary>
        /// ScriptCommand to be set, Default = "Open"
        /// e.g. "Open" for FileList.Commands(CommandManager).Commands(DynamicDictionary[IScriptCommand]).Open
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Point to value in ParameterDic which store an IScriptCommand, Default = "{Value}"
        /// </summary>
        public string ValueKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SetScriptCommand>();

        public SetScriptCommand()
            : base("SetScriptCommand")
        {
            ControlKey = null;
            Target = "Open";
            ValueKey = "{Value}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var vm = pm.GetValue<ISupportCommandManager>(ControlKey);
            var vmDic = vm.Commands.CommandDictionary as DynamicDictionary<IScriptCommand>;
            
            IScriptCommand cmd = pm.GetValue<IScriptCommand>(ValueKey);

            if (cmd != null)
            {
                logger.Debug(String.Format("Set {0}.{1} to {2} ({3})", ControlKey, Target, ValueKey, cmd));
                vmDic.Dictionary[Target] = cmd;
            }

            return NextCommand;


        }

    }
}
