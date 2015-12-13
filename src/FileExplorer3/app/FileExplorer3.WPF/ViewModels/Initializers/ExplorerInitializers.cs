using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public static class ExplorerInitializers
    {
        public static IViewModelInitializer<IExplorerViewModel> StartupDirectory(IEntryModel startupDir)
        { return new StartupDirInitializer(startupDir); }

        public static IViewModelInitializer<IExplorerViewModel> Parameter(IExplorerParameters parameter)
        { return Do(evm => { evm.Parameters = parameter; }); }

        public static IViewModelInitializer<IExplorerViewModel> Parameter(IFileListParameters parameter)
        { return Do(evm => { evm.FileList.Parameters = parameter; }); }

        //public static IViewModelInitializer<IExplorerViewModel> ViewMode(string viewMode, int itemSize)
        //{ return Do(evm => { evm.FileList.Parameters.ViewMode = viewMode; evm.FileList.Parameters.ItemSize = itemSize; }); }


        public static IViewModelInitializer<IExplorerViewModel> Do(Action<IExplorerViewModel> action)
        {
            return new DoViewModelInitializer(action);
        }

        public static IViewModelInitializer<IExplorerViewModel> DoAsync(Func<IExplorerViewModel, Task> action)
        {
            return new DoAsyncViewModelInitializer(action);
        }

    }

    internal class StartupDirInitializer : IViewModelInitializer<IExplorerViewModel>
    {
        private IEntryModel _entryModel;
        public StartupDirInitializer(IEntryModel entryModel)
        {
            _entryModel = entryModel;
        }

        public async Task InitalizeAsync(IExplorerViewModel viewModel)
        {
            if (viewModel != null)
                await viewModel.GoAsync(_entryModel);
        }


    }

    public class DoViewModelInitializer : IViewModelInitializer<IExplorerViewModel>
    {
        private Action<IExplorerViewModel> _action;

        public DoViewModelInitializer(Action<IExplorerViewModel> action)
        {
            _action = action;
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            _action(explorerModel);
        }
    }

    public class DoAsyncViewModelInitializer : IViewModelInitializer<IExplorerViewModel>
    {
        private Func<IExplorerViewModel, Task> _action;

        public DoAsyncViewModelInitializer(Func<IExplorerViewModel, Task> action)
        {
            _action = action;
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            await _action(explorerModel);
        }
    }


}
