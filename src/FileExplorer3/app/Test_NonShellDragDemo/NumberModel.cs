using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_NonShellDragDemo
{
    public class NumberModel : NotifyPropertyChanged
    {
        public int Value { get; set; }

        public NumberModel(int number)
        {
            Value = number;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is NumberModel) && (obj as NumberModel).Value == Value;
        }
    }
}
