using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.Models;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.Views;
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
using System.Windows.Shapes;
using WPF.MDI;

namespace TestApp
{

    /// <summary>
    /// Interaction logic for MdiWindow.xaml
    /// </summary>
    public partial class MdiWindow : Window
    {
        public MdiWindow(IWindowManager wm, IEventAggregator events, IEntryModel[] rootModels)
        {
            InitializeComponent();

            _rootDirectories = rootModels;

            _initializer = new ScriptCommandInitializer()
            {
                WindowManager = wm,
                Events = events,
                OnModelCreated = IOInitializeHelpers.Explorer_Initialize_Default,
                OnViewAttached = UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot(),
                RootModels = rootModels,
                StartupParameters = new ParameterDic()
                {
                    { "MdiWindow", this }
                }
            };

            //_initializer = AppViewModel.getInitializer(_windowManager, _events,  _root,
            //    new ColumnInitializers(),
            //    new ScriptCommandsInitializers(_windowManager, _events),
            //    new ToolbarCommandsInitializers(_windowManager));

            //_initializer.Initializers.Add(new MdiWindowInitializers(_initializer, Container));

        }

        public IExplorerInitializer _initializer;
        public IEventAggregator _events = new EventAggregator();
        public IWindowManager _windowManager = new WindowManager();
        public IProfile _profileEx = null;
        public IEntryModel[] _rootDirectories = null;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }



        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            var profiles = _rootDirectories.Select(em => em.Profile).Distinct().ToArray();
            ScriptRunner.RunScript(
                new ParameterDic()
                {
                    {"ExplorerWidth", 500 },
                    {"ExplorerHeight", 334 },                                       
                },
                TestAppCommands.ExplorerNewMdiWindow(Container, profiles, _rootDirectories, "{Explorer}"));
            //new TestApp.Script.OpenInNewMdiWindowV1(Container, _initializer).Execute(new ParameterDic());
        }

        private void WPFMDI_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wpfmdi.codeplex.com/");
        }
    }


}
