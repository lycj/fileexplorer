using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    /// <summary>
    /// Indicate the view model contains a number of ICommands.
    /// </summary>
    public interface IExportCommandBindings
    {
        IEnumerable<IScriptCommandBinding> ExportedCommandBindings { get; }
    }

    public class ExportCommandBindings : IExportCommandBindings
    {
        public ExportCommandBindings(params IScriptCommandBinding[] exports)
        {
            ExportedCommandBindings = exports;
        }

        public IEnumerable<IScriptCommandBinding> ExportedCommandBindings
        {
            get;
            private set;
        }
    }

}
