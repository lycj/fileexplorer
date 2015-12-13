using FileExplorer.UIEventHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_IOCPanelTest
{
    public class ValueViewModel : NotifyPropertyChanged, ISelectable
    {
        public int Value { get; set; }        
        public int FontSize { get; set; }
        public int DigitSize { get; set; }
        public static Random rand = new Random();

        public ValueViewModel(int value)
        {
            Value = value;            
            FontSize = (rand.Next(30)) + 3;
            DigitSize = value.ToString().Length;
        }

        private bool _isSelected = false;
        private bool _isSelecting = false;

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyOfPropertyChanged(() => IsSelected); }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { _isSelecting = value; NotifyOfPropertyChanged(() => IsSelecting); }
        }

    }
}
