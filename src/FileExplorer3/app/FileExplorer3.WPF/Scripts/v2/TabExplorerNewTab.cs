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
        /// Serializable, open a tab.
        /// </summary>
        /// <param name="tabbedExplorerVariable"></param>
        /// <param name="directoryVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <returns></returns>
        public static IScriptCommand TabExplorerNewTab(string tabbedExplorerVariable = "{TabbedExplorer}",
            string directoryVariable = null,
            string destinationVariable = "{Explorer}", IScriptCommand nextCommand = null)
        {
            return new TabExplorerNewTab()
            {
                DirectoryEntryKey = directoryVariable,
                DestinationKey = destinationVariable,
                TabbedExplorerKey = tabbedExplorerVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    
    }

    public class TabExplorerNewTab : ScriptCommandBase
    {
        /// <summary>
        /// Point to TabbedExplorer (ITabbedExplorerViewModel) to be closed.  Default = "{TabbedExplorer}".
        /// </summary>
        public string TabbedExplorerKey { get; set; }

        /// <summary>
        /// Point to Directory (IEntryModel) to be opened.  Default = null, 
        /// if not specified root directory will be opened.
        /// </summary>
        public string DirectoryEntryKey { get; set; }

        /// <summary>
        /// Store newly created Explorer (IExplorerViewModel). Default = "{Explorer}".
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<TabExplorerNewTab>();

        public TabExplorerNewTab()
            : base("NewExplorerTab")
        {
            TabbedExplorerKey = "{TabbedExplorer}";
            DestinationKey = "{Explorer}";
            DirectoryEntryKey = null;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            DestinationKey = DestinationKey ?? "{Explorer}";
            var tevm = pm.GetValue<ITabbedExplorerViewModel>(TabbedExplorerKey);
            if (tevm == null)
                return ResultCommand.Error(new ArgumentNullException(TabbedExplorerKey));

            var dm = DirectoryEntryKey == null ? null :
                (await pm.GetValueAsEntryModelArrayAsync(DirectoryEntryKey)).FirstOrDefault();
            if (dm != null && !dm.IsDirectory)
                dm = null;

            var destTab = tevm.OpenTab(dm);
            logger.Info(String.Format("New Tab #{0}", tevm.GetTabIndex(destTab)));
            logger.Debug(String.Format("{0} = {1}", DestinationKey, destTab));
            pm.SetValue(DestinationKey, destTab);
            return NextCommand;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return pm.HasValue(TabbedExplorerKey);
        }
    }
}
