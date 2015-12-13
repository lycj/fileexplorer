using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Models;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Test_UIScriptCommands
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IEventAggregator _events = new EventAggregator();
            IWindowManager _windowManager = new AppWindowManager();
            IProfile _exProfile = new FileSystemInfoExProfile(_events, _windowManager);
            IProfile _ioProfile = new FileSystemInfoProfile(_events);

            IProfile[] _profiles = new IProfile[] { _exProfile, _ioProfile };
            IEntryModel[] _rootDirs = new IEntryModel[] { AsyncUtils.RunSync(() => _exProfile.ParseAsync("")) };

            explorer.WindowManager = _windowManager;
            explorer.ViewModel.Initializer =
                new ScriptCommandInitializer()
                {
                    OnModelCreated = ScriptCommands.Run("{OnModelCreated}"),
                    OnViewAttached = ScriptCommands.Run("{OnViewAttached}"),
                    RootModels = _rootDirs,
                    WindowManager = _windowManager,
                    StartupParameters = new ParameterDic()
                    {
                         { "Profiles", _profiles },
                         { "RootDirectories", _rootDirs },	 
                         { "GlobalEvents", _events },
                         { "WindowManager", _windowManager },
                         { "StartupPath", "" },                         
                         { "ViewMode", "List" }, 
                         { "ItemSize", 16 },
                         { "EnableDrag", true }, 
                         { "EnableDrop", true }, 
                         { "FileListNewWindowCommand", NullScriptCommand.Instance }, //Disable NewWindow Command.
                         { "EnableMultiSelect", true},
                         { "ShowToolbar", true }, 
                         { "ShowGridHeader", true }, 
                         { "OnModelCreated", IOInitializeHelpers.Explorer_Initialize_Default }, 
                         { "OnViewAttached", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }
                    }
                };

            cbCommand.ItemsSource = ScriptCommandDictionary.CommandList;
        }

        ScriptCommandSerializer _serializer = new ScriptCommandSerializer(
            typeof(Int32[]), //Needed by ArithmeticCommand.
             typeof(BaseScriptCommands),
             typeof(FileExplorer3Commands),
             typeof(FileExplorer3IOCommands),
             typeof(FileExplorer3WPFCommands));

        private void cbCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IScriptCommand command = ScriptCommandDictionary.Dictionary[cbCommand.SelectedValue as string];

            using (var sr = new StreamReader(_serializer.SerializeScriptCommand(command)))
            {
                tbCommand.Text = sr.ReadToEnd();
            }
        }

        private void execute_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(tbCommand.Text);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            IScriptCommand cmd = _serializer.DeserializeScriptCommand(ms);
            ParameterDic pd1 = new ParameterDic();
            pd1.SetValue("{tbDirectory}", tbDirectory);
            pd1.SetValue("{Now}", DateTime.Now);
            if (cmd != null)
                explorer.ViewModel.Commands.ExecuteAsync(cmd, pd1);            
        }

        
    }
}
