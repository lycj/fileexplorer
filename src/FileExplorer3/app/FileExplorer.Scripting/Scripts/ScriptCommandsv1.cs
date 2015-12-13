using MetroLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ScriptCommands
    {
        public static NoScriptCommand NoCommand = new NoScriptCommand();


        public static IScriptCommand If(Func<ParameterDic, bool> condition, IScriptCommand ifTrue, IScriptCommand otherwise)
        {
            return new IfScriptCommand(condition, ifTrue, otherwise);
        }

        [Obsolete("Use RunCommands")]
        public static IScriptCommand RunInSequence(params IScriptCommand[] scriptCommands)
        {
            return new RunInSequenceScriptCommand(scriptCommands);
        }

        [Obsolete("Use RunCommands")]
        public static IScriptCommand RunInSequence(IScriptCommand[] scriptCommands, IScriptCommand nextCommand)
        {
            return new RunInSequenceScriptCommand(scriptCommands, nextCommand);
        }


        [Obsolete("Use ForEach(NonGeneric)")]
        public static IScriptCommand ForEach<T>(T[] source, Func<T, IScriptCommand> commandFunc, IScriptCommand nextCommand = null)
        {
            return new ForEachCommand<T>(source, commandFunc, nextCommand);
        }

        
    }

  



    public class IfScriptCommand : ScriptCommandBase
    {
        private Func<ParameterDic, bool> _condition;
        private IScriptCommand _otherwiseCommand;
        private IScriptCommand _ifTrueCommand;
        private bool _continueOnCaptureContext = false;
        public IfScriptCommand(Func<ParameterDic, bool> condition,
            IScriptCommand ifTrueCommand, IScriptCommand otherwiseCommand)
            : base("IfScriptCommand")
        { _condition = condition; _ifTrueCommand = ifTrueCommand; _otherwiseCommand = otherwiseCommand; }

        
        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (_condition(pm))
                return _ifTrueCommand;
            return _otherwiseCommand;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            if (_condition != null && _condition(pm))
                return _ifTrueCommand == null || _ifTrueCommand.CanExecute(pm);
            else return _otherwiseCommand == null || _otherwiseCommand.CanExecute(pm);
        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            if (_condition(pm))
                return _ifTrueCommand;
            return _otherwiseCommand;
        }

    }

    public class ForEachCommand<T> : ScriptCommandBase
    {
        private T[] _source;
        private Func<T, IScriptCommand> _commandFunc;
        public ForEachCommand(T[] source, Func<T, IScriptCommand> commandFunc, IScriptCommand nextCommand)
            : base("ForEach", nextCommand)
        {
            _source = source;
            _commandFunc = commandFunc;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            List<IScriptCommand> outputCommands = new List<IScriptCommand>();
            foreach (var s in _source)
            {
                var command = _commandFunc(s);
                var outputCommand = command.Execute(pm);
                if (pm.Error != null)
                    return outputCommand;
                if (outputCommand != ResultCommand.NoError && outputCommand != ResultCommand.OK)
                    outputCommands.Add(outputCommand);
            }
            return new RunInSequenceScriptCommand(outputCommands.ToArray(), _nextCommand);
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            List<IScriptCommand> outputCommands = new List<IScriptCommand>();
            foreach (var s in _source)
            {
                var outputCommand = await _commandFunc(s).ExecuteAsync(pm);
                if (pm.Error != null)
                    return outputCommand;

                if (outputCommand != ResultCommand.NoError && outputCommand != ResultCommand.OK)
                    outputCommands.Add(outputCommand);
            }
            return outputCommands.Count() == 0 ?
                _nextCommand :
                new RunInSequenceScriptCommand(outputCommands.ToArray(), _nextCommand);
        }

        public virtual bool CanExecute(ParameterDic pm)
        {
            return _source.Count() > 0 && _commandFunc(_source.First()).CanExecute(pm);
        }
    }



    /// <summary>
    /// Run a number of ScriptCommand in a sequence.
    /// </summary>
    [Obsolete]
    public class RunInSequenceScriptCommand : ScriptCommandBase //IScriptCommand
    {
        private IScriptCommand[] _scriptCommands;
        private IScriptCommand _nextCommand = ResultCommand.NoError;
        public IScriptCommand[] ScriptCommands { get { return _scriptCommands; } }
        public RunInSequenceScriptCommand(params IScriptCommand[] scriptCommands)
            : base("RunInSequence")
        {
            if (scriptCommands.Length == 0) throw new ArgumentException(); _scriptCommands = scriptCommands;
        }

        public RunInSequenceScriptCommand(IScriptCommand[] scriptCommands, IScriptCommand nextCommand)
        {
            if (scriptCommands.Length == 0) throw new ArgumentException(); _scriptCommands = scriptCommands;
            _nextCommand = nextCommand;
        }

        public string CommandKey
        {
            get { return String.Join(",", _scriptCommands.Select(c => c.CommandKey)); }
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            ScriptRunner.RunScript(pm, ScriptCommands);
            if (pm.Error != null)
                return ResultCommand.Error(pm.Error);
            else return _nextCommand;
        }

        public virtual bool CanExecute(ParameterDic pm)
        {
            return _scriptCommands.First().CanExecute(pm);
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            await ScriptRunner.RunScriptAsync(pm, ScriptCommands);
            if (pm.Error != null)
                return ResultCommand.Error(pm.Error);
            else return _nextCommand;
        }

        public bool ContinueOnCaptureContext
        {
            get { return _scriptCommands.Any(c => c.ContinueOnCaptureContext); }
        }
    }





    [Obsolete("Use ResultCommand")]
    public class NoScriptCommand : IScriptCommand
    {

        public string CommandKey
        {
            get { return "None"; }
        }

        public IScriptCommand Execute(ParameterDic pm)
        {
            return null;
        }

        public bool CanExecute(ParameterDic pm)
        {
            return false;
        }

        public async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            return Execute(pm);
        }

        public bool ContinueOnCaptureContext { get { return false; } }


        public ScriptCommandBase NextCommand
        {
            get { return null; }
        }
    }
}
