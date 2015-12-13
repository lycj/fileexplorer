using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cinch;
using FileExplorer;
using FileExplorer.Script;

namespace FileExplorer.WPF.Utils
{
    internal class ICommandScriptCommand : ScriptCommandBase
    {
        private ICommand _command;
        private IParameterDicConverter _parameterDicConverter;
        public ICommandScriptCommand(ICommand command, IParameterDicConverter parameterDicConverter)
            : base(command.ToString())
        {
            _command = command;
            _parameterDicConverter = parameterDicConverter;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return _command.CanExecute(pm.GetValue("{Parameter}"));
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            try
            {
                _command.Execute(pm.GetValue("{Parameter}"));
            }
            catch (Exception ex) { return ResultCommand.Error(ex); }

            return ResultCommand.NoError;
        }
    }
   

}
