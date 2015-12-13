using FileExplorer.Models;
using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.IO
{
    public class DiskIOHelperBase : IDiskIOHelper
    {

        protected DiskIOHelperBase(IDiskProfile profile)
        {
            Mapper = new IODiskPatheMapper();
            Profile = profile;            
        }

        public IDiskProfile Profile { get; protected set; }
        public IDiskPathMapper Mapper { get; protected set; }        
        

        public virtual Task<Stream> OpenStreamAsync(IEntryModel entryModel,
            FileExplorer.Defines.FileAccess access, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        [Obsolete]
        public virtual IScriptCommand GetTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal)
        {
            return IOScriptCommands.DiskTransfer(srcModel, destDirModel, removeOriginal, false);
        }

        public virtual IScriptCommand GetTransferCommand(string sourceKey, string destinationDirKey, string destinationKey, 
            bool removeOriginal, IScriptCommand nextCommand)
        {
            return IOScriptCommands.DiskTransfer(sourceKey, destinationDirKey, destinationKey,  removeOriginal, false, nextCommand);
        }
    }
}
