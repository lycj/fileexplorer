using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileExplorer.UIEventHub
{
    internal class LBSelectable : ISelectable
    {
        private ListBoxItem _lbItem;
        public LBSelectable(ListBoxItem lbItem)
        {
            _lbItem = lbItem;
        }
        public bool IsSelected
        {
            get
            {
                return (bool)_lbItem.GetValue(ListBoxItem.IsSelectedProperty);
            }
            set
            {
                _lbItem.SetValue(ListBoxItem.IsSelectedProperty, value);
            }
        }

        public bool IsSelecting
        {
            get
            {
                return (bool)_lbItem.GetValue(UIEventHubProperties.IsSelectingProperty);
            }
            set
            {
                _lbItem.SetValue(UIEventHubProperties.IsSelectingProperty, value);
            }
        }
    }
}
