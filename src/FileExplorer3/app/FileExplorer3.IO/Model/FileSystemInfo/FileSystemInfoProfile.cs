using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Models;
using FileExplorer.WPF;
using FileExplorer.Defines;
//using FileExplorer.WPF.UserControls.DragDrop;

namespace FileExplorer.Models
{
    public class FileSystemInfoProfile : DiskProfileBase, IWPFProfile
    {
        #region Cosntructor

        public FileSystemInfoProfile(IEventAggregator events)
            : base(events)
        {
            ProfileName = "FileSystem";
            DiskIO = new HardDriveDiskIOHelper(this);
            HierarchyComparer = PathComparer.LocalDefault;
            MetadataProvider = new ExMetadataProvider();
            //CommandProviders = new List<ICommandProvider>();
            //PathMapper = IODiskPatheMapper.Instance;
            DragDrop = new FileBasedDragDropHandler(this, em => false);
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            return obj is FileSystemInfoProfile;
        }

        public IComparer<IEntryModel> GetComparer(ColumnInfo column)
        {
            return new ValueComparer<IEntryModel>(p => p.FullPath);
        }

        internal DirectoryInfo createDirectoryInfo(string path)
        {
            if (path.EndsWith(":"))
                return new DirectoryInfo(path + "\\");
            else return new DirectoryInfo(path);
        }

        internal FileInfo createFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public override Task<IEntryModel> ParseAsync(string path)
        {
            IEntryModel retVal = null;
            if (Directory.Exists(path))
                retVal = new FileSystemInfoModel(this, createDirectoryInfo(path));
            else
                if (File.Exists(path))
                    retVal = new FileSystemInfoModel(this, createFileInfo(path));
            return Task.FromResult<IEntryModel>(retVal);
        }

        public override async Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false)
        {
            if (filter == null)
                filter = (m) => true;

            List<IEntryModel> retVal = new List<IEntryModel>();
            if (entry.IsDirectory)
            {
                DirectoryInfo di = createDirectoryInfo(entry.FullPath);
                retVal.AddRange(from fsi in di.GetFileSystemInfos()
                                let m = new FileSystemInfoModel(this, fsi)
                                where filter(m)
                                select m);
            }
            return (IList<IEntryModel>)retVal;
        }

        private Icon getFolderIcon()
        {
            return new Icon(System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("TestApp.WPF.Model.folder.ico"));
        }


        public override IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
        {
            yield return GetDefaultIcon.Instance;
            if (!entry.IsDirectory)
                yield return GetFromIconExtractIcon.Instance;
        }


        #endregion

        #region Data

        private Bitmap _folderBitmap;

        #endregion

        #region Public Properties


        #endregion






















    }
}
