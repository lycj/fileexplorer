using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Script;

namespace FileExplorer.Models
{
    /// <summary>
    /// Implemented by IProfile for document how to handle when certain action is taken place
    /// (e.g. double click on an item in file list)
    /// Because it's run by IExplorerViewModel, certain parameter is always available:
    /// - Profile:IProfile
    /// - Explorer:IExplorerViewModel
    /// </summary>
    public interface IProfileCommands
    {
        /// <summary>
        /// Rename an entry (SourceEntry:IEntryModel) to a new file name (DestName:string)
        /// </summary>
        IScriptCommand Rename { get; }
        
        /// <summary>
        /// Transfer(Mode:string) a group of entries (SourceEntries:IEntryModel[]) to destination (DestDirectory:IEntryModel)
        /// </summary>
        IScriptCommand Transfer { get; }        
    }
}
