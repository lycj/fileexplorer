using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{
    public partial class HubScriptCommands
    {
        public static IScriptCommand SetRoutedEventHandled(IScriptCommand nextCommand = null)
        {
            return ScriptCommands.SetPropertyValue<bool>("{EventArgs}", "Handled", true, nextCommand);
        }

        public static IScriptCommand IfRoutedEventHandled(IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return ScriptCommands.IfTrue("{EventArgs.Handled}", trueCommand, otherwiseCommand);
        }

        public static IScriptCommand IfNotRoutedEventHandled(IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfRoutedEventHandled(otherwiseCommand, trueCommand);
        }

    }
}
