using Caliburn.Micro;
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
        /// Serializable, Create a new explorer window (IExplorerViewModel), and show it.
        /// </summary>
        /// <param name="onModelCreatedVariable"></param>
        /// <param name="onViewAttachedVariable"></param>
        /// <param name="windowManagerVariable"></param>
        /// <param name="eventAggregatorVariable"></param>
        /// <param name="explorerVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerNewWindow(
           string onModelCreatedVariable = "{OnModelCreated}", string onViewAttachedVariable = "{OnViewAttached}",
            string windowManagerVariable = "{WindowManager}", string eventAggregatorVariable = "{GlobalEvents}",
          string explorerVariable = "{Explorer}", IScriptCommand nextCommand = null
          )
        {
            return ExplorerCreate(ExplorerMode.Normal, onModelCreatedVariable, onViewAttachedVariable, windowManagerVariable, eventAggregatorVariable, 
                explorerVariable,
                explorerShow(windowManagerVariable, explorerVariable, null, null, null, nextCommand)); 
        }


        private static IScriptCommand explorerShow(
            string windowManagerVariable = "{WindowManager}", 
            string explorerVariable = "{Explorer}", string dialogResultVariable = "{DialogResult}",
            string selectionEntriesVariable = "{Selection}", string selectionPathsVariable = "{SelectionPaths}",
            IScriptCommand nextCommand = null
            )
        {
            return new ExplorerShow()
            {
                WindowManagerKey = windowManagerVariable,
                ExplorerKey = explorerVariable,                
                DialogResultKey = dialogResultVariable,
                SelectionEntriesKey = selectionEntriesVariable,
                SelectionPathsKey = selectionPathsVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Serializable, Create a new directory or file picker explorer window (IExplorerViewModel), and show it.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="onModelCreatedVariable"></param>
        /// <param name="onViewAttachedVariable"></param>
        /// <param name="windowManagerVariable"></param>
        /// <param name="eventAggregatorVariable"></param>
        /// <param name="selectionVariable"></param>
        /// <param name="selectionPathsVariable"></param>
        /// <param name="nextCommand"></param>
        /// <param name="cancelCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ExplorerPick(ExplorerMode mode = ExplorerMode.FileSave, string onModelCreatedVariable = "{OnModelCreated}", string onViewAttachedVariable = "{OnViewAttached}",
            string windowManagerVariable = "{WindowManager}", string eventAggregatorVariable = "{GlobalEvents}",
            string selectionVariable = null,
            string selectionPathsVariable = "{SelectionPaths}",
            IScriptCommand nextCommand = null, IScriptCommand cancelCommand = null)
        {
            string dialogResultVariable = "{ExplorerPick-DialogResult}";
            string explorerVariable = "{ExplorerPick-Explorer}";

            return ExplorerCreate(mode, onModelCreatedVariable, onViewAttachedVariable,
                  windowManagerVariable, eventAggregatorVariable, explorerVariable, 
                  UIScriptCommands.explorerShow(windowManagerVariable, explorerVariable, dialogResultVariable, selectionVariable,
                        selectionPathsVariable, 
                        ScriptCommands.IfTrue(dialogResultVariable, nextCommand, cancelCommand)));
        }
    }

    
    public class ExplorerShow : ScriptCommandBase
    {
        /// <summary>
        /// WindowManager used to show the window, optional, Default={WindowManager}
        /// </summary>
        public string WindowManagerKey { get; set; }
        
        /// <summary>
        /// Show this IExplorerViewModel, default={Explorer}
        /// </summary>
        public string ExplorerKey { get; set; }


        /// <summary>
        /// Point to a boolean, indicate whether the dialog is confirmed or cancel, Default = {DialogResult}
        /// </summary>
        public string DialogResultKey { get; set; } 

          /// <summary>
        /// Point to an IEntryModel of selected file or folders, in FileOpenMode only , Default = {Selection}
        /// </summary>
        public string SelectionEntriesKey { get; set; }

        /// <summary>
        /// Point to a string[] (string if FileSave) of selected file or folders, Default = {SelectionPaths}
        /// </summary>
        public string SelectionPathsKey { get; set; }


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ExplorerShow>();

        public ExplorerShow()
            : base("ExplorerShow")
        {
            WindowManagerKey = "{WindowManager}";
            ExplorerKey = "{Explorer}";
            DialogResultKey = "{DialogResult}";
            SelectionEntriesKey = "{Selection}";
            SelectionPathsKey = "{SelectionPaths}";
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IWindowManager wm = pm.GetValue<IWindowManager>(WindowManagerKey) ?? new WindowManager();
            IExplorerViewModel evm = pm.GetValue<IExplorerViewModel>(ExplorerKey);
            if (evm == null)
                return ResultCommand.Error(new KeyNotFoundException(ExplorerKey));
            logger.Info(String.Format("Showing {0}", evm));
            
            if (evm is DirectoryPickerViewModel)
            {
                DirectoryPickerViewModel dpvm = evm as DirectoryPickerViewModel;
                 bool result = wm.ShowDialog(dpvm).Value;
                pm.SetValue(DialogResultKey, result);

                if (result)
                {
                    pm.SetValue(SelectionPathsKey, dpvm.SelectedDirectory.FullPath);
                            pm.SetValue(SelectionEntriesKey, dpvm.SelectedDirectory);
                }
            }
            else  if (evm is FilePickerViewModel)
            {
                FilePickerViewModel fpvm = evm as FilePickerViewModel;
                 bool result = wm.ShowDialog(fpvm).Value;
                pm.SetValue(DialogResultKey, result);
                if (result)
                {
                    switch (fpvm.PickerMode)
                    {
                        case FilePickerMode.Save:
                            pm.SetValue(SelectionPathsKey, fpvm.FileName);
                            break;
                        case FilePickerMode.Open:
                            pm.SetValue(SelectionPathsKey, fpvm.SelectedFiles.Select(m => m.FullPath).ToArray());
                            pm.SetValue(SelectionEntriesKey, fpvm.SelectedFiles);
                            break;
                    }
                }

            }
            else
            {
                 wm.ShowWindow(evm);
            }

            return NextCommand;
        }
    }
}
