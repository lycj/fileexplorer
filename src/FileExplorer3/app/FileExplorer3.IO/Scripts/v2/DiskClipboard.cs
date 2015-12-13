using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Models;
using System.IO;
using System.Windows;
using FileExplorer.Models;

namespace FileExplorer.Script
{
    public static partial class IOScriptCommands
    {
        private static IScriptCommand diskClipboardOp(ClipboardOperation op, string entriesVariable = "{Entries}", 
            string currentDirectoryVariable = "{CurrentDirectory}", string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new DiskClipboard()
            {
                Operation = op,
                EntriesKey = entriesVariable,
                CurrentDirectoryEntryKey = currentDirectoryVariable,
                DestinationKey = destinationVariable, NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DiskCut(string entriesVariable = "{Entries}", IScriptCommand nextCommand = null)
        {
            return diskClipboardOp(ClipboardOperation.Cut, entriesVariable, null, null, nextCommand);
        }

        public static IScriptCommand DiskCopy(string entriesVariable = "{Entries}", IScriptCommand nextCommand = null)
        {
            return diskClipboardOp(ClipboardOperation.Copy, entriesVariable, null, null, nextCommand);
        }
        public static IScriptCommand DiskPaste(string currentDirectoryVariable = "{CurrentDirectory}", string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return diskClipboardOp(ClipboardOperation.Paste, null, currentDirectoryVariable, destinationVariable, nextCommand);
        }
    }

    public enum ClipboardOperation { Cut, Copy, Paste }

    public class DiskClipboard : ScriptCommandBase
    {
        private static byte[] preferCopy = new byte[] { 5, 0, 0, 0 };
        private static byte[] preferCut = new byte[] { 2, 0, 0, 0 };

        /// <summary>
        /// Source entries if copy or cut, Default = {Entries}.
        /// </summary>
        public string EntriesKey { get; set; }

        /// <summary>
        /// Current directory (IEntryModel) if paste, Default = {CurrentDirectory}.
        /// </summary>
        public string CurrentDirectoryEntryKey { get; set; }

        /// <summary>
        /// Destination entries (IEntryModel[]), Default = {Destination}.
        /// </summary>
        public string DestinationKey { get; set; }

        public ClipboardOperation Operation { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DiskClipboard>();

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            switch (Operation)
            {
                case ClipboardOperation.Copy:
                case ClipboardOperation.Cut:
                    var _srcModels = await pm.GetValueAsEntryModelArrayAsync(EntriesKey);
                    var da = _srcModels.First().Profile.DragDrop.GetDataObject(_srcModels);
                    byte[] moveEffect = Operation == ClipboardOperation.Cut ? preferCut : preferCopy;
                    MemoryStream dropEffect = new MemoryStream();
                    dropEffect.Write(moveEffect, 0, moveEffect.Length);
                    da.SetData("Preferred DropEffect", dropEffect);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(da, true);
                    break;
                case ClipboardOperation.Paste:
                    var currentDirectory = await pm.GetValueAsEntryModelAsync(CurrentDirectoryEntryKey);
                    if (currentDirectory != null)
                    {
                        IDataObject da1 = Clipboard.GetDataObject();
                        if (da1 != null)
                        {
                            IEntryModel[] srcModels = currentDirectory.Profile.DragDrop.GetEntryModels(da1).ToArray();
                            string sourceModelKey = "{Clipboard-SourceModels}";
                            return ScriptCommands.Assign(sourceModelKey, srcModels, false, 
                                IOScriptCommands.DiskTransfer(sourceModelKey, CurrentDirectoryEntryKey, DestinationKey, false, true, NextCommand));
                        }
                    }
                    break;
            }

            return NextCommand;
        }
    }
}
