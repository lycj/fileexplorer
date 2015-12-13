using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Models;
using FileExplorer.WPF;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static class UIInitializeHelpers
    {



        #region FileList_ColumnInfo_For_DiskBased_Items

        public static ColumnInfo[] FileList_ColumList = new ColumnInfo[] 
                {
                    ColumnInfo.FromTemplate("Name", "GridLabelTemplate", "EntryModel.Label", new ValueComparer<IEntryModel>(p => p.Label), 200),   
                    ColumnInfo.FromBindings("Type", "EntryModel.Description", "", new ValueComparer<IEntryModel>(p => p.Description), 200),                                       
                               ColumnInfo.FromBindings("Time", "EntryModel.LastUpdateTimeUtc", "", 
                        new ValueComparer<IEntryModel>(p => p.LastUpdateTimeUtc), 200), 
                };
        #endregion

        #region FileList_ColumnFilter_For_DiskBased_Items
        public static ColumnFilter[] FileList_ColumnFilter = new ColumnFilter[]
                {
                    ColumnFilter.CreateNew<IEntryModel>("0 - 9", "EntryModel.Label", e => Regex.Match(e.Label, "^[0-9]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("A - H", "EntryModel.Label", e => Regex.Match(e.Label, "^[A-Ha-h]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("I - P", "EntryModel.Label", e => Regex.Match(e.Label, "^[I-Pi-i]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("Q - Z", "EntryModel.Label", e => Regex.Match(e.Label, "^[Q-Zq-z]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("The rest", "EntryModel.Label", e => Regex.Match(e.Label, "^[^A-Za-z0-9]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("Today", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year && 
                                e.LastUpdateTimeUtc.Month == dt.Month && 
                                e.LastUpdateTimeUtc.Day == dt.Day;
                        }),
                    ColumnFilter.CreateNew<IEntryModel>("Earlier this month", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year && e.LastUpdateTimeUtc.Month == dt.Month;
                        }),
                     ColumnFilter.CreateNew<IEntryModel>("Earlier this year", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year;
                        }), 
                    ColumnFilter.CreateNew<IEntryModel>("A long time ago", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year != dt.Year;
                        }),    
                    ColumnFilter.CreateNew<IEntryModel>("Directories", "EntryModel.Description", e => e.IsDirectory),
                    ColumnFilter.CreateNew<IEntryModel>("Files", "EntryModel.Description", e => !e.IsDirectory)
                };
        #endregion

        public static IScriptCommand FileList_Open =
             UIScriptCommands.FileListAssignSelection("{Selection}",                        //Assign Selection
               ScriptCommands.IfArrayLength(ComparsionOperator.Equals, "{Selection}", 1,    //If Selection.Length = 1
                 ScriptCommands.AssignArrayItem("{Selection}", 0, "{FirstSelected}",        //FirstSelected = Selection[0]
                   ScriptCommands.IfAssigned("{FirstSelected[0].LinkPath}", 
                     CoreScriptCommands.ParsePath("{Profiles}", "{FirstSelected.LinkPath}", "{Link-Entry}",
                       ScriptCommands.IfAssigned("{Link-Entry}", 
                         ScriptCommands.IfTrue("{Link-Entry.IsDirectory}", 
                            UIScriptCommands.ExplorerGoTo("{Explorer}", "{Link-Entry}"),
                            ResultCommand.NoError))), //Non-Directory, do nothing.
                     ScriptCommands.IfPropertyIsTrue("{FirstSelected}", "IsDirectory",        //FirstSelected.IsDirectory?                   
                        UIScriptCommands.NotifyDirectoryChanged("{Selection}"),             //True -> Broadcast ChangeDirectory using {GlobalEvents}
                        ResultCommand.NoError                         //False -> Do nothing
                        )
               ))));


        //public static IScriptCommand FileList_Delete_For_DiskBased_Items =
              

        //public static IScriptCommand FileList_NewFolder_ForDiskBased_Items =
        //    UIScriptCommands.ExplorerAssignCurrentDirectory("{FileList}", "{CurrentDirectory}",
        //        CoreScriptCommands.DiskCreateFolder("{CurrentDirectory.Profile}", "{CurrentDirectory.FullPath}\\NewFolder",
        //            "{NewFolder}", NameGenerationMode.Rename,
        //            UIScriptCommands.FileListRefreshThenSelect("{FileList}", "{NewFolder}", true, ResultCommand.OK)));

        public static IScriptCommand FileList_Delete =
             ScriptCommands.AssignCanExecuteCondition(
                  UIScriptCommands.FileListAssignSelection("{Selection}",
                  ScriptCommands.IfValue<int>(ComparsionOperator.GreaterThanOrEqual, "{Selection.Length}", 1, //If DeleteEntries Length >= 1                        
                    ScriptCommands.IfAssigned("{Selection[0].Profile.DeleteCommand}", ResultCommand.OK))),
              UIScriptCommands.FileListAssignSelection("{DeleteEntries}",                     //Assign Selection                
                ScriptCommands.Run("{DeleteEntries[0].Profile.DeleteCommand}", true)));

        public static IScriptCommand FileList_NewFolder = ScriptCommands.AssignCanExecuteCondition(
                  UIScriptCommands.ExplorerAssignCurrentDirectory("{FileList}", "{CurrentFolder}",
                    ScriptCommands.IfAssigned("{CurrentFolder.Profile.CreateFolderCommand}", ResultCommand.OK)),
                UIScriptCommands.ExplorerAssignCurrentDirectory("{FileList}", "{BaseFolder}",
                    ScriptCommands.Assign("{FolderName}", "New Folder", false,
                        ScriptCommands.Run("{BaseFolder.Profile.CreateFolderCommand}", true,
                            UIScriptCommands.FileListRefreshThenSelect("{FileList}", "{CreatedFolder}", true, ResultCommand.OK)))));

        public static IScriptCommand FileList_Selection_Is_One_Folder =
          UIScriptCommands.FileListAssignSelection("{Selection}",                     //Assign Selection
           ScriptCommands.IfArrayLength(ComparsionOperator.Equals, "{Selection}", 1,
             ScriptCommands.IfTrue("{Selection[0].IsDirectory}", ResultCommand.OK)));

        public static Func<IScriptCommand, IScriptCommand> If_FileList_Selection_Is_One_Folder = thenCommand =>
            ScriptCommands.AssignCanExecuteCondition(FileList_Selection_Is_One_Folder, thenCommand);       

        #region FileList_NewWindow, NewTabbedWindow, OpenTab

        
        public static Func<string, IScriptCommand> NewWindow = (dirVariable) =>
            UIScriptCommands.ExplorerGetParameter("{Explorer}", ExplorerParameterType.RootModels, "{RootDirectories}",
           UIScriptCommands.ExplorerNewWindow("{OnModelCreated}", "{OnViewAttached}",
               "{WindowManager}", "{GlobalEvents}", "{Explorer}",
                   UIScriptCommands.ExplorerGoTo("{Explorer}", dirVariable)));

        public static Func<string, IScriptCommand> NewTabbedWindow = (dirVariable) =>
             UIScriptCommands.ExplorerGetParameter("{Explorer}", ExplorerParameterType.RootModels, "{RootDirectories}",
                    UIScriptCommands.ExplorerNewTabWindow("{OnModelCreated}", "{OnViewAttached}", "{OnTabExplorerCreated}", "{OnTabExplorerAttached}",
                        "{WindowManager}", "{GlobalEvents}", "{TabbedExplorer}",
                        UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", dirVariable, "{Explorer}", null)));

        public static Func<string, IScriptCommand> OpenTab = (dirVariable) =>
            UIScriptCommands.ExplorerGetParameter("{Explorer}", ExplorerParameterType.RootModels, "{RootDirectories}",
                   UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", dirVariable, "{Explorer}", null));

        public static IScriptCommand FileList_NewWindow = If_FileList_Selection_Is_One_Folder(NewWindow("{Selection[0]}"));          
        public static IScriptCommand FileList_NewTabbedWindow = If_FileList_Selection_Is_One_Folder(NewTabbedWindow("{Selection[0]}"));                       
        public static IScriptCommand FileList_OpenTab = If_FileList_Selection_Is_One_Folder(OpenTab("{Selection[0]}"));                        
                  
        #endregion

        #region DirectoryTree_NewWindow, NewTabbedWindow, OpenTab
        

        public static IScriptCommand DirectoryTree_NewWindow = UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}", NewWindow("{CurrentDirectory}"));
        public static IScriptCommand DirectoryTree_NewTabbedWindow = UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}", NewTabbedWindow("{CurrentDirectory}"));
        public static IScriptCommand DirectoryTree_OpenTab = UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}", OpenTab("{CurrentDirectory}"));
        #endregion

        #region Map/Unmap

        public static IScriptCommand DirectoryTree_SupportMapping =
          ScriptCommands.IfTrue("{EnableMap}",         
            ScriptCommands.IfValue(ComparsionOperator.GreaterThan, "{Profiles.Length}", 1, 
                ResultCommand.OK));            

        public static IScriptCommand DirectoryTree_Map_From_Profiles =
          ScriptCommands.AssignCanExecuteCondition(DirectoryTree_SupportMapping,
            UIScriptCommands.ProfilePicker("{Profiles}", "{Profile}", "{WindowManager}",
                   CoreScriptCommands.ParsePath("{Profile}", "", "{RootDirectories}",
                   ScriptCommands.AssignProperty("{RootDirectories}", "FullPath", "{StartupPath}",
                    UIScriptCommands.ExplorerPick(ExplorerMode.DirectoryOpen, "{OnModelCreated}", "{OnViewAttached}",
                     "{WindowManager}", null, "{Selection}", "{SelectionPath}",
                      UIScriptCommands.NotifyRootCreated("{Selection}",
                        UIScriptCommands.ExplorerGoTo("{Explorer}", "{Selection}")))))));

        public static IScriptCommand DirectoryTree_SupportMapping_And_IsFirstLevel =
            ScriptCommands.IfTrue("{EnableMap}",         
            ScriptCommands.IfValue(ComparsionOperator.GreaterThan, "{Profiles.Length}", 1,
                Explorer.DoSelection(ems =>
                Script.ScriptCommands.If(pd => (ems.First() as FileExplorer.WPF.ViewModels.IDirectoryNodeViewModel).Selection.IsFirstLevelSelector(), 
                ResultCommand.OK, ResultCommand.NoError))));        

        //To-Do: Update to new ScriptCommand.
        public static IScriptCommand DirectoryTree_Unmap =
          ScriptCommands.AssignCanExecuteCondition(DirectoryTree_SupportMapping_And_IsFirstLevel,
            Explorer.DoSelection(ems =>
                Script.ScriptCommands.If(pd => (ems.First() as FileExplorer.WPF.ViewModels.IDirectoryNodeViewModel).Selection.IsFirstLevelSelector(),
                        Script.WPFScriptCommands.IfOkCancel(new Caliburn.Micro.WindowManager(), pd => "Unmap",
                            pd => String.Format("Unmap {0}?", ems.First().EntryModel.Label),
                            Explorer.BroadcastRootChanged(FileExplorer.WPF.Defines.RootChangedEvent.Deleted(ems.Select(em => em.EntryModel).ToArray())),
                            ResultCommand.OK),
                        Script.ScriptCommands.NoCommand), Script.ScriptCommands.NoCommand));

        #endregion


        public static string Explorer_Initialize_Default_Retain_Script_Parameters =
                        "{RootDirectories},{Profiles},{GlobalEvents},{OnViewAttached},{OnModelCreated}," +
                        "{EnableDrag},{EnableDrop},{EnableMultiSelect},{EnableContextMenu},{EnableBookmark},{EnableTabsWhenOneTab}," +
                        "{EnableMap}";

        /// <summary>
        /// <para>Call ExplorerDefault(), ExplorerDefaultToolbarCommands() 
        /// and assign the followings parameter to CommandManager: (for use when OpenInNewWindow)
        /// OnViewAttached, OnModelCreated, EnableDrag/Drop/MultiSelect.</para>
        /// 
        /// <para>ExplorerDefault() allow you to change these parameters (in additional to above):
        /// RootDirectories, ColumnList, ColumnFilters, FilterString, ViewMode, ItemSize, ShowToolbar/Sidebar/GridHeader.</para>
        ///
        /// <para>ExplorerDefaultToolbarCommands() assign the CommandModels in Toolbar and ContextMenu, they are not
        /// Customizable currently.  You can only disable the command.</para>
        ///
        /// </summary>
        public static IScriptCommand Explorer_Initialize_Default =
            ScriptCommands.RunSequence(null,
                   UIScriptCommands.ExplorerDefault(),
                   UIScriptCommands.ExplorerDefaultToolbarCommands(),
                   UIScriptCommands.ExplorerAssignScriptParameters("{Explorer}",
                       Explorer_Initialize_Default_Retain_Script_Parameters)
                   );

        public static IScriptCommand TabbedExplorer_Initialize_Default =
          ScriptCommands.Assign("{FileListNewWindowCommand}", UIInitializeHelpers.FileList_NewTabbedWindow, false,
          ScriptCommands.Assign("{FileListOpenTabCommand}", UIInitializeHelpers.FileList_OpenTab, false,
          ScriptCommands.Assign("{DirectoryTreeNewWindowCommand}", UIInitializeHelpers.DirectoryTree_NewWindow, false,
          ScriptCommands.Assign("{DirectoryTreeOpenTabCommand}", UIInitializeHelpers.DirectoryTree_OpenTab, false,
          Explorer_Initialize_Default))));

    }
}
