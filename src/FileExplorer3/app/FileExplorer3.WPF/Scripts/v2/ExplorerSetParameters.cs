using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.Script
{

    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable, change parameters of an IExplorerViewModel.
        /// </summary>
        /// <param name="explorerVariable"></param>
        /// <param name="parameterType"></param>
        /// <param name="valueVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerSetParameter(string explorerVariable = "{Explorer}",
           ExplorerParameterType parameterType = ExplorerParameterType.EnableDrag,
           object valueVariable = null, IScriptCommand nextCommand = null)
        {
            return new ExplorerParam()
            {
                Direction = ParameterDirection.Set,
                ExplorerKey = explorerVariable,
                ParameterType = parameterType,
                ValueKey = valueVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ExplorerSetParameter(
           ExplorerParameterType parameterType = ExplorerParameterType.EnableDrag,
           object valueVariable = null, IScriptCommand nextCommand = null)
        {
            return ExplorerSetParameter("{Explorer}", parameterType, valueVariable, nextCommand);
        }

        public static IScriptCommand ExplorerGetParameter(string explorerVariable = "{Explorer}",
           ExplorerParameterType parameterType = ExplorerParameterType.EnableDrag,
           string valueVariable = null, IScriptCommand nextCommand = null)
        {
            return new ExplorerParam()
            {
                Direction = ParameterDirection.Get,
                ExplorerKey = explorerVariable,
                ParameterType = parameterType,
                ValueKey = valueVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ExplorerGetParameter(
           ExplorerParameterType parameterType = ExplorerParameterType.EnableDrag,
           string valueVariable = null, IScriptCommand nextCommand = null)
        {
            return ExplorerGetParameter("{Explorer}", parameterType, valueVariable, nextCommand);
        }

        public static IScriptCommand ExplorerCopyParameter(string fromExplorerVariable = "{Explorer}",
           string toExplorerVariable = "{Explorer1}",
           ExplorerParameterType parameterType = ExplorerParameterType.EnableDrag,
           IScriptCommand nextCommand = null)
        {
            string copyParameterVariable = "{ExplorerCopyParameter}";
            return ExplorerGetParameter(fromExplorerVariable, parameterType, copyParameterVariable,
                ExplorerSetParameter(toExplorerVariable, parameterType, copyParameterVariable, nextCommand));
        }
    }

    public enum ExplorerParameterType
    {
        //Explorer
        RootModels,
        ExplorerWidth, ExplorerHeight, ExplorerPosition,

        //FilePicker
        FileName, FilePickerMode, FilterString,

        //FileList
        EnableContextMenu,
        EnableDrag, EnableDrop, EnableMultiSelect,
        ColumnList, ColumnFilters, ViewMode, ItemSize,
        ShowToolbar, ShowSidebar, ShowGridHeader, 

        //Breadcrumb
        EnableBookmark
    }

    public enum ParameterDirection
    {
        Set, Get
    }

    public class ExplorerParam : ScriptCommandBase
    {
        /// <summary>
        /// Point to Explorer (IExplorerViewModel) to be used.  Default = "{Explorer}".
        /// </summary>
        public string ExplorerKey { get; set; }

        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// The Parameter type to work with.
        /// </summary>
        public ExplorerParameterType ParameterType { get; set; }

        /// <summary>
        /// The value to set to, can be a key e.g. ({AnotherVariable}) or actual value (true).
        /// </summary>
        public object ValueKey { get; set; }


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerParam>();

        public ExplorerParam()
            : base("ExplorerSetParameters")
        {
            ContinueOnCaptureContext = true;
            ExplorerKey = "{Explorer}";
            ParameterType = ExplorerParameterType.EnableDrag;
            ValueKey = "true";
            Direction = ParameterDirection.Set;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);

            if (evm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));

            return
                Direction == ParameterDirection.Set ?
                await setParameterAsync(pm, evm) :
                await getParameterAsync(pm, evm);
        }

        private async Task<IScriptCommand> getParameterAsync(ParameterDic pm, IExplorerViewModel evm)
        {
            string ValueKeyString = ValueKey as string;
            if (ValueKeyString != null)
                switch (ParameterType)
                {
                    case ExplorerParameterType.EnableContextMenu:
                        pm.SetValue(ValueKeyString, evm.FileList.EnableContextMenu);
                        break;
                    case ExplorerParameterType.EnableDrag:
                        pm.SetValue(ValueKeyString, evm.FileList.EnableDrag);
                        break;
                    case ExplorerParameterType.EnableDrop:
                        pm.SetValue(ValueKeyString, evm.FileList.EnableDrop);
                        break;
                    case ExplorerParameterType.EnableMultiSelect:
                        pm.SetValue(ValueKeyString, evm.FileList.EnableMultiSelect);
                        break;
                    case ExplorerParameterType.ExplorerWidth :
                        pm.SetValue(ValueKeyString, evm.Parameters.Width);
                        break;
                    case ExplorerParameterType.ExplorerHeight:
                        pm.SetValue(ValueKeyString, evm.Parameters.Height);
                        break;
                    case ExplorerParameterType.ExplorerPosition:
                        pm.SetValue(ValueKeyString, evm.Parameters.Position);
                        break;
                    case ExplorerParameterType.RootModels:
                        pm.SetValue(ValueKeyString, evm.RootModels);
                        break;
                    case ExplorerParameterType.FileName :
                        if (evm is FilePickerViewModel)
                            pm.SetValue(ValueKeyString, (evm as FilePickerViewModel).FileName);
                        break;
                    case ExplorerParameterType.FilePickerMode:
                        if (evm is FilePickerViewModel)
                            pm.SetValue(ValueKeyString, (evm as FilePickerViewModel).PickerMode);
                        break;
                    case ExplorerParameterType.FilterString:
                        pm.SetValue(ValueKeyString, evm.FilterStr);
                        break;
                    case ExplorerParameterType.ColumnList:
                        pm.SetValue(ValueKeyString, evm.FileList.Columns.ColumnList);
                        break;
                    case ExplorerParameterType.ColumnFilters:
                        pm.SetValue(ValueKeyString, evm.FileList.Columns.ColumnFilters);
                        break;

                    case ExplorerParameterType.ViewMode:
                        pm.SetValue(ValueKeyString, evm.FileList.Parameters.ViewMode);
                        break;

                    case ExplorerParameterType.ItemSize:
                        pm.SetValue(ValueKeyString, evm.FileList.Parameters.ItemSize);
                        break;
                    case ExplorerParameterType.ShowToolbar:
                        pm.SetValue(ValueKeyString, evm.FileList.ShowToolbar);
                        break;
                    case ExplorerParameterType.ShowSidebar:
                        pm.SetValue(ValueKeyString, evm.FileList.ShowSidebar);
                        break;
                    case ExplorerParameterType.ShowGridHeader:
                        pm.SetValue(ValueKeyString, evm.FileList.ShowGridHeader);
                        break;
                    case ExplorerParameterType.EnableBookmark:
                        pm.SetValue(ValueKeyString, evm.Breadcrumb.EnableBookmark);
                        break;
                    default: return ResultCommand.Error(new NotSupportedException(ParameterType.ToString()));
                }

            logger.Debug(String.Format("Set {0} to ParameterDic[{1}]", ParameterType, ValueKey));

            return NextCommand;
        }

        private async Task<IScriptCommand> setParameterAsync(ParameterDic pm, IExplorerViewModel evm)
        {
            object value = ValueKey is string && (ValueKey as string).StartsWith("{") ? pm.GetValue<object>(ValueKey as string) : ValueKey;

            switch (ParameterType)
            {
                case ExplorerParameterType.EnableContextMenu:
                    if (value is bool)
                        evm.FileList.EnableContextMenu = evm.DirectoryTree.EnableContextMenu = true.Equals(value);
                    break;
                case ExplorerParameterType.EnableDrag:
                    if (value is bool)
                        evm.FileList.EnableDrag = evm.DirectoryTree.EnableDrag = true.Equals(value);
                    break;
                case ExplorerParameterType.EnableDrop:
                    if (value is bool)
                        evm.FileList.EnableDrop = evm.DirectoryTree.EnableDrop = true.Equals(value);
                    break;
                case ExplorerParameterType.EnableMultiSelect:
                    if (value is bool)
                        evm.FileList.EnableMultiSelect = true.Equals(value);
                    break;

                case ExplorerParameterType.ExplorerWidth:
                    if (value is int)
                        evm.Parameters.Width = (int)value;
                    break;
                case ExplorerParameterType.ExplorerHeight:
                    if (value is int)
                        evm.Parameters.Height = (int)value;
                    break;
                case ExplorerParameterType.ExplorerPosition:
                    if (value is Point)
                        evm.Parameters.Position = (Point)value;
                    break;
                case ExplorerParameterType.RootModels:
                    if (ValueKey == null)
                        return ResultCommand.Error(new ArgumentNullException("ValueKey"));

                    IEntryModel[] rootModels = ValueKey is string ?
                        await pm.GetValueAsEntryModelArrayAsync(ValueKey as string, null) :
                        ValueKey as IEntryModel[];
                    if (rootModels == null)
                        return ResultCommand.Error(new KeyNotFoundException(ValueKey.ToString()));
                    evm.RootModels = rootModels;

                    break;
                case ExplorerParameterType.FileName:
                    if (evm is FilePickerViewModel)
                        (evm as FilePickerViewModel).FileName = value as string;
                    break;
                case ExplorerParameterType.FilePickerMode:
                    var mode = pm.GetValue(ValueKey as string);
                    FilePickerMode pickerMode;
                    if (mode is FilePickerMode)
                        pickerMode = (FilePickerMode)mode;
                    else if (mode is string)
                        Enum.TryParse<FilePickerMode>(mode as string, out pickerMode);
                    else break;
                    if (evm is FilePickerViewModel)
                        (evm as FilePickerViewModel).PickerMode = pickerMode;
                    break;
                case ExplorerParameterType.FilterString:
                    string filterStr = pm.ReplaceVariableInsideBracketed(ValueKey as string);
                    if (filterStr != null)
                        evm.FilterStr = filterStr;
                    break;
                case ExplorerParameterType.ColumnList:
                    ColumnInfo[] columnInfo = pm.GetValue<ColumnInfo[]>(ValueKey as string);
                    if (columnInfo != null)
                        evm.FileList.Columns.ColumnList = columnInfo;
                    break;
                case ExplorerParameterType.ColumnFilters:
                    ColumnFilter[] columnfilters = pm.GetValue<ColumnFilter[]>(ValueKey as string);
                    if (columnfilters != null)
                        evm.FileList.Columns.ColumnFilters = columnfilters;
                    break;

                case ExplorerParameterType.ViewMode:
                    if (value is string)
                        evm.FileList.Parameters.ViewMode = value as string;
                    break;

                case ExplorerParameterType.ItemSize:

                    if (value is int)
                        evm.FileList.Parameters.ItemSize = (int)value;
                    break;

                case ExplorerParameterType.ShowToolbar:
                    if (value is bool)
                        evm.FileList.ShowToolbar = true.Equals(value);
                    break;
                case ExplorerParameterType.ShowSidebar:
                    if (value is bool)
                        evm.FileList.ShowSidebar = true.Equals(value);
                    break;
                case ExplorerParameterType.ShowGridHeader:
                    if (value is bool)
                        evm.FileList.ShowGridHeader = true.Equals(value);
                    break;
                case ExplorerParameterType.EnableBookmark:
                    if (value is bool)
                        evm.Breadcrumb.EnableBookmark = true.Equals(value);
                    break;
                default: return ResultCommand.Error(new NotSupportedException(ParameterType.ToString()));
            }

            logger.Debug(String.Format("Set {0} to {1} ({2})", ParameterType, ValueKey, value));

            return NextCommand;
        }
    }
}
