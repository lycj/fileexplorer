using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public abstract class DoCommandBase<VM> : ScriptCommandBase
    {
        private Func<VM, ParameterDic, Task<IScriptCommand>> _commandFunc;
        private string _viewModelName;
        protected DoCommandBase(string viewModelName, Func<VM, ParameterDic, Task<IScriptCommand>> commandFunc)
            : base(viewModelName, viewModelName)
        {
            _viewModelName = viewModelName;
            _commandFunc = commandFunc;
        }

        protected DoCommandBase(string viewModelName, Func<VM, ParameterDic, IScriptCommand> commandFunc)
            : this(viewModelName, (vm, pd) => Task.Run(() => commandFunc(vm, pd)))
        {

        }

        protected DoCommandBase(string viewModelName, Func<VM, Task<IScriptCommand>> commandFunc)
            : this(viewModelName, (vm, pd) => commandFunc(vm))
        {
        }

        protected DoCommandBase(string viewModelName, Func<VM, IScriptCommand> commandFunc)
            : this(viewModelName, (vm, pd) => commandFunc(vm))
        {

        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            VM evm = (VM)pm[_viewModelName];
            if (evm == null)
                return ResultCommand.Error(new ArgumentException(_viewModelName));
            return await _commandFunc(evm, pm);
        }

        public override bool CanExecute(ParameterDic pm)
        {
            VM evm = (VM)pm[_viewModelName];
            return evm != null;// && AsyncUtils.RunSync(() => _commandFunc(evm)).CanExecute(pm);
        }
    }

}
