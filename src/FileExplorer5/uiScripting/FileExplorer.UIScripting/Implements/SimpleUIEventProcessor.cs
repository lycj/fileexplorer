using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer.Script;
using FileExplorer;
using System.Windows.Input;
using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{

   

    public class SimpleUIEventProcessor : UIEventProcessorBase, IUIEventProcessor
    {
        #region Constructors

        #endregion

        #region Methods


        protected override IScriptCommand onEvent(RoutedEvent eventId)
        {
            if (_processEvents.ContainsKey(eventId))
                return _processEvents[eventId];
            return ResultCommand.NoError;
        }

        protected void registerEvent(RoutedEvent onEvent, IScriptCommand command)
        {
            _processEvents.Add(onEvent, command);
        }


        //protected void registerEvent(RoutedEvent onEvent, 
        //    Func<ParameterDic, IScriptCommand> actionFunc, 
        //    Func<ParameterDic, bool> canExecuteFunc = null,
        //    string commandKey = "SimpleUIEventProcessor")
        //{
        //    registerEvent(onEvent, new SimpleScriptCommand(commandKey, actionFunc, canExecuteFunc));
        //}



        #endregion

        #region Data

        Dictionary<RoutedEvent, IScriptCommand> _processEvents = new Dictionary<RoutedEvent, IScriptCommand>();

        #endregion

        #region Public Properties

        public override IEnumerable<RoutedEvent> ProcessEvents { get { return _processEvents.Keys; } }

        #endregion
    }

}
