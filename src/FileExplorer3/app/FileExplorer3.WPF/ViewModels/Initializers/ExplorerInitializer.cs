using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.WPF.Defines;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Script;

namespace FileExplorer.WPF.ViewModels
{
    public class ExplorerInitializer : IExplorerInitializer
    {
        #region Constructors

        public ExplorerInitializer(IWindowManager wm, IEventAggregator events, IEntryModel[] rootModels,
            params IViewModelInitializer<IExplorerViewModel>[] initializers)
        {
            WindowManager = wm ?? new WindowManager();
            Events = events ?? new EventAggregator();
            RootModels = rootModels;
            Initializers = new List<IViewModelInitializer<IExplorerViewModel>>(initializers);
        }

        public ExplorerInitializer(IEntryModel[] rootModels,
          params IViewModelInitializer<IExplorerViewModel>[] initializers)
            : this(null, null, rootModels, initializers)
        {

        }

        protected ExplorerInitializer(ExplorerInitializer initializer)
            : this(initializer.WindowManager, initializer.Events, initializer.RootModels,
            initializer.Initializers.ToArray())
        {

        }

        #endregion

        #region Methods

        public IExplorerInitializer Clone()
        {
            return new ExplorerInitializer(WindowManager, Events, RootModels, Initializers.ToArray());
        }

        public Task InitializeModelCreatedAsync(IExplorerViewModel evm)
        {
            return Task.Delay(0);
        }

        public async Task InitializeViewAttachedAsync(IExplorerViewModel evm)
        {
            evm.RootModels = RootModels;
            //Initializers.Add(ExplorerInitializers.StartupDirectory(RootModels.FirstOrDefault()));
            //Initializers.EnsureOneStartupDirectoryOnly();
            if (!Initializers.Any(i => i is StartupDirInitializer))
            {
                var firstDir = RootModels.FirstOrDefault();
                if (firstDir != null)
                    await evm.GoAsync(firstDir);
            }

            await Initializers.InitalizeAsync(evm);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public IEventAggregator Events { get; set; }
        public IWindowManager WindowManager { get; set; }
        public IEntryModel[] RootModels { get; set; }
        public IParameters Parameters { get; set; }
        public List<IViewModelInitializer<IExplorerViewModel>> Initializers { get; set; }

        #endregion




    }
}
