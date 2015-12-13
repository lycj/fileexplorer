using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_ShellDragDemo
{
    public class SettingsViewModel : NotifyPropertyChanged
    {
        private int _dragDropMode = 0;        
        private int _multiSelectMode = 0;


        public bool EnableDragDrop { get { return _dragDropMode == 2; } }
        public bool EnableDragDropLite { get { return _dragDropMode == 1; } }        
        public int DragDropMode
        {
            get { return _dragDropMode; }
            set
            {
                _dragDropMode = value;
                NotifyOfPropertyChanged(() => DragDropMode);
                NotifyOfPropertyChanged(() => EnableDragDrop);
                NotifyOfPropertyChanged(() => EnableDragDropLite);
            }
        }

        public bool EnableMultiSelect { get { return _multiSelectMode == 1; } }
        public int MultiSelectMode
        {
            get { return _multiSelectMode; }
            set
            {
                _multiSelectMode = value;
                NotifyOfPropertyChanged(() => MultiSelectMode);
                NotifyOfPropertyChanged(() => EnableMultiSelect);                
            }
        }
    }
}
