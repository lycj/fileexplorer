using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestTemplate.WPF
{

    public class TabControlViewModel : NotifyPropertyChanged, ISupportDragHelper, 
        ITabControlViewModel<TabViewModel>
    {
        public TabControlViewModel(int numberOfTabs = 20)
        {
            Items = new ObservableCollection<TabViewModel>();
            for (int i = 0; i < numberOfTabs; i++)
                Items.Add(new TabViewModel(this, String.Format("Tab {0}", i)));
            DragHelper = new TabControlDragHelper<TabViewModel>(this);
        }

        public ObservableCollection<TabViewModel> Items { get; set; }
        private int _selectedIdx = 0;
        public int SelectedIndex
        {
            get { return _selectedIdx; }
            set { _selectedIdx = value; NotifyOfPropertyChanged(() => SelectedIndex); }
        }

        public ISupportDrag DragHelper
        {
            get;
            set;
        }

        public int GetTabIndex(TabViewModel evm)
        {
            return Items.IndexOf(evm);
        }

        public void MoveTab(int srcIdx, int targetIdx)
        {
            Items.Move(srcIdx, targetIdx);
        }

        public void ActivateItem(object item)
        {
            int idx = (item is TabViewModel) ? GetTabIndex(item as TabViewModel) : -1;
            if (idx != -1)
                SelectedIndex = idx;
        }




        public TabViewModel SelectedItem
        {
            get
            {
                return Items[SelectedIndex];
            }
            set
            {
                ActivateItem(value);
            }
        }
    }

   





    
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
