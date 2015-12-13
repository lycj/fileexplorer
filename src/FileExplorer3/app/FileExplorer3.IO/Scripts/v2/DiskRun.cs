using FileExplorer.IO;
using FileExplorer.Models;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Models;
using System.Diagnostics;
using System.IO;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        /// <summary>
        /// Serializable, Invoke an entry in shell.
        /// </summary>
        /// <param name="entryVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand DiskRun(string entryVariable = "{Entry}", IScriptCommand nextCommand = null)
        {
            return new DiskRun()
            {
                EntryKey = entryVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Serializable, Invoke an entry in shell.
    /// </summary>
    public class DiskRun : ScriptCommandBase
    {
        /// <summary>
        /// Point to Entry (IEntryModel with IDiskProfile) to run, Default = {Entry}
        /// </summary>
        public string EntryKey { get; set; }

        /// <summary>
        /// Executable (string, e.g. c:\Windows\Notepad.exe) to run the entry, Default = null (n/a).
        /// Set to OpenAs to open the OpenAs dialog.
        /// </summary>
        public string ExecutableKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DiskRun>();

        public DiskRun()
            : base("DiskRun")
        {
            EntryKey = "{Entry}";
            ExecutableKey = null;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IEntryModel entry = pm.GetValue<IEntryModel>(EntryKey);
            if (entry == null)
                return ResultCommand.Error(new KeyNotFoundException(EntryKey));
            IDiskProfile profile = entry.Profile as IDiskProfile;
            if (profile == null)
                return ResultCommand.Error(new ArgumentException(EntryKey + "'s Profile is not IDiskProfile"));

            var entryMapping = profile.DiskIO.Mapper[entry];
            if (entryMapping.IsVirtual)
                await profile.DiskIO.WriteToCacheAsync(entry, pm.CancellationToken, true);

            if (ExecutableKey == null) //Default implementation
            {
                if (entry.IsDirectory)
                {
                    try { Process.Start(entryMapping.IOPath); }
                    catch (Exception ex) { return ResultCommand.Error(ex); }
                }
                else
                {
                    if (File.Exists(entryMapping.IOPath))
                    {
                        ProcessStartInfo psi = null;
                        if (ExecutableKey == "OpenAs")
                        {
                            psi = new ProcessStartInfo("Rundll32.exe");
                            psi.Arguments = String.Format(" shell32.dll, OpenAs_RunDLL {0}", entryMapping.IOPath);
                        }
                        else psi = new ProcessStartInfo(entryMapping.IOPath);

                        if (psi != null)
                            try { Process.Start(psi); }
                            catch (Exception ex) { return ResultCommand.Error(ex); }
                    }
                }
            }
            else
            {

            }

            return NextCommand;
        }
    }
}
