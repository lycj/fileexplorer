using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public class ScriptCommandInitializer : IExplorerInitializer
    {
        public ScriptCommandInitializer()
        {
            RootModels = new FileExplorer.Models.IEntryModel[] { };
            OnModelCreated = UIScriptCommands.ExplorerDefault();
        }

        public IScriptCommand OnViewAttached { get; set; }

        public IScriptCommand OnModelCreated { get; set; }

        public ParameterDic StartupParameters { get; set; }

        public Caliburn.Micro.IEventAggregator Events
        {
            get;
            set;
        }

        public Caliburn.Micro.IWindowManager WindowManager
        {
            get;
            set;
        }

        public FileExplorer.Models.IEntryModel[] RootModels
        {
            get;
            set;
        }

        public IExplorerInitializer Clone()
        {
            return new ScriptCommandInitializer() { Events = Events, OnModelCreated = OnModelCreated, 
                OnViewAttached = OnViewAttached, RootModels = RootModels, 
                StartupParameters = StartupParameters, WindowManager = WindowManager };
        }

        public async Task InitializeModelCreatedAsync(IExplorerViewModel evm)
        {

            if (Events != null)
                evm.Events = Events;
            if (WindowManager != null)
                evm.WindowManager = WindowManager;

            if (OnModelCreated != null)
                await evm.Commands.ExecuteAsync(
                    new IScriptCommand[] {
                    OnModelCreated }
                    , StartupParameters);

            
        }

        public async Task InitializeViewAttachedAsync(IExplorerViewModel evm)
        {
            if (OnViewAttached != null)
                await evm.Commands.ExecuteAsync(
                    new IScriptCommand[] { OnViewAttached }, StartupParameters);
        }


        public List<IViewModelInitializer<IExplorerViewModel>> Initializers
        {
            get { return new List<IViewModelInitializer<IExplorerViewModel>>(); }
        }
    }
}
