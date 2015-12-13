using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    /// <summary>
    /// Initialize a view model
    /// </summary>
    public interface IViewModelInitializer<VM>
    {
        Task InitalizeAsync(VM viewModel);
    }

    public class ViewModelInitializer<VM> : IViewModelInitializer<VM>
    {
        private Action<VM> _initAction;
        public ViewModelInitializer(Action<VM> initAction)
        {
            _initAction = initAction;
        }
        public async Task InitalizeAsync(VM viewModel)
        {
            _initAction(viewModel);
        }
    }
}
