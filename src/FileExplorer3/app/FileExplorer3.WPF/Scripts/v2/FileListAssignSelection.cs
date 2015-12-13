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
        /// Serializable, assign selected models (IEntryModel[]) to a variable.
        /// </summary>
        /// <param name="fileListVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListAssignSelection(string fileListVariable = "{FileList}",
            string destinationVariable = "{Selection}", IScriptCommand nextCommand = null)
        {
            return new FileListAssignSelection()
            {
                FileListKey = fileListVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, assign selected models (IEntryModel[]) to a variable.
        /// </summary>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FileListAssignSelection(string destinationVariable = "{Selection}", 
            IScriptCommand nextCommand = null)
        {
            return FileListAssignSelection("{FileList}", destinationVariable, nextCommand);
        }

        public static IScriptCommand FileListIfSelectionLength(string fileListVariable = "{FileList}", 
            ComparsionOperator op = ComparsionOperator.Equals, int value = 1, 
            IScriptCommand thenCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string fileListSelectionVariable = ParameterDic.CombineVariable(fileListVariable, "Selection");
            return  UIScriptCommands.FileListAssignSelection(fileListSelectionVariable,
               ScriptCommands.IfArrayLength(op, fileListSelectionVariable, value, thenCommand, otherwiseCommand));
        }

        public static IScriptCommand FileListIfSelectionLength(
            ComparsionOperator op = ComparsionOperator.Equals, int value = 1, 
            IScriptCommand thenCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return FileListIfSelectionLength("{FileList}", op, value, thenCommand, otherwiseCommand);
        }
    }

    public class FileListAssignSelection : ScriptCommandBase
    {
        /// <summary>
        /// Key for FileList (IFileListVIewModel) or Explorer (IExplorerViewModel), Default = {FileList}
        /// </summary>
        public string FileListKey { get; set; }

        /// <summary>
        /// Key to set the select value (IEntryModel[]) to, Default = {Selection}
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FileListAssignSelection>();

        public FileListAssignSelection()
            : base("FileListAssignSelection")
        {
            FileListKey = "{FileList}";
            DestinationKey = "{Selection}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var flValue = pm.GetValue(FileListKey);
            IFileListViewModel flvm = flValue is IExplorerViewModel ?
                (flValue as IExplorerViewModel).FileList :
                flValue as IFileListViewModel;

            if (flvm == null)
                return ResultCommand.Error(new KeyNotFoundException(FileListKey));

            return ScriptCommands.Assign(DestinationKey, 
                flvm.Selection.SelectedItems.Select(evm => evm.EntryModel).ToArray(), false, NextCommand);
        }

    }
}
