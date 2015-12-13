using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class SimpleScriptCommand : ScriptCommandBase
    {
      
                #region Constructor

        public SimpleScriptCommand(string commandKey,
            Func<ParameterDic, IScriptCommand> executeFunc,
            Func<ParameterDic, bool> canExecuteFunc = null, bool continueOnCaptureContext = true)
            : base(commandKey)
        {
            _executeFunc = executeFunc;
            _canExecuteFunc = canExecuteFunc;
            ContinueOnCaptureContext = continueOnCaptureContext;
        }

        #endregion

        #region Methods

        public override bool CanExecute(ParameterDic pm)
        {
            if (_canExecuteFunc == null)
                return true;
            else return _canExecuteFunc(pm);
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            return _executeFunc(pm);
        }

        #endregion

        #region Data

        private Func<ParameterDic, bool> _canExecuteFunc;
        private Func<ParameterDic, IScriptCommand> _executeFunc;

        #endregion

        #region Public Properties
        
        #endregion

    }

    public class SimpleScriptCommandAsync : ScriptCommandBase
    {

        #region Constructor

        public SimpleScriptCommandAsync(string commandKey,
            Func<ParameterDic, Task<IScriptCommand>> executeFunc,
            Func<ParameterDic, bool> canExecuteFunc = null, bool continueOnCaptureContext = true)
            : base(commandKey)
        {
            _executeFunc = executeFunc;
            _canExecuteFunc = canExecuteFunc;
            ContinueOnCaptureContext = continueOnCaptureContext;
        }

        #endregion

        #region Methods

        public override bool CanExecute(ParameterDic pm)
        {
            if (_canExecuteFunc == null)
                return true;
            else return _canExecuteFunc(pm);
        }
        
        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            return await _executeFunc(pm);
        }

        #endregion

        #region Data

        private Func<ParameterDic, bool> _canExecuteFunc;
        private Func<ParameterDic, Task<IScriptCommand>> _executeFunc;

        #endregion

        #region Public Properties

        #endregion

    }
}
