using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using System.Threading;
using FileExplorer.WPF.Models;
using FileExplorer.IO;

namespace FileExplorer.Models
{
    public class GoogleExportCommandModel : DirectoryCommandModel
    {

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModelFunc">Root directory used when saving.</param>
        public GoogleExportCommandModel(Func<IEntryModel[]> rootModelFunc)
        {
            _rootModelFunc = rootModelFunc;
            this.HeaderIconExtractor = 
                ResourceIconExtractor<ICommandModel>
                    .ForSymbol(0xE118);
            this.Header = "Export";
            this.IsEnabled = false;
        }

        #endregion

        #region Methods

        public override void NotifySelectionChanged(IEntryModel[] appliedModels)
        {
            List<ICommandModel> subItemList = new List<ICommandModel>();
            if (appliedModels.Count() == 1)
            {
                GoogleDriveItemModel model = appliedModels[0] as GoogleDriveItemModel;
                if (model != null && model.Metadata != null && model.Metadata.ExportLinks != null)
                    foreach (var mimeType in model.Metadata.ExportLinks.Keys)
                    {
                        string url = model.Metadata.ExportLinks[mimeType];

                        string ext = null;
                        var match = Regex.Match(url, "[&]exportFormat=(?<ext>[\\w]*)$", RegexOptions.IgnoreCase);
                        if (match.Success)
                            ext = "." + match.Groups["ext"].Value;
                        else ext = ShellUtils.MIMEType2Extension(mimeType);

                        string filter = String.Format("{0} ({1})|*{1}", mimeType, ext);
                        string defaultName = System.IO.Path.ChangeExtension(appliedModels[0].Name, ext);

                        if (ext != null)
                            subItemList.Add(
                                new CommandModel(
                                    //SaveFile --> ParseOrCreatePath -> ShowProgress -> Download -> HideProgress
                                    //         --> OK (If cancel)
                                    WPFScriptCommands.SaveFilePicker(WindowManager, Events, 
                                     _rootModelFunc(), filter, defaultName,
                                     (fi) => WPFScriptCommands.ParseOrCreatePath(fi.Profile as IDiskProfile, 
                                         fi.FileName, false, 
                                         (m) => WPFScriptCommands.ShowProgress(WindowManager, "Saving", 
                                                    WPFScriptCommands.Download(url, m, 
                                                    (appliedModels[0].Profile as GoogleDriveProfile)
                                                    .HttpClientFunc(), 
                                                    new HideProgress()))), 
                                         ResultCommand.OK)
                                )
                                    {
                                        Header = String.Format("{0} ({1})", mimeType, ext),
                                        IsEnabled = true,
                                        IsVisibleOnMenu = true
                                    });
                    }
            }
            
            SubCommands = subItemList;
            this.IsEnabled = subItemList.Count() > 0;
        }

        #endregion

        #region Data

        private Func<IEntryModel[]> _rootModelFunc;

        #endregion

        #region Public Properties

        public IWindowManager WindowManager { get; set; }
        public IEventAggregator Events { get; set; }

        #endregion


        
    }
}
