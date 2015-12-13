using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    /// <summary>
    /// Provide a fixed number of commands.
    /// </summary>
    public class StaticCommandProvider : ICommandProvider
    {
        private ICommandModel[] _commands;
        public StaticCommandProvider(params ICommandModel[] commands)
        {
            _commands = commands;
        }

        public IEnumerable<ICommandModel> GetCommandModels()
        {
            return _commands.ToList();
        }
    }
}
