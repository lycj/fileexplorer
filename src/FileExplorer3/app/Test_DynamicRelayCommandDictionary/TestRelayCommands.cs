using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_DynamicRelayCommandDictionary
{
    public static class TestRelayCommands
    {
        public static IScriptCommand ClearCommand(string rootKey = "{RootVM}", IScriptCommand nextCommand = null)
        {
            return new ClearCommand()
            {
                RootKey = rootKey,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ModifyValueCommand(string itemKey = "{ItemVM}", int delta = 1, IScriptCommand nextCommand = null)
        {
            return new ModifyValueCommand()
            {
                ItemKey = itemKey,
                Delta = delta,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class ClearCommand : ScriptCommandBase
    {


        public string RootKey { get; set; }

        public override IScriptCommand Execute(FileExplorer.ParameterDic pm)
        {
            var rvm = pm.GetValue<RootViewModel>(RootKey);
            rvm.Items.Clear();

            return NextCommand;
        }
    }

    public class ModifyValueCommand : ScriptCommandBase
    {
        public string ItemKey { get; set; }

        public int Delta { get; set; }

        public override IScriptCommand Execute(FileExplorer.ParameterDic pm)
        {
            var ivm = pm.GetValue<ItemViewModel>(ItemKey);
            ivm.Value += Delta;
            return NextCommand;
        }
    }
}
