using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;

namespace FileExplorer.WPF.ViewModels
{
    //public interface IDirectoryNodeBroadcastHandler
    //{
    //    Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel, HierarchicalResult result,
    //        IDirectoryNodeBroadcastHandler[] allHandlers);
    //    HierarchicalResult AppliedResult { get; }
    //}

    //public abstract class DirectoryNodeBroadcastHandlerBase : IDirectoryNodeBroadcastHandler
    //{
    //    public DirectoryNodeBroadcastHandlerBase(HierarchicalResult appliedResult)
    //    {
    //        AppliedResult = appliedResult;
    //    }

    //    public abstract Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel,
    //        HierarchicalResult result, IDirectoryNodeBroadcastHandler[] allHandlers);
    //    public HierarchicalResult AppliedResult { get; protected set; }
    //}

    //public class BroadcastSubEntry : DirectoryNodeBroadcastHandlerBase
    //{
    //    protected BroadcastSubEntry(IEntryModel lookupModel, bool firstMatchedOnly,
    //        Func<IDirectoryNodeViewModel, HierarchicalResult, bool> loadFunc)
    //        : base(HierarchicalResult.Child)
    //    {
    //        _lookupModel = lookupModel;
    //        _firstMatchedOnly = firstMatchedOnly;
    //        _loadFunc = loadFunc;
    //    }

    //    public static BroadcastSubEntry FirstMatchedOnly(IEntryModel lookupModel,
    //        Func<IDirectoryNodeViewModel, HierarchicalResult, bool> loadFunc)
    //    {
    //        return new BroadcastSubEntry(lookupModel, true, loadFunc);
    //    }

    //    public static BroadcastSubEntry AllMatched(IEntryModel lookupModel,
    //        Func<IDirectoryNodeViewModel, HierarchicalResult, bool> loadFunc)
    //    {
    //        return new BroadcastSubEntry(lookupModel, false, loadFunc);
    //    }

    //    public static BroadcastSubEntry All(IEntryModel lookupModel,
    //        Func<IDirectoryNodeViewModel, HierarchicalResult, bool> loadFunc)
    //    {
    //        return new BroadcastSubEntry(lookupModel, false, loadFunc) { AppliedResult = HierarchicalResult.All };
    //    }

    //    private bool _firstMatchedOnly;
    //    private bool _loadIfNotLoaded = false;
    //    private IEntryModel _lookupModel;
    //    private bool _lookupCompleted = false;
    //    private Func<IDirectoryNodeViewModel, HierarchicalResult, bool> _loadFunc;

    //    public override async Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel,
    //        HierarchicalResult result, IDirectoryNodeBroadcastHandler[] allHandlers)
    //    {
    //        if (currentModel.IsDummyNode) return;
    //        if (_lookupCompleted) return;

    //        if (_loadFunc(currentModel, result))
    //            await currentModel.LoadAsync(false);

    //        var comparer = currentModel.CurrentDirectory.EntryModel.Profile.HierarchyComparer;

    //        if (_lookupModel == null || AppliedResult == HierarchicalResult.All)
    //        {
    //            foreach (var sub in currentModel.Subdirectories)
    //                await sub.BroadcastSelectAsync(sender, _lookupModel, allHandlers);
    //        }
    //        else
    //            foreach (var sub in currentModel.Subdirectories)
    //                if (!sub.IsDummyNode)
    //                {
    //                    var compareResult = comparer.CompareHierarchy(sub.CurrentDirectory.EntryModel,
    //                        _lookupModel);
    //                    switch (compareResult)
    //                    {
    //                        case HierarchicalResult.Current:
    //                        case HierarchicalResult.Child:
    //                            if (compareResult == HierarchicalResult.Current)
    //                                _lookupCompleted = true;
    //                            await sub.BroadcastSelectAsync(sender, _lookupModel, allHandlers);
    //                            if (_firstMatchedOnly)
    //                                return;
    //                            break;
    //                    }
    //                }
    //    }
    //}

    //public class UpdateIsSelected : DirectoryNodeBroadcastHandlerBase
    //{
    //    public static UpdateIsSelected Instance = new UpdateIsSelected();

    //    public UpdateIsSelected()
    //        : base(HierarchicalResult.Current)
    //    {
    //    }

    //    public override async Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel,
    //        HierarchicalResult result, IDirectoryNodeBroadcastHandler[] allHandlers)
    //    {
    //        await Task.Run(() =>
    //            {
    //                if (!currentModel.IsSelected)
    //                {
    //                    var selectingEntry = sender.SelectingEntry;
    //                    if (selectingEntry != null && selectingEntry.Equals(currentModel.CurrentDirectory.EntryModel))
    //                        currentModel.IsSelected = true;
    //                }
    //            });
    //    }
    //}

    //public class ResetIsChildSelected : DirectoryNodeBroadcastHandlerBase
    //{
    //    public static ResetIsChildSelected Instance = new ResetIsChildSelected();

    //    public ResetIsChildSelected()
    //        : base(HierarchicalResult.All)
    //    {
    //    }

    //    public override async Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel,
    //        HierarchicalResult result, IDirectoryNodeBroadcastHandler[] allHandlers)
    //    {
    //        await Task.Run(() =>
    //        {
    //            if (currentModel.IsChildSelected)
    //                currentModel.IsChildSelected = false;
    //        });
    //    }

    //}

    //public class UpdateIsExpanded : DirectoryNodeBroadcastHandlerBase
    //{
    //    public static UpdateIsExpanded Instance = new UpdateIsExpanded();

    //    public UpdateIsExpanded()
    //        : base(HierarchicalResult.All)
    //    {
    //    }

    //    public override async Task HandleBroadcastAsync(IDirectoryTreeViewModel sender, IDirectoryNodeViewModel currentModel,
    //        HierarchicalResult result, IDirectoryNodeBroadcastHandler[] allHandlers)
    //    {
    //        await Task.Run(() =>
    //        {
    //            switch (result)
    //            {
    //                case HierarchicalResult.Child:
    //                    currentModel.IsExpanded = true;
    //                    break;
    //                case HierarchicalResult.Unrelated:
    //                    currentModel.IsExpanded = false;
    //                    break;
    //            }
    //        });
    //    }

    //}
}
