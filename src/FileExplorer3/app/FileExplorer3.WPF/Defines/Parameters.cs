using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.Defines
{
    public interface IParameters : INotifyPropertyChanged
    {
        IExplorerParameters Explorer { get; set; }
        IFileListParameters FileList { get; set; }
    }

    public interface IExplorerParameters : INotifyPropertyChanged
    {
        /// <summary>
        /// Default is 1.
        /// </summary>
        float UIScale { get; set; }

        /// <summary>
        /// In grid length format (e.g. 2*)
        /// </summary>
        string FileListSize { get; set; }

        /// <summary>
        /// In grid length format (e.g. *)
        /// </summary>
        string DirectoryTreeSize { get; set; }

        /// <summary>
        /// Actual pixel size (default 30)
        /// </summary>
        int NavigationSize { get; set; }

        /// <summary>
        /// Actual pixel size (default 30)
        /// </summary>
        int StatusbarSize { get; set; }
        //float SidebarSize { get; set; }
        int Width { get; set; }

        int Height { get; set; }

        Point Position { get; set; }
    }


    public interface IFileListParameters : INotifyPropertyChanged
    {
        int ItemSize { get; set; }
        string ViewMode { get; set; }
    }

    public class Parameters : NotifyPropertyChanged, IParameters
    {
        public Parameters()
        {
            Explorer = new ExplorerParameters();
            FileList = new FileListParameters();
        }

        public IExplorerParameters Explorer { get; set; }
        public IFileListParameters FileList { get; set; }
    }

    public class ExplorerParameters : NotifyPropertyChanged, IExplorerParameters
    {
        private float _uiScale = 1f;
        private string _directoryTreeSize = "*";
        private string _fileListSize = "2*";
        private int _navigationSize = 30;
        private int _statusbarSize = 30;
        private int _width = 800;
        private int _height = 500;
        private Point _position = new Point(0,0);

        public float UIScale { get { return _uiScale; } set { _uiScale = value; NotifyOfPropertyChanged(() => UIScale); } }
        public string DirectoryTreeSize { get { return _directoryTreeSize; } set { _directoryTreeSize = value; NotifyOfPropertyChanged(() => DirectoryTreeSize); } }
        public string FileListSize { get { return _fileListSize; } set { _fileListSize = value; NotifyOfPropertyChanged(() => FileListSize); } }

        public int NavigationSize { get { return _navigationSize; } set { _navigationSize = value; NotifyOfPropertyChanged(() => NavigationSize); } }
        public int StatusbarSize { get { return _statusbarSize; } set { _statusbarSize = value; NotifyOfPropertyChanged(() => StatusbarSize); } }

        public int Height { get { return _height; }
            set { _height = value; NotifyOfPropertyChanged(() => Height); } }
        public int Width { get { return _width; } set { _width = value; NotifyOfPropertyChanged(() => Width); } }

        public Point Position { get { return _position; } set { _position = value; NotifyOfPropertyChanged(() => Position); } }

        //public float SidebarSize { get { return _sidebarSize; } set { _sidebarSize = value; NotifyOfPropertyChanged(() => SidebarSize); } } 
    }

    public class FileListParameters : NotifyPropertyChanged, IFileListParameters
    {
        private int _itemSize = 65;
        private string _viewMode = "Icon";
        public int ItemSize
        {
            get { return _itemSize; }
            set { _itemSize = value; NotifyOfPropertyChanged(() => ItemSize); }
        }
        public string ViewMode
        {
            get { return _viewMode; }
            set { _viewMode = value; NotifyOfPropertyChanged(() => ViewMode); }
        }
    }
}
