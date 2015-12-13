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
    public class BasicParamInitalizers : IViewModelInitializer<IExplorerViewModel>
    {
        private bool _expand;
        private bool _multiSelect;
        private bool _enableDrag;
        private bool _enableDrop;
        public BasicParamInitalizers(bool expand, bool multiSelect, bool enableDrag, bool enableDrop)
        {
            _expand = expand;
            _multiSelect = multiSelect;
            _enableDrag = enableDrag;
            _enableDrop = enableDrop;
        }

        public async Task InitalizeAsync(IExplorerViewModel viewModel)
        {
            viewModel.FileList.EnableDrag = _enableDrag;
            viewModel.FileList.EnableDrop = _enableDrop;
            viewModel.DirectoryTree.EnableDrag = _enableDrag;
            viewModel.DirectoryTree.EnableDrop = _enableDrop;
            viewModel.FileList.EnableMultiSelect = _multiSelect;
            if (_expand)
                viewModel.DirectoryTree.ExpandRootEntryModels();
        }
    }

}


