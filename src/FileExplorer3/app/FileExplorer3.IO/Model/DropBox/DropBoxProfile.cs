using Caliburn.Micro;
using DropNet;
using DropNet.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class DropBoxProfile : DiskProfileBase, IWPFProfile
    {

        #region Constructors

        /// <summary>
        /// For DropBox
        /// </summary>
        /// <param name="events"></param>
        /// <param name="requestToken"></param>
        public DropBoxProfile(IEventAggregator events, 
            string clientId, string clientSecret,
            Func<UserLogin> loginFunc,
            string aliasMask = "{0}'s DropBox")
            : base(events)
        {
            ProfileName = "DropBox";
            ProfileIcon = ResourceUtils.GetEmbeddedResourceAsByteArray(this, "/Model/DropBox/DropBox_Logo.png");
            ModelCache = new EntryModelCache<DropBoxItemModel>(m => m.FullPath, () => Alias, true);
            //_accessToken = accessToken;
            Path = PathHelper.Web;
            _loginFunc = loginFunc;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _aliasMask = aliasMask;

            _thumbnailExtractor = new DropBoxModelThumbnailExtractor(() => GetClient());


            DiskIO = new DropBoxDiskIOHelper(this);
            HierarchyComparer = PathComparer.WebDefault;
            MetadataProvider = new DropBoxMetadataProvider(() => GetClient());
            //CommandProviders = new List<ICommandProvider>();
            SuggestSource = new NullSuggestSource();
            //PathMapper = new SkyDriveDiskPathMapper(this, null);
            DragDrop = new FileBasedDragDropHandler(this);


        }



        #endregion

        #region Methods

        public string ConvertRemotePath(string path)
        {
            path = ModelCache.CheckPath(path);

            if (path == Alias)
                return "/";

            if (path.StartsWith(Alias))
                return "/" + path.Substring(Alias.Length + 1);
            return "/" + path.TrimStart('/');
        }

        public override async Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct,
            Func<IEntryModel, bool> filter = null, bool refresh = false)
        {
            checkLogin();

            if (filter == null)
                filter = em => true;

            List<IEntryModel> retList = new List<IEntryModel>();
            List<DropBoxItemModel> cacheList = new List<DropBoxItemModel>();

            var dirModel = (entry as DropBoxItemModel);
            if (dirModel != null)
            {
                if (!refresh)
                {
                    var cachedChild = ModelCache.GetChildModel(dirModel);
                    if (cachedChild != null)
                        return cachedChild.Where(m => filter(m)).Cast<IEntryModel>().ToList();
                }
                ct.ThrowIfCancellationRequested();

                var fetchedMetadata = await GetClient().GetMetaDataTask(ConvertRemotePath(entry.FullPath));
                foreach (var metadata in fetchedMetadata.Contents)
                {
                    var retVal = new DropBoxItemModel(this, metadata, dirModel.FullPath);
                    cacheList.Add(ModelCache.RegisterModel(retVal));
                    if (filter(retVal))
                        retList.Add(retVal);
                }
                ModelCache.RegisterChildModels(dirModel, cacheList.ToArray());
            }
            return retList;

        }

        public override async Task<IEntryModel> ParseAsync(string path)
        {
            checkLogin();
            if (path == "" || path == Alias)
                return new DropBoxItemModel(this);

            string fullPath = ModelCache.GetUniqueId(ModelCache.CheckPath(path));
            if (fullPath != null)
                return ModelCache.GetModel(fullPath);

            string remotePath = ConvertRemotePath(path);
            try
            {
                var fetchedMetadata = await GetClient().GetMetaDataTask(remotePath);
                return ModelCache.RegisterModel(
                    new DropBoxItemModel(this, fetchedMetadata, Path.GetDirectoryName(path)));
            }
            catch
            {
                return null; //Not found
            }
        }




        public override IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
        {
            checkLogin();

            foreach (var extractor in base.GetIconExtractSequence(entry))
                yield return extractor;

            DropBoxItemModel model = entry as DropBoxItemModel;
            //if (model.ImageUrl != null)
            //    yield return new GetUriIcon(e => new Uri((e as SkyDriveItemModel).ImageUrl));
            //else
            //if (model.Name.IndexOf('.') != -1)
                yield return GetFromSystemImageListUsingExtension.Instance;

            if (model.FullPath == Alias)
                yield return EntryModelIconExtractors.ProvideValue(ProfileIcon);

            else if (model.Metadata != null && model.Metadata.Thumb_Exists)
                yield return _thumbnailExtractor;


        }

        internal void checkLogin()
        {
            if (_login == null)
            {
                _login = _loginFunc();
                if (_login == null)
                    throw new Exception("Login failed.");

                var client = new DropNetClient(_clientId, _clientSecret) { UserLogin = _login };
                Alias = String.Format(_aliasMask, client.AccountInfo().display_name);
            }
        }

        public DropNetClient GetClient()
        {
            checkLogin();
            //if (_client != null)
            //    return _client;

            _client = new DropNetClient(_clientId, _clientSecret) { UserLogin = _login };




            return _client;
        }

        #endregion

        #region Data
        //private GetResourceIcon DropBoxLogo;
        private DropBoxModelThumbnailExtractor _thumbnailExtractor;
        private UserLogin _login = null;
        private DropNetClient _client;
        private Func<UserLogin> _loginFunc;
        private string _clientId;
        private string _clientSecret;
        private string _aliasMask;
        #endregion

        #region Public Properties

        //public IEntryModel RootModel { get; protected set; }
        public string Alias { get; protected set; }
        public IEntryModelCache<DropBoxItemModel> ModelCache { get; private set; }

        #endregion       
    }
}
