using FileExplorer.Defines;
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
        /// Serializable, refresh file list.
        /// </summary>
        /// <param name="fileListVariable"></param>
        /// <param name="force"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListRefresh(string fileListVariable = "{FileList}", bool force = false, 
            IScriptCommand nextCommand = null)
        {
            return new FileListRefresh()
            {
                FileListKey = fileListVariable,
                Force = force,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, refresh the file list.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListRefresh(bool force = false,
            IScriptCommand nextCommand = null)
        {
            return FileListRefresh("{FileList}", force, nextCommand);
        }
    }

    public class FileListRefresh : ScriptCommandBase
    {
        /// <summary>
        /// Key for FileList (IFileListVIewModel) or Explorer (IExplorerViewModel), Default = {FileList}
        /// </summary>
        public string FileListKey { get; set; }

        /// <summary>
        /// Tell the IO to completely refresh, not just from cache.
        /// </summary>
        public bool Force { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FileListRefresh>();

        public FileListRefresh()
            : base("FileListRefresh")
        {
            FileListKey = "{FileList}";
            Force = false;
            ContinueOnCaptureContext = true;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var flValue = pm.GetValue(FileListKey);
            IFileListViewModel flvm = flValue is IExplorerViewModel ?
                (flValue as IExplorerViewModel).FileList :
                flValue as IFileListViewModel;

            if (flvm == null)
                return ResultCommand.Error(new KeyNotFoundException(FileListKey));

            await flvm.ProcessedEntries.EntriesHelper.LoadAsync(UpdateMode.Update, Force);
            return NextCommand;
        }
        
    }
}
