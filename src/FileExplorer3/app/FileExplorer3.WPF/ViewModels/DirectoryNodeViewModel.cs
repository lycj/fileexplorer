using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Actions;
using FileExplorer.WPF.ViewModels.Helpers;
using System.Windows;
using System.Threading;
using FileExplorer.Models;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{

    public class DirectoryNodeViewModel : EntryViewModel, IDirectoryNodeViewModel, ISupportDropHelper
    {
        public enum NodeState { IsCreated, IsLoading, IsLoaded, IsError, IsInvalid }

        #region Cosntructor

        #region DirectoryNodeDropHelper
        internal class DirectoryNodeDropHelper : LambdaShellDropHelper<IEntryModel>
        {
            private static IEnumerable<IEntryModel> dataObjectFunc(IDataObject da,
                ITreeSelector<IDirectoryNodeViewModel, IEntryModel> selection)
            {
                var profiles = selection.RootSelector.EntryHelper.All.Select(rvm => rvm.EntryModel.Profile);
                foreach (var p in profiles)
                {
                    var retVal = p.DragDrop.GetEntryModels(da);
                    if (retVal != null)
                        return retVal;
                }
                return null;
            }
         
            public DirectoryNodeDropHelper(IEntryModel curDir, IEntriesHelper<IDirectoryNodeViewModel> entries,
                ITreeSelector<IDirectoryNodeViewModel, IEntryModel> selection)
                : base(
                 new LambdaValueConverter<IEntryViewModel, IEntryModel>(
                    (evm) => evm.EntryModel,
                    (em) => EntryViewModel.FromEntryModel(em)),

                new LambdaValueConverter<IEnumerable<IEntryModel>, IDataObject>(
                        ems => curDir.Profile.DragDrop.GetDataObject(ems),
                        da => dataObjectFunc(da, selection)), 

                (ems, eff) => curDir.Profile.DragDrop.QueryDrop(ems, curDir, eff),                                
                (ems, da, eff) => curDir.Profile.DragDrop.OnDropCompleted(ems, da, curDir, eff))
            {
                
            }
        }
        #endregion

        public static IDirectoryNodeViewModel DummyNode = new DirectoryNodeViewModel();

        /// <summary>
        /// For dummy node.
        /// </summary>
        private DirectoryNodeViewModel() 
            : base()
        {

        }

        /// <summary>
        /// For displaying contents only (e.g. DragAdorner).
        /// </summary>
        /// <param name="curDirModel"></param>
        public DirectoryNodeViewModel(IEntryModel curDirModel) 
            : base(curDirModel)
        {
            
        }

        public DirectoryNodeViewModel(IEventAggregator events, IDirectoryTreeViewModel rootModel, IEntryModel curDirModel,
            IDirectoryNodeViewModel parentModel)
            : base(curDirModel)
        {

            _events = events;
            _rootModel = rootModel;

            Entries = new EntriesHelper<IDirectoryNodeViewModel>(loadEntriesTask) { ClearBeforeLoad = true };
            Selection = new TreeSelector<IDirectoryNodeViewModel, IEntryModel>(curDirModel, this, 
                parentModel == null ? rootModel.Selection : parentModel.Selection, Entries);            
            Selection.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "IsSelected")
                        IsSelected = Selection.IsSelected;
                };
            DropHelper = new DirectoryNodeDropHelper(curDirModel, Entries, Selection) { DisplayName = curDirModel.Label };
        }



        #endregion

        #region Methods

        async Task<IEnumerable<IDirectoryNodeViewModel>> loadEntriesTask(bool refresh)
        {
            IEntryModel currentDir = Selection.Value;
            var subDir = await currentDir.Profile.ListAsync(currentDir, CancellationToken.None, em => em.IsDirectory, refresh);
            return subDir.Select(s => CreateSubmodel(s));
        }

        public IDirectoryNodeViewModel CreateSubmodel(IEntryModel entryModel)
        {
            return new DirectoryNodeViewModel(_events, _rootModel, entryModel, this);
        }
          

        #endregion

        #region Data

        IEventAggregator _events;
        bool _showCaption = true; 
        bool _isBringIntoView;
        private IDirectoryTreeViewModel _rootModel;

        #endregion

        #region Public Properties

        public bool ShowCaption { get { return _showCaption; } set { _showCaption = value; NotifyOfPropertyChange(() => ShowCaption); } }        
        /// <summary>
        /// Bind to TreeViewItemEx.IsBringIntoView, when true, Call tvItem.BringIntoView().
        /// </summary>
        public bool IsBringIntoView { get { return _isBringIntoView; } set { _isBringIntoView = value; NotifyOfPropertyChange(() => IsBringIntoView); } }
        public ITreeSelector<IDirectoryNodeViewModel, IEntryModel> Selection { get; set; }
        public IEntriesHelper<IDirectoryNodeViewModel> Entries { get; set; }
        public ISupportDrop DropHelper { get; set; }


        #endregion










     
    }
}
