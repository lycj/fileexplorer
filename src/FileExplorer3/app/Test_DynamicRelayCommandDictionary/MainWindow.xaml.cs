using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test_DynamicRelayCommandDictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RootViewModel _rvm;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _rvm = new RootViewModel();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Assign commands in run time.

            //RootVM is defined in ParameterDicConverter, where Parameter is from ICommand.CommandParameter.
            _rvm.Commands.AddRandom =
                ScriptCommands.AssignMethodResult("{RootVM}", "AddRandomNumber", null, "{Output}",
                    ScriptCommands.PrintDebug("{Output} Added."));            
            //-> thus _rvm.Commands.AddRandomCommand is Bindable RelayCommand.
            _rvm.Commands.Add = 
                ScriptCommands.ExecuteMethod("{RootVM}", "AddNumber", new object[] {"{Parameter}" },
                    ScriptCommands.PrintDebug("{Parameter} Added."));
            //Not necessary use ExecuteMethod, as one can define their own commands.


        }
    }
}
