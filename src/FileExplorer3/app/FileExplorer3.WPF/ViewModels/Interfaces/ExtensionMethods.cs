using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.ViewModels;

namespace FileExplorer.WPF
{
    public static partial class ExtensionMethods
    {
        public static async Task InitalizeAsync<VM>(this List<IViewModelInitializer<VM>> initializers, VM viewModel)
        {
            foreach (var i in initializers)
                await i.InitalizeAsync(viewModel);
        }
    }
}
