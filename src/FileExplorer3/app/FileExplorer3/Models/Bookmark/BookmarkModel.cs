using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Models.Bookmark
{
    public class BookmarkModel : NotifyPropertyChanged, IEntryLinkModel
    {
        private string _fullPath;
        private string _name;
        private IProfile _profile;
        #region fields

        public enum BookmarkEntryType { Root, Directory, Link }

        #endregion

        #region constructors

        public BookmarkModel()
        {
            SubModels = new List<BookmarkModel>();
        }

        public BookmarkModel(BookmarkProfile profile, BookmarkEntryType type, string fullPath)
            : this()
        {
            Profile = profile;
            this.Type = type;
            FullPath = fullPath;

            IsRenamable = true;
        }

        #endregion

        #region events


        #endregion

        #region properties

        [XmlIgnore]
        public IProfile Profile
        {
            get { return _profile; }
            internal set { _profile = value; foreach (var m in SubModels) m.Profile = value; }
        }

        public BookmarkEntryType Type
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsDirectory
        {
            get { return Type != BookmarkEntryType.Link; }
        }

        public IEntryModel Parent
        {
            get
            {
                string parentPath = Profile.Path.GetDirectoryName(FullPath);
                return AsyncUtils.RunSync(() => Profile.ParseAsync(parentPath));
            }
        }

        [XmlIgnore]
        public string Label
        {
            get { return Name; }
            //protected set { Name = value; }
        }

        [XmlIgnore]
        public string Name
        {
            get { return _name; }
            set { renameAsync(value); }
        }

        private async Task renameAsync(string value)
        {
            switch (Type)
            {
                case BookmarkEntryType.Link:
                    FullPath = Profile.Path.Combine(Profile.Path.GetDirectoryName(FullPath), value);
                    break;
                case BookmarkEntryType.Directory:
                case BookmarkEntryType.Root:
                    string newFullPath = Profile.Path.Combine(Profile.Path.GetDirectoryName(FullPath), value);
                    var subEntries = await Profile.ListRecursiveAsync(this, System.Threading.CancellationToken.None, em => true, em => true);
                    foreach (var subEntry in subEntries.Cast<BookmarkModel>())
                        subEntry.FullPath = subEntry.FullPath.Replace(FullPath, newFullPath);
                    FullPath = newFullPath;
                    break;
            }
            NotifyOfPropertyChanged(() => Name, () => FullPath, () => Label);
        }

        public string Description
        {
            get;
            set;
        }

        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                _fullPath = value;
                _name = PathHelper.Web.GetFileName(value);
                NotifyOfPropertyChanged(() => FullPath, () => Label, () => Name);
            }
        }

        public bool IsRenamable
        {
            get;
            set;
        }

        public DateTime CreationTimeUtc
        {
            get;
            set;
        }

        public DateTime LastUpdateTimeUtc
        {
            get;
            set;
        }

        public List<BookmarkModel> SubModels
        {
            get;
            set;
        }

        #endregion

        #region methods

        public bool Equals(IEntryModel other)
        {
            return other is BookmarkModel && (other as BookmarkModel).FullPath.Equals(FullPath);
        }

        public override string ToString()
        {
            return FullPath;
        }

        public BookmarkModel AddLink(string label, string linkPath)
        {
            var nameGenerator = FileNameGenerator.Rename(label);
            while (SubModels.Any(bm => bm.Label == label))
                label = nameGenerator.Generate();

            var retVal = new BookmarkModel(Profile as BookmarkProfile, BookmarkEntryType.Link,
                FullPath + "/" + label) { LinkPath = linkPath, Name = label };
            SubModels.Add(retVal);
            (Profile as BookmarkProfile).RaiseEntryChanged(new EntryChangedEvent(ChangeType.Created, retVal.FullPath));
            return retVal;
        }

        public BookmarkModel AddFolder(string label)
        {
            var nameGenerator = FileNameGenerator.Rename(label);
            while (SubModels.Any(bm => bm.Label == label))
                label = nameGenerator.Generate();

            var retVal = new BookmarkModel(Profile as BookmarkProfile, BookmarkEntryType.Directory,
                FullPath + "/" + label) { Name = label };
            SubModels.Add(retVal);
            (Profile as BookmarkProfile).RaiseEntryChanged(new EntryChangedEvent(ChangeType.Created, retVal.FullPath));
            return retVal;
        }

        public void Remove(string label)
        {
            var link2Remove = SubModels.FirstOrDefault(bm => bm.Label.Equals(label, StringComparison.CurrentCultureIgnoreCase));
            if (link2Remove != null)
            {
                SubModels.Remove(link2Remove);
                (Profile as BookmarkProfile).RaiseEntryChanged(new EntryChangedEvent(ChangeType.Deleted, link2Remove.FullPath));
            }
        }

        #endregion


        public bool IsDragging { get; set; }
        public string DisplayName { get { return this.Label; } }
        public string LinkPath { get; set; }
    }
}
