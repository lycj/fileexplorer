using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using QuickZip.Translation;

namespace QuickZip.UserControls.Input
{
    public static class ExplorerCommands
    {
        //FileList
        public static RoutedUICommand Settings = new RoutedUICommand(Texts.strSettings, "Settings", typeof(ExplorerCommands));

        //FileList
        public static RoutedUICommand NewFolder = new RoutedUICommand("New Folder", "NewFolder", typeof(ExplorerCommands));

        //Preview
        public static RoutedUICommand TogglePreview = new RoutedUICommand("", "TogglePreview", typeof(ExplorerCommands));           

        //Breadcrumb
        public static RoutedUICommand ToggleBookmark = new RoutedUICommand("Bookmark", "ToggleBookmark", typeof(ExplorerCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));

        public static RoutedUICommand Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(ExplorerCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.R, ModifierKeys.Control) }));

        public static void UpdateTranslations()
        {
            Settings.Text = Texts.strSettings;
        }
    }
}
