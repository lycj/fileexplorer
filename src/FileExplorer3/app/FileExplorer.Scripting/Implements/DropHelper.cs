using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    #region DropHelper
    public abstract class DropHelper : NotifyPropertyChanged, ISupportDrop
    {
        #region Constructor

        public DropHelper()
        {
            IsDroppable = true;
            DropTargetLabel = "{MethodLabel} {ItemLabel} to {ISupportDrop.DisplayName}";
        }

        #endregion

        #region Methods

        public abstract QueryDropEffects QueryDrop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects);
        public abstract DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects);

        public virtual void OnDragOver()
        {

        }

        public virtual void OnDragLeave()
        {

        }

        #endregion

        #region Data

        private bool _isDraggingOver = false;

        #endregion

        #region Public Properties

        public virtual bool IsDraggingOver
        {
            get
            {
                return _isDraggingOver;
            }
            set
            {
                if (_isDraggingOver != value)
                {
                    _isDraggingOver = value;
                    if (value)
                        OnDragOver();
                    else OnDragLeave();
                    NotifyOfPropertyChanged(() => IsDraggingOver);
                }
            }
        }


        public virtual bool IsDroppable
        {
            get;
            set;
        }

        public virtual string DropTargetLabel
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        #endregion

       
    }

    #endregion

    #region NoDropHelper

    public class NoDropHelper : DropHelper
    {
        public static ISupportDrop Instance = new NoDropHelper();

        public NoDropHelper()
            : base()
        {
            IsDroppable = false;
        }

        public override QueryDropEffects QueryDrop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            return QueryDropEffects.None;
        }

        public override DragDropEffectsEx Drop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            return DragDropEffectsEx.None;
        }
    }

    #endregion

}
