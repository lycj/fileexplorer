using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.Models;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {
        /// <summary>
        /// Not Serializable, Broadcast an event to the specified IEventAggregator.
        /// </summary>
        /// <param name="EventsKey"></param>
        /// <param name="evnt"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand BroadcastEvent(string eventsVariable = "{Events}", object evnt = null,
            IScriptCommand nextCommand = null)
        {
            return new BroadcastEventCommand()
            {
                EventsKey = eventsVariable,
                Event = evnt,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    public class BroadcastEventCommand : ScriptCommandBase
    {
        /// <summary>
        /// EventAggregator (IEventAggregator) used to broadcast the event, Default = "{Events}".
        /// </summary>
        public string EventsKey { get; set; }

        public object Event { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<BroadcastEventCommand>();

        public BroadcastEventCommand()
            : base("BroadcastEventCommand")
        {
            EventsKey = "{Events}";
            Event = null;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var events = pm.GetValue<IEventAggregator>(EventsKey);
            if (events == null)
                return ResultCommand.Error(new ArgumentNullException(EventsKey));

            logger.Info(String.Format("Broadcasting {0}", Event));
            events.PublishOnUIThread(Event);

            return NextCommand;

        }
    }
}
