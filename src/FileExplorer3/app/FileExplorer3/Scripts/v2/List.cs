using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;
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
        /// Serializable, List content from EntryKey to DestinationKey.
        /// </summary>
        /// <param name="entryVariable"></param>
        /// <param name="destVariable"></param>
        /// <param name="maskVariable">Specify as mask directly (e.g. *), or reference another variable (e.g. {masks})</param>
        /// <param name="options"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand List(string entryVariable = "{Entry}", string destVariable = "{Destination}",
            string maskVariable = "*",
            ListOptions options = ListOptions.File | ListOptions.Folder,
            IScriptCommand nextCommand = null)
        {
            return new List()
            {
                EntryKey = entryVariable,
                DestinationKey = destVariable,
                MaskKey = maskVariable,
                ListOptions = options,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand List(IEntryModel directoryEntry, string destVariable = "{Destination}",
            string maskVariable = "*",
            ListOptions options = ListOptions.File | ListOptions.Folder,
            IScriptCommand nextCommand = null)
        {
        return ScriptCommands.Assign("{List-DirectoryEntry}", directoryEntry, false, 
            List("{List-DirectoryEntry}", destVariable, maskVariable,options,
            ScriptCommands.Reset(nextCommand, "{List-DirectoryEntry}")));
        }
    }

    /// <summary>
    /// Serializable, List a directory contents.
    /// </summary>
    public class List : ScriptCommandBase
    {
        /// <summary>
        /// Entry to list, default = "Entry"
        /// </summary>
        public string EntryKey { get; set; }

        /// <summary>
        /// Where to store list result (IEntryModel[]), default = "Destination"
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// File based mask, support * and ?, comma separated, default = "*"
        /// </summary>
        public string MaskKey { get; set; }

        /// <summary>
        /// Whether return folder result, default = File | Folder
        /// </summary>
        public ListOptions ListOptions { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<List>();

        public List()
            : base("List")
        {
            EntryKey = "{Entry}";
            DestinationKey = "{Destination}";
            MaskKey = "*";
            ListOptions = Defines.ListOptions.File | Defines.ListOptions.Folder;
        }

        public static Func<IEntryModel, bool> createFilter(ListOptions listOptions, string mask)
        {
            if (!listOptions.HasFlag(ListOptions.File))
                return em => em.IsDirectory && StringUtils.MatchFileMasks(em.Name, mask);
            else if (!listOptions.HasFlag(ListOptions.Folder))
                return em => !em.IsDirectory && StringUtils.MatchFileMasks(em.Name, mask);
            else return em => StringUtils.MatchFileMasks(em.Name, mask);
        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IEntryModel[] entryModel;
            try
            {
                entryModel = await pm.GetValueAsEntryModelArrayAsync(EntryKey);
            }
            catch
            {
                return ResultCommand.Error(new ArgumentException(EntryKey + " is not found or not a directory IEntryModel or IEntryModel[]"));
            }
            string masks = pm.ReplaceVariableInsideBracketed(MaskKey);
            var filter = createFilter(ListOptions, masks);


            IEntryModel[] listItems = (await EntryUtils.ListAsync(entryModel, pm.CancellationToken, filter, false)).ToArray();

            logger.Debug(String.Format("{0} = IEntryModel[{1}]", DestinationKey, listItems.Length));
            pm.SetValue(DestinationKey, listItems);

            return NextCommand;
        }

    }

}
