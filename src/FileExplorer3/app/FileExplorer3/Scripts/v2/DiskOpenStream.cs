using FileExplorer.Defines;
using FileExplorer.IO;
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
        /// Serializable, Take an Entry (from entryVariable) and open it's stream (to streamVariable).
        /// Then close the stream after NextCommand is executed, and return ThenCommand.
        /// </summary>
        /// <param name="entryVariable"></param>
        /// <param name="streamVariable"></param>
        /// <param name="access"></param>
        /// <param name="streamCommand"></param>
        /// <param name="thenCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskOpenStream(string entryVariable = "{Entry}", string streamVariable = "{Stream}",
            Defines.FileAccess access = Defines.FileAccess.Read, IScriptCommand streamCommand = null, IScriptCommand thenCommand = null)
        {
            return new DiskOpenStream()
            {
                EntryKey = entryVariable,
                StreamKey = streamVariable,
                Access = access,
                NextCommand = (ScriptCommandBase)streamCommand,
                ThenCommand = (ScriptCommandBase)thenCommand
            };
        }
    }

    /// <summary>
    /// Serializable, Take an Entry (from EntryKey) and open it's stream (to StreamKey).
    /// Then close the stream after NextCommand is executed, and return ThenCommand.
    /// </summary>
    public class DiskOpenStream : ScriptCommandBase
    {
        /// <summary>
        /// Entry (IEntryModel) to be used to open stream, default = Entry
        /// </summary>
        public string EntryKey { get; set; }

        /// <summary>
        /// Key for the stream opened, default = Stream
        /// </summary>
        public string StreamKey { get; set; }

        /// <summary>
        /// Access mode, default = Read.
        /// </summary>
        public Defines.FileAccess Access { get; set; }

        public ScriptCommandBase ThenCommand { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DiskOpenStream>();

        public DiskOpenStream()
            : base("DiskOpenStream")
        {
            EntryKey = "{Entry}";
            StreamKey = "{Stream}";
            Access = FileAccess.Read;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IEntryModel entryModel = pm.GetValue<IEntryModel>(EntryKey);
            if (entryModel == null)
                return ResultCommand.Error(new ArgumentException(EntryKey + " is not found or not IEntryModel"));

            IDiskProfile profile = entryModel.Profile as IDiskProfile;
            if (profile == null)
                return ResultCommand.Error(new NotSupportedException(EntryKey + "'s Profile is not IDiskProfile"));

            using (var stream = await profile.DiskIO.OpenStreamAsync(entryModel, Access, pm.CancellationToken))
            {
                ParameterDic pmClone = pm.Clone();
                pmClone.SetValue(StreamKey, stream);
                logger.Debug(String.Format("{0} = Stream of {1}", StreamKey, EntryKey));
                await ScriptRunner.RunScriptAsync(pmClone, NextCommand);
            }

            if (Access == FileAccess.ReadWrite || Access == FileAccess.Write)
                return CoreScriptCommands.NotifyEntryChangedProfile( ChangeType.Changed, null, EntryKey, ThenCommand);
            else return ThenCommand;
        }
    }
}
