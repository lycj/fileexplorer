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
        /// Serializable, broadcast DirectoryChangedEvent to EventAggregator, indicate file system changed. 
        /// </summary>
        /// <param name="eventsVariable"></param>
        /// <param name="directoryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand NotifyDirectoryChanged(string eventsVariable, 
            string directoryVariable,IScriptCommand nextCommand = null)
        {
            return new NotifyDirectoryChanged()
            {
                EventsKey = eventsVariable,
                DirectoryEntryKey = directoryVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, broadcast DirectoryChangedEvent to EventAggregator {GlobalEvents}, indicate file system changed.
        /// </summary>
        /// <param name="directoryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand NotifyDirectoryChanged(
            string directoryVariable = "{Directory}", IScriptCommand nextCommand = null)
        {
            return NotifyDirectoryChanged("{Events}", directoryVariable, nextCommand);
        }
    }

    public enum NotifyEventType { DirectoryChanged, EntryChanged, EntryMoved, EntryCreated, EntryDeleted }

    public class NotifyDirectoryChanged : ScriptCommandBase
    {
        /// <summary>
        /// EventAggregator (IEventAggregator) used to broadcast the event.
        /// </summary>
        public string EventsKey { get; set; }

        public string DirectoryEntryKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<NotifyDirectoryChanged>();

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {          
            IEntryModel[] entryModels = DirectoryEntryKey == null ? new IEntryModel[] { } :
              (await pm.GetValueAsEntryModelArrayAsync(DirectoryEntryKey));

            object evnt = new DirectoryChangedEvent(this, entryModels.FirstOrDefault(), null);
            return CoreScriptCommands.BroadcastEvent(EventsKey, evnt, NextCommand);

        }
    }
}
