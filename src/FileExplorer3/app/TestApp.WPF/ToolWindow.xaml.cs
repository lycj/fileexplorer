using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using FileExplorer.UIEventHub;
using System.Windows;
using FileExplorer.Defines;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for ToolWindowTest.xaml
    /// </summary>
    public partial class ToolWindowTest : Window
    {
        private IEntryModel[] _rootDirs;
        private string _filterStr;
        private string _selectedPath;
        private IProfile[] _profiles;        

        public ToolWindowTest(IProfile[] profiles, IEntryModel[] rootDirs, string mask, string selectedPath = "c:\\")
        {
            InitializeComponent();
            _profiles = profiles;
            _rootDirs = rootDirs;
            _filterStr = mask;
            _selectedPath = selectedPath;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            

            cbExplorerMode.ItemsSource = Enum.GetValues(typeof(FileExplorer.WPF.UserControls.Explorer.ExplorerMode));
            cbExplorerMode.SelectedValue = FileExplorer.WPF.UserControls.Explorer.ExplorerMode.ToolWindow;

            //For use in ToolWindow2 mode.
            //explorer.ViewModel.Parameters.NavigationSize = 25;
            //explorer.ViewModel.Parameters.DirectoryTreeSize = "2*";
            //explorer.ViewModel.Parameters.FileListSize = "*";

            explorer.ViewModel.Initializer = 
                new ScriptCommandInitializer()
                {
                    OnModelCreated = IOInitializeHelpers.Explorer_Initialize_Default,
                    OnViewAttached = UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot(),
                    RootModels = _rootDirs,
                    StartupParameters = new ParameterDic()
                    {
                         { "Profiles", _profiles },
                         { "RootDirectories", _rootDirs },	    
                         { "StartupPath", _selectedPath },
                         { "FilterString", _filterStr },
                         { "ViewMode", "List" }, 
                         { "ItemSize", 16 },
                         { "EnableDrag", true }, 
                         { "EnableDrop", true }, 
                         { "FileListNewWindowCommand", NullScriptCommand.Instance }, //Disable NewWindow Command.
                         { "EnableMultiSelect", true},
                         { "ShowToolbar", false }, 
                         { "ShowGridHeader", false }
                    }
                };
            testDroppable.DataContext = new TestDroppableViewModel();

            #region Obsoluted
            //exp.RootDirectories = _rootDirs;
            //exp.ViewModel.FileList.ShowToolbar = false;
            //exp.ViewModel.FileList.ShowGridHeader = false;
            //exp.ViewModel.FileList.Parameters.ViewMode = "List";
            //exp.ViewModel.FileList.Parameters.ItemSize = 16;
            //exp.ViewModel.FileList.EnableDrag = true;
            //exp.ViewModel.FileList.EnableDrop = false;
            //exp.ViewModel.FileList.EnableMultiSelect = false;
            //exp.ViewModel.FilterStr = _mask;
           

            //if (_selectedPath != null)
            //    exp.ViewModel.GoAsync(_selectedPath);

            //FileSystemInfoExProfile profile = new FileSystemInfoExProfile(exp.ViewModel.Events, exp.ViewModel.WindowManager);
            //var rootModel = AsyncUtils.RunSync(() => profile.ParseAsync(""));

            //or exp.ViewModel.Commands.ExecuteAsync(new IScriptCommand[] { Explorer.GoTo("C:\\") });
            #endregion
        }
    }

    public class TestDroppableViewModel : NotifyPropertyChanged, ISupportDropHelper
    {

        #region Constructors

        public TestDroppableViewModel()
        {
            IProfile exProfile = new FileSystemInfoExProfile(null, null);
            DropHelper = new LambdaShellDropHelper<IEntryModel>(
                 new LambdaValueConverter<IEntryViewModel, IEntryModel>(
                    (evm) =>  evm.EntryModel,                    
                    (em) => EntryViewModel.FromEntryModel(em)),

                 new LambdaValueConverter<IEnumerable<IEntryModel>, IDataObject>(
                     (ems) => exProfile.DragDrop.GetDataObject(ems),
                     (da) => exProfile.DragDrop.GetEntryModels(da)), 

                 (ems, eff) =>
                 {
                     return QueryDropEffects.CreateNew(DragDropEffectsEx.Copy);
                 },
                (ems, da, eff) =>
                {
                    if (ems.Count() > 1)
                        Label = ems.Count() + " items.";
                    else Label = ems.First().FullPath;
                    return DragDropEffectsEx.Copy;
                }) { DisplayName = "TestLabel" };
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        private string _label = "Drop here";

        #endregion

        #region Public Properties

        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyOfPropertyChanged(() => Label); }
        }


        #endregion







        ISupportDrop _dropHelper;

        public ISupportDrop DropHelper
        {
            get { return _dropHelper; }
            set { _dropHelper = value; }
        }
    }
}
