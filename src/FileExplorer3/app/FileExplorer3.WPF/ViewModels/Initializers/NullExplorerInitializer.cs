using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public class NullExplorerInitializer : IExplorerInitializer
    {
        public static NullExplorerInitializer Instance = new NullExplorerInitializer();

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
            return this;
        }

        public Task InitializeModelCreatedAsync(IExplorerViewModel evm)
        {
            return Task.Delay(0);
        }

        public Task InitializeViewAttachedAsync(IExplorerViewModel evm)
        {
            return Task.Delay(0);
        }


        public List<IViewModelInitializer<IExplorerViewModel>> Initializers
        {
            get { return new List<IViewModelInitializer<IExplorerViewModel>>(); }
        }
    }
}
