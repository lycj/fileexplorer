using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramingDemo
{
    public class ItemViewModel : NotifyPropertyChanged, IPositionAware, IDraggable, ISelectable, IResizable
    {

        #region Constructor

        public ItemViewModel(string displayName)
        {
            DisplayName = displayName;
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        private static Random rand = new Random();
        private bool _isDragging = false;
        private bool _isSelected;
        private bool _isSelecting;
        private double _width = rand.Next(25) + 25;
        private double _height = rand.Next(25) + 25;
        private double _top = rand.Next(500);
        private double _left = rand.Next(500);
        #endregion

        #region Public Properties


        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChanged(() => IsSelected);
            }
        }

        public bool IsSelecting
        {
            get
            {
                return _isSelecting;
            }
            set
            {
                _isSelecting = value;
                NotifyOfPropertyChanged(() => IsSelecting);
            }
        }

        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
                NotifyOfPropertyChanged(() => Left);
            }
        }

        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
                NotifyOfPropertyChanged(() => Top);
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value > 0)
                    _width = value;
                NotifyOfPropertyChanged(() => Width);
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value > 0)
                    _height = value;
                NotifyOfPropertyChanged(() => Height);
            }
        }
        public string DisplayName
        {
            get;
            private set;
        }

        public bool IsDragging
        {
            get
            {
                return _isDragging;
            }
            set
            {
                _isDragging = value;
                NotifyOfPropertyChanged(() => IsDragging);
            }
        }
        #endregion
    }
}
