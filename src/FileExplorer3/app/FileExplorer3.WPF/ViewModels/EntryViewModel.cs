using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINRT
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Threading;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.ViewModels
{

    public class EntryViewModel : ViewAware, IEntryViewModel
    {
        #region Cosntructor

        public static IEntryViewModel DummyNode = new EntryViewModel() { EntryModel = EntryModelBase.DummyModel };

        protected EntryViewModel()
        {

        }

        protected EntryViewModel(IEntryModel model)
        {
            EntryModel = model;
            _iconExtractSequences = model.Profile.GetIconExtractSequence(model);
            IsRenamable = model.IsRenamable;
        }

        public static EntryViewModel FromEntryModel(IEntryModel model)
        {
            return new EntryViewModel(model);

        }

        public IEntryViewModel Clone()
        {
            return EntryViewModel.FromEntryModel(this.EntryModel);
        }

        #endregion



        #region Methods

        private void loadIcon()
        {


            //var icon = AsyncUtils.RunSync(() => _iconExtractSequences.Last().GetIconForModelAsync(EntryModel, CancellationToken.None));
            //Icon = icon;

            Task loadIconTask =
                Task.Run<BitmapSource>(async () =>
                {
                    var sequence = _iconExtractSequences.ToList();
                    byte[] bytes = await _iconExtractSequences.Last()
                        .GetIconBytesForModelAsync(EntryModel, CancellationToken.None);
                    if (bytes != null && bytes.Length > 0)
                        return BitmapSourceUtils.CreateBitmapSourceFromBitmap(bytes);
                    else return null;
                })

                .ContinueWith((tsk) =>
                    {
                        if (tsk.IsCompleted && !tsk.IsFaulted && tsk.Result != null)
                        {
                            Icon = tsk.Result;
                        };
                    }, TaskScheduler.FromCurrentSynchronizationContext()
                    );

            //for (int i = 1; i < sequence.Count - 1; i++)
            //{
            //    loadIconTask = loadIconTask.ContinueWith<ImageSource>(
            //        (tsk) => AsyncUtils.RunSync(() => sequence[i].GetIconForModelAsync(EntryModel)))
            //        .ContinueWith((tsk) =>
            //        {
            //            if (tsk.IsCompleted && !tsk.IsFaulted && tsk.Result != null)
            //                Icon = tsk.Result;
            //        }, TaskScheduler.FromCurrentSynchronizationContext()
            //        );
            //}

        }

        //private async Task loadIcon()
        //{




        //    Action<Task<ImageSource>> updateIcon = (tsk) =>
        //        {
        //            if (tsk.IsCompleted && !tsk.IsFaulted && tsk.Result != null)
        //                Icon = tsk.Result;
        //        };

        //    foreach (var ext in _iconExtractSequences)
        //    {
        //        await ext.GetIconForModelAsync(EntryModel).ContinueWith(updateIcon);                
        //    }
        //}

        public override bool Equals(object obj)
        {
            return
                obj is EntryViewModel &&
                (obj as EntryViewModel).EntryModel.FullPath.Equals(this.EntryModel.FullPath);
                //this.EntryModel.Profile.HierarchyComparer
                //.CompareHierarchy(this.EntryModel, (obj as EntryViewModel).EntryModel)
                //== FileExplorer.Defines.HierarchicalResult.Current;
        }

        public override string ToString()
        {
            return "evm-" + this.EntryModel.ToString();
        }

        #endregion

        #region Data

        bool _isSelected = false, _isRenaming = false, _isRenamable = false, _isIconLoaded = false;
        private ImageSource _icon = null;
        private IEnumerable<IModelIconExtractor<IEntryModel>> _iconExtractSequences;
        private bool _isDragging = false;
        private bool _isSelecting = false;

        #endregion

        #region Public Properties

        public string DisplayName { get { return EntryModel.Label; } }

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set
            {
                _isRenaming = value;
                NotifyOfPropertyChange(() => EntryModel);
                NotifyOfPropertyChange(() => IsRenaming);
            }
        }
        public bool IsRenamable
        {
            get { return _isRenamable; }
            set
            {
                _isRenamable = value;
                NotifyOfPropertyChange(() => IsRenamable);

            }
        }

        public IEntryModel EntryModel { get; private set; }
        public IEntryModel Model { get { return EntryModel; } }

        public ImageSource Icon
        {
            get
            {
                if (!_isIconLoaded)
                {
                    _isIconLoaded = true;
                    loadIcon();
                }
                return _icon;
            }
            set { _icon = value; NotifyOfPropertyChange(() => Icon); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value) { _isSelected = value; NotifyOfPropertyChange(() => IsSelected); }
            }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set
            {
                if (_isSelecting != value) { _isSelecting = value; NotifyOfPropertyChange(() => IsSelecting); }
            }
        }

        public bool IsDragging { get { return _isDragging; } set { _isDragging = value; NotifyOfPropertyChange(() => IsDragging); } }

        #endregion
    }

}
