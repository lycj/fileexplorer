using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public class CommandModel : PropertyChangedBase, ICommandModel, IRoutedCommandModel
    {

        #region Constructor

        public CommandModel(IScriptCommand command, object parameter = null)
        {
            _command = command;
            _parameter = parameter;
        }

        public CommandModel(RoutedUICommand routedCommand, object parameter = null)
        {
            _routedCommand = routedCommand;
            _parameter = parameter;
            IsEnabled = true;
            Header = routedCommand.Text;
        }

        public CommandModel()
        {
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
        }

        #endregion

        #region Data
        private string _commandType;
        private IScriptCommand _command;
        private RoutedUICommand _routedCommand = null;

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
        public string CommandType { get { return _commandType; } set { _commandType = value; NotifyOfPropertyChange(() => CommandType); } }
        public IScriptCommand Command { get { return _command; } set { _command = value; NotifyOfPropertyChange(() => Command); } }
        public RoutedUICommand RoutedCommand { get { return _routedCommand; } set { _routedCommand = value; NotifyOfPropertyChange(() => RoutedCommand); } }        
        /// <summary>
        /// Lookup from http://www.adamdawes.com/windows8/win8_segoeuisymbol.html
        /// </summary>
        public char? Symbol { get { return _symbol; } set { _symbol = value; NotifyOfPropertyChange(() => Symbol); } }
        public IModelIconExtractor<ICommandModel> HeaderIconExtractor { get { return _headerIconExtractor; }
            set { _headerIconExtractor = value; NotifyOfPropertyChange(() => HeaderIconExtractor); }
        }
      
        public string ToolTip { get { return _toolTip; } set { _toolTip = value; NotifyOfPropertyChange(() => ToolTip); } }
        public string Header { get { return _header; } set { _header = value; NotifyOfPropertyChange(() => Header); } }
        public bool IsHeaderVisible { get { return _isHeaderVisible; } set { _isHeaderVisible = value; NotifyOfPropertyChange(() => IsHeaderVisible); } }
        public bool IsHeaderAlignRight { get { return _isHeaderAlignRight; } set { _isHeaderAlignRight = value; NotifyOfPropertyChange(() => IsHeaderAlignRight); } }
        public bool IsChecked { get { return _isChecked; } set { _isChecked = value; NotifyOfPropertyChange(() => IsChecked); } }
        public bool IsEnabled { get { return _isEnabled.HasValue ? _isEnabled.Value : true; } set { _isEnabled = value; NotifyOfPropertyChange(() => IsEnabled); } }
        public bool IsVisibleOnToolbar { get { return _isVisibleOnToolbar; } set { _isVisibleOnToolbar = value; NotifyOfPropertyChange(() => IsVisibleOnToolbar); } }
        public bool IsVisibleOnMenu { get { return _isVisibleOnMenu; } set { _isVisibleOnMenu = value; NotifyOfPropertyChange(() => IsVisibleOnMenu); } }

        public int Priority { get { return _priority; } set { _priority = value; NotifyOfPropertyChange(() => Priority); } }

        #endregion



    }
}
