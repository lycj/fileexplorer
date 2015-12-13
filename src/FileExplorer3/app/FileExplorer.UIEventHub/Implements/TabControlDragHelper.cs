using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    /// <summary>
    /// Implemented by both Tab and TabControl's ViewModel, allow re-index tab using Drag n Drop.
    /// </summary>
    /// <typeparam name="T">Tab View Model</typeparam>
    public class TabControlDragHelper<T> : ShellDragHelper<T>
       where T : class, IDraggable, ISelectable
    {
        
        #region Constructors

        public TabControlDragHelper(ITabControlViewModel<T> tcvm)            
            : base(LambdaValueConverter.ConvertUsingCast<IDraggable, T>())
        {
            _tcvm = tcvm;
            HasDraggables = true;
        }

        #endregion

        #region Methods

        public override IEnumerable<T> GetModels()
        {
            yield return _tcvm.SelectedItem;
        }

        public override IDataObject GetDataObject(IEnumerable<T> models)
        {
            return new DataObject(typeof(IEnumerable<T>), models);
        }

        public override DragDropEffectsEx QueryDrag(IEnumerable<T> models)
        {
            return DragDropEffectsEx.Move;
        }

        public override void OnDragCompleted(IEnumerable<T> models, IDataObject da, DragDropEffectsEx effect)
        {            
        }

        #endregion

        #region Data

        private ITabControlViewModel<T> _tcvm;


        #endregion

        #region Public Properties

        #endregion
    }
}
