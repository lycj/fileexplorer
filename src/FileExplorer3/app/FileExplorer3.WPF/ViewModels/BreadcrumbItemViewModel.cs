using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public class BreadcrumbItemViewModel : EntryViewModel, IBreadcrumbItemViewModel
    {
        #region Constructor

        public BreadcrumbItemViewModel(IEventAggregator events, IBreadcrumbViewModel rootModel,
            IEntryModel curDirModel, IBreadcrumbItemViewModel parentModel)
            : base(curDirModel)
        {
            _events = events;
            _rootModel = rootModel;

            Entries = new EntriesHelper<IBreadcrumbItemViewModel>(loadEntriesTask);
            Selection = new TreeSelector<IBreadcrumbItemViewModel, IEntryModel>(curDirModel, this,
                parentModel == null ? rootModel.Selection : parentModel.Selection, Entries);
        }

        #endregion

        #region Methods

        async Task<IEnumerable<IBreadcrumbItemViewModel>> loadEntriesTask(bool refresh)
        {
            IEntryModel currentDir = Selection.Value;
            var subDir = await currentDir.Profile.ListAsync(currentDir, CancellationToken.None, em => em.IsDirectory, refresh);
            return subDir.Select(s => CreateSubmodel(s));
        }

        public IBreadcrumbItemViewModel CreateSubmodel(IEntryModel entryModel)
        {
            return new BreadcrumbItemViewModel(_events, _rootModel, entryModel, this);
        }


        #endregion

        #region Data

        IEventAggregator _events;
        bool _showCaption = true; bool _isShown = false;
        private IBreadcrumbViewModel _rootModel;
        private bool _isOverflowed;


        #endregion

        #region Public Properties

        public ITreeSelector<IBreadcrumbItemViewModel, IEntryModel> Selection { get; set; }
        public IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }

        public bool IsShown
        {
            get { return _isShown; }
            set { _isShown = value; if (value) Entries.LoadAsync(UpdateMode.Replace, false); NotifyOfPropertyChange(() => IsShown); }
        }

        public bool ShowCaption
        {
            get { return _showCaption; }
            set { _showCaption = value; NotifyOfPropertyChange(() => ShowCaption); }
        }

        public bool IsOverflowedOrRoot { get { return _isOverflowed || Selection.IsRoot; } set { } }
        public bool IsOverflowed
        {
            get { return _isOverflowed; }
            set
            {
                _isOverflowed = value;
                NotifyOfPropertyChange(() => IsOverflowed);
                NotifyOfPropertyChange(() => IsOverflowedOrRoot);
            }
        }

        #endregion
    }
}
