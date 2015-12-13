using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable, broadcast RootChangedEvent to EventAggregator, indicate root items changed.
        /// </summary>
        /// <param name="eventsVariable"></param>
        /// <param name="changeType"></param>
        /// <param name="directoryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand NotifyRootChanged(string eventsVariable, ChangeType changeType, 
            string directoryVariable,IScriptCommand nextCommand = null)
        {
            return new NotifyRootChanged()
            {
                EventsKey = eventsVariable,
                ChangeType = changeType, 
                DirectoryEntryKey = directoryVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand NotifyRootCreated(string directoryVariable = "{Directory}", 
            IScriptCommand nextCommand = null)
        {
            return NotifyRootChanged("{GlobalEvents}", ChangeType.Created, directoryVariable, nextCommand);
        }

        public static IScriptCommand NotifyRootChanged(string directoryVariable = "{Directory}",
            IScriptCommand nextCommand = null)
        {
            return NotifyRootChanged("{GlobalEvents}", ChangeType.Changed, directoryVariable, nextCommand);
        }

        public static IScriptCommand NotifyRootDeleted(string directoryVariable = "{Directory}",
            IScriptCommand nextCommand = null)
        {
            return NotifyRootChanged("{GlobalEvents}", ChangeType.Deleted, directoryVariable, nextCommand);
        }

        public static IScriptCommand NotifyRootCreated(IEntryModel[] directories,
           IScriptCommand nextCommand = null)
        {
            string directoryVariable = "{Notify-RootDirs}";
            return ScriptCommands.Assign(directoryVariable, directories, false,
                NotifyRootCreated(directoryVariable, nextCommand));
        }

        public static IScriptCommand NotifyRootChanged(IEntryModel[] directories,
            IScriptCommand nextCommand = null)
        {
            string directoryVariable = "{Notify-RootDirs}";
            return ScriptCommands.Assign(directoryVariable, directories, false,
                NotifyRootChanged(directoryVariable, nextCommand));
        }

        public static IScriptCommand NotifyRootDeleted(IEntryModel[] directories,
            IScriptCommand nextCommand = null)
        {
            string directoryVariable = "{Notify-RootDirs}";
            return ScriptCommands.Assign(directoryVariable, directories, false,
                NotifyRootDeleted(directoryVariable, nextCommand));
        }
    }

    public class NotifyRootChanged : ScriptCommandBase
    {
        /// <summary>
        /// EventAggregator (IEventAggregator) used to broadcast the event, Default = {GlobalEvents}
        /// </summary>
        public string EventsKey { get; set; }

        public ChangeType ChangeType { get; set; }

        /// <summary>
        /// Key of root directory (IDirectoryInfo or IDirectoryInfo[]).
        /// </summary>
        public string DirectoryEntryKey { get; set; }

      

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<NotifyRootChanged>();

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {          
            IEntryModel[] entryModels = DirectoryEntryKey == null ? new IEntryModel[] { } :
              (await pm.GetValueAsEntryModelArrayAsync(DirectoryEntryKey));

            object evnt = new RootChangedEvent(ChangeType, entryModels);;
            return CoreScriptCommands.BroadcastEvent(EventsKey, evnt, NextCommand);

        }
    }
}
