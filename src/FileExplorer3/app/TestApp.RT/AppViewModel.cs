using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.ViewModels;
using TestApp.RT.Model;
using Windows.Storage.Pickers;

namespace TestApp.RT
{
    public class AppViewModel : Screen
    {

        #region Cosntructor

        public AppViewModel(IEventAggregator events)
        {
            FileListModel = new FileListViewModel(events);
        }

        #endregion

        #region Methods



        public async Task Load()
        {
            FolderPicker fp = new FolderPicker();
            fp.FileTypeFilter.Add("*");
            var outputFolder = await fp.PickSingleFolderAsync();
            var profile = new StorageItemProfile();
            var parentModel = profile.GetModelFor(outputFolder);

            await FileListModel.LoadAsync(profile, parentModel, null);

            //ActionExecutionContext context = new ActionExecutionContext();
            //foreach (var r in FileListModel.Load(profile, parentModel, null))
            //    await r.ExecuteAsync(context);
        }

        public IEnumerable<IResult> ChangeView(string viewMode)
        {
            //if (String.IsNullOrEmpty(viewMode))
            //    new List<IResult>();

            //return FileListModel.ChangeView(viewMode);
            yield break;
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public IEventAggregator Events { get; private set; }
        public FileListViewModel FileListModel { get; private set; }

        #endregion


    }
}
