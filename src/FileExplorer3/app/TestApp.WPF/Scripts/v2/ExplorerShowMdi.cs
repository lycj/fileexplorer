using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.Script;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.Views;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TestApp
{
    public static partial class TestAppCommands
    {
        public static IScriptCommand FileList_NewMdiWindow = 
             UIScriptCommands.FileListAssignSelection("{Selection}",                     //Assign Selection
                         ScriptCommands.IfArrayLength(ComparsionOperator.Equals, "{Selection}", 1,
                         ScriptCommands.Assign("{StartupPath}", "{Selection[0].FullPath}", false,
                         TestAppCommands.ExplorerNewMdiWindow())));

        public static IScriptCommand MdiExplorer_Initialize_Default =
                ScriptCommands.Assign("{FileListNewWindowCommand}", TestAppCommands.FileList_NewMdiWindow, false,          
                IOInitializeHelpers.Explorer_Initialize_Default);
        

        public static IScriptCommand ExplorerNewMdiWindow(WPF.MDI.MdiContainer container,
            IProfile[] profiles, IEntryModel[] rootDirectories,
           string explorerVariable = "{Explorer}", IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{MdiContainer}", container},
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },
                    {"{OnModelCreated}", ScriptCommands.RunSequence(null,
                        TestAppCommands.MdiExplorer_Initialize_Default, 
                        UIScriptCommands.ExplorerAssignScriptParameters("{Explorer}", "{MdiContainer},{RootDirectories}"))}, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }                    
                }, false,
                  TestAppCommands.ExplorerNewMdiWindow("{MdiContainer}", "{WindowManager}", "{GlobalEvents}", "{Explorer}", nextCommand));                  
        }

        public static IScriptCommand ExplorerNewMdiWindow(string mdiContainerVariable = "{MdiContainer}",
            string windowManagerVariable = "{WindowManager}", string eventsVariable = "{GlobalEvents}", string explorerVariable = "{Explorer}",
            IScriptCommand nextCommand = null)
        {
            return
                  UIScriptCommands.ExplorerCreate(ExplorerMode.Normal, "{OnModelCreated}", "{OnViewAttached}",
                  windowManagerVariable, eventsVariable, explorerVariable,
                    TestAppCommands.ExplorerShowMdi(mdiContainerVariable, windowManagerVariable, explorerVariable,
                    nextCommand));
        }

        public static IScriptCommand ExplorerShowMdi(string mdiContainerVariable = "{MdiContainer}",
            string windowManagerKey = "{WindowManager}", string explorerKey = "{Explorer}",
            IScriptCommand nextCommand = null)
        {
            return new ExplorerShowMdi()
            {
                MdiContainerKey = mdiContainerVariable,
                WindowManagerKey = windowManagerKey,
                ExplorerKey = explorerKey,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class ExplorerShowMdi : ScriptCommandBase
    {

        public string MdiContainerKey { get; set; }

        /// <summary>
        /// WindowManager used to show the window, optional, Default={WindowManager}
        /// </summary>
        public string WindowManagerKey { get; set; }

        /// <summary>
        /// Show this IExplorerViewModel, default={Explorer}
        /// </summary>
        public string ExplorerKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerShowMdi>();

        public ExplorerShowMdi()
            : base("ExplorerShowMdi")
        {
            MdiContainerKey = "{MdiContainer}";
            WindowManagerKey = "{WindowManager}";
            ExplorerKey = "{Explorer}";
        }

        public override IScriptCommand Execute(FileExplorer.ParameterDic pm)
        {
            IWindowManager wm = pm.GetValue<IWindowManager>(WindowManagerKey) ?? new WindowManager();

            WPF.MDI.MdiContainer container = pm.GetValue<WPF.MDI.MdiContainer>(MdiContainerKey);

            if (container == null)
                return ResultCommand.Error(new KeyNotFoundException("MdiContainerKey"));

            IExplorerViewModel explorer = pm.GetValue<IExplorerViewModel>(ExplorerKey);

            if (explorer == null)
                return ResultCommand.Error(new KeyNotFoundException("ExplorerKey"));

            var view = new ExplorerView();
            Caliburn.Micro.Bind.SetModel(view, explorer); //Set the ViewModel using this command.
            var mdiChild = new WPF.MDI.MdiChild
            {
                DataContext = explorer,
                ShowIcon = true,
                Content = view                
            };

            mdiChild.SetBinding(WPF.MDI.MdiChild.TitleProperty, new Binding("DisplayName") { Mode = BindingMode.OneWay });
            mdiChild.SetBinding(WPF.MDI.MdiChild.IconProperty, new Binding("CurrentDirectory.Icon") { Mode = BindingMode.OneWay });
            mdiChild.SetBinding(WPF.MDI.MdiChild.WidthProperty, new Binding("Parameters.Width") { Mode = BindingMode.TwoWay });
            mdiChild.SetBinding(WPF.MDI.MdiChild.HeightProperty, new Binding("Parameters.Height") { Mode = BindingMode.TwoWay });
            mdiChild.SetBinding(WPF.MDI.MdiChild.PositionProperty, new Binding("Parameters.Position") { Mode = BindingMode.TwoWay });
            container.Children.Add(mdiChild);

            return NextCommand;
        }
    }
}
