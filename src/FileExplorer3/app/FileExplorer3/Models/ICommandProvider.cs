using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Script;

namespace FileExplorer.Models
{
    /// <summary>
    /// Provide CommandModels given entries.
    /// </summary>
    public interface ICommandProvider
    {
        IEnumerable<ICommandModel> GetCommandModels();
    }

}
