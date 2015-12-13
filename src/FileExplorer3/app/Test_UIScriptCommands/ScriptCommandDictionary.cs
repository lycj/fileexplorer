using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Test_UIScriptCommands
{
    public static class ScriptCommandDictionary
    {
        private static Dictionary<string, IScriptCommand> _scriptCommandDictionary = new Dictionary<string, IScriptCommand>()
        {
            { "UIScriptCommands.ExplorerGoto", 
                CoreScriptCommands.ParsePath("{Profiles}", "{tbDirectory.Text}", "{Directory}",
                    UIScriptCommands.ExplorerGoTo("{Directory}")) }, 

            {"UIScriptCommands.DirectoryTreeToggleExpand",  
                CoreScriptCommands.ParsePath("{Profiles}", "{tbDirectory.Text}", "{Directory}",
                    UIScriptCommands.DirectoryTreeToggleExpand("{Directory}")) },

            {"UIScriptCommands.DirectoryTreeToggleCollapse",  
                CoreScriptCommands.ParsePath("{Profiles}", "{tbDirectory.Text}", "{Directory}",
                    UIScriptCommands.DirectoryTreeToggleCollapse("{Directory}")) },

            {"UIScriptCommands.ExplorerAssignCurrentDirectory", 
                UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}",                        
                    ScriptCommands.SetProperty("{tbDirectory}", (TextBlock tb) => tb.Text, "{CurrentDirectory.FullPath}")) },

            {"UIScriptCommands.ExplorerAssignScriptParameters (Step 1)", 
                ScriptCommands.Assign("{LastEdit}", DateTime.Now, false,
                    ScriptCommands.Assign("{Today}", DateTime.Now.DayOfWeek, false,
                        UIScriptCommands.ExplorerAssignScriptParameters("{Explorer}", "{LastEdit},{Today}"))) },
                
            {"UIScriptCommands.ExplorerAssignScriptParameters (Step 2)", 
                UIScriptCommands.MessageBoxOK("ExplorerAssignScriptParameters", "LastEdit = {LastEdit}, Today = {Today}") },

            {"UIScriptCommands.ExplorerSetParameter", 
                UIScriptCommands.ExplorerGetParameter(ExplorerParameterType.ExplorerWidth, "{Width}", 
                    UIScriptCommands.ExplorerGetParameter(ExplorerParameterType.ExplorerHeight, "{Height}", 
                        ScriptCommands.AddValue("{Width}", -150, "{Width}",
                            ScriptCommands.AddValue("{Height}", -150, "{Height}",
                                UIScriptCommands.MessageBoxOK("SetParameter", "Creating new window size {Width} x {Height}", 
                                    UIScriptCommands.ExplorerNewWindow("{OnModelCreated}", "{OnViewAttached}", "{WindowManager}", "{GlobalEvents}", "{TestExplorer}", 
                                        UIScriptCommands.ExplorerSetParameter("{TestExplorer}", ExplorerParameterType.ExplorerWidth, "{Width}", 
                                            UIScriptCommands.ExplorerSetParameter("{TestExplorer}", ExplorerParameterType.ExplorerHeight, "{Height}")))))))) },

             {"UIScriptCommands.ExplorerNewWindow", 
                ScriptCommands.Assign("{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default, true,
                    ScriptCommands.Assign("{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot(), true, 
                        UIScriptCommands.ExplorerNewWindow("{OnModelCreated}", "{OnViewAttached}", "{WindowManager}", "{GlobalEvents}", "{TestExplorer}"))) },
             
             {"UIScriptCommands.ExplorerPick", 
                ScriptCommands.Assign("{OnModelCreated_Pick}", 
                    ScriptCommands.RunSequence(    
                        ScriptCommands.Assign("{Filter}", "Text File|*.txt", false, 
                            UIScriptCommands.ExplorerSetParameter(ExplorerParameterType.FilterString, "{Filter}")), 
                                IOInitializeHelpers.Explorer_Initialize_Default), true,
                        //Also See IOScriptCommands.FilePick
                        UIScriptCommands.ExplorerPick(ExplorerMode.FileOpen, "{OnModelCreated_Pick}", 
                            "{OnViewAttached}", "{WindowManager}", "{GlobalEvents}", "{SelectedEntries}", "{SelectedPaths}", 
                            UIScriptCommands.MessageBoxOK("ExplorerPick", "{SelectedPaths[0]}"))) },

             {"UIScriptCommands.FileListSelect", 
                UIScriptCommands.FileListAssignAll("{CurrentFileList}",  
                    ScriptCommands.FilterArray<string>("{CurrentFileList}", "FullPath", ComparsionOperator.EndsWithIgnoreCase, ".txt", "{TextFilesOnly}",             
                    UIScriptCommands.FileListSelect("{TextFilesOnly}", ResultCommand.NoError))) },

             {"UIScriptCommands.FileListRefresh", 
                UIScriptCommands.FileListRefresh(true, UIScriptCommands.MessageBoxOK("Refresh", "Refreshed filelist")) },
      
              {"UIScriptCommands.MessageBoxYesNo", 
                UIScriptCommands.MessageBoxYesNo("MessageBoxYesNo", "{Now} is Holiday?", 
                    UIScriptCommands.MessageBoxOK("MessageBoxOK", "Rest"), 
	                    UIScriptCommands.MessageBoxOK("MessageBoxOK", "Work")) },

              {"UIScriptCommands.NotifyDirectoryChanged", 
                UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}",
                    UIScriptCommands.NotifyDirectoryChanged("{CurrentDirectory}")) },

            {"UIScriptCommands.NotifyRootChanged", 
                CoreScriptCommands.ParsePath("{Profiles}", "{tbDirectory.Text}", "{newRoot}", 
                    UIScriptCommands.NotifyRootCreated("{newRoot}")) },

            {"UIScriptCommands.ProfilePicker", 
                UIScriptCommands.ProfilePicker("{Profiles}", "{SelectedProfile}", "{WindowManager}", 
                    UIScriptCommands.MessageBoxOK("", "{SelectedProfile}")) },

            {"UIScriptCommands.SetScriptCommand (Set)", 
                UIScriptCommands.SetScriptCommand("{FileList}", "Open", 
                    UIScriptCommands.FileListAssignSelection("{Selection}",
                        UIScriptCommands.MessageBoxOK("SetScriptCommand", "Double clicked {Selection.Length} items")), 
                    UIScriptCommands.MessageBoxOK("SetScriptCommand", "Open Command overrided, now try doubleclick an item.")) },                 

            {"UIScriptCommands.SetScriptCommand (Reset)", 
                UIScriptCommands.SetScriptCommand("{FileList}", "Open", IOInitializeHelpers.FileList_Open_For_DiskBased_Items, 
                    UIScriptCommands.MessageBoxOK("SetScriptCommand", "Open Command resetted.")) },
                
             {"UIScriptCommands.ExplorerNewTabWindow", 
                CoreScriptCommands.ParsePath("{Profiles}", "{tbDirectory.Text}", "{newRoot}", 
                    UIScriptCommands.ExplorerNewTabWindow("{OnModelCreated}", "{OnViewAttached}", null, null, "{WindowManager}", "{GlobalEvents}", "{TabbedExplorer}", 
                        UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", "{newRoot}", "{Explorer}", null ))) } 
                    
        };

        public static Dictionary<string, IScriptCommand> Dictionary { get { return _scriptCommandDictionary; } }
        public static IEnumerable<string> CommandList { get { return _scriptCommandDictionary.Keys; } }

    }
}
