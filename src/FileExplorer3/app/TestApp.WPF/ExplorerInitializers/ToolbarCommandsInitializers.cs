using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.WPF.Models;
using FileExplorer.WPF;
using FileExplorer.Defines;
using FileExplorer.IO;

namespace TestApp
{    
    public class DummyMetadataProvider : IMetadataProvider
    {
        public async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels,
            int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();

            if (selectedModels.Count() == 0)
            {
                retList.Add(new Metadata(DisplayType.Number, MetadataStrings.strCategoryTest, "Number", 10000) { IsVisibleInSidebar = true });
                retList.Add(new Metadata(DisplayType.Percent, MetadataStrings.strCategoryTest, "Percent", 10) { IsVisibleInSidebar = true });
                retList.Add(new Metadata(DisplayType.Boolean, MetadataStrings.strCategoryTest, "Boolean", true, false) { IsVisibleInSidebar = true });
            }
            return retList;

        }

    }



    public class ToolbarCommandsInitializers : IViewModelInitializer<IExplorerViewModel>
    {
        private IWindowManager _windowManager;

        public ToolbarCommandsInitializers(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public static void InitializeToolbarCommands(IExplorerViewModel explorerModel, IWindowManager windowManager)
        {
            explorerModel.Sidebar.Metadata.ExtraMetadataProviders = new[] {
                new DummyMetadataProvider()
            };



            explorerModel.FileList.Commands.ToolbarCommands.ExtraCommandProviders = new[] {                                               
                new StaticCommandProvider(
                     //new CommandModel(ExplorerCommands.CloseTab) { IsEnabled = true, Header="CloseTab", IsVisibleOnToolbar = true },
                    new FileExplorer.Models.SevenZipSharp.SzsCommandModel(explorerModel.Initializer),
                    new SeparatorCommandModel(),
                    new SelectGroupCommand( explorerModel.FileList),    
                    new ViewModeCommand( explorerModel.FileList),
                    new GoogleExportCommandModel(() => explorerModel.RootModels)
                    { IsVisibleOnMenu = true, WindowManager = windowManager },
                    
                    new SeparatorCommandModel(),
                    new CommandModel(ExplorerCommands.NewFolder) { IsVisibleOnToolbar = true, 
                        HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE188)
                        //Symbol = Convert.ToChar(0xE188) 
                    },
                    new DirectoryCommandModel(
                        new CommandModel(ExplorerCommands.NewFolder) { Header = Strings.strFolder, IsVisibleOnMenu = true },
                        new CommandModel(FileExplorer.Models.SevenZipSharp.SzsCommandProvider.NewZip) { Header = "Zip", IsVisibleOnMenu = true },
                        new CommandModel(FileExplorer.Models.SevenZipSharp.SzsCommandProvider.New7z) { Header = "7z", IsVisibleOnMenu = true }
                        )
                        { IsVisibleOnMenu = true, Header = Strings.strNew, IsEnabled = true},
                    new ToggleVisibilityCommand(explorerModel.FileList.Sidebar, ExplorerCommands.TogglePreviewer)                    
                    //new CommandModel(ExplorerCommands.TogglePreviewer) { IsVisibleOnMenu = false, Header = "", IsHeaderAlignRight = true, Symbol = Convert.ToChar(0xE239) }
                    )
            };

            explorerModel.DirectoryTree.Commands.ToolbarCommands.ExtraCommandProviders = new[] { 
                new StaticCommandProvider(
                    new CommandModel(ExplorerCommands.NewWindow) { IsVisibleOnMenu = true },
                    new CommandModel(ExplorerCommands.OpenTab) { IsVisibleOnMenu = true },
                     //new CommandModel(ApplicationCommands.New) { IsVisibleOnMenu = true },
                    new CommandModel(ExplorerCommands.Refresh) { IsVisibleOnMenu = true },
                    new CommandModel(ApplicationCommands.Delete) { IsVisibleOnMenu = true },
                    new CommandModel(ExplorerCommands.Rename)  { IsVisibleOnMenu = true },
              
                    new CommandModel(ExplorerCommands.Map)  { 
                        //Symbol = Convert.ToChar(0xE17B), 
                        HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE17B),
                        IsEnabled = true,
                        IsHeaderVisible = false, IsVisibleOnToolbar = true
                    },
                    new CommandModel(ExplorerCommands.Unmap)  { 
                        HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE17A),
                        IsVisibleOnMenu = true,
                        IsVisibleOnToolbar = true
                    }
                    )
              };
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            InitializeToolbarCommands(explorerModel, _windowManager);
        }
    }
}


