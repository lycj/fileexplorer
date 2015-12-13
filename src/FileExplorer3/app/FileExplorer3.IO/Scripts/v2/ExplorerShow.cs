using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        public static IScriptCommand ExplorerNewWindow(IProfile[] profiles, IEntryModel[] rootDirectories,
            string explorerVariable = "{Explorer}", IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },
                    {"{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }

                }, false,
                  UIScriptCommands.ExplorerNewWindow("{OnModelCreated}", "{OnViewAttached}", "{WindowManager}",
                        "{GlobalEvents}", explorerVariable, nextCommand));
        }

        public static IScriptCommand ExplorerNewTabWindow(IProfile[] profiles, IEntryModel[] rootDirectories,
            string tabbedExplorerVariable = "{TabbedExplorer}", IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },
                    {"{OnModelCreated}", IOInitializeHelpers.TabbedExplorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() },

                }, false,
                  UIScriptCommands.ExplorerNewTabWindow("{OnModelCreated}", "{OnViewAttached}", 
                  "{OnTabExplorerCreated}", "{OnTabExplorerAttached}",
                  "{WindowManager}", "{GlobalEvents}", tabbedExplorerVariable, UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", null, "{Explorer}", nextCommand)));
        }

        public static IScriptCommand FileOpen(IProfile[] profiles, IEntryModel[] rootDirectories,            
           bool enableMultiSelect = true, string filterString = "All files (*.*)|*.*",
           string selectionPathsVariable = "{Selection}",
           IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },                    
                    {"{EnableDrag}", false },
                    {"{EnableDrop}", false },
                    {"{FilterString}", filterString },
                    {"{EnableMultiSelect}", enableMultiSelect },
                    { "{FileListNewWindowCommand}", NullScriptCommand.Instance },
                    {"{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }
                }, false,
                  UIScriptCommands.ExplorerPick(ExplorerMode.FileOpen, "{OnModelCreated}", "{OnViewAttached}", "{WindowManager}",
                        "{GlobalEvents}", null, selectionPathsVariable, nextCommand, cancelCommand));
        }

        public static IScriptCommand FileSave(IProfile[] profiles, IEntryModel[] rootDirectories,            
            string filterString = "All files (*.*)|*.*", 
            string selectionPathsVariable = "{Selection}",
            IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },                    
                    {"{EnableDrag}", false },
                    {"{EnableDrop}", false },
                    {"{FilterString}", filterString },
                    {"{EnableMultiSelect}", false },
                    {"{FileListNewWindowCommand}", NullScriptCommand.Instance },
                    {"{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }
                }, false,
                  UIScriptCommands.ExplorerPick(ExplorerMode.FileSave, "{OnModelCreated}", "{OnViewAttached}", "{WindowManager}",
                        "{GlobalEvents}", null, selectionPathsVariable, nextCommand, cancelCommand));
        }


        public static IScriptCommand FileSave(
           string filterString = "All files (*.*)|*.*",
           string selectionPathsVariable = "{Selection}",
           IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {                                     
                    {"{EnableDrag}", false },
                    {"{EnableDrop}", false },
                    {"{FilterString}", filterString },
                    {"{EnableMultiSelect}", false },
                    {"{FileListNewWindowCommand}", NullScriptCommand.Instance },
                    {"{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }
                }, false,
                  UIScriptCommands.ExplorerPick(ExplorerMode.FileSave, "{OnModelCreated}", "{OnViewAttached}", "{WindowManager}",
                        "{GlobalEvents}", null, selectionPathsVariable, nextCommand, cancelCommand));
        }

        public static IScriptCommand DirectoryPick(IProfile[] profiles, IEntryModel[] rootDirectories,
            string selectionVariable = "{Selection}", string selectionPathVariable = "{SelectionPaths}",
            IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
        {
            return ScriptCommands.Assign(new Dictionary<string, object>()
                {
                    {"{Profiles}", profiles },
                    {"{RootDirectories}", rootDirectories },                    
                    {"{EnableDrag}", false },
                    {"{EnableDrop}", false },                    
                    {"{EnableMultiSelect}", false },
                    {"{FileListNewWindowCommand}", NullScriptCommand.Instance },
                    {"{OnModelCreated}", IOInitializeHelpers.Explorer_Initialize_Default }, 
                    {"{OnViewAttached}", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot() }
                }, false,
                 UIScriptCommands.ExplorerPick(ExplorerMode.DirectoryOpen, "{OnModelCreated}", "{OnViewAttached}", "{WindowManager}",
                       "{GlobalEvents}", selectionVariable, selectionPathVariable, nextCommand, cancelCommand));
        }


         //public static IScriptCommand DirectoryPick(string onModelCreatedVariable = "{OnModelCreated}", string onViewAttachedVariable = "{OnViewAttached}",
        //   string windowManagerVariable = "{WindowManager}", string eventAggregatorVariable = "{GlobalEvents}",
         //   string selectedEntryVariable = "{Selection}", string selectedPathVariable = "{SelectionPath}", 
         //   IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
    }
}
