using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Script
{        
    public abstract class ScriptCommandBase : IScriptCommand
    {
        #region Constructor

        /// <summary>
        /// Used by serializer only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScriptCommandBase()
        {

        }
        
        protected ScriptCommandBase(string commandKey, params string[] parameters)
        {
            CommandKey = commandKey;
            CommandParameters = parameters;
            _nextCommand = ResultCommand.NoError;
        }

        protected ScriptCommandBase(string commandKey, IScriptCommand nextCommand, params string[] parameters)
        {
            CommandKey = commandKey;
            CommandParameters = parameters;

            _nextCommand = nextCommand ?? ResultCommand.NoError;
        }

        #endregion

        #region Methods

        public virtual IScriptCommand Execute(ParameterDic pm)
        {
            return AsyncUtils.RunSync(() => ExecuteAsync(pm));
        }

        public virtual async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            return Execute(pm);
        }

        public virtual bool CanExecute(ParameterDic pm)
        {
            return true;
        }      

        #endregion

        #region Data

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected IScriptCommand _nextCommand; //Use NextCommand.
        private bool _continueOnCaptureContext = false;

        #endregion

        #region Public Properties

        [DefaultValue(false)]
        public virtual bool ContinueOnCaptureContext
        {
            get { return _continueOnCaptureContext; }
            set { _continueOnCaptureContext = value; }
        }
        
        [XmlIgnore]
        public string CommandKey { get; set; }
        [XmlIgnore]
        public string[] CommandParameters { get; set; }
        public ScriptCommandBase NextCommand { get { return (ScriptCommandBase)_nextCommand; } set { _nextCommand = value; } }


        #endregion
    }

    public abstract class ScriptCommandBase<T> : ScriptCommandBase
    {
        #region Constructor

        /// <summary>
        /// Used by serializer only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScriptCommandBase()
            : base ()
        {

        }

        //protected ScriptCommandBase(string commandKey, params string[] parameters)
        //    : base(commandKey, parameters)
        //{
        //    CommandKey = commandKey;
        //    CommandParameters = parameters;
        //    _nextCommandConstructor = ScriptCommandConstructor<T>.ReturnCommand(ResultCommand.NoError);
        //}

        //protected ScriptCommandBase(string commandKey, IScriptCommand nextCommand, params string[] parameters)
        //    : base(commandKey, parameters)
        //{
        //    CommandKey = commandKey;
        //    CommandParameters = parameters;

        //    _nextCommandConstructor = ScriptCommandConstructor<T>.ReturnCommand(nextCommand ?? ResultCommand.NoError);
        //}

        //protected ScriptCommandBase(string commandKey, IScriptCmdCtor<T> nextCommandConstructor, params string[] parameters)
        //    : base(commandKey, parameters)
        //{
        //    CommandKey = commandKey;
        //    CommandParameters = parameters;

        //    _nextCommandConstructor = nextCommandConstructor ?? ScriptCommandConstructor<T>.ReturnCommand(ResultCommand.NoError);
        //}
        
        #endregion

        #region Methods
        
        #endregion

        #region Data

        //private IScriptCmdCtor<T> _nextCommandConstructor;
        
        #endregion

        #region Public Properties

        //public new ScriptCommandConstructor<T> NextCommand { get { return (ScriptCommandConstructor<T>)_nextCommandConstructor; } set { _nextCommandConstructor = value; } }

        #endregion       
    }


}
