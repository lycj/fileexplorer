using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cinch;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using FileExplorer.Models;

namespace FileExplorer.Script
{
    public static partial class WPFExtensionMethods
    {
        public static Func<ParameterDic, IEntryModel[]> GetCurrentDirectoryFunc =
           pd => pd.ContainsKey("DirectoryTree") && pd["DirectoryTree"] is IDirectoryTreeViewModel ?
               new [] { (pd["DirectoryTree"] as IDirectoryTreeViewModel).Selection.RootSelector.SelectedValue }               
               : new IEntryModel[] { };

        public static Func<ParameterDic, IEntryViewModel[]> GetCurrentDirectoryVMFunc =
           pd => pd.ContainsKey("DirectoryTree") && pd["DirectoryTree"] is IDirectoryTreeViewModel ?
               new[] { (pd["DirectoryTree"] as IDirectoryTreeViewModel).Selection.RootSelector.SelectedViewModel }
               : new IEntryViewModel[] { };

        public static Func<ParameterDic, IEntryModel[]> GetFileListItemsFunc =
            pd => pd.ContainsKey("FileList") && pd["FileList"] is IFileListViewModel ?
                (pd["FileList"] as IFileListViewModel).ProcessedEntries.EntriesHelper.AllNonBindable
                .Select(evm => evm.EntryModel).ToArray()
                : new IEntryModel[] { };

        public static Func<ParameterDic, IEntryViewModel[]> GetFileListItemsVMFunc =
            pd => pd.ContainsKey("FileList") && pd["FileList"] is IFileListViewModel ?
                (pd["FileList"] as IFileListViewModel).ProcessedEntries.EntriesHelper.AllNonBindable
                .ToArray()
                : new IEntryViewModel[] { };

        public static Func<ParameterDic, IEntryModel[]> GetFileListSelectionFunc =
            pd => pd.ContainsKey("FileList") && pd["FileList"] is IFileListViewModel ?
                (pd["FileList"] as IFileListViewModel).Selection.SelectedItems
                .Select(evm => evm.EntryModel).ToArray()
                : new IEntryModel[] { };

        public static Func<ParameterDic, IEntryViewModel[]> GetFileListSelectionVMFunc =
            pd => pd.ContainsKey("FileList") && pd["FileList"] is IFileListViewModel ?
                (pd["FileList"] as IFileListViewModel).Selection.SelectedItems
                .ToArray()
                : new IEntryViewModel[] { };

        public static Func<ParameterDic, IEntryModel> GetFileListCurrentDirectoryFunc =
            pd => pd.ContainsKey("FileList") && pd["FileList"] is IFileListViewModel ?
                (pd["FileList"] as IFileListViewModel).CurrentDirectory                
                : null;

        public static VMParameterDic AsVMParameterDic(this ParameterDic dic)
        {
            if (dic is VMParameterDic)
                return (VMParameterDic)dic;

            var retVal = new VMParameterDic();
            foreach (var pp in dic)
                retVal.Add(pp.Key, pp.Value);
            return retVal;
        }

        //public static IEnumerable<IDirectoryNodeViewModel> GetHierarchy(
        //    this IDirectoryNodeViewModel node, bool includeCurrent)
        //{
        //    if (includeCurrent)
        //        yield return node;

        //    IDirectoryNodeViewModel current = node.ParentNode;
        //    while (current != null)
        //    {
        //        yield return current;
        //        current = current.ParentNode;
        //    }
        //}

        public static IEnumerable<IEntryModel> GetHierarchy(
            this IEntryModel node, bool includeCurrent)
        {
            if (includeCurrent)
                yield return node;

            IEntryModel current = node.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }


        public static void RegisterCommand(this IExportCommandBindings container, UIElement ele, ScriptBindingScope scope)
        {
            foreach (var c in container.ExportedCommandBindings)
                if (scope.HasFlag(c.Scope))
                {
                    var binding = c.CommandBinding;
                    if (binding != null)
                        ele.CommandBindings.Add(binding);
                }
        }

        public static IScriptCommand Rename(this IEntryModel entryModel, string newName)
        {
            return new RenameFileBasedEntryCommand(pm => entryModel, newName);
        }

        public static List<IViewModelInitializer<IExplorerViewModel>>
          EnsureOneStartupDirectoryOnly(this List<IViewModelInitializer<IExplorerViewModel>> initializers)
        {
            List<IViewModelInitializer<IExplorerViewModel>>
            allStartup = initializers.FindAll(i => i is StartupDirInitializer);

            if (allStartup.Count > 1) //If more than one startup
            {
                allStartup.RemoveAt(0); //Remove the first from the list first, 
                foreach (var startup in allStartup)//then remove the else from initializers.
                    initializers.Remove(startup);
            }
         
            return initializers;
        }


    }
}
