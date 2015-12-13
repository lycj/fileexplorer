using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.WPF.Models;
using FileExplorer.WPF;
using FileExplorer.Defines;
using FileExplorer.IO;

namespace TestApp
{
    

    public class MdiWindowInitializers : IViewModelInitializer<IExplorerViewModel>
    {

        private IExplorerInitializer _initializer;
        private WPF.MDI.MdiContainer _container;
        public MdiWindowInitializers(IExplorerInitializer initializer, WPF.MDI.MdiContainer container)
        {
            _container = container;
            _initializer = initializer;
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            explorerModel.DirectoryTree.Commands.CommandDictionary.NewWindow =
              new TestApp.Script.OpenInNewMdiWindowV1(_container, _initializer, FileExplorer.Script.WPFExtensionMethods.GetCurrentDirectoryFunc);

            explorerModel.FileList.Commands.CommandDictionary.NewWindow =
                new TestApp.Script.OpenInNewMdiWindowV1(_container, _initializer, FileExplorer.Script.WPFExtensionMethods.GetFileListSelectionFunc);
        }
    }

    
}


