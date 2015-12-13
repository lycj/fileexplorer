using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using Cinch;
using System.ComponentModel;
using System.Windows.Input;
using QuickZip.IO.PIDL.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.IO.Tools;
using QuickZip.IO.PIDL.UserControls.Model;

namespace QuickZip.IO.PIDL.UserControls
{
    public class DirectoryTreeCommands : SharedCommands
    {
        public DirectoryTreeCommands(DirectoryTree dtree, DirectoryTreeViewModel rootModel)
        {            
            Func<ExModel[]> getSelectionFunc = new Func<ExModel[]>(() => { return new ExModel[] { rootModel.SelectedDirectoryModel.EmbeddedModel }; });
            Func<DirectoryModel> getCurrentFunc = new Func<DirectoryModel>(() => { return rootModel.SelectedDirectoryModel.EmbeddedDirectoryModel; });
            Func<System.Drawing.Point> getMousePositionFunc = new Func<System.Drawing.Point>(() => 
            { Point pt = dtree.PointToScreen(Mouse.GetPosition(dtree)); return new System.Drawing.Point((int)pt.X, (int)pt.Y); });
            SetupCommands(getSelectionFunc, getCurrentFunc, getMousePositionFunc);
            SetupCommands(dtree, rootModel);

            SimpleRoutedCommand.Register(typeof(DirectoryTree), RefreshCommand);
            SimpleRoutedCommand.Register(typeof(DirectoryTree), RenameCommand, new KeyGesture(Key.F2));
            SimpleRoutedCommand.Register(typeof(DirectoryTree), ContextMenuCommand);
            SimpleRoutedCommand.Register(typeof(DirectoryTree), PropertiesCommand);
            SimpleRoutedCommand.Register(typeof(DirectoryTree), CopyCommand);
            SimpleRoutedCommand.Register(typeof(DirectoryTree), PasteCommand);
            SimpleRoutedCommand.Register(typeof(DirectoryTree), DeleteCommand, new KeyGesture(Key.Delete));

            dtree.AddHandler(TreeViewItem.MouseRightButtonUpEvent, new MouseButtonEventHandler(
                (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
                {
                    ApplicationCommands.ContextMenu.Execute(null, dtree);
                }));

        }


        #region Methods
        public void SetupCommands(DirectoryTree dtree, DirectoryTreeViewModel rootModel)
        {

            #region RefreshCommand
            RefreshCommand = new SimpleRoutedCommand(NavigationCommands.Refresh)
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => rootModel.SelectedDirectoryModel.Refresh()
            };
            #endregion

            #region RenamdCommand
            RenameCommand = new SimpleRoutedCommand(ApplicationCommands.SaveAs)
            {
                CanExecuteDelegate = x =>
                {
                    return rootModel.SelectedDirectoryModel != null &&
                        rootModel.SelectedDirectoryModel.EmbeddedModel.IsEditable;
                },
                ExecuteDelegate = x =>
                {
                    if (dtree._lastSelectedContainer != null)
                    {
                        DirectoryTree.SetIsEditing(dtree._lastSelectedContainer, true);
                    }
                }
            };
            #endregion RenamdCommand            
        }

        #endregion

        #region Data


        #endregion

        #region Public Properties
        
        public override SimpleRoutedCommand RefreshCommand { get; set; }
        public override SimpleRoutedCommand RenameCommand { get; set; }        

        #endregion
    }
}
