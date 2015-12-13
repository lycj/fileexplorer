using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.Models;
using System.ComponentModel.Composition;

namespace FileExplorer.WPF.ViewModels
{
    [Export(typeof(IExplorerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DirectoryPickerViewModel : ExplorerViewModel
    {
        #region Constructor

        private void init()
        {
            FileList.Selection.SelectionChanged += (o, e) =>
            {
                int selectedCount = FileList.Selection.SelectedItems.Count();
                CanOpen =
                    selectedCount == 0 ||
                    (selectedCount == 1 && FileList.Selection.SelectedItems[0].EntryModel.IsDirectory);
            };
            FileList.EnableDrag = false;
            FileList.EnableDrop = false;
            FileList.EnableMultiSelect = false;
        }

        public DirectoryPickerViewModel(IExplorerInitializer initializer)
            : base(initializer)
        {
            init();
        }

        [ImportingConstructor]
        public DirectoryPickerViewModel(IWindowManager windowManager, IEventAggregator events)
            : base(windowManager, events)
        {
            init();
        }

        public DirectoryPickerViewModel(IEventAggregator events, IWindowManager windowManager, params IEntryModel[] rootModels)
            : this(new ExplorerInitializer(windowManager, events, rootModels))
        {

        }

        #endregion

        #region Methods

        public void Open()
        {
            if (FileList.Selection.SelectedItems.Count() == 1)
                SelectedDirectory = FileList.Selection.SelectedItems[0].EntryModel;
            else SelectedDirectory = FileList.CurrentDirectory;

            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

        #endregion

        #region Data

        private static ColumnFilter _directoryOnlyFilter =
             ColumnFilter.CreateNew<IEntryModel>("DirectoryOnly", "IsDirectory", evm => evm.IsDirectory);
        IEntryModel _selectedDirectory;
        bool _canOpen = false;

        #endregion

        #region Public Properties

        public bool CanOpen { get { return _canOpen; } set { _canOpen = value; NotifyOfPropertyChange(() => CanOpen); } }
        public IEntryModel SelectedDirectory
        {
            get { return _selectedDirectory; }
            set { _selectedDirectory = value; NotifyOfPropertyChange(() => SelectedDirectory); }
        }

        #endregion

    }
}
