using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.IO.COFE;
using QuickZip.Converters;
using System.IO;
using System.Windows;
using System.IO.Tools;
using System.Diagnostics;
using QuickZip.UserControls.MVVM.Command.Model;
using System.Collections.Specialized;
using QuickZip.UserControls.MVVM.Notification.Model;
using QuickZip.UserControls.Logic.Tools.IconExtractor;

namespace QuickZip.UserControls.MVVM
{

    public abstract class Profile<FI, DI, FSI>
        where FI : FSI
        where DI : FSI
    {
        public abstract DirectoryModel<FI, DI, FSI> ConstructDirectoryModel(DI dir);
        public abstract FileModel<FI, DI, FSI> ConstructFileModel(FI file);
        public abstract FSI ConstructEntry(string parseName);
        public abstract string GetDiskPath(FSI entry, out bool isDir, bool createNowIfNotExist = true);

        /// <summary>
        /// Lookup is used by SearchViewModel (For the SimpleAutoCompleteTextBox in Breadcrumb) to suggest result based on input.
        /// </summary>
        /// <param name="lookupText"></param>
        /// <returns></returns>
        public abstract IEnumerable<Suggestion> Lookup(string lookupText);
        public abstract IEnumerable<GenericMetadataModel> GetMetadata(EntryModel<FI, DI, FSI>[] appliedModels);
        public abstract IEnumerable<NotificationSourceModel> GetNotificationSources();
        public abstract IEnumerable<CommandModel> GetCommands(EntryModel<FI, DI, FSI>[] appliedModels);
        public abstract string ShowContextmenu(Point screenPos, EntryModel<FI, DI, FSI>[] appliedModels);
        public abstract void ShowProperties(Point screenPos, EntryModel<FI, DI, FSI>[] appliedModels);

        public abstract AddActions GetSupportedAddActions(FSI[] entries, DirectoryModel<FI, DI, FSI> directoryModel);
        public abstract void Copy(FSI[] entries, DirectoryModel<FI, DI, FSI> directoryModel, bool allowThread = true);
        public abstract void Link(FSI[] entries, DirectoryModel<FI, DI, FSI> directoryModel);

        public abstract DI CreateDirectory(DirectoryModel<FI, DI, FSI> directoryModel, string name, string type);

        public Profile(IconExtractor<FSI> iconExtractor)
        {
            IconExtractor = iconExtractor;
        }

        /// <summary>
        /// Check the specified entryModel and see if it's under any of the root directories.
        /// </summary>
        /// <param name="entryModel"></param>
        /// <returns></returns>
        public bool IsInsideScope(EntryModel<FI, DI, FSI> entryModel)
        {
            foreach (var rd in RootDirectories)
                if (entryModel.HasParent(rd) || entryModel.EmbeddedEntry.Equals(rd))
                    return true;

            return false;
        }

        /// <summary>
        /// Check the specified entry and see if it's under any of the root directories.
        /// </summary>
        /// <param name="entryModel"></param>
        /// <returns></returns>
        public bool IsInsideScope(FSI entry)
        {
            return IsInsideScope(ConstructEntryModel(entry));
        }

        #region GetDiskPath

        public string GetDiskPath(FSI entry, bool createNowIfNotExist = true)
        {
            bool isDir;
            return GetDiskPath(entry, out isDir, createNowIfNotExist);
        }

        //public virtual IAsyncResult BeginCreateEntries(FSI[] entries)
        //{

        //}

        ////public virtual Dictionary<FSI, Tuple<bool, string>> GetDiskPath(FSI[] entries, bool createNowIfNotExist = true)
        ////{
        ////    Dictionary<FSI, Tuple<bool, string>> retVal = new Dictionary<FSI, Tuple<bool, string>>();

        ////    foreach (var entry in entries)
        ////    {
        ////        bool isDir;
        ////        string diskPath = GetDiskPath(entry, out isDir, createNowIfNotExist);
        ////        retVal.Add(entry, new Tuple<bool, string>(isDir, diskPath));
        ////    }
        ////    return retVal;
        ////}

        #endregion

        #region Put, Copy

        public virtual void Put(FSI[] entries, DirectoryModel<FI, DI, FSI> directoryModel, bool allowThread = true,
            AddActions supportedActions = AddActions.All)
        {
            if ((GetSupportedAddActions(entries, directoryModel) & AddActions.Copy) != 0 &&
                (supportedActions & AddActions.Copy) != 0)
                Copy(entries, directoryModel, allowThread);
            else
                if ((GetSupportedAddActions(entries, directoryModel) & AddActions.Link) != 0 &&
                (supportedActions & AddActions.Link) != 0)
                    Link(entries, directoryModel);
                else Trace.WriteLine("Profile.Put, not supported.");
        }

        public void Put(FSI entry, DirectoryModel<FI, DI, FSI> directoryModel, bool allowThread = true)
        {
            Put(new FSI[] { entry }, directoryModel, allowThread);
        }

        public void Put(EntryModel<FI, DI, FSI>[] entryModels, DirectoryModel<FI, DI, FSI> directoryModel, bool allowThread = true)
        {
            var entries = from m in entryModels select m.EmbeddedEntry;
            Put(entries.ToArray(), directoryModel, allowThread);
        }

        public void Put(EntryModel<FI, DI, FSI> entryModel, DirectoryModel<FI, DI, FSI> directoryModel, bool allowThread = true)
        {
            Put(new EntryModel<FI, DI, FSI>[] { entryModel }, directoryModel, allowThread);
        }

        public void Put(FSI[] entries, DI directory, bool allowThread = true)
        {
            DirectoryModel<FI, DI, FSI> directoryModel = ConstructDirectoryModel(directory);
            Put(entries, directoryModel, allowThread);
        }

        public void Put(FSI entry, DI directory, bool allowThread = true)
        {
            Put(new FSI[] { entry }, directory, allowThread);
        }

        #endregion Delete

        #region Commands
        public virtual IEnumerable<CommandModel> GetCommands(EntryModel<FI, DI, FSI> appliedModel)
        {
            return GetCommands(new EntryModel<FI, DI, FSI>[] { appliedModel });
        }
        #endregion

        #region Bookmarks


        /// <summary>
        /// Whether bookmarking is enabled for the specified entry.
        /// </summary>
        /// <param name="appliedModel"></param>
        /// <returns></returns>
        public virtual bool GetBookmarkEnabled(EntryModel<FI, DI, FSI> appliedModel)
        {
            return false;
        }

        /// <summary>
        /// If enabled, get if it's bookmarked.
        /// </summary>
        /// <param name="appliedModel"></param>
        /// <returns></returns>
        public virtual bool GetIsBookmarked(EntryModel<FI, DI, FSI> appliedModel)
        {
            return false;
        }


        public virtual string AddBookmark(EntryModel<FI, DI, FSI> appliedModel)
        {
            return null;
        }

        public virtual string RemoveBookmark(EntryModel<FI, DI, FSI> appliedModel)
        {
            return null;
        }

        #endregion

        #region Delete

        public virtual void Delete(FSI[] entries, DirectoryModel<FI, DI, FSI> directoryModel)
        {
            directoryModel.Delete(entries);
        }

        public virtual void Delete(FSI entry, DirectoryModel<FI, DI, FSI> directoryModel)
        {
            Delete(new FSI[] { entry }, directoryModel);
        }

        public void Delete(EntryModel<FI, DI, FSI>[] entryModels, DirectoryModel<FI, DI, FSI> directoryModel)
        {
            var entries = from m in entryModels select m.EmbeddedEntry;
            Delete(entries.ToArray(), directoryModel);
        }

        public void Delete(EntryModel<FI, DI, FSI> entryModel, DirectoryModel<FI, DI, FSI> directoryModel)
        {
            Delete(new EntryModel<FI, DI, FSI>[] { entryModel }, directoryModel);
        }

        #endregion

        #region ContextMenu, Properties

        public string ShowContextmenu(Point screenPos, EntryModel<FI, DI, FSI> appliedModel)
        {
            return ShowContextmenu(screenPos, new EntryModel<FI, DI, FSI>[] { appliedModel });
        }

        public void ShowProperties(Point screenPos, EntryModel<FI, DI, FSI> appliedModel)
        {
            ShowProperties(screenPos, new EntryModel<FI, DI, FSI>[] { appliedModel });
        }

        #endregion

        #region Open File / Directory


        public virtual void Open(DI directory)
        {
            string diskPath = GetDiskPath(directory);
            Process.Start(diskPath);
        }

        public virtual void Open(DirectoryModel<FI, DI, FSI> directoryModel)
        {
            Open(directoryModel.EmbeddedDirectory);
        }

        public virtual void Open(FI entry, OpenWithInfo openInfo = null)
        {
            string diskPath = GetDiskPath(entry);
            ProcessStartInfo psi = null;
            if (File.Exists(diskPath))
                if (openInfo != null)
                {
                    if (!openInfo.Equals(OpenWithInfo.OpenAs))
                        psi = new ProcessStartInfo(OpenWithInfo.GetExecutablePath(openInfo.OpenCommand), diskPath);
                    else
                    {
                        //http://bytes.com/topic/c-sharp/answers/826842-call-windows-open-dialog
                        psi = new ProcessStartInfo("Rundll32.exe");
                        psi.Arguments = String.Format(" shell32.dll, OpenAs_RunDLL {0}", diskPath);
                    }
                }
                else psi = new ProcessStartInfo(diskPath);

            if (psi != null)
                try { Process.Start(psi); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public virtual void Open(FileModel<FI, DI, FSI> fileModel, OpenWithInfo openInfo = null)
        {
            Open(fileModel.EmbeddedFile, openInfo);
        }

        public void Open(EntryModel<FI, DI, FSI> entryModel, OpenWithInfo openInfo = null)
        {
            if (entryModel is FileModel<FI, DI, FSI>)
                Open(entryModel as FileModel<FI, DI, FSI>, openInfo);
            else Open(entryModel as DirectoryModel<FI, DI, FSI>, openInfo);
        }
        #endregion

        #region Clipboard related

        public virtual void CopyToClipboard(FSI[] appliedEntries)
        {
            try
            {
                StringCollection fileList = new StringCollection();

                foreach (var entry in appliedEntries)
                    fileList.Add(GetDiskPath(entry));

                Clipboard.Clear();
                Clipboard.SetFileDropList(fileList);
            }
            catch (Exception ex)
            {
                throw new IOException("Copy to clipboard failed.", ex);
            }
        }

        public virtual void CopyToClipboard(EntryModel<FI, DI, FSI>[] appliedModels)
        {
            var appliedEntries = from m in appliedModels select m.EmbeddedEntry;
            CopyToClipboard(appliedEntries.ToArray());
        }

        public virtual void PasteFromClipboard(DirectoryModel<FI, DI, FSI> targetDirectory)
        {
            try
            {
                EntryModel<FI, DI, FSI>[] clipboardModels = getClipboardModels();
                if (clipboardModels.Length > 0)
                    Put(clipboardModels, targetDirectory);
            }
            catch (Exception ex)
            {
                throw new IOException("Paste from clipboard failed.", ex);
            }
        }

        protected virtual FSI[] getClipboardFiles()
        {
            List<FSI> entryList = new List<FSI>();

            foreach (string item in Clipboard.GetFileDropList())
                entryList.Add(ConstructEntry(item));

            return entryList.ToArray();
        }

        protected virtual EntryModel<FI, DI, FSI>[] getClipboardModels()
        {
            var modelList = from e in getClipboardFiles() select ConstructEntryModel(e);
            return modelList.ToArray();
        }

        #endregion

        #region ConstructModel - Implemented
        public EntryModel<FI, DI, FSI> ConstructEntryModel(FSI entry)
        {
            if (entry is DI)
                return ConstructDirectoryModel((DI)entry);
            else if (entry is FI)
                return ConstructFileModel((FI)entry);
            return null;
        }

        public FileViewModel<FI, DI, FSI> ConstructFileViewModel(FileModel<FI, DI, FSI> fileModel)
        {
            return new FileViewModel<FI, DI, FSI>(this, fileModel);
        }
        public DirectoryViewModel<FI, DI, FSI> ConstructDirectoryViewModel(DirectoryModel<FI, DI, FSI> dirModel)
        {
            return new DirectoryViewModel<FI, DI, FSI>(this, dirModel);
        }

        public DirectoryViewModel<FI, DI, FSI> ConstructDirectoryViewModel(DI dir)
        {
            return ConstructDirectoryViewModel(ConstructDirectoryModel(dir));
        }

        public FileViewModel<FI, DI, FSI> ConstructFileViewModel(FI file)
        {
            return ConstructFileViewModel(ConstructFileModel(file));
        }

        public EntryViewModel<FI, DI, FSI> ConstructEntryViewModel(EntryModel<FI, DI, FSI> model)
        {
            if (model is FileModel<FI, DI, FSI>)
                return ConstructFileViewModel(model as FileModel<FI, DI, FSI>);
            else if (model is DirectoryModel<FI, DI, FSI>)
                return ConstructDirectoryViewModel(model as DirectoryModel<FI, DI, FSI>);

            return null;
        }
        public EntryViewModel<FI, DI, FSI> ConstructEntryViewModel(FSI entry)
        {
            return ConstructEntryViewModel(ConstructEntryModel(entry));
        }
        #endregion

        #region Public Properties
                
        public IconExtractor<FSI> IconExtractor { get; private set; }
        public DI[] RootDirectories { get; set; }
        public DI DefaultRootDirectory { get; set; }

        #endregion
    }


}
