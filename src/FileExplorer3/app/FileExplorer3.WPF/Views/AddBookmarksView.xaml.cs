using FileExplorer.Models;
using FileExplorer.Script;
using FileExplorer.WPF.ViewModels;
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

namespace FileExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for AddBookmarksView.xaml
    /// </summary>
    public partial class AddBookmarksView : UserControl
    {
        public AddBookmarksView()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var vm = DataContext as AddBookmarksViewModel;
            var rootDir = new IEntryModel[] { vm.Profile.RootModel };

            expFolderPicker.ViewModel.Initializer =
                new ScriptCommandInitializer()
                {
                    OnModelCreated = UIScriptCommands.ExplorerDefault(),
                    OnViewAttached = ScriptCommands.Assign("{StartupDir}", vm.CurrentBookmarkDirectory, false, 
                       ScriptCommands.Assign("{StartupPath}", "{StartupDir.FullPath}", false, 
                        UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot())),
                    RootModels = rootDir,
                    StartupParameters = new ParameterDic()
                    {
                         { "Profiles", vm.Profile },
                         { "RootDirectories", rootDir },	    
                         //{  "StartupPath", vm.CurrentBookmarkDirectory.FullPath },
                         //{ "StartupPath", _selectedPath },
                         //{ "FilterString", _filterStr },
                         { "ViewMode", "List" }, 
                         { "ItemSize", 16 },
                         { "EnableDrag", true }, 
                         { "EnableDrop", true }, 
                         { "EnableMap", false },
                         { "FileListNewWindowCommand", NullScriptCommand.Instance }, //Disable NewWindow Command.
                         { "EnableMultiSelect", true},
                         { "ShowToolbar", false }, 
                         { "ShowGridHeader", false }
                    }
                };

            expFolderPicker.ViewModel.FileList.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "CurrentDirectory")
                        vm.CurrentBookmarkDirectory = expFolderPicker.ViewModel.FileList.CurrentDirectory;
                };
            vm.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "CurrentBookmarkDirectory")
                    expFolderPicker.ViewModel.FileList.CurrentDirectory = vm.CurrentBookmarkDirectory;
            };
        }
    }
}
