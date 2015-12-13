using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test_TabDragDemo
{    
    public class TabViewModel : NotifyPropertyChanged, IDraggable, ISelectable, ISupportDropHelper
    {
        public TabViewModel(TabControlViewModel tcvm, string header)
        {
            _tcvm = tcvm;
            Header = header;
            DropHelper = new TabDropHelper<TabViewModel>(this, tcvm);
        }

        public string Header { get; set; }
        public string DisplayName
        {
            get { return Header; }
        }


        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                _isDragging = value;
                NotifyOfPropertyChanged(() => IsDragging);
                NotifyOfPropertyChanged(() => HeaderOpacity);
            }
        }



        private TabControlViewModel _tcvm;
        private bool _isDragging = false;
        private bool _isSelected = false;
        private bool _isSelecting = false;


        public ISupportDrop DropHelper
        {
            get;
            set;
        }

        public float HeaderOpacity { get { return _isDragging ? 0.5f : 1f; } }
        
        public override string ToString()
        {
            return Header;
        }




        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChanged(() => IsSelected);                
            }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set
            {
                _isSelecting = value;
                NotifyOfPropertyChanged(() => IsSelecting);
            }
        }
    }
}
