using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using QuickZip.IO.PIDL.UserControls.Model;
using System.Diagnostics;
using vbAccelerator.Components.Shell;
using System.IO;
using System.Windows;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class FileListViewItemViewModel : ExViewModel
    {

        #region Constructor
        public FileListViewItemViewModel(FileListViewModel rootModel, Model.ExModel model)
            : base(rootModel, model)
        {
            _rootModel = rootModel;

            _expandCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => Expand(_rootModel, (ExModel)x)
            };            
        }
        #endregion

        #region Methods

        private static void Run(string path, string param)
        {
            try
            {
                Process.Start(path, param);
            }
            catch { }
        }

        public void Expand()
        {
            Expand(_rootModel, this.EmbeddedModel);
        }

        public void Expand(FileListViewModel _rootModel, ExModel dirModel)
        {
            if (_rootModel != null)
            {
                if (EmbeddedModel is DirectoryModel)
                    _rootModel.CurrentDirectory = (EmbeddedModel as DirectoryModel).EmbeddedDirectoryEntry;
                else
                {                    
                    try
                    {
                        FileSystemInfoEx entry = EmbeddedModel.EmbeddedEntry;                        
                        if (PathEx.GetExtension(entry.Name).ToLower() == ".lnk")
                            using (ShellLink sl = new ShellLink(entry.FullName))
                            {
                                string linkPath = sl.Target;
                                if (DirectoryEx.Exists(linkPath) && sl.Arguments == "")
                                    _rootModel.CurrentDirectory = FileSystemInfoEx.FromString(linkPath) as DirectoryInfoEx;
                                else Run(linkPath, sl.Arguments);
                            }
                        else
                            Run(entry.FullName, "");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, "Expand Failed");                        
                    }
                }

            }

        }

        #endregion

        #region Data

        private FileListViewModel _rootModel = null;
        private SimpleCommand _expandCommand;
        private CurrentDirectoryViewModel _curModel = null;
        private bool _isSelected = false;
        private bool _isEditing = false;

        #endregion

        #region Public Properties

        public SimpleCommand ExpandCommand { get { return _expandCommand; } }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    //Debug.WriteLine(this.EmbeddedModel.ParseName);
                    if (_rootModel.CurrentDirectoryModel != null && _isSelected != value)
                        _rootModel.CurrentDirectoryModel.NotifySelectionChanged(this, value);
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (value != _isEditing)
                {
                    _isEditing = value;
                    if (_curModel != null)
                        _curModel.NotifyIsEditingChanged(this);
                    NotifyPropertyChanged("IsEditing");
                }
            }
        }
        
        #endregion
    }
}
