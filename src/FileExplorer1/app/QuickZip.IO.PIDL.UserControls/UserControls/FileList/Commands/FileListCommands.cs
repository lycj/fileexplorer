using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using Cinch;
using System.ComponentModel;
using System.Windows.Input;
using QuickZip.IO.PIDL.Tools;
using QuickZip.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using QuickZip.IO.PIDL.UserControls.Model;
using QuickZip.IO.PIDL.UserControls;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls
{
    public class FileListCommands : SharedCommands
    {
        public FileListCommands(FileList flist, FileListViewModel rootModel)
        {
            Func<ExModel[]> getSelectionFunc = new Func<ExModel[]>(() => 
            { return (from vm in rootModel.CurrentDirectoryModel.SelectedViewModels select vm.EmbeddedModel).ToArray(); });
            Func<DirectoryModel> getCurrentFunc = new Func<DirectoryModel>(() => { return rootModel.CurrentDirectoryModel.EmbeddedDirectoryModel; });
            Func<System.Drawing.Point> getMousePositionFunc = new Func<System.Drawing.Point>(() =>
            { Point pt = flist.PointToScreen(Mouse.GetPosition(flist)); return new System.Drawing.Point((int)pt.X, (int)pt.Y); });

            SetupCommands(getSelectionFunc, getCurrentFunc, getMousePositionFunc);
            SetupCommands(flist, rootModel);

            SimpleRoutedCommand.Register(typeof(FileList), RefreshCommand);
            SimpleRoutedCommand.Register(typeof(FileList), ContextMenuCommand);
            SimpleRoutedCommand.Register(typeof(FileList), NewFolderCommand);
            SimpleRoutedCommand.Register(typeof(FileList), OpenCommand, new KeyGesture(Key.Enter));
            SimpleRoutedCommand.Register(typeof(FileList), CopyCommand);
            SimpleRoutedCommand.Register(typeof(FileList), PasteCommand);
            SimpleRoutedCommand.Register(typeof(FileList), SelectAllCommand, new KeyGesture(Key.A, ModifierKeys.Control));
            SimpleRoutedCommand.Register(typeof(FileList), DeleteCommand, new KeyGesture(Key.Delete));
            SimpleRoutedCommand.Register(typeof(FileList), PropertiesCommand);
            SimpleRoutedCommand.Register(typeof(FileList), RenameCommand, new KeyGesture(Key.F2));
            SimpleRoutedCommand.Register(typeof(FileList), FindCommand, new KeyGesture(Key.F, ModifierKeys.Control));

            flist.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
                (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
                {
                    ApplicationCommands.ContextMenu.Execute(null, flist);
                }));

        }


        #region Methods

        public void SetupCommands(FileList flist, FileListViewModel rootModel)
        {
            #region IndividualItemCommand - Open, Rename
            OpenCommand = new SimpleRoutedCommand(ApplicationCommands.Open)
            {
                CanExecuteDelegate = x =>
                {
                    return rootModel.CurrentDirectoryModel != null && rootModel.CurrentDirectoryModel.SelectedCount == 1;
                },
                ExecuteDelegate = x =>
                {
                    rootModel.CurrentDirectoryModel.SelectedViewModels[0].Expand(rootModel,
                        rootModel.CurrentDirectoryModel.SelectedViewModels[0].EmbeddedModel);
                }
            };


            RenameCommand = new SimpleRoutedCommand(ApplicationCommands.SaveAs)
            {
                CanExecuteDelegate = x =>
                {
                    return rootModel.CurrentDirectoryModel != null && rootModel.CurrentDirectoryModel.SelectedCount == 1;
                },
                ExecuteDelegate = x =>
                {
                    rootModel.CurrentDirectoryModel.IsEditing = !rootModel.CurrentDirectoryModel.IsEditing;
                }
            };
           
            #endregion

            #region NewFolderCommand
            NewFolderCommand = new SimpleRoutedCommand(FileListCommands.NewFolder)
            {
                CanExecuteDelegate = x =>
                {
                    if ((rootModel.CurrentDirectoryModel.EmbeddedDirectoryModel.EmbeddedDirectoryEntry.Attributes & FileAttributes.ReadOnly) != 0)                    
                        return false;

                    if (x == null)
                        return true;

                    if (x is string)
                    {
                        string type = (string)x;
                        switch (type.ToLower())
                        {
                            case "zip":
                            case "7z": return true;
                        }
                    }

                    return false;
                },
                ExecuteDelegate = x =>
                {
                    string type = x as string;
                    if (x != null) type = type.ToLower();
                    rootModel.CurrentDirectoryModel.NewFolder();
                }
            };
            #endregion

            #region SelectAllCommand
            SelectAllCommand = new SimpleRoutedCommand(ApplicationCommands.SelectAll)
            {
                CanExecuteDelegate = x =>
                {
                    return rootModel.CurrentDirectoryModel != null && rootModel.CurrentDirectoryModel.HasSubEntries;
                },
                ExecuteDelegate = x =>
                {
                    if (rootModel.CurrentDirectoryModel.SelectedCount == rootModel.CurrentDirectoryModel.SubEntriesInternal.Count)
                        rootModel.CurrentDirectoryModel.UnselectAll();
                    else rootModel.CurrentDirectoryModel.SelectAll();
                }

            };
            #endregion

            #region RefreshCommand
            RefreshCommand = new SimpleRoutedCommand(NavigationCommands.Refresh)
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => rootModel.CurrentDirectoryModel.Refresh()
            };
            #endregion

            #region FindCommand

            FindCommand = new SimpleRoutedCommand(ApplicationCommands.Find)
            {
                CanExecuteDelegate = x =>
                {
                    return rootModel.CurrentDirectoryModel != null && !rootModel.CurrentDirectoryModel.IsEditing;
                },
                ExecuteDelegate = x =>
                {
                    flist.LookupAdorner.UpdateVisibilty(true);
                    FocusManager.SetFocusedElement(flist, flist.LookupAdorner);                                        
                }
            };


            #endregion
        }

        #endregion

        #region Data


        #endregion

        #region Public Properties

        public static RoutedUICommand NewFolder = new RoutedUICommand("New Folder", "NewFolder", typeof(FileListCommands));        

        public SimpleRoutedCommand SelectAllCommand { get; set; }        
        public SimpleRoutedCommand NewFolderCommand { get; set; }
        public override SimpleRoutedCommand RefreshCommand { get; set; }
        public override SimpleRoutedCommand RenameCommand { get; set; }
        public SimpleRoutedCommand FindCommand { get; set; }

        #endregion
    }
}
