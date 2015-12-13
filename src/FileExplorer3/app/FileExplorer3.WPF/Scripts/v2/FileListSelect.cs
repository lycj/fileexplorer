using FileExplorer.Defines;
using FileExplorer.Models;
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
        /// Serializable, select the specified entries (IEnumerable or IEntryModel[]) in the filelist.
        /// </summary>
        /// <param name="fileListVariable"></param>
        /// <param name="entriesVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListSelect(string fileListVariable = "{FileList}", string entriesVariable = "{Entries}",
            IScriptCommand nextCommand = null)
        {
            return new FileListSelect()
            {
                FileListKey = fileListVariable,
                EntriesKey = entriesVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, refresh and select the specified entries (IEnumerable or IEntryModel[]) in the filelist.
        /// </summary>
        /// <param name="fileListVariable"></param>
        /// <param name="entriesVariable"></param>
        /// <param name="force"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListRefreshThenSelect(string fileListVariable = "{FileList}",
            string entriesVariable = "{Entries}", bool force = false,
            IScriptCommand nextCommand = null)
        {
            return UIScriptCommands.FileListRefresh(force,
                FileListSelect(fileListVariable, entriesVariable, nextCommand));
        }

        /// <summary>
        /// Serializable, select the specified entries (IEnumerable or IEntryModel[]) in the filelist.
        /// </summary>
        /// <param name="entriesVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListSelect(string entriesVariable = "{Entries}",
            IScriptCommand nextCommand = null)
        {
            {
                return FileListSelect("{FileList}", entriesVariable, nextCommand);
            }
        }
    }

    public class FileListSelect : ScriptCommandBase
    {
        /// <summary>
        /// Key for FileList (IFileListVIewModel) or Explorer (IExplorerViewModel), Default = {FileList}
        /// </summary>
        public string FileListKey { get; set; }

        ///// <summary>
        ///// File based mask, support * and ?, comma separated, default = "*"
        ///// </summary>
        //public string MaskKey { get; set; }

        ///// <summary>
        ///// Whether return folder result, default = File | Folder
        ///// </summary>
        //public ListOptions ListOptions { get; set; }

        /// <summary>
        /// Entries (IEntryModel or IEntryModel[]) to select, default = {Selection}
        /// </summary>
        public string EntriesKey { get; set; }


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FileListSelect>();

        public FileListSelect()
            : base("FileListSelect")
        {
            FileListKey = "{FileList}";
            EntriesKey = "{Selection}";
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var flValue = pm.GetValue(FileListKey);
            IFileListViewModel flvm = flValue is IExplorerViewModel ?
                (flValue as IExplorerViewModel).FileList :
                flValue as IFileListViewModel;
            IEntryModel[] ems = await pm.GetValueAsEntryModelArrayAsync(EntriesKey, null);

            if (flvm == null)
                return ResultCommand.Error(new KeyNotFoundException(FileListKey));

            logger.Info(String.Format("Select {0} {1}", FileListKey, EntriesKey));
            using (var releaser = await flvm.ProcessedEntries.EntriesHelper.LoadingLock.LockAsync())
            {
                flvm.Selection.Select(evm => evm != null && ems.Contains(evm.EntryModel));
            }

            return NextCommand;
        }

    }

}
