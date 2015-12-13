using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public interface ISupportEntriesHelper<VM>
    {
        IEntriesHelper<VM> Entries { get; set; }
    }

    /// <summary>
    /// Helper view model class that provide support of loading sub-entries.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    public interface IEntriesHelper<VM> : INotifyPropertyChanged
    {
        /// <summary>
        /// Call to load sub-entries.
        /// </summary>
        /// <param name="force">Load sub-entries even if it's already loaded.</param>
        /// <returns></returns>
        Task<IEnumerable<VM>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false, object parameter = null);

        Task UnloadAsync();

        /// <summary>
        /// Used to preload sub-entries, fully overwrite entries stored in the helper.
        /// </summary>
        /// <param name="viewModels"></param>
        void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params VM[] viewModels);

        /// <summary>
        /// Load when expand the first time.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Whether subentries loaded.
        /// </summary>
        bool IsLoaded { get; set; }

        bool IsLoading { get; set; }

        event EventHandler EntriesChanged;
        
        IEnumerable<VM> AllNonBindable { get; }

        /// <summary>
        /// A list of sub-entries, after loaded.
        /// </summary>
        ObservableCollection<VM> All { get; }

        AsyncLock LoadingLock { get; }
    }
}
