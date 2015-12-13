using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_NonShellDragDemo
{
    public class NumberViewModel : NotifyPropertyChanged, IDraggable, ISelectable
    {


        public NumberViewModel(NumberModel model)
        {
            Model = model;
            DisplayName = model.Value.ToString();
        }

        public NumberViewModel(int i)
            : this(new NumberModel(i))
        {

        }
        


        #region Methods

        public override bool Equals(object obj)
        {
            return obj is NumberViewModel && (obj as NumberViewModel).Model.Equals(this.Model);
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        #endregion

        #region Data

        private bool _isDragging = false;
        private bool _isSelected = false;
        private bool _isSelecting = false;

        #endregion

        #region Public Properties

        public NumberModel Model
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public bool IsDragging { get { return _isDragging; } set { _isDragging = value; NotifyOfPropertyChanged(() => IsDragging); } }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyOfPropertyChanged(() => IsSelected); }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { _isSelecting = value; NotifyOfPropertyChanged(() => IsSelecting); }
        }
        #endregion

    }
}
