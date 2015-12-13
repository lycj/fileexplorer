using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models.Bookmark
{
    internal class CommandModelBase : NotifyPropertyChanged, ICommandModel
    {

        #region Constructor

        public CommandModelBase(Func<IEntryModel[], IScriptCommand> commandFunc, object parameter = null)
        {
            _commandFunc = commandFunc;
            _parameter = parameter;

            _command = _commandFunc(null);
            
        }

        public CommandModelBase(IScriptCommand command, object parameter = null)
           : this(en => command, parameter)
        {            
            _parameter = parameter;
        }        


        #endregion

        #region Methods
        

        public int CompareTo(object obj)
        {
            if (obj is ICommandModel)
                return CompareTo(obj as ICommandModel);
            return -1;
        }

        public int CompareTo(ICommandModel other)
        {
            if (other != null)
                return -this.Priority.CompareTo(other.Priority);
            return -1;
        }

        public virtual void NotifySelectionChanged(IEntryModel[] appliedModels)
        {
            _command = _commandFunc(appliedModels);
        }

        #endregion

        #region Data
        private string _commandType;
        private IScriptCommand _command;
        private Func<IEntryModel[], IScriptCommand> _commandFunc;

        private char? _symbol;
        private IModelIconExtractor<ICommandModel> _headerIconExtractor;
        private string _toolTip;

        private string _header;
        private int _priority;
        private bool _isChecked, _isHeaderVisible = true, _isHeaderAlignRight;
        private bool? _isEnabled;
        private object _parameter;
        private bool _isVisibleOnToolbar = false, _isVisibleOnMenu = false;

        #endregion

        #region Public Properties
        public string CommandType { get { return _commandType; } set { _commandType = value; NotifyOfPropertyChanged(() => CommandType); } }
        public IScriptCommand Command { get { return _command; } set { _command = value; NotifyOfPropertyChanged(() => Command); } }
        /// <summary>
        /// Lookup from http://www.adamdawes.com/windows8/win8_segoeuisymbol.html
        /// </summary>
        public char? Symbol { get { return _symbol; } set { _symbol = value; NotifyOfPropertyChanged(() => Symbol); } }
        public IModelIconExtractor<ICommandModel> HeaderIconExtractor
        {
            get { return _headerIconExtractor; }
            set { _headerIconExtractor = value; NotifyOfPropertyChanged(() => HeaderIconExtractor); }
        }

        public string ToolTip { get { return _toolTip; } set { _toolTip = value; NotifyOfPropertyChanged(() => ToolTip); } }
        public string Header { get { return _header; } set { _header = value; NotifyOfPropertyChanged(() => Header); } }
        public bool IsHeaderVisible { get { return _isHeaderVisible; } set { _isHeaderVisible = value; NotifyOfPropertyChanged(() => IsHeaderVisible); } }
        public bool IsHeaderAlignRight { get { return _isHeaderAlignRight; } set { _isHeaderAlignRight = value; NotifyOfPropertyChanged(() => IsHeaderAlignRight); } }
        public bool IsChecked { get { return _isChecked; } set { _isChecked = value; NotifyOfPropertyChanged(() => IsChecked); } }
        public bool IsEnabled { get { return _isEnabled.HasValue ? _isEnabled.Value : true; } set { _isEnabled = value; NotifyOfPropertyChanged(() => IsEnabled); } }
        public bool IsVisibleOnToolbar { get { return _isVisibleOnToolbar; } set { _isVisibleOnToolbar = value; NotifyOfPropertyChanged(() => IsVisibleOnToolbar); } }
        public bool IsVisibleOnMenu { get { return _isVisibleOnMenu; } set { _isVisibleOnMenu = value; NotifyOfPropertyChanged(() => IsVisibleOnMenu); } }

        public int Priority { get { return _priority; } set { _priority = value; NotifyOfPropertyChanged(() => Priority); } }

        #endregion

    }
}
