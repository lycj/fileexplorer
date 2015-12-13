using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINRT
using Windows.UI.Xaml.Media;
#else
//using System.Windows.Media;
#endif
//using Caliburn.Micro;
//using FileExplorer.WPF.BaseControls;
using System.Windows;
//using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Script;
using System.Threading;
using System.ComponentModel;
using FileExplorer.Defines;
using Caliburn.Micro;

namespace FileExplorer.Models
{
   
    public interface IProfile : INotifyPropertyChanged
    {
        
        #region Methods

        

        IComparer<IEntryModel> GetComparer(ColumnInfo column);

        /// <summary>
        /// Return the entry that represent the path, or null if not exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IEntryModel> ParseAsync(string path);

        Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct , Func<IEntryModel, bool> filter = null, bool refresh = false);

        /// <summary>
        /// Return the sequence of icon is extracted and returned, EntryViewModel will run each extractor 
        /// and set Icon to it's GetIconForModel() result.
        /// Default is GetDefaultIcon.Instance then GetFromProfile.Instance.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry);


        /// <summary>
        /// Whether a path should be parsed (via ParseAsync) by this profile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool MatchPathPattern(string path);


        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        /// <summary>
        /// If specified (not null), used as Regex to determine if a path should be parsable by this profile.
        /// </summary>
        string[] PathPatterns { get; }
        string ProfileName { get; }
        byte[] ProfileIcon { get; }

        /// <summary>
        /// Convert Entry Model to another type.
        /// </summary>
        IConverterProfile[] Converters { get; }

        string RootDisplayName { get; }
        IPathHelper Path { get; }

        IEntryHierarchyComparer HierarchyComparer { get; }
        IMetadataProvider MetadataProvider { get; }
        IList<ICommandProvider> CommandProviders { get; }
        IDragDropHandler DragDrop { get; }
                
        IEventAggregator Events { get; }
        ISuggestSource SuggestSource { get; }

        /// <summary>
        /// Given {DeleteEntries}, Delete the entries, cannot delete if DeleteCommand is Null.
        /// </summary>
        IScriptCommand DeleteCommand { get; }

        /// <summary>
        /// Given {FolderName}, {BaseFolder}, Create a new folder and store it (IEntryModel) to {CreatedFolder}
        /// </summary>
        IScriptCommand CreateFolderCommand { get; }

        event EventHandler<EntryChangedEvent> OnEntryChanged;
        
        #endregion
    }

    
}
