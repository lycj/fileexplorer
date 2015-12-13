using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_DynamicRelayCommandDictionary
{
    public class RootViewModel : NotifyPropertyChanged
    {
        #region Constructor

        public RootViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            Commands = new DynamicRelayCommandDictionary()
            {
                ParameterDicConverter = new ParameterDicConverterBase(((pm, pms) => new ParameterDic()
                    {
                        { "RootVM", this },                        
                    }), ((pm, pms) => null), ParameterDicConverters.ConvertParameterOnly), 
                ScriptRunner = new ScriptRunner()
            };
    
        }

        #endregion

        #region Methods

        private static Random random = new Random();
        public void AddNumber(int number)
        {
            Items.Add(new ItemViewModel(this, number));
            Commands.Clear = TestRelayCommands.ClearCommand("{RootVM}", 
                ScriptCommands.PrintDebug("Added clear command"));
        }

        public int AddRandomNumber()
        {
            int number = random.Next();
            AddNumber(number);
            return number;
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public dynamic Commands { get; set; }        

        #endregion
    }
}
