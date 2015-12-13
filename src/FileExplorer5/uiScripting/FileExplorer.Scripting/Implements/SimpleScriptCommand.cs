using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{
    public class SimpleScriptCommand : ScriptCommandBase
    {
      
                #region Constructor

        public SimpleScriptCommand(string commandKey,
            Func<IParameterDic, IScriptCommand> executeFunc,
            Func<IParameterDic, bool> canExecuteFunc = null, bool continueOnCaptureContext = true)
            : base(commandKey)
        {
            _executeFunc = executeFunc;
            _canExecuteFunc = canExecuteFunc;
            ContinueOnCaptureContext = continueOnCaptureContext;
        }

        #endregion

        #region Methods

        public override bool CanExecute(IParameterDic pm)
        {
            if (_canExecuteFunc == null)
                return true;
            else return _canExecuteFunc(pm);
        }

        public override IScriptCommand Execute(IParameterDic pm)
        {
            return _executeFunc(pm);
        }

        #endregion

        #region Data

        private Func<IParameterDic, bool> _canExecuteFunc;
        private Func<IParameterDic, IScriptCommand> _executeFunc;

        #endregion

        #region Public Properties
        
        #endregion

    }

    public class SimpleScriptCommandAsync : ScriptCommandBase
    {

        #region Constructor

        public SimpleScriptCommandAsync(string commandKey,
            Func<IParameterDic, Task<IScriptCommand>> executeFunc,
            Func<IParameterDic, bool> canExecuteFunc = null, bool continueOnCaptureContext = true)
            : base(commandKey)
        {
            _executeFunc = executeFunc;
            _canExecuteFunc = canExecuteFunc;
            ContinueOnCaptureContext = continueOnCaptureContext;
        }

        #endregion

        #region Methods

        public override bool CanExecute(IParameterDic pm)
        {
            if (_canExecuteFunc == null)
                return true;
            else return _canExecuteFunc(pm);
        }
        
        public override async Task<IScriptCommand> ExecuteAsync(IParameterDic pm)
        {
            return await _executeFunc(pm);
        }

        #endregion

        #region Data

        private Func<IParameterDic, bool> _canExecuteFunc;
        private Func<IParameterDic, Task<IScriptCommand>> _executeFunc;

        #endregion

        #region Public Properties

        #endregion

    }
}
