using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.ViewModels.Helpers;
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
        /// Serializable, toggle a node specified in an variable in [DirectoryTree] to expand or collapse, recrusively.
        /// </summary>
        /// <example>
        ///   cmd = CoreScriptCommands.ParsePath("{Profiles}", tbDirectory.Text, "{Directory}",
        ///      UIScriptCommands.DirectoryTreeToggleNode("{DirectoryTree}", "{Directory}", DirectoryTreeToggleMode.Collapse)));
        ///</example>
        /// <param name="explorerVariable"></param>
        /// <param name="directoryEntryVariable"></param>
        /// <param name="toggleMode"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DirectoryTreeToggleNode(string explorerVariable = "{DirectoryTree}",
           string directoryEntryVariable = "{Directory}",
          DirectoryTreeToggleMode toggleMode = DirectoryTreeToggleMode.Expand, IScriptCommand nextCommand = null)
        {
            return new DirectoryTreeToggleNode()
            {
                ExplorerKey = explorerVariable,
                DirectoryEntryKey = directoryEntryVariable,
                ToggleMode = toggleMode,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, toggle a node specified in an variable in [DirectoryTree] to expand.
        /// </summary>
        /// <param name="directoryEntryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DirectoryTreeToggleExpand(
            string directoryEntryVariable = "{Directory}", IScriptCommand nextCommand = null)
        {
            return DirectoryTreeToggleNode("{DirectoryTree}", directoryEntryVariable, DirectoryTreeToggleMode.Expand,
                nextCommand);
        }

        /// <summary>
        /// Serializable, toggle a node specified in an variable in [DirectoryTree] to collapse.
        /// </summary>
        /// <param name="directoryEntryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DirectoryTreeToggleCollapse(
           string directoryEntryVariable = "{Directory}", IScriptCommand nextCommand = null)
        {
            return DirectoryTreeToggleNode("{DirectoryTree}", directoryEntryVariable, DirectoryTreeToggleMode.Collapse,
                nextCommand);
        }
    }


    public enum DirectoryTreeToggleMode { Expand, Collapse }

    /// <summary>
    /// Expand or Collapse 
    /// </summary>
    public class DirectoryTreeToggleNode : ScriptCommandBase
    {
        /// <summary>
        /// Key for Explorer (IExplorerViewModel) or DirectoryTree (IDirectoryTreeViewModel), default = {DirectoryTree}
        /// </summary>
        public string ExplorerKey { get; set; }

        /// <summary>
        /// Point to entry (IEntryModel) to expand, Default = {Directory}
        /// </summary>
        public string DirectoryEntryKey { get; set; }

        /// <summary>
        /// Whether to expand the selected entry or collapse, Default = Expand
        /// </summary>
        public DirectoryTreeToggleMode ToggleMode { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DirectoryTreeToggleNode>();

        public DirectoryTreeToggleNode()
            : base("DirectoryTreeToggleExpand")
        {
            ExplorerKey = "{DirectoryTree}";
            DirectoryEntryKey = "{Directory}";
            ToggleMode = DirectoryTreeToggleMode.Expand;
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IExplorerViewModel evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);
            IDirectoryTreeViewModel dvm = evm != null ? evm.DirectoryTree :
                pm.GetValue<IDirectoryTreeViewModel>(ExplorerKey);
            IEntryModel em = pm.GetValue<IEntryModel>(DirectoryEntryKey);

            if (dvm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));
            if (em == null)
                return ResultCommand.Error(new KeyNotFoundException(DirectoryEntryKey));

            switch (ToggleMode)
            {
                case DirectoryTreeToggleMode.Expand:
                    await dvm.Selection.LookupAsync(em,
                            RecrusiveSearch<IDirectoryNodeViewModel, IEntryModel>.LoadSubentriesIfNotLoaded,
                            SetCollapsed<IDirectoryNodeViewModel, IEntryModel>.WhenNotRelated,
                            SetExpanded<IDirectoryNodeViewModel, IEntryModel>.WhenChildSelected,
                            SetExpanded<IDirectoryNodeViewModel, IEntryModel>.WhenSelected,
                            SetBringIntoView.WhenSelected);
                    break;
                case DirectoryTreeToggleMode.Collapse:
                    await dvm.Selection.LookupAsync(em,
                            RecrusiveSearch<IDirectoryNodeViewModel, IEntryModel>.LoadSubentriesIfNotLoaded,
                            SetCollapsed<IDirectoryNodeViewModel, IEntryModel>.WhenChildSelected);
                    break;

                default: return ResultCommand.Error(new NotSupportedException(ToggleMode.ToString()));
            }

            logger.Info(String.Format("{0} {1}", ToggleMode, em));

            return NextCommand;
        }

    }
}
