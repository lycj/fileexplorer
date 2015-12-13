using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_DynamicRelayCommandDictionary
{
    public class ItemViewModel : NotifyPropertyChanged
    {

        private bool _isSelected;

        public ItemViewModel(RootViewModel root, int value)
        {
            Value = value;
            _isSelected = false;

            Commands = new DynamicRelayCommandDictionary()
            {
                ParameterDicConverter = new ParameterDicConverterBase(((pm, pms) => new ParameterDic()
                    {
                        {"RootVM", root},
                        { "ItemVM", this },
                        { "Parameter", pm }
                    }), ((pm, pms) => null)),
                ScriptRunner = new ScriptRunner()
            };

            Commands.AddOne = TestRelayCommands.ModifyValueCommand("{ItemVM}", 1);
            Commands.SubtractOne = TestRelayCommands.ModifyValueCommand("{ItemVM}", -1);
            Commands.MouseEnter = ScriptCommands.PrintDebug("MouseEnter -> {ItemVM.Value}");
        }

        private int _value = 0;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; NotifyOfPropertyChanged(() => IsSelected); } }
        public int Value { get { return _value; } set { _value = value; NotifyOfPropertyChanged(() => Value); } }
        public dynamic Commands { get; set; }

    }
}
