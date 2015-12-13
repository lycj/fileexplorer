using FileExplorer.Defines;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    /// <summary>
    /// Implemented by TabControl's ViewModel to handle Drop.
    /// </summary>
    /// <typeparam name="T">Tab View Model</typeparam>
    public class TabDropHelper<T> : ShellDropHelper<T>
        where T : class, IDraggable, ISelectable
    {
        #region Constructors

        public TabDropHelper(T tvm, ITabControlViewModel<T> tcvm)
            : base(LambdaValueConverter.ConvertUsingCast<IDraggable, T>())
        {
            _tvm = tvm;
            _tcvm = tcvm;
            this.DropTargetLabel = "{ISupportDrop.DisplayName}";
            this.DisplayName = _tvm.DisplayName;
        }

        #endregion

        #region Methods

        public override IEnumerable<T> QueryDropModels(IDataObject da)
        {
            if (da.GetDataPresent(typeof(IEnumerable<T>)))
                return da.GetData(typeof(IEnumerable<T>)) as IEnumerable<T> ?? new List<T>();
            return new List<T>();
        }

        public override QueryDropEffects QueryDrop(IEnumerable<T> models, DragDropEffectsEx allowedEffects)
        {
            if (models.Count() > 0)
                return QueryDropEffects.CreateNew(DragDropEffectsEx.Move);

            _tcvm.SelectedItem = _tvm;
            return QueryDropEffects.None;
        }        

        public override DragDropEffectsEx Drop(IEnumerable<IDraggable> sourceDraggable, IDataObject da, DragDropEffectsEx allowedEffects)
        {
            var sourceVM = sourceDraggable.FirstOrDefault() as T;
            if (sourceVM != null)
            {
                int thisIdx = _tcvm.GetTabIndex(_tvm);
                int srcIdx = _tcvm.GetTabIndex(sourceVM);
                if (srcIdx != -1 && thisIdx != -1)
                {
                    int destIdx = thisIdx > srcIdx ? thisIdx - 1 : thisIdx;
                    if (destIdx != -1 && srcIdx != destIdx)
                    {
                        _tcvm.MoveTab(srcIdx, destIdx);
                        return DragDropEffectsEx.Move;
                    }
                }
            }
            return DragDropEffectsEx.None;
        }

        public override void OnDragOver()
        {
            base.OnDragOver();
            NotifyOfPropertyChanged(() => ShowPlaceHolder);
        }

        public override void OnDragLeave()
        {
            base.OnDragOver();
            NotifyOfPropertyChanged(() => ShowPlaceHolder);
        }

        #endregion

        #region Data

        private ITabControlViewModel<T> _tcvm;
        private T _tvm;


        #endregion

        #region Public Properties

        public bool ShowPlaceHolder
        {
            get { return !_tvm.IsDragging && IsDraggingOver; }
        }


        #endregion
    }
}
