using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Collections.ObjectModel;

namespace WPFDemo
{
    public class DirectoryListEx
    {
        private static FileSystemWatcherEx watcher =
            new FileSystemWatcherEx(DirectoryInfoEx.DesktopDirectory);

        private ObservableCollection<DirectoryInfoEx> dirs = new ObservableCollection<DirectoryInfoEx>();
        public ObservableCollection<DirectoryInfoEx> Directories { get { return dirs; } }


        private void invalidate()
        {
            //Only changes the ones required.
            List<DirectoryInfoEx> addedList = new List<DirectoryInfoEx>();
            List<DirectoryInfoEx> removedList = new List<DirectoryInfoEx>(dirs);

            foreach (DirectoryInfoEx subdir in rootDir.GetDirectories())
            {
                if (!dirs.Contains(subdir))
                    addedList.Add(subdir);
                if (removedList.Contains(subdir))
                    removedList.Remove(subdir);
            }

            foreach (DirectoryInfoEx subdir in addedList)
                dirs.Add(subdir);
            foreach (DirectoryInfoEx subdir in removedList)
                if (dirs.Contains(subdir))
                    dirs.Remove(subdir);

            //dirs.Clear();
            //foreach (DirectoryInfoEx subdir in rootDir.GetDirectories())
            //    dirs.Add(subdir);
        }

        private DirectoryInfoEx rootDir;
        internal DirectoryListEx(DirectoryInfoEx dir)
        {
            rootDir = dir;
            invalidate();

            var handler = (FileSystemEventHandlerEx)delegate(object sender, FileSystemEventArgsEx args)
            {
                string path1 = args.FullPath;
                if (String.Equals(path1, rootDir.FullName, StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(PathEx.GetDirectoryName(path1), rootDir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    invalidate();
            };

            var renamehandler = (RenameEventHandlerEx)delegate(object sender, RenameEventArgsEx args)
            {
                string path1 = args.OldFullPath;

                if (String.Equals(path1, rootDir.FullName, StringComparison.InvariantCultureIgnoreCase) ||
                    String.Equals(PathEx.GetDirectoryName(path1), rootDir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    invalidate();
            };

            //watcher.OnChanged += handler;
            watcher.OnCreated += handler;
            watcher.OnDeleted += handler;
            watcher.OnRenamed += renamehandler;
        }
    }


    public class GetDirectoriesConverterEx : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is DirectoryInfoEx && !value.Equals(DirectoryInfoEx.NetworkDirectory))
                    return (new DirectoryListEx(value as DirectoryInfoEx)).Directories;
                    //return (value as DirectoryInfoEx).GetDirectories();
            }
            catch
            {                
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
