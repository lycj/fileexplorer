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
        /// Serializable, close a tab.
        /// </summary>
        /// <param name="tabbedExplorerVariable"></param>
        /// <param name="tabVariable"></param>
        /// <returns></returns>
        public static IScriptCommand TabExplorerCloseTab(string tabbedExplorerVariable = "{TabbedExplorer}", 
            string tabVariable = "{Explorer}")
        {
            return new TabExplorerCloseTab()
            {
                TabKey = tabVariable,
                TabbedExplorerKey = tabbedExplorerVariable
            };
        }

        /// <summary>
        /// Serializable, close a tab.
        /// </summary>
        /// <param name="tabbedExplorerVariable"></param>
        /// <returns></returns>
        public static IScriptCommand TabExplorerCloseTab(string tabbedExplorerVariable = "{TabbedExplorer}")
        {
            return new TabExplorerCloseTab()
            {
                TabKey = null,
                TabbedExplorerKey = tabbedExplorerVariable
            };
        }
    }

    public class TabExplorerCloseTab : ScriptCommandBase
    {
        /// <summary>
        /// Point to Explorer (IExplorerViewModel) to be closed. Default = "{Explorer}".
        /// </summary>
        public string TabKey { get; set; }

        /// <summary>
        /// Point to TabbedExplorer (ITabbedExplorerViewModel) to be closed.  Default = "{TabbedExplorer}".
        /// </summary>
        public string TabbedExplorerKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<TabExplorerCloseTab>();

        public TabExplorerCloseTab()
            : base("CloseExplorerTab")
        {
            TabKey = "{Explorer}";
            TabbedExplorerKey = "{TabbedExplorer}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var tevm = pm.GetValue<ITabbedExplorerViewModel>(TabbedExplorerKey);
            if (tevm == null)
                return ResultCommand.Error(new ArgumentNullException(TabbedExplorerKey));

            var evm = pm.GetValue<IExplorerViewModel>("{Parameter}") ??
                (TabKey == null ? tevm.ActiveItem as IExplorerViewModel : 
                pm.GetValue<IExplorerViewModel>(TabKey));                        
            if (tevm == null)
                return ResultCommand.Error(new ArgumentNullException(TabKey));

            logger.Info(String.Format("Closing {0}", evm.CurrentDirectory.DisplayName));
            tevm.CloseTab(evm); 
            return ResultCommand.NoError;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return pm.HasValue(TabbedExplorerKey);
        }
    }
}
