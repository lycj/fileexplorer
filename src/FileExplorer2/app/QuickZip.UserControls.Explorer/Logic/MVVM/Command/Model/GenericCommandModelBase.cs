using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Windows.Input;
using QuickZip.UserControls.MVVM.Command.ViewModel;
using System.Windows;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    public abstract class CommandModel : ValidatingObject, IComparable, IComparable<CommandModel>
    {
        protected static string uriRootThemeResources = "pack://application:,,,/QuickZip.UserControls.Explorer;component/Themes/Resources/";

        #region Constructor

        public CommandModel()
        {
        }
     

        #endregion

        #region Methods

        protected Bitmap GetResourceBitmap(string root, string path)
        {
            return GetResourceBitmap(new Uri(root + path));
        }

        protected Bitmap GetResourceBitmap(Uri uri)
        {         
            return new Bitmap(Application.GetResourceStream(uri).Stream);        
        }


        public CommandViewModel ToViewModel() { return new CommandViewModel(this); }

        public virtual bool CanExecute(object param)
        {
            return true;
        }

        public virtual void Execute(object param)
        {
            
        }

        public int CompareTo(object obj)
        {
            if (obj is CommandModel)
                return CompareTo(obj as CommandModel);
            return -1;
        }

        public int CompareTo(CommandModel other)
        {
            if (other != null)
                return -this.Priority.CompareTo(other.Priority);
            return -1;
        }



        #endregion

        #region Data

        private bool _isDirectory = false, _isChecked = false;
        private string _header = "Undefined";
        private string _toolTip = null;
        private Bitmap _headerIcon = null;
        private bool _isRightAligned = false;
        private int _extraItemHeight = 0;        
        private bool _isExecutable = false;
        private int _priority = 0;
        

        #endregion

        #region Public Properties

        public int ExtraItemHeight { get { return _extraItemHeight; } set { _extraItemHeight = value; NotifyPropertyChanged("ExtraItemHeight"); } }
        public bool IsRightAligned { get { return _isRightAligned; } set { _isRightAligned = value; NotifyPropertyChanged("IsRightAligned"); } }
        public bool IsChecked { get { return _isChecked; } internal set { _isChecked = value; NotifyPropertyChanged("IsChecked"); } }
        public bool IsDirectory { get { return _isDirectory; } set { _isDirectory = value; NotifyPropertyChanged("IsDirectory"); } }
        public string Header { get { return _header; } set { _header = value; NotifyPropertyChanged("Header"); } }
        public Bitmap HeaderIcon { get { return _headerIcon; } set { _headerIcon = value; NotifyPropertyChanged("HeaderIcon"); } }
        public string ToolTip { get { return _toolTip; } set { _toolTip = value; NotifyPropertyChanged("ToolTip"); } }
        public int Priority { get { return _priority; } set { _priority = value; NotifyPropertyChanged("Priority"); } }

        /// <summary>
        /// Implemented Execute function
        /// </summary>
        public bool IsExecutable { get { return _isExecutable; } protected set { _isExecutable = value; NotifyPropertyChanged("IsExecutable"); } }

        #endregion
    

}
}
