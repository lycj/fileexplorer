using FileExplorer.IO;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models.SevenZipSharp
{

    /// <summary>
    /// Implemented by SzsRootModel and SzsChildModel.
    /// </summary>
    public interface ISzsItemModel : IEntryModel
    {
        SzsRootModel Root { get; }
        string RelativePath { get; }
    }


    /// <summary>
    /// Model for an Sevenzipsharp archive.
    /// </summary>
    public class SzsRootModel : DiskEntryModelBase, ISzsItemModel, IConvertedEntryModel
    {

        #region Constructors

        public SzsRootModel(IEntryModel referencedModel, SzsProfile profile)
            : base(profile)
        {
            if (!(referencedModel.Profile is IDiskProfile))
                throw new ArgumentException("RefrencedModel.Profile must implement IDiskProfile.");
            ReferencedFile = referencedModel;

            IsDirectory = true;
            _isRenamable = referencedModel.IsRenamable;
            Label = referencedModel.Label;
            Description = referencedModel.Description;
            FullPath = referencedModel.FullPath;
           
            if (referencedModel is DiskEntryModelBase)
                Size = (referencedModel as DiskEntryModelBase).Size;
        }

        #endregion

        #region Methods

        #endregion

        #region Data


        #endregion

        #region Public Properties

        public IEntryModel ReferencedFile { get; private set; }
        public SzsRootModel Root { get { return this; } }
        public string RelativePath { get { return ""; } }

        public IEntryModel OriginalEntryModel
        {
            get { return ReferencedFile; }
        }

        public override string Name
        {
            get { return ReferencedFile.Name; }
            set { ReferencedFile.Name = value; }
        }
        #endregion

        
    }

    public class SzsChildModel : DiskEntryModelBase, ISzsItemModel
    {
        #region Constructors

        public SzsChildModel(SzsRootModel root, SevenZip.ArchiveFileInfo afi)
            : base(root.Profile)
        {
            _afi = afi;
            Root = root;
            RelativePath = afi.FileName;            
            base.IsDirectory = afi.IsDirectory;
            _parentFunc = getParent;
            _isRenamable = root.IsRenamable;            
            Name = Label = PathHelper.Disk.GetFileName(afi.FileName);
            Description = "";// referencedModel.Description;
            FullPath = PathHelper.Disk.Combine(root.FullPath, afi.FileName);;
            Size = (long)afi.Size;
        }

        public SzsChildModel(SzsRootModel root, string relativePath, bool isDirectory)
            : base(root.Profile)
        {
            Root = root;
            RelativePath = relativePath;
            base.IsDirectory = isDirectory;
            _parentFunc = getParent;
            _isRenamable = root.IsRenamable;
            Name = Label = PathHelper.Disk.GetFileName(relativePath);
            Description = "";// referencedModel.Description;
            FullPath = PathHelper.Disk.Combine(root.FullPath, relativePath); ;
        }

        #endregion

        #region Methods

        private ISzsItemModel getParent()
        {
            string parentRelPath = PathHelper.Disk.GetDirectoryName(RelativePath);
            if (String.IsNullOrEmpty(parentRelPath))
                return Root;
            else return new SzsChildModel(Root, parentRelPath, true);
        }

        #endregion

        #region Data

        internal SevenZip.ArchiveFileInfo _afi;

        #endregion

        #region Public Properties

        
        public SzsRootModel Root { get; private set; }
        public string RelativePath { get; private set; }

        #endregion

    }
}
