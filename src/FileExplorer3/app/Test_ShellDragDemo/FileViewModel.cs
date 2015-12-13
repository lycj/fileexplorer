using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_ShellDragDemo
{
    public class FileViewModel : NotifyPropertyChanged, IDraggable, ISelectable
    {
        public FileViewModel(FileModel model)
        {
            Model = model;

        }

        public FileViewModel(string fileName)
            : this(new FileModel(fileName))
        {
            
        }

        public FileModel Model { get; set; }
        public string DisplayName { get { return Path.GetFileName(Model.FileName); } }
        
        private bool _isDragging = false;
        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; NotifyOfPropertyChanged(() => IsDragging); }
        }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyOfPropertyChanged(() => IsSelected); }
        }

        private bool _isSelecting = false;
        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { _isSelecting = value; NotifyOfPropertyChanged(() => IsSelecting); }
        }

        public override bool Equals(object obj)
        {
            return obj is FileViewModel && (obj as FileViewModel).Model.Equals(Model);
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public override string ToString()
        {
            return "FVM - " + DisplayName;
        }
    }
}
