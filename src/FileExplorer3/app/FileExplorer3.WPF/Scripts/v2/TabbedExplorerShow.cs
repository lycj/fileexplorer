using Caliburn.Micro;
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
        /// Serializable, Create a new tab explorer window (IExplorerViewModel), and show it.
        /// </summary>
        /// <param name="onModelCreatedVariable"></param>
        /// <param name="onViewAttachedVariable"></param>
        /// <param name="onTabExplorerCreatedVariable"></param>
        /// <param name="onTabExplorerAttachedVariable"></param>
        /// <param name="windowManagerVariable"></param>
        /// <param name="eventAggregatorVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerNewTabWindow(
            string onModelCreatedVariable = "{OnModelCreated}", string onViewAttachedVariable = "{OnViewAttached}",
            string onTabExplorerCreatedVariable = "{OnTabExplorerCreated}", string onTabExplorerAttachedVariable = "{OnTabExplorerAttached}",
            string windowManagerVariable = "{WindowManager}", string eventAggregatorVariable = "{GlobalEvents}",
            string destinationVariable = "{TabbedExplorer}", IScriptCommand nextCommand = null)
        {
            return new TabbedExplorerShow()
            {
                OnModelCreatedKey = onModelCreatedVariable,
                OnViewAttachedKey = onViewAttachedVariable,
                OnTabExplorerCreatedKey = onTabExplorerCreatedVariable,
                OnTabExplorerAttachedKey = onTabExplorerAttachedVariable,
                WindowManagerKey = windowManagerVariable,
                EventAggregatorKey = eventAggregatorVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    public class TabbedExplorerShow : ScriptCommandBase
    {
        /// <summary>
        /// IScriptCommand to run when the Explorer model is created.
        /// </summary>
        public string OnModelCreatedKey { get; set; }

        /// <summary>
        /// IScriptCommand to run when Explorer view is attached to Explorer model. (UI commands)
        /// </summary>
        public string OnViewAttachedKey { get; set; }

        /// <summary>
        /// IScriptCommand to run when the TabbedExplorer model is created.
        /// </summary>
        public string OnTabExplorerCreatedKey { get; set; }

        /// <summary>
        /// IScriptCommand to run when TabbedExplorer view is attached to TabbedExplorer model. (UI commands)
        /// </summary>
        public string OnTabExplorerAttachedKey { get; set; }

        /// <summary>
        /// WindowManager used to show the window, optional, Default={WindowManager}
        /// </summary>
        public string WindowManagerKey { get; set; }

        /// <summary>
        /// Global Event Aggregator, Default={GlobalEvents}
        /// </summary>
        public string EventAggregatorKey { get; set; }


        /// <summary>
        /// Output the ITabbedExplorerViewModel, Default= {TabbedExplorer}
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<TabbedExplorerShow>();

        public TabbedExplorerShow()
            : base("TabbedExplorerShow")
        {
            WindowManagerKey = "{WindowManager}";
            EventAggregatorKey = "{GlobalEvents}";
            OnModelCreatedKey = "{OnModelCreated}";
            OnTabExplorerCreatedKey = "{OnTabExplorerCreated}";
            OnViewAttachedKey = "{OnViewAttached}";
            OnTabExplorerAttachedKey = "{OnTabExplorerAttached}";
            DestinationKey = "{TabbedExplorer}";
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IWindowManager wm = pm.GetValue<IWindowManager>(WindowManagerKey) ?? new WindowManager();
            IEventAggregator events = pm.GetValue<IEventAggregator>(EventAggregatorKey) ?? new EventAggregator();


            TabbedExplorerViewModel tevm = new TabbedExplorerViewModel(wm, events);
            pm.SetValue(DestinationKey, tevm);
            tevm.Initializer = new ScriptCommandInitializer()
            {
                StartupParameters = pm,
                WindowManager = wm,
                Events = events,
                OnModelCreated = ScriptCommands.Run(OnModelCreatedKey),
                OnViewAttached = ScriptCommands.Run(OnViewAttachedKey)
            };

            if (pm.HasValue(OnTabExplorerCreatedKey))
                await tevm.Commands.ExecuteAsync(pm.GetValue<IScriptCommand>(OnTabExplorerCreatedKey));
            tevm.OnTabExplorerAttachedKey = OnTabExplorerAttachedKey;

            object enableTabsWhenOneTab = pm.GetValue("{EnableTabsWhenOneTab}");
            tevm.EnableTabsWhenOneTab = !(enableTabsWhenOneTab is bool) || (bool)enableTabsWhenOneTab;
            logger.Info(String.Format("Showing {0}", tevm));
            wm.ShowWindow(tevm);


            return NextCommand;
        }

    }
}
