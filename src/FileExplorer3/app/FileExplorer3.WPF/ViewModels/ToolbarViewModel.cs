using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    //public class ToolbarViewModel : PropertyChangedBase, IToolbarViewModel
    //{
    //    #region Constructor

    //    public ToolbarViewModel(IEventAggregator events)
    //    {
    //        List<ICommandModel> cmList = new List<ICommandModel>()
    //        {
    //            new CommandModel() { Header = "Test" }
    //        };

    //        Commands = new EntriesHelper<ICommandViewModel>(cmList.Select(cm => new CommandViewModel(cm)).ToArray());
    //        //NotifyOfPropertyChange(() => Commands);
    //        //Commands = new EntriesHelper<ICommandViewModel>(loadCommands);
            
    //    }

    //    #endregion

    //    #region Methods

    //    //private async Task<IEnumerable<ICommandViewModel>> loadCommands()
    //    //{
          
    //    //}

    //    #endregion

    //    #region Data

    //    #endregion

    //    #region Public Properties

    //    public EntriesHelper<ICommandViewModel> Commands { get; private set; }

    //    #endregion
    //}
}
