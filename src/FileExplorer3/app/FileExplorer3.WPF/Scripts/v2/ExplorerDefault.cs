using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        public static IScriptCommand ExplorerDefault(string explorerVariable, 
            string fileListVariable, string directoryTreeVariable, 
            string breadcrumbVariable, IScriptCommand nextCommand = null)
        {
            return new ExplorerDefault()
            {
                ExplorerKey = explorerVariable,
                FileListKey = fileListVariable,
                DirectoryTreeKey = directoryTreeVariable,
                BreadcrumbKey = breadcrumbVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ExplorerDefault(IScriptCommand nextCommand = null)
        {
            return ExplorerDefault("{Explorer}", "{FileList}", "{DirectoryTree}", "{Breadcrumb}", nextCommand);
        }
    }

    /// <summary>
    /// Set default ScriptCommand and parameter for DiskBased use.
    /// </summary>
    public class ExplorerDefault : ScriptCommandBase
    {
        /// <summary>
        /// Point to Explorer (IExplorerViewModel).
        /// </summary>
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Point to FileList (IFileListViewModel).
        /// </summary>
        public string FileListKey { get; set; }

        /// <summary>
        /// Point to DirectoryTree (IDirectoryTreeViewModel).
        /// </summary>
        public string DirectoryTreeKey { get; set; }

        /// <summary>
        /// Point to Breadcrumb (IBreadcrumbViewModel).
        /// </summary>
        public string BreadcrumbKey { get; set; }


        public string WindowManagerKey { get; set; }

        public ExplorerDefault()
            : base("ExplorerDefault")
        {
            ExplorerKey = "{Explorer}";
            FileListKey = "{FileList}";
            DirectoryTreeKey = "{DirectoryTree}";
            BreadcrumbKey = "{Breadcrumb}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {            
            return 
                ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    { "{ColumnList}", UIInitializeHelpers.FileList_ColumList }, 
                    { "{ColumnFilters}", UIInitializeHelpers.FileList_ColumnFilter }, 
                    { "{FileListOpenCommand}", UIInitializeHelpers.FileList_Open }, 
                    { "{FileListDeleteCommand}", UIInitializeHelpers.FileList_Delete }, 
                    { "{FileListNewFolderCommand}", UIInitializeHelpers.FileList_NewFolder },                     
                    //{ "{FileListCutCommand}", UIInitializeHelpers.FileList_Cut },
                    //{ "{FileListCopyCommand}", UIInitializeHelpers.FileList_Copy },
                    //{ "{FileListPasteCommand}", UIInitializeHelpers.FileList_Paste },    
                    { "{FileListNewWindowCommand}", UIInitializeHelpers.FileList_NewWindow },                    
                    { "{DirectoryTreeMapCommand}", UIInitializeHelpers.DirectoryTree_Map_From_Profiles },
                    { "{DirectoryTreeUnmapCommand}", UIInitializeHelpers.DirectoryTree_Unmap },
                    { "{DirectoryTreeNewWindowCommand}", UIInitializeHelpers.DirectoryTree_NewWindow },                    
                }, true,
                ScriptCommands.RunSequence(NextCommand,
                        UIScriptCommands.ExplorerAssignScriptParameters(ExplorerKey, "{Profiles}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.RootModels, "{RootDirectories}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.FileName, "{FileName}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.EnableDrag, "{EnableDrag}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.EnableDrop, "{EnableDrop}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.EnableMultiSelect, "{EnableMultiSelect}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ColumnList, "{ColumnList}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ColumnFilters, "{ColumnFilters}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.FilterString, "{FilterString}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ViewMode, "{ViewMode}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ItemSize, "{ItemSize}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ShowToolbar, "{ShowToolbar}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ShowSidebar, "{ShowSidebar}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ShowGridHeader, "{ShowGridHeader}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ExplorerWidth, "{ExplorerWidth}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ExplorerHeight, "{ExplorerHeight}"),
                        UIScriptCommands.ExplorerSetParameter(ExplorerKey, ExplorerParameterType.ExplorerPosition, "{ExplorerPosition}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "Open", "{FileListOpenCommand}"),                        
                        UIScriptCommands.SetScriptCommand(FileListKey, "Delete", "{FileListDeleteCommand}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "NewFolder", "{FileListNewFolderCommand}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "OpenTab", "{FileListOpenTabCommand}"),                            
                        UIScriptCommands.SetScriptCommand(FileListKey, "Cut", "{FileListCutCommand}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "Copy", "{FileListCopyCommand}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "Paste", "{FileListPasteCommand}"),
                        UIScriptCommands.SetScriptCommand(FileListKey, "NewWindow", "{FileListNewWindowCommand}"),
                        UIScriptCommands.SetScriptCommand(DirectoryTreeKey, "OpenTab", "{DirectoryTreeOpenTabCommand}"),
                        UIScriptCommands.SetScriptCommand(DirectoryTreeKey, "NewWindow", "{DirectoryTreeNewWindowCommand}"),
                        UIScriptCommands.SetScriptCommand(DirectoryTreeKey, "Map", "{DirectoryTreeMapCommand}"),
                        UIScriptCommands.SetScriptCommand(DirectoryTreeKey, "Unmap", "{DirectoryTreeUnmapCommand}")      
                       )                
                );
        }
    }
}
