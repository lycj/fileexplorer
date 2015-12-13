using Caliburn.Micro;
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
        /// Serializable, Create a new Explorer IExplorerViewModel instance in ParameterDic, but does not show it.
        /// </summary>
        /// <param name="explorerMode"></param>
        /// <param name="onModelCreatedVariable"></param>
        /// <param name="onViewAttachedVariable"></param>
        /// <param name="windowManagerVariable"></param>
        /// <param name="eventAggregatorVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerCreate(ExplorerMode explorerMode,
            string onModelCreatedVariable = "{OnModelCreated}", string onViewAttachedVariable = "{OnViewAttached}",
            string windowManagerVariable = "{WindowManager}", string eventAggregatorVariable = "{GlobalEvents}",
            string destinationVariable = "{Explorer}", IScriptCommand nextCommand = null
            )
        {
            return new ExplorerCreate()
            {
                ExplorerMode = explorerMode,
                OnModelCreatedKey = onModelCreatedVariable,
                OnViewAttachedKey = onViewAttachedVariable,
                WindowManagerKey = windowManagerVariable,
                EventAggregatorKey = eventAggregatorVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
       
        
    }

    public enum ExplorerMode { Normal, FileOpen, FileSave, DirectoryOpen }

    public class ExplorerCreate : ScriptCommandBase
    {
        /// <summary>
        /// IScriptCommand to run when the Explorer model is created.
        /// </summary>
        public string OnModelCreatedKey { get; set; }

        /// <summary>
        /// IScriptCommand to run when Explorer view is attached to Explorer model. (UI commands)
        /// </summary>
        public string OnViewAttachedKey { get; set; }

        public ExplorerMode ExplorerMode { get; set; }

        /// <summary>
        /// WindowManager used to show the window, optional, Default={WindowManager}
        /// </summary>
        public string WindowManagerKey { get; set; }

        /// <summary>
        /// Global Event Aggregator, Default={GlobalEvents}
        /// </summary>
        public string EventAggregatorKey { get; set; }

        /// <summary>
        /// Output the created IExplorerViewModel, default={Explorer}
        /// </summary>
        public string DestinationKey { get; set; }        

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerCreate>();

        public ExplorerCreate()
            : base("ExplorerCreate")
        {
            WindowManagerKey = "{WindowManager}";
            EventAggregatorKey = "{GlobalEvents}";
            ExplorerMode = Script.ExplorerMode.Normal;
            OnModelCreatedKey = "{OnModelCreated}";
            OnViewAttachedKey = "{OnViewAttached}";
            DestinationKey = "{Explorer}";            
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IWindowManager wm = pm.GetValue<IWindowManager>(WindowManagerKey) ?? new WindowManager();
            IEventAggregator events = pm.GetValue<IEventAggregator>(EventAggregatorKey) ?? new EventAggregator();

            IExplorerInitializer initializer = new ScriptCommandInitializer()
             {
                 StartupParameters = pm,
                 WindowManager = wm,
                 Events = events,
                 OnModelCreated = ScriptCommands.Run(OnModelCreatedKey),
                 OnViewAttached = ScriptCommands.Run(OnViewAttachedKey)
             };

            ExplorerViewModel evm = null;
            switch (ExplorerMode)
            {
                case Script.ExplorerMode.Normal:
                    evm = new ExplorerViewModel(wm, events) { Initializer = initializer };
                    break;
                case Script.ExplorerMode.FileOpen:
                    evm = new FilePickerViewModel(wm, events)
                    {
                        Initializer = initializer,
                        PickerMode = FilePickerMode.Open
                    };
                    break;
                case Script.ExplorerMode.FileSave:
                    evm = new FilePickerViewModel(wm, events)
                    {
                        Initializer = initializer,
                        PickerMode = FilePickerMode.Save
                    };
                    break;
                case Script.ExplorerMode.DirectoryOpen:
                    evm = new DirectoryPickerViewModel(wm, events)
                        {
                            Initializer = initializer
                        };
                    break;
                default:
                    return ResultCommand.Error(new NotSupportedException(ExplorerMode.ToString()));
            }

            logger.Info(String.Format("Creating {0}", evm));
            pm.SetValue(DestinationKey, evm, false);          

            return NextCommand;
        }
    }
}
