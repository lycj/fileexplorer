using FileExplorer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.IO
{
    /// <summary>
    /// The actual file exists in IO.
    /// </summary>
    public class IODiskPatheMapper : IDiskPathMapper
    {
        public static IODiskPatheMapper Instance = new IODiskPatheMapper();

          #region Constructor

        public IODiskPatheMapper()
        {            
        }

        #endregion

        #region Methods

        public Task UpdateCacheAsync(IEntryModel model)
        {
            return Task.Delay(0);
        }

        public Task UpdateSourceAsync(IEntryModel model)
        {
            return Task.Delay(0);
        }
        
        #endregion

        #region Data

        #endregion

        #region Public Properties


        public DiskMapInfo this[IEntryModel model]
        {
            get { return new DiskMapInfo(model.FullPath, true, false); }
        }


        #endregion


        
    }

    /// <summary>
    /// Indicated that there's no file support
    /// </summary>
    public class NullDiskPatheMapper : IDiskPathMapper
    {
        public static NullDiskPatheMapper Instance = new NullDiskPatheMapper();

        #region Constructor

        #endregion

        #region Methods

        public Task UpdateCacheAsync(IEntryModel model)
        {
            return Task.Delay(0);
        }

        public Task UpdateSourceAsync(IEntryModel model)
        {
            return Task.Delay(0);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties


        public DiskMapInfo this[IEntryModel model]
        {
            get { return null; }
        }


        #endregion



    }
}
