using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Models;
using Windows.Storage;

namespace TestApp.RT.Model
{
    public class StorageItemModel : EntryModelBase
    { 
        
        #region Cosntructor

        public StorageItemModel(IStorageItem entry)
        {
            _storageItem = entry;
            this.Label = entry.Name;
            this.FullPath = entry.Path;
            this.IsDirectory = entry.IsOfType(StorageItemTypes.Folder);
            string parentPath = System.IO.Path.GetDirectoryName(entry.Path);
            this.Parent = null;// String.IsNullOrEmpty(parentPath) ? null : new FileSystemInfoModel(new DirectoryInfo(parentPath));
            this.Description = entry.GetType().ToString();
        }

        #endregion

        #region Methods

        //public async Task<IStorageItem> GetEntry()
        //{
        //    return IsDirectory ?
        //            (IStorageItem)await StorageFolder.GetFolderFromPathAsync(FullPath) :
        //            (IStorageItem)await StorageFile.GetFileFromPathAsync(FullPath);
        //}
        
        #endregion

        #region Data

        IStorageItem _storageItem;

        #endregion

        #region Public Properties

        public IStorageItem StorageItem { get { return _storageItem; } }
        
        #endregion
    }
}
