using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable, assign current directory (IEntryModel) to a variable.
        /// </summary>
        /// <example>
        ///  cmd = UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}",                        
        ///          ScriptCommands.SetProperty("{tbDirectory}", (TextBlock tb) => tb.Text, "{CurrentDirectory.FullPath}"));
        /// </example> 
        /// <param name="fileListVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerAssignCurrentDirectory(string explorerVariable = "{Explorer}",
            string destinationVariable = "{CurrentDirectory}", IScriptCommand nextCommand = null)
        {
            return new ExplorerAssignCurrentDirectory()
            {
                ExplorerKey = explorerVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, assign current directory (IEntryModel) to a variable.
        /// </summary>
        /// <example>
        ///  cmd = UIScriptCommands.ExplorerAssignCurrentDirectory("{CurrentDirectory}",                        
        ///          ScriptCommands.SetProperty("{tbDirectory}", (TextBlock tb) => tb.Text, "{CurrentDirectory.FullPath}"));
        /// </example>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerAssignCurrentDirectory(string destinationVariable = "{CurrentDirectory}", 
            IScriptCommand nextCommand = null)
        {
            return ExplorerAssignCurrentDirectory("{Explorer}", destinationVariable, nextCommand);
        }       
    }

    public class ExplorerAssignCurrentDirectory : ScriptCommandBase
    {
        /// <summary>
        /// Key for Explorer (IExplorerViewModel), FileList, Directory Tree or Breadcrumb, Default = {Explorer}
        /// </summary>
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Key to set the select value (IEntryModel[]) to, Default = {CurrentDirectory}
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerAssignCurrentDirectory>();

        public ExplorerAssignCurrentDirectory()
            : base("ExplorerAssignCurrentDirectory")
        {
            ExplorerKey = "{Explorer}";
            DestinationKey = "{CurrentDirectory}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var flValue = pm.GetValue(ExplorerKey);
            IEntryModel em = flValue is IExplorerViewModel ? (flValue as IExplorerViewModel).CurrentDirectory.EntryModel :
                flValue is IFileListViewModel ? (flValue as IFileListViewModel).CurrentDirectory : 
                flValue is IDirectoryTreeViewModel ? (flValue as IDirectoryTreeViewModel).Selection.SelectedChild : 
                flValue is IBreadcrumbViewModel ? (flValue as IBreadcrumbViewModel).Selection.SelectedChild : null;

            if (em == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));

            pm.SetValue(DestinationKey, em, true);
            return NextCommand;                
        }

    }
}
