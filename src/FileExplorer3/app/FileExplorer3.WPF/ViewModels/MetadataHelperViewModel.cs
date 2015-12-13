using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public interface IMetadataHelperViewModel : IEntriesHelper<IMetadataViewModel>
    {
       IMetadataProvider[] ExtraMetadataProviders { get; set;}
    }

    public class MetadataHelperViewModel : EntriesHelper<IMetadataViewModel>, IMetadataHelperViewModel
    {
        #region Cosntructor

        public MetadataHelperViewModel(Func<IMetadata, bool> filter = null)
            : base()
        {
            _loadSubEntryFunc = (b,p) => loadEntriesTask(p as IFileListViewModel); 
            _filter = filter ?? (m => true);
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<IMetadataViewModel>> loadEntriesTask(IFileListViewModel flvm)
        {
            if (flvm == null)
                return new List<IMetadataViewModel>();

            var retList = new List<IMetadata>();
            var selectedItems = flvm.Selection.SelectedItems.Select(evm => evm.EntryModel).ToList();
            var allCount =  flvm.ProcessedEntries.All.Count;

            foreach (var mp in ExtraMetadataProviders)
                retList.AddRange(await mp.GetMetadataAsync(selectedItems, allCount, 
                    flvm.CurrentDirectory));

            retList.AddRange(await flvm.CurrentDirectory.Profile.MetadataProvider.GetMetadataAsync(
                    flvm.Selection.SelectedItems.Select(evm => evm.EntryModel),
                    flvm.ProcessedEntries.All.Count,
                    flvm.CurrentDirectory));
                    
            return retList.Where(m => _filter(m))
                    .Distinct()
                    .Select(m => MetadataViewModel.FromMetadata(m));;
        }


        #endregion

        #region Data

        private Func<IMetadata, bool> _filter;
        private IMetadataProvider[] _extraMetadataProviders = new IMetadataProvider[] { };
        //private IFileListViewModel _flvm;


        #endregion

        #region Public Properties

        public IMetadataProvider[] ExtraMetadataProviders
        {
            get { return _extraMetadataProviders; }
            set { _extraMetadataProviders = value; NotifyOfPropertyChanged(() => ExtraMetadataProviders); }
        }

        #endregion




    }
}

