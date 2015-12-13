using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {
        public static IScriptCommand ThrottleTouchDrag(int divider = 5, 
            IScriptCommand thenCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return new ThrottleTouchDrag()
            {
                Divider = divider,
                NextCommand = (ScriptCommandBase)thenCommand,
                OtherwiseCommand = (ScriptCommandBase)otherwiseCommand
            };
        }
    }

    public class ThrottleTouchDrag : UIScriptCommandBase
    {

        public int Divider { get; set; }

        public ScriptCommandBase OtherwiseCommand { get; set; }

        public ThrottleTouchDrag()
            : base("ThrottleTouchDrag")
        {
            Divider = 5;
            OtherwiseCommand = ResultCommand.NoError;
        }

        public override Script.IScriptCommand Execute(ParameterDic pm)
        {
            IList<IUIInputProcessor> inpProcs = pm.GetValue<IList<IUIInputProcessor>>(InputProcessorsKey);
            var processor = inpProcs.First(p => p is TouchDragMoveCountInputProcessor) as TouchDragMoveCountInputProcessor;
            IUIInput input = pm.GetValue<IUIInput>(InputKey);

            if (input.InputType != Defines.UIInputType.Touch || processor.DragMoveCount % Divider == 0)
                return NextCommand;

            else return OtherwiseCommand;
        }
    }
}
