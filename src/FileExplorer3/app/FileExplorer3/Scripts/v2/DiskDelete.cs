using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {
        public static IScriptCommand DiskDelete(string entryKey, bool notifyChange,
            IScriptCommand nextCommand = null)
        {
            if (notifyChange)
                nextCommand = CoreScriptCommands.NotifyEntryChangedProfile(ChangeType.Deleted, null, entryKey, nextCommand);

            return new DiskDelete()
            {
                EntryKey = entryKey,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DiskDelete(string entryKey = "{Entry}", IScriptCommand nextCommand = null)
        {
            return DiskDelete(entryKey, false, nextCommand);
        }

        public static IScriptCommand DiskDeleteMultiple(string entriesKey, bool notifyChange, IScriptCommand nextCommand = null)
        {
            if (notifyChange)
                nextCommand = CoreScriptCommands.NotifyEntryChanged(Defines.ChangeType.Deleted, null, entriesKey, nextCommand);
            return ScriptCommands.ForEach(entriesKey, "{CurrentDeleteItem}",
                CoreScriptCommands.DiskDelete("{CurrentDeleteItem}"), nextCommand);
        }

        public static IScriptCommand DiskDeleteMultiple(string entriesKey = "{Entries}", IScriptCommand nextCommand = null)
        {
            return DiskDeleteMultiple(entriesKey, false, nextCommand);
        }
    }

    /// <summary>    
    /// Serializable, Call DiskProfile.DiskIO.DeleteAsync() to remove items.
    /// </summary>
    public class DiskDelete : ScriptCommandBase
    {
        /// <summary>
        /// Entry (IEntryModel) to delete, default = "{Entry}".
        /// Entry.Profile must be IDiskProfile.
        /// </summary>
        public string EntryKey { get; set; }

        public DiskDelete()
        {
            EntryKey = "{Entry}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {            
            IEntryModel entry = pm.GetValue<IEntryModel>(EntryKey, null);
            if (entry == null)
                return ResultCommand.Error(new KeyNotFoundException(EntryKey));
            IDiskProfile profile = entry.Profile as IDiskProfile;
            if (profile == null)
                return ResultCommand.Error(new ArgumentException(EntryKey + ".Profile is not an IDiskProfile"));

            await profile.DiskIO.DeleteAsync(entry, pm.CancellationToken);

            return NextCommand;
        }
    }
}
