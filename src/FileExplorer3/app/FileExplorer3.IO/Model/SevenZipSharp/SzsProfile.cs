using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.IO.Compress;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models.SevenZipSharp
{

    /// <summary>
    /// Convert .zip .7z etc file based entry models to SzsRootModel, which is directories.
    /// </summary>
    public class SzsProfile : DiskProfileBase, IConverterProfile, IWPFProfile
    {

        #region Constructors

        public SzsProfile(IEventAggregator events)
            : base(events)
        {            
            DragDrop = new FileBasedDragDropHandler(this, em => !(em is SzsRootModel));
            MetadataProvider = new SzsMetadataProvider();
            DiskIO = new SzsDiskIOHelper(this);            
            _wrapper = new SevenZipWrapper();
        }

        #endregion

        #region Methods

        public IEntryModel Convert(IEntryModel entryModel)
        {
            if (entryModel != null && !entryModel.IsDirectory && entryModel.Profile is IDiskProfile && _wrapper.IsArchive(entryModel.Name))
                return new SzsRootModel(entryModel, this);

            return entryModel;
        }

        public override async Task<IList<IEntryModel>> ListAsync(IEntryModel entry, System.Threading.CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false)
        {
            var retList = new List<IEntryModel>();

            using (var releaser = await WorkingLock.LockAsync())
            {
                if (entry is ISzsItemModel)
                {
                    filter = filter ?? (em => true);
                    var szsEntry = entry as ISzsItemModel;
                    var szsRoot = szsEntry.Root;
                    var referencedEntry = szsRoot.ReferencedFile;
                    var refEntryProfile = referencedEntry.Profile as IDiskProfile;
                    if (refEntryProfile != null && !referencedEntry.IsDirectory)
                    {
                        string listPattern = String.Format(RegexPatterns.CompressionListPattern, Regex.Escape(szsEntry.RelativePath));

                        using (var stream = await refEntryProfile.DiskIO.OpenStreamAsync(referencedEntry,
                            Defines.FileAccess.Read, CancellationToken.None))
                            foreach (object afi in _wrapper.List(stream, listPattern))
                            {
                                var em = new SzsChildModel(szsRoot, (SevenZip.ArchiveFileInfo)afi);
                                lock (VirtualModels)
                                    if (VirtualModels.Contains(em))
                                        VirtualModels.Remove(em);
                                if (filter(em))
                                    retList.Add(em);
                            }

                        retList.AddRange(VirtualModels.Where(sem =>
                            Path.GetDirectoryName(sem.RelativePath).Equals(szsEntry.RelativePath) && filter(sem)));
                    }
                }
            }

            return retList;
        }

        public override async Task<IEntryModel> ParseAsync(string path)
        {
            return await _baseProfile.LookupAsync(path, CancellationToken.None);
        }

        public override IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
        {
            yield return GetFromSystemImageListUsingExtension.Instance;

            if (entry is SzsRootModel)
                yield return DrawOverlayTextExtractor.Create(GetFromSystemImageListUsingExtension.Instance,
                    em => em.GetExtension().ToLower(), em => em.GetExtension().ToLower(), System.Drawing.Color.MediumBlue);

            //return base.GetIconExtractSequence(entry);
        }

        public void SetOwner(IProfile profile)
        {
            _baseProfile = profile;
            base.HierarchyComparer = profile.HierarchyComparer;
            base.Events = profile.Events;
        }

        #endregion

        #region Data

        private AsyncLock _workingLock = new AsyncLock();
        private SevenZipWrapper _wrapper;
        private IProfile _baseProfile;
        private List<ISzsItemModel> _virtualModels = new List<ISzsItemModel>();

        #endregion

        #region Public Properties

        public AsyncLock WorkingLock { get { return _workingLock; } }
        internal SevenZipWrapper Wrapper { get { return _wrapper; } }
        internal List<ISzsItemModel> VirtualModels { get { return _virtualModels; } }

        #endregion

       
    }
}
