using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer
{
    /// <summary>
    /// Backend store for ParameterDic, implement IDictionary[string,object].
    /// </summary>
    public interface IParameterDicStore : IDictionary<string, object>
    {
        Task SaveAsync();
        Task LoadAsync();

        IParameterDicStore Clone();
    }
}
