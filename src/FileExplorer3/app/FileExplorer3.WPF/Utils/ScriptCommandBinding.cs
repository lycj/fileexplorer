using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cinch;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.Utils
{
    public enum ScriptBindingScope { Application, Explorer, Local }

    public interface IScriptCommandBinding : INotifyPropertyChanged
    {
        IScriptCommand ScriptCommand { get; set; }
        ICommand Command { get; set; }
        RoutedUICommand UICommandKey { get; }
        CommandBinding CommandBinding { get; }
        ScriptBindingScope Scope { get; }
    }

   

    public class ScriptCommandBinding : NotifyPropertyChanged, IScriptCommandBinding
    {
        #region Constructor

        public static IScriptCommandBinding ForRoutedUICommand(RoutedUICommand uiCommandKey)
        {
            return new ScriptCommandBinding(null, uiCommandKey, null, ScriptBindingScope.Local);
        }

        public static IScriptCommandBinding FromScriptCommand<T>(RoutedUICommand uiCommandKey,
         T targetObject, Func<T, IScriptCommand> scriptCommandFunc, IParameterDicConverter parameterDicConverter = null, ScriptBindingScope scope = ScriptBindingScope.Application)
        {
            return new ScriptCommandBinding<T>(uiCommandKey, targetObject, scriptCommandFunc, parameterDicConverter) { Scope = scope };
        }

        public ScriptCommandBinding(RoutedUICommand uICommandKey, ICommand command, IParameterDicConverter parameterDicConverter = null,
            ScriptBindingScope scope = ScriptBindingScope.Application)
        {
            Scope = scope;
            Command = command;
            UICommandKey = uICommandKey == null ? ApplicationCommands.NotACommand : uICommandKey;            
            ParameterDicConverter = parameterDicConverter == null ? ParameterDicConverters.ConvertParameterOnly : parameterDicConverter;            
        }

        public ScriptCommandBinding(RoutedUICommand uICommandKey, IScriptCommand scriptCommand,
            IParameterDicConverter parameterDicConverter = null, ScriptBindingScope scope = ScriptBindingScope.Application)
        {
            Scope = scope;
            ScriptCommand = scriptCommand;
            UICommandKey = uICommandKey == null ? ApplicationCommands.NotACommand : uICommandKey;            
            ParameterDicConverter = parameterDicConverter == null ? ParameterDicConverters.ConvertParameterOnly : parameterDicConverter;
           
        }

        public ScriptCommandBinding(RoutedUICommand uICommandKey, Func<object, bool> canExecuteFunc, Action<object> executeFunc, 
            IParameterDicConverter parameterDicConverter = null, ScriptBindingScope scope = ScriptBindingScope.Application)
            : this(uICommandKey, new SimpleCommand() { CanExecuteDelegate = (p) => canExecuteFunc == null || canExecuteFunc(p), 
                ExecuteDelegate = executeFunc, UICommand = uICommandKey }, parameterDicConverter, scope)
        {
        }

        protected ScriptCommandBinding(RoutedUICommand uiCommandKey, IParameterDicConverter parameterDicConverter = null, 
            ScriptBindingScope scope = ScriptBindingScope.Application)
        {
            Scope = scope;
            UICommandKey = uiCommandKey;
            ParameterDicConverter = parameterDicConverter == null ? ParameterDicConverters.ConvertParameterOnly : parameterDicConverter;
        }

        #endregion

        #region Methods

        private CommandBinding getCommandBiniding()
        {
            return new CommandBinding(UICommandKey,
               (ExecutedRoutedEventHandler)delegate(object sender, ExecutedRoutedEventArgs e)
               {
                   var pd = ParameterDicConverter.Convert(e.Parameter, "Executed", sender, e);
                   Command.Execute(pd);
                   e.Handled = true;
               },
               (CanExecuteRoutedEventHandler)delegate(object sender, CanExecuteRoutedEventArgs e)
               {
                   var pd = ParameterDicConverter.Convert(e.Parameter, "Executed", sender, e);
                   e.CanExecute = Command.CanExecute(pd);
               });
        }

   
        protected void setScriptCommand(IScriptCommand value)
        {
            _scriptCommand = value;
            _command = new SimpleCommand()
            {
                CanExecuteDelegate = (p) => !(ScriptCommand is NullScriptCommand) &&
                    ScriptCommand.CanExecute(ParameterDicConverter.Convert(p)),
                ExecuteDelegate = (p) => ScriptRunner.RunScriptAsync(ParameterDicConverter.Convert(p), ScriptCommand)
            };
        }

        public override string ToString()
        {
            return "ScriptCommandBinding" +  UICommandKey.ToString();
        }

        protected void setCommand(ICommand value)
        {
            _command = value;
            _scriptCommand = new ICommandScriptCommand(Command, ParameterDicConverter);
        }
      
        #endregion

        #region Data

        private IScriptCommand _scriptCommand = null;
        private ICommand _command = null;

        #endregion

        #region Public Properties

        public virtual IScriptCommand ScriptCommand { get { return _scriptCommand; } set { setScriptCommand(value); } }
        public ICommand Command { get { return _command; } set { setCommand(value); } }        
        private IParameterDicConverter ParameterDicConverter { get; set; }
        public RoutedUICommand UICommandKey { get; private set; }
        public CommandBinding CommandBinding { get { return getCommandBiniding(); } }
        public ScriptBindingScope Scope { get; set; }

        #endregion




       
    }

    public class ScriptCommandBinding<T> : ScriptCommandBinding
    {
        #region Constructor

        public ScriptCommandBinding(RoutedUICommand uICommandKey,
            T targetObject, Func<T, IScriptCommand> scriptCommandFunc,
            IParameterDicConverter parameterDicConverter = null)
            : base(uICommandKey, parameterDicConverter)
        {
            _targetObject = targetObject;
            _scriptCommandFunc = scriptCommandFunc;
            
            setScriptCommand(null); //Assign Command to call ScriptCommand

        }

     

        #endregion

        #region Methods

        #endregion

        #region Data

        public Func<T, IScriptCommand> _scriptCommandFunc;
        public T _targetObject;

        #endregion

        #region Public Properties

        public override IScriptCommand ScriptCommand { get { var retVal = _scriptCommandFunc(_targetObject); 
            return retVal == null ? ResultCommand.NoError : retVal; } }

        #endregion
    }
}
