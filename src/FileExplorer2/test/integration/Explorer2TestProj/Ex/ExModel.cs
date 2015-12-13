using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using QuickZip.Converters;
using System.Windows.Media;
using System.Drawing;
using QuickZip.UserControls.MVVM.ViewModel;
using System.IO;

namespace QuickZip.UserControls.MVVM.Model
{
    //public static class ExModelHelper
    //{
    //    public static EntryModel ToModel(this FileSystemInfoEx entry)
    //    {
    //        if (entry is DirectoryInfoEx)
    //            return new ExDirectoryModel(entry as DirectoryInfoEx);
    //        else if (entry is FileInfoEx)
    //            return new ExFileModel(entry as FileInfoEx);
    //        return null;
    //    }
    //}

    public class ExDirectoryModel : DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
    {
        internal static ExIconExtractor _iconExtractor = new ExIconExtractor();

        #region Constructor

        public ExDirectoryModel(DirectoryInfoEx dir)
            : base(dir)
        {
            IsSupportAdd = true;
        }
        #endregion

        #region Methods

        public override bool Open()
        {
            Process.Start(EmbeddedEntry.FullName);
            return true;
        }

        public override void Refresh(bool full = true)
        {
            Name = EmbeddedDirectory.Name;
            Label = EmbeddedDirectory.Label;
            ParseName = EmbeddedDirectory.FullName;

            if (full)
            {
                EntryTypeName = "Directory";
                HasSubDirectories = EmbeddedDirectory.HasSubFolder;
                CreationTime = EmbeddedDirectory.CreationTime;
                LastAccessTime = EmbeddedDirectory.LastAccessTime;
                LastWriteTime = EmbeddedDirectory.LastWriteTime;
            }
        }

        protected override IEnumerable<EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>> getSubEntries(string filter = "*", bool showDirectory = true, bool showFile = true)
        {
            IEnumerator<FileSystemInfoEx> enumerator =
                EmbeddedDirectory.EnumerateFileSystemInfos(filter).GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current is FileInfoEx ||
                    enumerator.Current is DirectoryInfoEx)
                {
                    if (enumerator.Current is FileInfoEx && showFile)
                        yield return new ExFileModel(enumerator.Current as FileInfoEx);
                    else if (enumerator.Current is DirectoryInfoEx && showDirectory)
                        yield return new ExDirectoryModel(enumerator.Current as DirectoryInfoEx);
                }
#if DEBUG
                else Debug.WriteLine("Error : ExAModel.GetSubEntries() - " + enumerator.Current.Name);
#endif

        }

        public override bool Equals(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> other)
        {
            if (other == null)
                return false;
            if (this.ParseName.Equals(other.ParseName))
                return true;
            if (this.EmbeddedEntry.Equals(other.EmbeddedEntry))
                return true;

            return false;
        }

        public override bool HasParent(DirectoryInfoEx directory)
        {
            return IOTools.HasParent(this.EmbeddedEntry, directory);
        }

        public override bool HasChild(FileSystemInfoEx entry)
        {
            return IOTools.HasParent(entry, this.EmbeddedDirectory);
        }

        public override void Rename(string newName)
        {
            IOTools.Rename(EmbeddedDirectory.FullName, newName);
        }

        public override string ToString()
        {
            return ParseName;
        }

        //public override void Put(FileSystemInfoEx[] entries)
        //{
        //    foreach (FileSystemInfoEx entry in entries)
        //        IOTools.Copy(entry.FullName, PathEx.Combine(EmbeddedDirectory.FullName, entry.Name));
        //}

        #endregion





        protected override DirectoryInfoEx getParent()
        {
            return EmbeddedEntry.Parent;
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Delete(FileSystemInfoEx[] entries)
        {
            throw new NotImplementedException();
        }


    }



    public class ExFileModel : FileModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
    {
        #region Constructor

        public ExFileModel(FileInfoEx file)
            : base(file)
        {
        }
        #endregion

        #region Methods

        public override bool Open()
        {
            Process.Start(EmbeddedEntry.FullName);
            return true;
        }

        public override void Refresh(bool full = true)
        {
            Name = EmbeddedFile.Name;
            Label = EmbeddedFile.Label;
            ParseName = EmbeddedFile.FullName;

            if (full)
            {
                Length = EmbeddedFile.Length;
                CreationTime = EmbeddedFile.CreationTime;
                LastAccessTime = EmbeddedFile.LastAccessTime;
                LastWriteTime = EmbeddedFile.LastWriteTime;
                EntryTypeName = "File";
            }
        }

        public override bool Equals(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> other)
        {
            return this.EmbeddedEntry.FullName.Equals(other.ParseName);
        }

        public override bool HasParent(DirectoryInfoEx directory)
        {
            return IOTools.HasParent(this.EmbeddedEntry, directory);
        }

        public override void Rename(string newName)
        {
            IOTools.Rename(EmbeddedFile.FullName, newName);
        }



        protected override DirectoryInfoEx getParent()
        {
            return EmbeddedFile.Parent;
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
