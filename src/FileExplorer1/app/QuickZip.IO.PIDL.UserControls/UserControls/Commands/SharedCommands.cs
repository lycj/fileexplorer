using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using QuickZip.IO.PIDL.UserControls.Model;
using System.Windows.Input;
using QuickZip.IO.PIDL.Tools;
using System.Diagnostics;
using System.IO.Tools;
using System.IO;
using System.Windows;
using System.Collections.Specialized;

namespace QuickZip.IO.PIDL.UserControls
{
    public abstract class SharedCommands
    {

        protected void SetupCommands(Func<ExModel[]> getSelectionFunc,
            Func<DirectoryModel> getCurrentFunc, Func<System.Drawing.Point> getMousePositionFunc)
        {
            #region OpenCommand and ContextMenuCommand
            OpenCommand = new SimpleRoutedCommand(ApplicationCommands.Open)
            {
                CanExecuteDelegate = x => { return getCurrentFunc() != null; },
                ExecuteDelegate = x => { Process.Start(getCurrentFunc().EmbeddedDirectoryEntry.FullName); }
            };

            ContextMenuCommand = new SimpleRoutedCommand(ApplicationCommands.ContextMenu)
            {
                CanExecuteDelegate = x =>
                {
                    return getSelectionFunc() != null && getSelectionFunc().Length > 0;
                },
                ExecuteDelegate = x =>
                {
                    ContextMenuWrapper _cmw = new ContextMenuWrapper();

                    _cmw.OnBeforeInvokeCommand += (InvokeCommandEventHandler)delegate(object sender, InvokeCommandEventArgs args)
                    {
                        if (args.Command == "open")
                            args.ContinueInvoke = false;
                        if (args.Command == "openas" && args.SelectedItems != null
                            && args.SelectedItems.Length == 1)
                            args.ContinueInvoke = false;
                    };
                    var selectedItems = (from model in getSelectionFunc() select model.EmbeddedEntry).ToArray();

                    System.Drawing.Point pt = getMousePositionFunc();
                    string command = _cmw.Popup(selectedItems, pt);
                    switch (command)
                    {
                        case "open": OpenCommand.Execute(null); break;
                        case "openas": OpenCommand.Execute(null); break;
                        case "rename": RenameCommand.Execute(null); break;
                        case "refresh": RefreshCommand.Execute(null); break;
                    }
                }
            };
            #endregion

            #region Delete, Copy and Paste
            DeleteCommand = new SimpleRoutedCommand(ApplicationCommands.Delete)
            {
                CanExecuteDelegate = x =>
                {
                    if (getSelectionFunc() != null && getSelectionFunc().Length > 0) ;
                    {
                        var selectedItems = (from model in getSelectionFunc() select model.EmbeddedEntry).ToArray();
                        foreach (FileSystemInfoEx item in selectedItems)
                        {
                            if ((item.Attributes & FileAttributes.ReadOnly) != 0)
                                return false;
                        }
                        return true;
                    }
                    //return false;
                },
                ExecuteDelegate = x =>
                {
                    var selectedItems = (from model in getSelectionFunc() select model.EmbeddedEntry).ToArray();
                    int itemCount = selectedItems.Length;
                    if (System.Windows.Forms.MessageBox.Show(String.Format("Are you sure want to permanently remove these {0} item{1}?",
                        itemCount, itemCount > 1 ? "s" : ""), "Delete", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        foreach (FileSystemInfoEx item in selectedItems)
                            item.Delete();
                }
            };

            CopyCommand = new SimpleRoutedCommand(ApplicationCommands.Copy)
            {
                CanExecuteDelegate = x =>
                {
                    return getSelectionFunc() != null && getSelectionFunc().Length > 0;
                },
                ExecuteDelegate = x =>
                {
                    var selectedItems = (from model in getSelectionFunc() select model.EmbeddedEntry).ToArray();


                    StringCollection fileList = new StringCollection();

                    foreach (FileSystemInfoEx item in selectedItems)
                        fileList.Add(item.FullName);

                    Clipboard.Clear();
                    Clipboard.SetFileDropList(fileList);
                }
            };


            PasteCommand = new SimpleRoutedCommand(ApplicationCommands.Paste)
            {
                CanExecuteDelegate = x =>
                {
                    return
                        getCurrentFunc() != null &&
                        ((getCurrentFunc().EmbeddedDirectoryEntry.Attributes & FileAttributes.ReadOnly) != 0) &&
                        Clipboard.ContainsFileDropList();
                },
                ExecuteDelegate = x =>
                {
                    DirectoryInfoEx parentDir = getCurrentFunc().EmbeddedDirectoryEntry;
                    List<FileSystemInfoEx> entryList = new List<FileSystemInfoEx>();
                    foreach (string path in Clipboard.GetFileDropList())
                        IOTools.Copy(path, PathEx.Combine(parentDir.FullName, PathEx.GetFileName(path)));                        
                }
            };
            #endregion

            #region PropertiesCommand
            PropertiesCommand = new SimpleRoutedCommand(ApplicationCommands.Properties)
            {
                CanExecuteDelegate = x =>
                {
                    return getSelectionFunc() != null && getSelectionFunc().Length > 0;
                },
                ExecuteDelegate = x =>
                {
                    System.Windows.Point position = Mouse.GetPosition(null);
                    var selectedItems = (from model in getSelectionFunc() select model.EmbeddedEntry).ToArray();

                    ContextMenuHelper.InvokeCommand(getSelectionFunc()[0].EmbeddedEntry.Parent,
                        selectedItems, "properties", new System.Drawing.Point((int)position.X, (int)position.Y));
                }
            };
            #endregion
        }


        #region Data


        #endregion

        #region Public Properties

        public SimpleRoutedCommand ContextMenuCommand { get; set; }
        public SimpleRoutedCommand CopyCommand { get; set; }
        public SimpleRoutedCommand PasteCommand { get; set; }
        public SimpleRoutedCommand DeleteCommand { get; set; }
        public SimpleRoutedCommand PropertiesCommand { get; set; }
        public SimpleRoutedCommand OpenCommand { get; set; }

        public abstract SimpleRoutedCommand RefreshCommand { get; set; }
        public abstract SimpleRoutedCommand RenameCommand { get; set; }

        #endregion
    }
}
