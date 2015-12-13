using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;

namespace FileExplorer.Models.Bookmark
{
    public class BookmarkProfile : ProfileBase
    {
        #region fields

        //public static BookmarkProfile Instance = new BookmarkProfile();
        private string _rootLabel;
        private BookmarkModel _rootModel;
        private IDiskProfile _store;
        private string _fileName;
        private IProfile[] _profiles;
        private IList<BookmarkModel> _allBookmarks;

        #endregion

        #region constructors

        public BookmarkProfile(IEventAggregator events = null)
            : base(events)
        {
            ProfileName = "Bookmarks";
            HierarchyComparer = new PathHierarchyComparer<BookmarkModel>(StringComparison.CurrentCultureIgnoreCase, '/');
            MetadataProvider = new BasicMetadataProvider();
            Path = PathHelper.Web;
            _rootLabel = "Bookmarks";
            _rootModel = new BookmarkModel(this, BookmarkModel.BookmarkEntryType.Root, _rootLabel);
            //BookmarkSerializeTest.CreateTestData(this, rootLabel);
            //new BookmarkModel(BookmarkModel.BookmarkEntryType.Root, rootLabel);
            //CommandProviders.Add(new BookmarkCommandProvider(this));
            DeleteCommand = ScriptCommands.ForEach("{DeleteEntries}", "{CurrentEntry}",
                  ScriptCommands.ExecuteMethod("{CurrentEntry.Parent}", "Remove", new object[] { "{CurrentEntry.Label}" }));
            CreateFolderCommand = ScriptCommands.ExecuteFunc("{BaseFolder}", "AddFolder", new object[] {"{FolderName}" }, "{CreatedFolder}");
            PathPatterns = new string[] { _rootLabel + ".*" };
            DragDrop = new BookmarkDragDropHandler();
            AllBookmarks = new List<BookmarkModel>();
        }

        public BookmarkProfile(IDiskProfile store, string fileName, IProfile[] profiles)
            : this(null)
        {
            _store = store;
            _fileName = fileName;
            ProfileName = String.Format("Bookmarks {0}", _fileName);
            _profiles = profiles;            
            AsyncUtils.RunSync(() => LoadSettingsAsync());
        }

        #endregion

        #region events

        #endregion

        #region properties

        public BookmarkModel RootModel { get { return _rootModel; } }
        public IList<BookmarkModel> AllBookmarks { get { return _allBookmarks; } set { _allBookmarks = value; } }

        #endregion

        #region methods

        public async Task RefreshAllBookmarksAsync()
        {
            AllBookmarks = (await this.ListRecursiveAsync(this.RootModel, CancellationToken.None,
                   em => em is BookmarkModel,
                   em => (em as BookmarkModel).IsDirectory, false)).Cast<BookmarkModel>().ToList();
        }

        public async Task LoadSettingsAsync()
        {
            var settingsFile = await _store.ParseAsync(_fileName);
            if (settingsFile != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BookmarkModel));
                using (var stream = await _store.DiskIO.OpenStreamAsync(settingsFile, FileAccess.Read, CancellationToken.None))
                {
                    BookmarkModel bm = serializer.Deserialize(stream) as BookmarkModel;
                    bm.Profile = this;
                    _rootModel = bm;
                }
                await RefreshAllBookmarksAsync();
            }
        }

        public async Task SaveSettingsAsync()
        {
            if (_fileName != null)
            {
                var settingsFile = await _store.ParseAsync(_fileName);
                if (settingsFile == null)
                    settingsFile = await _store.DiskIO.CreateAsync(_fileName, false, CancellationToken.None);
                XmlSerializer serializer = new XmlSerializer(typeof(BookmarkModel));
                using (var stream = await _store.DiskIO.OpenStreamAsync(settingsFile, FileAccess.Write, CancellationToken.None))
                {
                    stream.SetLength(0);
                    serializer.Serialize(stream, _rootModel);
                }
            }
        }

        private BookmarkModel lookup(BookmarkModel lookupEntryModel, string[] pathSplits, int idx)
        {
            if (lookupEntryModel == null)
                return null;

            if (idx >= pathSplits.Length)
                return lookupEntryModel;

            return
                lookup(lookupEntryModel.SubModels.FirstOrDefault(sub => sub.Name.Equals(pathSplits[idx])),
                pathSplits, idx + 1);
        }

        public override async Task<IEntryModel> ParseAsync(string path)
        {
            if (path.Equals(_rootLabel))
                return _rootModel;

            if (path.StartsWith(_rootLabel, StringComparison.CurrentCultureIgnoreCase))
                return lookup(_rootModel, path.Split(new char[] { Path.Separator }, StringSplitOptions.RemoveEmptyEntries), 1);

            return null;
        }

        public override async Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false)
        {
            BookmarkModel bm = entry as BookmarkModel;
            filter = filter ?? (em => true);
            if (bm != null)
                return bm.SubModels.Where(sub => filter(sub)).Cast<IEntryModel>().ToList();

            throw new NotImplementedException();
        }

        internal void RaiseEntryChanged(EntryChangedEvent evnt)
        {
            raiseEntryChanged(evnt);
            SaveSettingsAsync();
            RefreshAllBookmarksAsync();
        }

        public override IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
        {
            BookmarkModel bm = entry as BookmarkModel;

            switch (bm.Type)
            {
                case BookmarkModel.BookmarkEntryType.Link :
                    return new List<IModelIconExtractor<IEntryModel>>() { new LoadFromLinkPath(_profiles) };
                case BookmarkModel.BookmarkEntryType.Root :
                    return new List<IModelIconExtractor<IEntryModel>>() { 
                        new LoadFromAssembly(typeof(BookmarkProfile).GetTypeInfo().Assembly, "FileExplorer.Themes.Resources.bookmark.ico") };
                default : 
                    return base.GetIconExtractSequence(entry);
            }            
             
        }

        #endregion


    }
}
