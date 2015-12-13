using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
//using IntelleSoft.BugTrap;
using System.Diagnostics;
using QuickZip.IO;
using System.IO;
using System.IO.Tools;
using System.Windows.Threading;

namespace FileExplorerText
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        public static bool AskOverwriteDialog(string srcName, long srcSize, DateTime srcTime, string srcCRC,
            string destName, long destSize, DateTime destTime, string destCRC)
        {
            bool overwrite = false;

            string compSize = "";
            string compTime = "";
            string compCRC = "";
            if (srcSize == destSize) compSize = "(Same)"; else compSize = "(Different)";
            if (srcTime == destTime) compTime = "(Same)"; else compTime = "(Different)";
            if (srcCRC == destCRC) compCRC = "(Same)"; else compCRC = "(Different)";


            string MsgText =
            String.Format("Overwrite {0}? \r\n" +
                          "Full path - {1} \r\n" +
                          "\r\n" +
                          "Original file - {2} \r\n" +
                          "Date - {3} \r\n" +
                          "CRC - {4} \r\n" +
                          "\r\n" +
                          "New file - {5} {6} \r\n" +
                          "Date - {7} {8} \r\n" +
                          "CRC - {9} {10} ",
                          new object[] 
                          {
                            PathEx.GetFileName(destName), srcName,
                            Helper.SizeInK((ulong)destSize), destTime, destCRC,
                            Helper.SizeInK((ulong)srcSize), compSize,
                            srcTime, compTime,
                            srcCRC, compCRC
                          });

            if (MessageBox.Show(MsgText, "Overwrite", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                overwrite = true;

            return overwrite;
        }

        public static void AskOverwriteDialog(FileSystemInfoEx srcEntry, FileSystemInfoEx destEntry,
            ref bool overwrite, ref bool applyAll)
        {
            string destName = destEntry.Name;
            long destSize = (destEntry is FileInfoEx) ? (destEntry as FileInfoEx).Length : 0;
            DateTime destTime = destEntry.LastWriteTime;
            string destCRC = "";
            //if (!destEntry.IsFolder || srcEntry.IsArchiveRoot)
            //    destCRC = destEntry.CRC;

            string srcName = srcEntry.Name;
            long srcSize = (srcEntry is FileInfoEx) ? (srcEntry as FileInfoEx).Length : 0;
            DateTime srcTime = srcEntry.LastWriteTime;
            string srcCRC = "";
            //if (!srcEntry.IsFolder || srcEntry.IsArchiveRoot)
            //    srcCRC = srcEntry.CRC;

            overwrite = AskOverwriteDialog(srcName, srcSize, srcTime, srcCRC, destName, destSize, destTime, destCRC);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //ExceptionHandler.AppName = "FileExplorer";
            //ExceptionHandler.Flags = FlagsType.DetailedMode | FlagsType.EditMail | FlagsType.AttachReport;
            //ExceptionHandler.DumpType = MinidumpType.NoDump;
            //ExceptionHandler.SupportEMail = "bugtrap@quickzip.org";
            //ExceptionHandler.SupportURL = "http://www.quickzip.org";


            WorkSpawner.WorkOverwrite += new WorkOverwriteEventHandler(WorkSpawner_WorkOverwrite);

            base.OnStartup(e);
        }

        void WorkSpawner_WorkOverwrite(object sender, WorkOverwriteEventArgs e)
        {
            bool overwrite = true;
            bool applyAll = false;
            AskOverwriteDialog(e.SrcEntry, e.DestEntry, ref overwrite, ref applyAll);
            if (overwrite)
                e.Overwrite = OverwriteMode.Replace;
            else e.Overwrite = OverwriteMode.KeepOriginal;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }
        
    }
}
