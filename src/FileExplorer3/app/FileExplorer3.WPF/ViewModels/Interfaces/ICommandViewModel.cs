using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public interface ICommandViewModel : IComparable<ICommandViewModel>, IComparable 
    {
        ICommandModel CommandModel { get; }
        IScriptCommandBinding CommandBinding { get; }
        ToolbarItemType CommandType { get; }
        IEntriesHelper<ICommandViewModel> SubCommands { get; }
    }
}
