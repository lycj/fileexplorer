using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TestApp.RT.Model
{
    public class StorageItemProfile : IProfile
    {
        #region Cosntructor

        #endregion

        #region Methods

        public IEntryModel GetModelFor(IStorageItem storageItem)
        {
            return new StorageItemModel(storageItem);
        }

        public async Task<IEntryModel> ParseAsync(string path)
        {
            return new StorageItemModel(await StorageFolder.GetFolderFromPathAsync(path));
        }

        public async Task<IEnumerable<IEntryModel>> ListAsync(IEntryModel entry, Func<IEntryModel, bool> filter = null)
        {
            var parentFolder = (entry as StorageItemModel).StorageItem as StorageFolder;
            return (await parentFolder.GetItemsAsync()).Select(si => new StorageItemModel(si));
        }



        private ImageSource ThumbnailToImageSource(StorageItemThumbnail bitmapSource)
        {
            if (bitmapSource == null)
                return null;
            var wb = new WriteableBitmap((int)bitmapSource.OriginalWidth, (int)bitmapSource.OriginalHeight);
            wb.SetSource(bitmapSource);
            return wb;
        }

        private async Task<ImageSource> getIconAsync(StorageFile file, int size)
        {
            try
            {
                var source = file.GetThumbnailAsync(
                    ThumbnailMode.ListView | ThumbnailMode.SingleItem,
                    (uint)size, ThumbnailOptions.ResizeThumbnail).AsTask().Result;

                return ThumbnailToImageSource(source);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<ImageSource> getIconAsync(StorageFolder folder, int size)
        {
            try
            {                
                var source = folder.GetThumbnailAsync(
                    ThumbnailMode.ListView | ThumbnailMode.SingleItem,
                    (uint)size, ThumbnailOptions.ResizeThumbnail).AsTask().Result;

                return ThumbnailToImageSource(source);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ImageSource> GetIconAsync(IEntryModel entry, int size)
        {
            var storageItem = (entry as StorageItemModel).StorageItem;
            if (storageItem is StorageFile)
                return await getIconAsync(storageItem as StorageFile, size);

            if (storageItem is StorageFolder)
                return await getIconAsync(storageItem as StorageFolder, size);

            return null;
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion
        public IComparer<IEntryModel> GetComparer(string property)
        {
            throw new NotImplementedException();
        }

      

      


        public Task<IEnumerable<IEntryModel>> TransferAsync(FileExplorer.Defines.TransferMode mode, IEntryModel[] source, IEntryModel dest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Rename(IEntryModel source, string newName)
        {
            throw new NotImplementedException();
        }
    }
}
