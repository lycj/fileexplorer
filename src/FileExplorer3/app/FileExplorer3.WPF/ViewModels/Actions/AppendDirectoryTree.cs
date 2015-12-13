using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;

namespace FileExplorer.WPF.ViewModels.Actions
{
    /// <summary>
    /// Append entrylist context["EntryList"] to context.Target (IDirectoryTreeViewModel)'s Subdirectories 
    /// </summary>
    //public class AppendDirectoryTree : IResult 
    //{
    //     #region Cosntructor

    //    public AppendDirectoryTree(IDirectoryNodeViewModel targetModel)
    //    {
    //        _targetModel = targetModel;
    //    }

    //    #endregion

    //    #region Methods

    //    public event EventHandler<ResultCompletionEventArgs> Completed;

    //    public void Execute(ActionExecutionContext context)
    //    {
    //        var entryModels = context["EntryList"] as IEnumerable<IEntryModel>;
    //        foreach (var em in entryModels)
    //        {
    //            var evm = _targetModel.CreateSubmodel(em);                
    //            _targetModel.Subdirectories.Add(evm);
    //        }
    //        Completed(this, new ResultCompletionEventArgs());
    //    }

    //    #endregion

    //    #region Data

    //    private IDirectoryNodeViewModel _targetModel;

    //    #endregion

    //    #region Public Properties

    //    #endregion

    //}
}
