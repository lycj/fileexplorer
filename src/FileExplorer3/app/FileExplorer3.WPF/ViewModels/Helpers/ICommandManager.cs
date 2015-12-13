using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer;

namespace FileExplorer.WPF.ViewModels
{
    /// <summary>
    /// Manage script commands for ViewModels
    /// </summary>
    public interface ICommandManager : IExportCommandBindings
    {
        /// <summary>
        /// A number of parameter have to be added to ParameterDic to run commands in ScriptCommands (e.g. FileList), 
        /// use this converter to add the parameter.
        /// </summary>
        IParameterDicConverter ParameterDicConverter { get; }
        
        /// <summary>
        /// An IScriptCommand DynamicDictionary, include changable commands.
        /// </summary>
        dynamic CommandDictionary { get; }

        /// <summary>
        /// Renamed to CommandDictionary.
        /// </summary>
        [Obsolete("Renamed to CommandDictionary")]
        dynamic Commands { get; }

        /// <summary>
        /// Return a list of Commands for Toolbar and ContextMenu.
        /// </summary>
        IToolbarCommandsHelper ToolbarCommands { get; }


         IScriptCommand GetCommandFromDictionary(string commandVariable = "{Command}",
           IScriptCommand defaultValue = null);
         void SetCommandToDictionary(string commandVariable = "{Command}", IScriptCommand cmd = null);
     
    }

    public interface ISupportCommandManager 
    {
        ICommandManager Commands { get; }
    }
}
