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
    #region DragHelper
    public abstract class DragHelper : NotifyPropertyChanged, ISupportDrag
    {
        #region Constructor

        public DragHelper()
        {
            HasDraggables = true;
        }

        #endregion

        #region Methods

        public abstract IEnumerable<IDraggable> GetDraggables();
        public abstract DragDropEffectsEx QueryDrag(IEnumerable<IDraggable> draggables);
        
        public virtual void OnDragCompleted(IEnumerable<IDraggable> draggables, DragDropEffectsEx effect)
        {

        }

        public virtual void OnDragStart()
        {

        }

        public virtual void OnDragEnd()
        {

        }

        #endregion

        #region Data

        private bool _isDraggingFrom = false;

        #endregion

        #region Public Properties

        public virtual bool HasDraggables
        {
            get;
            set;
        }

        public virtual bool IsDraggingFrom
        {
            get
            {
                return _isDraggingFrom;
            }
            set
            {
                if (_isDraggingFrom != value)
                {
                    _isDraggingFrom = value;
                    if (value)
                        OnDragStart();
                    else OnDragEnd();
                    NotifyOfPropertyChanged(() => IsDraggingFrom);
                }
            }
        }

        #endregion

    }

    #endregion    
}
