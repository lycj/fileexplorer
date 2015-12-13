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

   





}
