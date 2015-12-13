using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Tools;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Models
{
    public class ExCommandProvider : ICommandProvider
    {
        FileSystemInfoExProfile _profile;
        public ExCommandProvider(FileSystemInfoExProfile profile)
        {
            _profile = profile;
        }

        public IEnumerable<ICommandModel> GetCommandModels()
        {
            yield return new OpenWithCommandEx(_profile) { IsVisibleOnToolbar = true };                    
        }
    }




    /// <summary>
    /// Launch a file.
    /// </summary>
    public class OpenWithCommandEx : DirectoryCommandModel
    {

        FileSystemInfoExProfile _profile;
        CancellationTokenSource _cts = new CancellationTokenSource();

        public OpenWithCommandEx(FileSystemInfoExProfile profile)
            : base()
        {
            _profile = profile;
            Command = new OpenWithScriptCommand();
        }

        public override void NotifySelectionChanged(IEntryModel[] appliedModels)
        {
            base.NotifySelectionChanged(appliedModels);

            _cts.Cancel();
            _cts = new CancellationTokenSource();
            IsEnabled = appliedModels.Count() == 1 &&
                appliedModels[0] is FileSystemInfoExModel;
            if (IsEnabled)
            {
                Header = appliedModels[0].IsDirectory ? "Explore" : "Open";
                var appliedModel = appliedModels[0];
                HeaderIconExtractor = ModelIconExtractor<ICommandModel>
                    .FromTaskFunc(t => 
                            GetFromSystemImageList.Instance.GetIconBytesForModelAsync(appliedModel, 
                            CancellationToken.None));

                Task.Run(() =>
                    {
                        List<ICommandModel> subCommands = new List<ICommandModel>();
                        if (appliedModel is FileSystemInfoExModel)
                            subCommands.AddRange(GetCommands(appliedModel as FileSystemInfoExModel));
                        return subCommands;
                    }).ContinueWith((prevTsk) =>
                        { SubCommands = (prevTsk as Task<List<ICommandModel>>).Result; }, _cts.Token, TaskContinuationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }

        }

        public IEnumerable<ICommandModel> GetCommands(FileSystemInfoExModel appliedModel)
        {

            if (!appliedModel.IsDirectory)
            {
                string ext = PathEx.GetExtension(appliedModel.Name);
                foreach (OpenWithInfo info in FileTypeInfoProvider.GetFileTypeInfo(ext).OpenWithList)
                    if (info.OpenCommand != null)
                    {
                        string executePath = OpenWithInfo.GetExecutablePath(info.OpenCommand);
                        string exeName = Path.GetFileNameWithoutExtension(executePath);

                        if (info.OpenCommand != null && File.Exists(executePath))
                        {
                            IEntryModel exeModel = AsyncUtils.RunSync(() => _profile.ParseAsync(executePath));
                            if (exeModel != null)
                                yield return new CommandModel(new OpenWithScriptCommand(info))
                                    {
                                        Header = String.Format("{0} ({1})", exeName, info.KeyName),
                                        ToolTip = info.Description,
                                        HeaderIconExtractor = 
                                            ModelIconExtractor<ICommandModel>
                                            .FromTaskFunc(t =>
                                                _profile.GetIconExtractSequence(exeModel)
                                                .Last().GetIconBytesForModelAsync(exeModel, 
                                                CancellationToken.None)),
                                        IsEnabled = true
                                    };

                        }
                    }

                yield return new CommandModel(new OpenWithScriptCommand(OpenWithInfo.OpenAs))
                {
                    Header = "Open with...",
                    IsEnabled = true
                };

            }
        }
    }
}
