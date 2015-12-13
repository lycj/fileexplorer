using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using System.Net.Http;
using System.Net;
using System.Threading;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using System.IO;
using FileExplorer.WPF.Defines;
using FileExplorer.IO;
using System.ComponentModel;

namespace FileExplorer.Script
{

    public static partial class WPFScriptCommands
    {
        /// <summary>
        /// Return pd["Parameter"] as IEntryModel[]
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static IEntryModel[] GetEntryModelFromParameter(ParameterDic pd)
        {
            var evms = pd.GetValue("{FileList.Selection.SelectedItems}") as List<IEntryViewModel>;
            return (evms != null) ? evms.Select(evm => evm.EntryModel).ToArray() : new IEntryModel[] { };            
        }

        public static IEntryModel GetFirstEntryModelFromParameter(ParameterDic pd)
        {
            return GetEntryModelFromParameter(pd).FirstOrDefault();
        }

        public static IScriptCommand DoSelection(string commandKey,
            Func<ParameterDic, IEntryViewModel[]> getSelectionFunc,
            Func<IEntryViewModel[], IScriptCommand> nextCommandFunc,
            IScriptCommand noSelectionCommand)
        {
            return new DoSelection(commandKey, getSelectionFunc, nextCommandFunc, noSelectionCommand);
        }

        [Obsolete("ScriptCommands.Assign")]
        public static IScriptCommand AssignVariableToAnotherVariable(string sourceName, string targetName,
            IScriptCommand thenCommand)
        {
            return new SimpleScriptCommand("AssignVariable", pm =>
            {
                pm[targetName] = pm[sourceName];
                return thenCommand;
            }, pm => pm.ContainsKey(sourceName));
        }

        [Obsolete("ScriptCommands.Assign")]
        public static IScriptCommand AssignVariableToParameter(string sourceName,
           IScriptCommand thenCommand)
        {
            return AssignVariableToAnotherVariable(sourceName, "Parameter", thenCommand);
        }

    }

    #region MessageBox

    public static partial class WPFScriptCommands
    {
        public static IfOkCancel IfOkCancel(IWindowManager wm, Func<ParameterDic, string> captionFunc,
            Func<ParameterDic, string> messageFunc, IScriptCommand okCommand,
            IScriptCommand cancelCommand)
        { return new IfOkCancel(wm, captionFunc, messageFunc, okCommand, cancelCommand); }

        public static ShowFilePicker SaveFilePicker(IExplorerInitializer initializer, string filter, string defaultFileName,
          Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            return new ShowFilePicker(initializer, FilePickerMode.Save, filter, defaultFileName,
                successCommand, cancelCommand);
        }

        [Obsolete]
        public static ShowFilePicker SaveFilePicker(IWindowManager wm, IEventAggregator events,
            IEntryModel[] rootDirModels, string filter, string defaultFileName,
            Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            if (wm == null) wm = new WindowManager();
            return new ShowFilePicker(wm, events, rootDirModels, FilePickerMode.Save, filter, defaultFileName,
                successCommand, cancelCommand);
        }

        [Obsolete]
        public static ShowFilePicker SaveFilePicker(IEntryModel[] rootDirModels, string filter, string defaultFileName,
           Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            return SaveFilePicker(null, null, rootDirModels, filter, defaultFileName,
                successCommand, cancelCommand);
        }

        public static ShowDirectoryPicker ShowDirectoryPicker(IExplorerInitializer initializer, IProfile[] rootProfiles,
            Func<IEntryModel, IScriptCommand> nextCommandFunc, IScriptCommand cancelCommand)
        {
            return new ShowDirectoryPicker(initializer, rootProfiles, nextCommandFunc, cancelCommand);
        }

        public static ShowFilePicker OpenFileDialog(IExplorerInitializer initializer, string filter, string defaultFileName,
         Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            return new ShowFilePicker(initializer, FilePickerMode.Open, filter, defaultFileName,
                successCommand, cancelCommand);
        }

        [Obsolete]
        public static ShowFilePicker OpenFileDialog(IWindowManager wm, IEventAggregator events,
            IEntryModel[] rootDirModels, string filter, string defaultFileName,
            Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            return new ShowFilePicker(wm, events, rootDirModels, FilePickerMode.Open, filter,
                defaultFileName, successCommand, cancelCommand);
        }

        [Obsolete]
        public static ShowFilePicker OpenFileDialog(IEntryModel[] rootDirModels, string filter, string defaultFileName,
         Func<IEntryModelInfo, IScriptCommand> successCommand, IScriptCommand cancelCommand)
        {
            return OpenFileDialog(new WindowManager(), null, rootDirModels, filter, defaultFileName, successCommand, cancelCommand);
        }

        public static ShowMessageBox MessageBox(string caption, string message)
        {
            return new ShowMessageBox(caption, message);
        }

        
        [Obsolete("CoreScriptCommands.Download")]
        public static DownloadFile Download(string sourceUrl, IEntryModel entry,
           HttpClient httpClient, IScriptCommand nextCommand = null)
        {            
            return new DownloadFile(sourceUrl, entry, httpClient, nextCommand);
        }

        [Obsolete("CoreScriptCommands.Download")]
        public static IScriptCommand Download(string sourceUrl, IEntryModel entry, IScriptCommand nextCommand = null)
        {
            return new DownloadFile(sourceUrl, entry, null, nextCommand);
        }

        /// <summary>
        /// Show progress dialog, assign these variables to ParameterDic:
        /// ProgressHeader (header), Progress (ProgressDialogViewModel), CancellationToken (ProgressDialogViewModel's CancellationToken.)
        /// </summary>
        /// <param name="wm"></param>
        /// <param name="header"></param>
        /// <param name="nextCommand"></param>
        /// <param name="handleProgress">If true, run nextCommand inside the script command instead of return it.</param>
        /// <returns></returns>
        public static ShowProgress ShowProgress(IWindowManager wm, string header, IScriptCommand nextCommand, bool handleProgress = true)
        {
            return new ShowProgress(wm, header, nextCommand, handleProgress);
        }

        /// <summary>
        /// Show progress dialog, assign these variables to ParameterDic:
        /// ProgressHeader (header), Progress (ProgressDialogViewModel), CancellationToken (ProgressDialogViewModel's CancellationToken.)
        /// </summary>
        /// <param name="header"></param>
        /// <param name="nextCommand"></param>
        /// <param name="handleProgress">If true, run nextCommand inside the script command instead of return it.</param>
        /// <returns></returns>
        public static ShowProgress ShowProgress(string header, IScriptCommand nextCommand, bool handleProgress = true)
        {
            return new ShowProgress(new WindowManager(), header, nextCommand, handleProgress);
        }

        public static ReportProgress ReportProgress(TransferProgress progress, IScriptCommand nextCommand = null)
        {
            return new ReportProgress(progress, nextCommand);
        }

        public static HideProgress HideProgress(IScriptCommand nextCommand = null)
        {
            return new HideProgress(nextCommand);
        }

        /// <summary>
        /// If events is not assignd here, it must be assigned in ParameterDic (Events) when run.
        /// </summary>
        public static IScriptCommand PublishEvent(object evnt, IScriptCommand nextCommand = null, IEventAggregator events = null)
        {
            return new PublishEvent(events, evnt, nextCommand);
        }

        public static IScriptCommand BroadcastEvent(object evnt, IScriptCommand nextCommand = null, IEventAggregator events = null)
        {
            return new PublishEvent(events, new BroadcastEvent(evnt), nextCommand);
        }
    }


    /// <summary>
    /// If user clicked Ok, do first command, otherwise do second command.
    /// </summary>
    public class IfOkCancel : IfScriptCommand
    {
        private IScriptCommand _okCommand;
        private IScriptCommand _cancelCommand;
        internal IfOkCancel(IWindowManager wm, Func<ParameterDic, string> captionFunc,
            Func<ParameterDic, string> messageFunc, IScriptCommand okCommand,
            IScriptCommand cancelCommand)
            : base(
                   pd =>
                   {
                       var mdv = new MessageDialogViewModel(captionFunc(pd), messageFunc(pd),
                           MessageDialogViewModel.DialogButtons.Cancel | MessageDialogViewModel.DialogButtons.OK);
                       if (wm.ShowDialog(mdv).Value)
                           return mdv.SelectedButton == MessageDialogViewModel.DialogButtons.OK;
                       return false;
                   },
                    okCommand, cancelCommand)
        {
            _okCommand = okCommand;
            _cancelCommand = cancelCommand;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return _okCommand.CanExecute(pm);
        }
    }

    public class ShowFilePicker : ScriptCommandBase
    {

        private string _filter;
        private string _defaultFileName;
        private Func<IEntryModelInfo, IScriptCommand> _successCommandFunc;

        private IScriptCommand _cancelFunc;
        private FilePickerMode _mode;
        private IExplorerInitializer _initializer;

        internal ShowFilePicker(IExplorerInitializer initializer, FilePickerMode mode, string filter, string defaultFileName,
           Func<IEntryModelInfo, IScriptCommand> successCommandFunc, IScriptCommand cancelCommand)
            : base(mode.ToString() + "File")
        {
            _initializer = initializer;
            _filter = filter;
            _mode = mode;
            _defaultFileName = defaultFileName;
            _successCommandFunc = successCommandFunc;
            _cancelFunc = cancelCommand;
        }

        internal ShowFilePicker(IWindowManager wm, IEventAggregator events,
            IEntryModel[] rootDirModels, FilePickerMode mode, string filter, string defaultFileName,
            Func<IEntryModelInfo, IScriptCommand> successCommandFunc, IScriptCommand cancelCommand)
            : this(new ExplorerInitializer(wm, events, rootDirModels), mode,
            filter, defaultFileName, successCommandFunc, cancelCommand)
        {
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var filePicker = new FilePickerViewModel(_initializer, _filter, _mode);
            if (!String.IsNullOrEmpty(_defaultFileName))
                filePicker.FileName = _defaultFileName;
            if (_initializer.WindowManager.ShowDialog(filePicker).Value)
            {
                string mode = _mode == FilePickerMode.Open ? "Open" : "Save";

                pm[mode + "FileName"] = filePicker.FileName; //OpenFileName or SaveFileName
                pm[mode + "Profile"] = filePicker.Profile;  //OpenProfile or SaveProfile

                return _successCommandFunc(filePicker);
            }
            else return _cancelFunc;
        }
    }


    //Serializable
    public class ShowMessageBox : ScriptCommandBase
    {
        public string Caption { get; set; }
        public string Message { get; set; }


        /// <summary>
        /// Used by serializer only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ShowMessageBox()
            : base("ShowMessageBox")
        {

        }

        internal ShowMessageBox(string caption, string message)
            : base("ShowMessageBox")
        {
            Caption = caption;
            Message = message;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IWindowManager _wm = pm.ContainsKey("WindowManager") && pm["WindowManager"] is IWindowManager ?
                (IWindowManager)pm["WindowManager"] : new WindowManager();
            var mdv = new MessageDialogViewModel(Caption, Message,
                                 MessageDialogViewModel.DialogButtons.OK);
            _wm.ShowDialog(mdv);
            return ResultCommand.NoError;
        }
    }

    public class ShowProgress : ScriptCommandBase
    {
        private IWindowManager _wm;
        private string _header;
        private bool _handleProgress;

        internal ShowProgress(IWindowManager wm, string header, IScriptCommand nextCommand, bool handleProgress)
            : base("ShowProgress", nextCommand)
        {
            ContinueOnCaptureContext = true;
            _wm = wm;
            _header = header;
            _handleProgress = handleProgress;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            pm["ProgressHeader"] = _header;
            var pdv = new ProgressDialogViewModel(pm);
            pm["Progress"] = pdv;
            pm["CancellationToken"] = pdv.CancellationToken;

            _wm.ShowWindow(pdv);
            if (!_handleProgress)
                return _nextCommand;

            try
            {
                ScriptRunner.RunScript(pm, _nextCommand);
            }
            catch (Exception ex)
            {
                return WPFScriptCommands.ReportProgress(TransferProgress.Error(ex));
            }

            return new HideProgress();
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            pm["ProgressHeader"] = _header;
            var pdv = new ProgressDialogViewModel(pm);
            pm["Progress"] = pdv;
            pm["CancellationToken"] = pdv.CancellationToken;

            _wm.ShowWindow(pdv);
            if (!_handleProgress)
                return _nextCommand;

            try
            {
                await ScriptRunner.RunScriptAsync(pm, _nextCommand).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                return WPFScriptCommands.ReportProgress(TransferProgress.Error(ex));
            }

            return new HideProgress();
        }
    }

    public class ReportProgress : ScriptCommandBase
    {
        private TransferProgress _progress;
        public ReportProgress(TransferProgress progress, IScriptCommand nextCommand = null)
            : base("ReportProgress", nextCommand)
        {
            _progress = progress;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (pm.ContainsKey("Progress"))
            {
                var pdv = pm["Progress"] as ProgressDialogViewModel;
                pdv.Report(_progress);
            }
            return _nextCommand;
        }
    }

    public class HideProgress : ScriptCommandBase
    {
        public HideProgress(IScriptCommand nextCommand = null)
            : base("HideProgress", nextCommand)
        {

        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (pm.ContainsKey("Progress"))
            {
                var pdv = pm["Progress"] as ProgressDialogViewModel;
                pdv.TryClose();
            }
            return _nextCommand;
        }
    }

    /// <summary>
    /// Serializable, Download source Url to "Stream" property,
    /// 
    /// Variable in ParameterDic:
    /// Progress :  IProgress[TransferProgress] (Optional)
    /// HttpClientFunc : Func[HttpClient] (Optional)
    /// Stream : MemoryStream (Output)
    /// </summary>
    public class DownloadFile2 : ScriptCommandBase
    {
        public string SourceUrl { get; set; }
        public string Destination { get; set; }

        public DownloadFile2(IScriptCommand nextCommand)
            : base("Download2", nextCommand, "Progress", "HttpClientFunc", "Stream")
        {

        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var pdv = pm.ContainsKey("Progress") && pm["Progress"] is IProgress<TransferProgress>
                ? pm["Progress"] as IProgress<TransferProgress> :
                NullProgresViewModel.Instance;

            try
            {
                using (var httpClient =
                    pm.ContainsKey("HttpClientFunc") && pm["HttpClientFunc"] is Func<HttpClient> ? ((Func<HttpClient>)pm["HttpClientFunc"])() :
                    new HttpClient())
                {
                    var response = await httpClient.GetAsync(SourceUrl, HttpCompletionOption.ResponseHeadersRead, pm.CancellationToken);
                    if (!response.IsSuccessStatusCode)
                        throw new WebException(String.Format("{0} when downloading {1}", response.StatusCode, SourceUrl));

                    MemoryStream destStream;
                    pm["Stream"] = destStream = new MemoryStream();

                    using (Stream srcStream = await response.Content.ReadAsStreamAsync())
                    {
                        pdv.Report(TransferProgress.From(SourceUrl, Destination));
                        byte[] buffer = new byte[1024];
                        ulong totalBytesRead = 0;
                        ulong totalBytes = 0;
                        try { totalBytes = (ulong)srcStream.Length; }
                        catch (NotSupportedException) { }

                        int byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);
                        while (byteRead > 0)
                        {
                            await destStream.WriteAsync(buffer, 0, byteRead, pm.CancellationToken);
                            totalBytesRead = totalBytesRead + (uint)byteRead;
                            short percentCompleted = (short)((float)totalBytesRead / (float)totalBytes * 100.0f);
                            pdv.Report(TransferProgress.UpdateCurrentProgress(percentCompleted));

                            byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);

                        }
                        await destStream.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return ResultCommand.Error(ex);
            }


            return _nextCommand;
        }

    }


    public class DownloadFile : ScriptCommandBase
    {
        private string _sourceUrl;
        private bool _disposeHttpClient;
        private Func<HttpClient> _httpClientFunc;
        private Func<CancellationToken, Task<Stream>> _destStreamFunc;
        private string _destId = null;
        public DownloadFile(string sourceUrl, Func<CancellationToken, Task<Stream>> destStreamFunc,
            Func<HttpClient> httpClientFunc, IScriptCommand nextCommand = null)
            : base("Download", nextCommand)
        {
            _sourceUrl = sourceUrl;
            _destStreamFunc = destStreamFunc;
            _httpClientFunc = httpClientFunc;
            _disposeHttpClient = true;
        }

        public DownloadFile(string sourceUrl, Func<CancellationToken, Task<Stream>> destStreamFunc,
            HttpClient httpClient, IScriptCommand nextCommand = null)
            : base("Download", nextCommand)
        {
            _sourceUrl = sourceUrl;
            _destStreamFunc = destStreamFunc;
            _httpClientFunc = () => httpClient;
            _disposeHttpClient = false;
        }

        public DownloadFile(string sourceUrl, IEntryModel entry,
           HttpClient httpClient, IScriptCommand nextCommand = null)
            : this(sourceUrl, (ct) =>
                (entry.Profile as IDiskProfile).DiskIO.OpenStreamAsync(entry,
                FileExplorer.Defines.FileAccess.Write, ct), httpClient, nextCommand)
        {
            _destId = entry.FullPath;
        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var httpClient = _httpClientFunc();
            var pdv = pm.ContainsKey("Progress") && pm["Progress"] is IProgress<TransferProgress>
                ? pm["Progress"] as IProgress<TransferProgress> :
                NullProgresViewModel.Instance;

            if (httpClient != null)
                try
                {
                    var response = await httpClient.GetAsync(_sourceUrl, HttpCompletionOption.ResponseHeadersRead, pm.CancellationToken);
                    if (!response.IsSuccessStatusCode)
                        throw new WebException(String.Format("{0} when downloading {1}", response.StatusCode, _sourceUrl));

                    using (Stream srcStream = await response.Content.ReadAsStreamAsync())
                    using (Stream destStream = await _destStreamFunc(pm.CancellationToken))
                    {
                        pdv.Report(TransferProgress.From(_sourceUrl, _destId));
                        byte[] buffer = new byte[1024];
                        ulong totalBytesRead = 0;
                        ulong totalBytes = 0;
                        try { totalBytes = (ulong)srcStream.Length; }
                        catch (NotSupportedException) { }

                        int byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);
                        while (byteRead > 0)
                        {
                            await destStream.WriteAsync(buffer, 0, byteRead, pm.CancellationToken);
                            totalBytesRead = totalBytesRead + (uint)byteRead;
                            short percentCompleted = (short)((float)totalBytesRead / (float)totalBytes * 100.0f);
                            pdv.Report(TransferProgress.UpdateCurrentProgress(percentCompleted));

                            byteRead = await srcStream.ReadAsync(buffer, 0, buffer.Length, pm.CancellationToken);

                        }
                        await destStream.FlushAsync();
                    }
                }
                finally
                {
                    if (_disposeHttpClient && httpClient != null)
                        httpClient.Dispose();

                }

            return _nextCommand;
        }
    }

    internal class PublishEvent : ScriptCommandBase
    {
        private IEventAggregator _events;
        private object _evnt;
        public PublishEvent(IEventAggregator events, object evnt, IScriptCommand nextCommand)
            : base("PublishEvent", nextCommand)
        {
            _events = events;
            _evnt = evnt;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            _events = _events ?? pm.AsVMParameterDic().Events;
            _events.PublishOnUIThread(_evnt);
            return _nextCommand;
        }
    }

    #endregion


    [Obsolete]
    public static class TabbedExplorer
    {
        [Obsolete("UIScriptCommands.CloseExplorerTab")]
        public static IScriptCommand CloseTab(ITabbedExplorerViewModel tevm)
        {
            return new CloseTab(tevm);
        }

        [Obsolete("UIScriptCommands.NewExplorerTab")]
        public static IScriptCommand NewTab = new OpenTab();

        [Obsolete]
        public static IScriptCommand AssignActiveTabToParameter(IScriptCommand thenCommand)
        {
            return Do((tevm, pd) => { pd.Parameter = tevm.ActiveItem; return thenCommand; });
        }

        /// <summary>
        /// Open directory (specified as Parameter) in new tab.
        /// </summary>
        /// <param name="tevm"></param>
        /// <returns></returns>
        [Obsolete("UIScriptCommands.NewExplorerTab")]
        public static IScriptCommand OpenTab(ITabbedExplorerViewModel tevm)
        {
            return new OpenTab(m => m != null && m.IsDirectory, tevm);
        }

        [Obsolete]
        public static IScriptCommand Do(Func<ITabbedExplorerViewModel, ParameterDic, IScriptCommand> commandFunc)
        {
            return new DoTabbedExplorer(commandFunc);
        }


       
    }

    [Obsolete]
    internal class CloseTab : ScriptCommandBase
    {
        private ITabbedExplorerViewModel _tevm;
        public CloseTab(ITabbedExplorerViewModel tevm = null)
            : base("CloseTab", "Parameter", "TabbedExplorer")
        {
            _tevm = tevm;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var tevm = (_tevm ?? pm["TabbedExplorer"]) as ITabbedExplorerViewModel;
            if (tevm == null)
                return ResultCommand.Error(new ArgumentNullException("TabbedExplorer"));
            var expvm = pm.Parameter as IExplorerViewModel ??
                tevm.ActiveItem as IExplorerViewModel;
            if (expvm == null)
                return ResultCommand.Error(new ArgumentException("Parameter"));

            tevm.CloseTab(expvm); ;
            return ResultCommand.NoError;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            if (_tevm == null && !pm.ContainsKey("TabbedExplorer"))
                return false;

            if (!pm.ContainsKey("Parameter") || !(pm["Parameter"] is IExplorerViewModel))
                return false;

            return true;
        }
    }

    [Obsolete]
    internal class OpenTab : ScriptCommandBase
    {
        private Func<IEntryModel, bool> _filter;
        private ITabbedExplorerViewModel _tevm;
        public OpenTab(Func<IEntryModel, bool> filter = null, ITabbedExplorerViewModel tevm = null)
            : base("OpenTab", "Parameter", "TabbedExplorer")
        {
            _filter = filter;
            _tevm = tevm;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var pd = pm.AsUIParameterDic();
            IEntryModel dirModel = null;
            if (pd.Parameter is IEntryModel[])
                dirModel = (pd.Parameter as IEntryModel[]).FirstOrDefault();
            else if (pd.Parameter is IEntryModel)
                dirModel = pd.Parameter as IEntryModel;

            var tevm = (_tevm ?? pm["TabbedExplorer"]) as ITabbedExplorerViewModel;
            if (tevm == null)
                return ResultCommand.Error(new ArgumentNullException("TabbedExplorer"));
            if (_filter == null || _filter(dirModel))
                tevm.OpenTab(dirModel);
            return ResultCommand.NoError;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            if (_tevm == null && !pm.ContainsKey("TabbedExplorer"))
                return false;

            if (_filter == null)
                return true;

            var pd = pm.AsUIParameterDic();
            IEntryModel dirModel = null;
            if (pd.Parameter is IEntryModel[])
                dirModel = (pd.Parameter as IEntryModel[]).FirstOrDefault();
            else if (pd.Parameter is IEntryModel)
                dirModel = pd.Parameter as IEntryModel;

            return _filter(dirModel);
        }
    }

    [Obsolete]
    internal class DoTabbedExplorer : DoCommandBase<ITabbedExplorerViewModel>
    {
        internal DoTabbedExplorer(Func<ITabbedExplorerViewModel, ParameterDic, IScriptCommand> commandFunc)
            : base("TabbedExplorer", commandFunc)
        {
        }

        protected DoTabbedExplorer(Func<ITabbedExplorerViewModel, ParameterDic, Task<IScriptCommand>> commandFunc)
            : base("TabbedExplorer", commandFunc)
        {
        }

        internal DoTabbedExplorer(string commandKey, Func<ITabbedExplorerViewModel, ParameterDic, IScriptCommand> commandFunc)
            : base(commandKey, commandFunc)
        {
        }

        protected DoTabbedExplorer(string commandKey, Func<ITabbedExplorerViewModel, ParameterDic, Task<IScriptCommand>> commandFunc)
            : base(commandKey, commandFunc)
        {
        }
    }

    #region ExplorerBased

    public static class Explorer
    {
        //public static IScriptCommand TryCloseWindow =
        //    Explorer.Do(evm => { evm.TryClose(); return ResultCommand.NoError; });

        public static IScriptCommand Do(Func<IExplorerViewModel, IScriptCommand> commandFunc)
        {
            return new DoExplorer(commandFunc);
        }

        public static IScriptCommand DoSelection(Func<IEntryViewModel[], IScriptCommand> nextCommandFunc,
           IScriptCommand noSelectionCommand = null)
        {
            return new DoSelection("DoSelectionExplorer", WPFExtensionMethods.GetCurrentDirectoryVMFunc,
                nextCommandFunc, noSelectionCommand);
        }




        //public static IScriptCommand GoTo(IScriptCommand thenCommand = null)
        //{
        //    return new GotoDirectory(thenCommand);
        //}
        [Obsolete("UIScriptCommands.GoTo")]
        public static IScriptCommand GoTo(IEntryModel dir, IScriptCommand thenCommand = null)
        {
            return new GotoDirectory(dir, thenCommand);
        }

        [Obsolete("UIScriptCommands.GoTo")]
        public static IScriptCommand GoTo(string path, IScriptCommand thenCommand = null)
        {
            return new GotoDirectory(path, thenCommand);
        }

        public static IScriptCommand Zoom(ZoomMode mode, int multiplier = 1, IScriptCommand nextCommand = null)
        {
            nextCommand = nextCommand ?? ResultCommand.NoError;
            float offset = (float)(mode == ZoomMode.ZoomIn ? 0.1 : -0.1) * multiplier;
            return Do(evm => { evm.Parameters.UIScale += offset; return nextCommand; });
        }

        //public static IScriptCommand GoTo(string path, IScriptCommand thenCommand = null)
        //{
        //    return new GotoDirectory(path, thenCommand);
        //}

        //public static IScriptCommand NewWindow(IExplorerInitializer initializer,
        //    string selectedDirectoryPath, bool openIfNotFound = true)
        //{
        //    return ScriptCommands.ParsePath(initializer.RootModels.GetProfiles(), selectedDirectoryPath,
        //        dirM => Explorer.NewWindow(initializer, dirM),
        //        openIfNotFound ? Explorer.NewWindow(initializer) :
        //        ResultCommand.Error(new System.IO.FileNotFoundException(selectedDirectoryPath)));
        //}

        public static IScriptCommand NewWindow(IExplorerInitializer initializer)
        {
            return NewWindow(initializer, (IEntryModel)null);
        }

        public static IScriptCommand NewWindow(IExplorerInitializer initializer,
            IEntryModel startupDirectory)
        {
            return NewWindow(initializer, null, startupDirectory);
        }

        public static IScriptCommand NewWindow(IExplorerInitializer initializer, object context,
            IEntryModel startupDirectory)
        {
            var dic = startupDirectory == null ? null :
                new Dictionary<string, object>() { { "StartupDirectory", startupDirectory } };
            return new ShowNewExplorer(initializer, null, dic);
        }

        //public static IScriptCommand NewWindow(IExplorerInitializer initializer,
        //    Func<ParameterDic, IEntryModel[]> getSelectedDirectryFunc)
        //{
        //    return new OpenInNewWindowCommand(initializer, getSelectedDirectryFunc);
        //}

        public static IScriptCommand PickDirectory(IExplorerInitializer initializer,
          IProfile[] rootProfiles, Func<IEntryModel, IScriptCommand> nextCommandFunc, IScriptCommand cancelCommand = null)
        {
            return new ShowDirectoryPicker(initializer, rootProfiles, nextCommandFunc, cancelCommand);
        }

        [Obsolete("UIScriptCommands.NotifyRootChanged")]
        public static IScriptCommand ChangeRoot(ChangeType changeType,
            IEntryModel[] appliedRootDirectories, IScriptCommand nextCommand = null)
        {
            return new ChangeRootCommand(changeType, appliedRootDirectories, nextCommand);
        }

         [Obsolete("UIScriptCommands.NotifyRootChanged")]
        public static IScriptCommand BroadcastRootChanged(RootChangedEvent evnt, IEventAggregator events = null,
            IScriptCommand nextCommand = null)
        {
            return new BroadcastChangeRoot(evnt, events, nextCommand);
        }




    }

    internal abstract class DoCommandBase<VM> : ScriptCommandBase
    {
        private Func<VM, ParameterDic, Task<IScriptCommand>> _commandFunc;
        private string _viewModelName;
        protected DoCommandBase(string viewModelName, Func<VM, ParameterDic, Task<IScriptCommand>> commandFunc)
            : base(viewModelName, viewModelName)
        {
            _viewModelName = viewModelName;
            _commandFunc = commandFunc;
        }

        protected DoCommandBase(string viewModelName, Func<VM, ParameterDic, IScriptCommand> commandFunc)
            : this(viewModelName, (vm, pd) => Task.Run(() => commandFunc(vm, pd)))
        {

        }

        protected DoCommandBase(string viewModelName, Func<VM, Task<IScriptCommand>> commandFunc)
            : this(viewModelName, (vm, pd) => commandFunc(vm))
        {
        }

        protected DoCommandBase(string viewModelName, Func<VM, IScriptCommand> commandFunc)
            : this(viewModelName, (vm, pd) => commandFunc(vm))
        {

        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            VM evm = (VM)pm[_viewModelName];
            if (evm == null)
                return ResultCommand.Error(new ArgumentException(_viewModelName));
            return await _commandFunc(evm, pm);
        }

        public override bool CanExecute(ParameterDic pm)
        {
            if (pm.ContainsKey(_viewModelName))
            {
                VM evm = (VM)pm[_viewModelName];
                return evm != null;// && AsyncUtils.RunSync(() => _commandFunc(evm)).CanExecute(pm);
            }
            return false;
        }
    }

    internal class DoSelection : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryViewModel[]> _getSelectionFunc;
        private Func<IEntryViewModel[], IScriptCommand> _nextCommandFunc;
        private IScriptCommand _noSelectionCommand;

        internal DoSelection(string commandKey,
            Func<ParameterDic, IEntryViewModel[]> getSelectionFunc,
            Func<IEntryViewModel[], IScriptCommand> nextCommandFunc,
            IScriptCommand noSelectionCommand)
            : base(commandKey)
        {
            _getSelectionFunc = getSelectionFunc;
            _nextCommandFunc = nextCommandFunc;
            _noSelectionCommand = noSelectionCommand;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var selection = _getSelectionFunc(pm);
            if (selection == null || selection.Length == 0 || selection[0] == null)
                return _noSelectionCommand;
            else return _nextCommandFunc(selection);
        }

        public override bool CanExecute(ParameterDic pm)
        {
            IScriptCommand nextCommand = Execute(pm);
            return nextCommand != null && nextCommand.CanExecute(pm);
        }
    }

    internal class DoExplorer : DoCommandBase<IExplorerViewModel>
    {
        internal DoExplorer(Func<IExplorerViewModel, IScriptCommand> commandFunc)
            : base("Explorer", commandFunc)
        {
        }

        protected DoExplorer(Func<IExplorerViewModel, Task<IScriptCommand>> commandFunc)
            : base("Explorer", commandFunc)
        {
        }

        internal DoExplorer(string commandKey, Func<IExplorerViewModel, IScriptCommand> commandFunc)
            : base(commandKey, commandFunc)
        {
        }

        protected DoExplorer(string commandKey, Func<IExplorerViewModel, Task<IScriptCommand>> commandFunc)
            : base(commandKey, commandFunc)
        {
        }
    }



    internal class BroadcastChangeRoot : ScriptCommandBase
    {
        private RootChangedEvent _evnt;
        private IEventAggregator _events;

        internal BroadcastChangeRoot(RootChangedEvent evnt, IEventAggregator events = null,
            IScriptCommand nextCommand = null)
            : base("BroadcastChangeRoot", nextCommand, "Events")
        {
            _events = events;
            _evnt = evnt;
        }


        public override IScriptCommand Execute(ParameterDic pm)
        {
            _events = _events ?? pm["Events"] as IEventAggregator;
            if (_events != null)
                return WPFScriptCommands.BroadcastEvent(_evnt, _nextCommand, _events);

            return ResultCommand.Error(new ArgumentException("Events"));
        }

    }

    /// <summary>
    /// Used by Change root directory in current ExplorerViwModel 
    /// </summary>
    internal class ChangeRootCommand : ScriptCommandBase
    {
        private ChangeType? _changeType;
        private IEntryModel[] _appliedRootDirectories = null;
        #region Constructors

        public ChangeRootCommand(IScriptCommand nextCommand = null)
            : base("ChangeRoot", "Explorer", "ChangeType", "AppliedRootDirectories")
        {
            _nextCommand = nextCommand;
        }

        public ChangeRootCommand(ChangeType changeType, IEntryModel[] appliedRootDirectories, IScriptCommand nextCommand = null)
            : this(nextCommand)
        {
            _changeType = changeType;
            _appliedRootDirectories = appliedRootDirectories;
        }



        #endregion

        #region Methods

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IExplorerViewModel evm = pm["Explorer"] as IExplorerViewModel;
            if (evm != null && _appliedRootDirectories != null && _appliedRootDirectories.Length > 0)
            {
                try
                {
                    _changeType = _changeType.HasValue ? _changeType : (ChangeType)pm["ChangeType"];
                    _appliedRootDirectories = _appliedRootDirectories ?? (IEntryModel[])pm["AppliedRootDirectories"];

                    List<IEntryModel> currentList = evm.RootModels.ToList();
                    switch (_changeType)
                    {
                        case ChangeType.Created:
                            currentList.AddRange(_appliedRootDirectories);
                            break;
                        case ChangeType.Deleted:
                            foreach (var root in _appliedRootDirectories)
                                currentList.Remove(root);
                            break;
                        case ChangeType.Changed:
                            currentList = new List<IEntryModel>(_appliedRootDirectories);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    evm.RootModels = currentList.ToArray();

                    return _nextCommand ?? ResultCommand.NoError;
                }
                catch (Exception ex)
                {
                    return ResultCommand.Error(ex);
                }
            }
            return ResultCommand.Error(new ArgumentException("Explorer"));
        }


        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion
    }

    internal class GotoDirectory : ScriptCommandBase
    {
        private IEntryModel _dir = null;
        private string _dirPath = null;

        internal GotoDirectory(IScriptCommand thenCommand)
            : base("GoToDirectory", thenCommand, "Directory")
        {
        }

        public GotoDirectory(IEntryModel dir, IScriptCommand thenCommand)
            : this(thenCommand)
        {
            _dir = dir;
        }

        public GotoDirectory(string dirPath, IScriptCommand thenCommand)
            : this(thenCommand)
        {
            _dirPath = dirPath;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IExplorerViewModel evm = pm["Explorer"] as IExplorerViewModel;
            IEventAggregator events = pm["Events"] as IEventAggregator;
            if (evm != null)
            {
                if (_dirPath != null)
                    await evm.GoAsync(_dirPath);
                else
                {
                    _dir = _dir ?? (pm.ContainsKey("Directory") ? (IEntryModel)pm["Directory"] : null);
                    if (_dir != null)
                        await evm.GoAsync(_dir);
                }
                return _nextCommand ?? ResultCommand.NoError;
            }
            return ResultCommand.Error(new ArgumentException("Explorer"));
        }

        //public GotoDirectory(string path, IScriptCommand thenCommand)
        //    : base(evm =>
        //        Task.Run(async () => { await evm.GoAsync(path); return thenCommand ?? ResultCommand.NoError; }))
        //{

        //}
    }

    public class ShowNewExplorer : ScriptCommandBase
    {
        private IExplorerInitializer _initializer;
        private object _context;
        private IDictionary<string, object> _settings;

        internal ShowNewExplorer(IExplorerInitializer initializer,
            object context = null, IDictionary<string, object> settings = null)
            : base("NewWindow")
        {
            _context = context;
            _settings = settings;
            _initializer = initializer;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var evm = new ExplorerViewModel(_initializer);
            pm["Explorer"] = evm;
            _initializer.WindowManager.ShowWindow(evm, _context, _settings);
            if (_settings != null && _settings.ContainsKey("StartupDirectory") &&
                _settings["StartupDirectory"] is IEntryModel)
                return UIScriptCommands.ExplorerGoToValue(_settings["StartupDirectory"] as IEntryModel);
            return ResultCommand.NoError;
        }
    }

    public class ShowDirectoryPicker : ScriptCommandBase
    {
        private IExplorerInitializer _initializer;
        private IProfile[] _rootProfiles;
        private Func<IEntryModel, IScriptCommand> _nextCommandFunc;
        private IScriptCommand _cancelCommand;

        internal ShowDirectoryPicker(IExplorerInitializer initializer,
            IProfile[] rootProfiles, Func<IEntryModel, IScriptCommand> nextCommandFunc, IScriptCommand cancelCommand)
            : base("PickDirectory")
        {
            _initializer = initializer;
            _rootProfiles = rootProfiles ?? initializer.RootModels.Select(em => em.Profile).Distinct().ToArray();
            _nextCommandFunc = nextCommandFunc;
            _cancelCommand = cancelCommand;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            _nextCommandFunc = _nextCommandFunc ?? (em => ResultCommand.NoError);
            if (_rootProfiles != null && _rootProfiles.Length > 0)
            {
                if (_rootProfiles.Length == 1)
                {
                    var dpvm = new DirectoryPickerViewModel(_initializer.Events, _initializer.WindowManager,
                        _rootProfiles.First().ParseAsync("").Result);
                    if (_initializer.WindowManager.ShowDialog(dpvm).Value)
                        return _nextCommandFunc(dpvm.SelectedDirectory);
                }
                else
                {
                    var advm = new AddDirectoryViewModel(_initializer, _rootProfiles);
                    if (_initializer.WindowManager.ShowDialog(advm).Value)
                        return _nextCommandFunc(advm.SelectedDirectory);
                }
            }
            return _cancelCommand;
        }
    }

    #endregion

    #region DirectoryTreeBased

    public static class DirectoryTree
    {
        public static IScriptCommand ToggleRename =
          new ToggleRenameCommand(WPFExtensionMethods.GetCurrentDirectoryVMFunc);

        public static IScriptCommand ExpandSelected =
            new ExpandSelectedDirectory(WPFExtensionMethods.GetCurrentDirectoryVMFunc);

        public static IScriptCommand AssignSelectionToParameter(IScriptCommand thenCommand)
        {
            return new AssignSelectionToVariable(
                WPFExtensionMethods.GetCurrentDirectoryFunc, "Parameter", thenCommand);
        }

        public static IScriptCommand Do(Func<IDirectoryTreeViewModel, IScriptCommand> commandFunc)
        {
            return new DoDirectoryTree(commandFunc);
        }

    }

    internal class DoDirectoryTree : DoCommandBase<IDirectoryTreeViewModel>
    {
        internal DoDirectoryTree(Func<IDirectoryTreeViewModel, IScriptCommand> commandFunc)
            : base("DirectoryTree", commandFunc)
        {
        }
    }

    internal class ExpandSelectedDirectory : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryViewModel[]> _getSelectionFunc;

        /// <summary>
        /// Broadcast change directory to current selected directory, required FileList (IDirectoryTreeViewModel)
        /// </summary>
        public ExpandSelectedDirectory(Func<ParameterDic, IEntryViewModel[]> getSelectionVMFunc)
            : base("ExpandSelectedDirectory")
        {
            _getSelectionFunc = getSelectionVMFunc;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            var selectedItems = _getSelectionFunc(pm);
            return selectedItems.Length == 1 && selectedItems[0].EntryModel.IsDirectory;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var selectedItem = _getSelectionFunc(pm).FirstOrDefault();

            //IDirectoryTreeViewModel dlvm = pm["DirectoryTree"] as IDirectoryTreeViewModel;
            //IEventAggregator events = pm["Events"] as IEventAggregator;
            var entryHelper = (selectedItem as IDirectoryNodeViewModel).Entries;
            entryHelper.IsExpanded = !entryHelper.IsExpanded;

            return ResultCommand.OK;
        }
    }

    #endregion

    #region FileList based.

    public static partial class FileList
    {
        public static IScriptCommand Do(Func<IFileListViewModel, IScriptCommand> commandFunc)
        {
            return new DoFileList(commandFunc);
        }


        public static IScriptCommand If(Func<IFileListViewModel, bool> condition, IScriptCommand ifTrueCommand,
            IScriptCommand otherwiseCommand)
        {
            return new IfFileList(condition, ifTrueCommand, otherwiseCommand);
        }

        [Obsolete("UIScriptCommands.FileListIfSelectionLength")]
        public static IScriptCommand IfSelection(Func<IEntryViewModel[], bool> condition, IScriptCommand ifTrueCommand,
            IScriptCommand otherwiseCommand)
        {
            return new IfFileList(flvm => condition(flvm.Selection.SelectedItems.ToArray()), ifTrueCommand, otherwiseCommand);
        }

        public static IScriptCommand IfSelectionModel(Func<IEntryModel[], bool> condition, IScriptCommand ifTrueCommand,
            IScriptCommand otherwiseCommand)
        {
            return new IfFileList(flvm => condition(flvm.Selection.SelectedItems.Select(vm => vm.EntryModel).ToArray()),
                ifTrueCommand, otherwiseCommand);
        }

        public static IScriptCommand Zoom(ZoomMode mode, int multiplier = 1, IScriptCommand nextCommand = null)
        {
            nextCommand = nextCommand ?? ResultCommand.NoError;
            int offset = (mode == ZoomMode.ZoomIn ? 1 : -1) * multiplier;
            return Do(flvm =>
            {
                var viewModeModel = flvm.Commands.ToolbarCommands.CommandModels.AllNonBindable
                    .FirstOrDefault(cvm => cvm.CommandModel is ViewModeCommand).CommandModel as ViewModeCommand;

                if (viewModeModel != null)
                {
                    viewModeModel.SliderValue += offset;
                }
                return nextCommand;
            });
        }


        public static IScriptCommand AssignSelectionToParameter(IScriptCommand thenCommand, string variableName = "Parameter")
        {
            return new AssignSelectionToVariable(WPFExtensionMethods.GetFileListSelectionFunc,
                variableName, thenCommand);
        }

        public static IScriptCommand AssignCurrentDirectoryToDestination(IScriptCommand thenCommand, string variableName = "Destination")
        {
            return new AssignSelectionToVariable(WPFExtensionMethods.GetFileListCurrentDirectoryFunc,
                variableName, thenCommand);
        }

        [Obsolete("UIScriptCommands.BroadcastDirectoryChanged or ExplorerGoTo")]
        public static IScriptCommand OpenSelectedDirectory =
            new OpenSelectedDirectory(WPFExtensionMethods.GetFileListSelectionFunc);

        public static IScriptCommand ToggleRename =
            new ToggleRenameCommand(WPFExtensionMethods.GetFileListSelectionVMFunc);

        public static IScriptCommand Lookup(Func<IEntryModel, bool> lookupFunc,
            Func<IEntryModel, IScriptCommand> foundCommandFunc, IScriptCommand notFoundCommand)
        {
            return new LookupEntryCommand(lookupFunc, foundCommandFunc, notFoundCommand, WPFExtensionMethods.GetFileListItemsFunc);
        }

        /// <summary>
        /// Select or Deselect items in FileList based on querySelectionFunc.
        /// </summary>
        /// <param name="querySelectionProv"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand Select(Func<IEntryModel, bool> querySelectionFunc, IScriptCommand nextCommand)
        {
            return new SelectFileList(querySelectionFunc, nextCommand);
        }

        /// <summary>
        /// Wait until filelist finished loading.
        /// </summary>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand WaitLoad(IScriptCommand nextCommand)
        {
            return new WaitFileList(nextCommand);
        }

        /// <summary>
        /// Refresh the filelist.
        /// </summary>
        /// <param name="nextCommand"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static IScriptCommand Refresh(IScriptCommand nextCommand, bool force = false)
        {
            return new RefreshFileList(nextCommand, force);
        }


    }

    /// <summary>
    /// Do certain action using the filelist.
    /// required FileList (IFileListViewModel)
    /// </summary>
    internal class DoFileList : DoCommandBase<IFileListViewModel>
    {
        internal DoFileList(Func<IFileListViewModel, IScriptCommand> commandFunc)
            : base("FileList", commandFunc)
        {
            ContinueOnCaptureContext = true;
        }
    }



    /// <summary>
    /// If Condition for the file list is true, then do the first command, otherwise do the second command.
    /// required FileList (IFileListViewModel)
    /// </summary>
    internal class IfFileList : IfScriptCommand
    {
        /// <summary>
        /// If Condition for the file list is true, then do the first command, otherwise do the second command,
        /// required FileList (IFileListViewModel)
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ifTrueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        internal IfFileList(Func<IFileListViewModel, bool> condition, IScriptCommand ifTrueCommand,
            IScriptCommand otherwiseCommand)
            : base(pd =>
                {
                    if (!pd.ContainsKey("FileList"))
                        return false;
                    IFileListViewModel flvm = pd["FileList"] as IFileListViewModel;
                    return condition(flvm);
                }, ifTrueCommand, otherwiseCommand)
        {

        }



    }

    /// <summary>
    /// Set Selected item to parameter, so it can be used in Toolbar based command.
    /// required FileList (IFileListViewModel)
    /// </summary>
    internal class AssignSelectionToVariable : RunInSequenceScriptCommand
    {
        /// <summary>
        /// FileList.Selection.SelectedItems as IEntryModel[] -> Parameter, required FileList (IFileListViewModel)
        /// </summary>
        /// <param name="thenCommand"></param>
        public AssignSelectionToVariable(Func<ParameterDic, IEntryModel[]> getSelectionFunc,
            string variableName, IScriptCommand thenCommand)
            : base(
            new SimpleScriptCommand("AssignSelectionToVariableAsEntryModelArray",
                pd =>
                {
                    pd[variableName] = getSelectionFunc(pd);
                    return ResultCommand.NoError;
                }),
            thenCommand)
        {

        }

        public AssignSelectionToVariable(Func<ParameterDic, IEntryModel> getSelectionFunc,
           string variableName, IScriptCommand thenCommand)
            : base(
            new SimpleScriptCommand("AssignSelectionToVariableAsEntryModel",
                pd =>
                {
                    pd[variableName] = getSelectionFunc(pd);
                    return ResultCommand.NoError;
                }),
            thenCommand)
        {

        }
    }


    /// <summary>
    /// Refresh the file list.
    /// </summary>
    internal class RefreshFileList : ScriptCommandBase
    {
        private bool _force;
        /// <summary>
        ///  Refresh the filelist.
        /// required FileList (IFileListViewModel)
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ifTrueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        internal RefreshFileList(IScriptCommand nextCommand, bool force)
            : base("Refresh", "FileList")
        {
            ContinueOnCaptureContext = true;
            _nextCommand = nextCommand;
            _force = force;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            if (!pm.ContainsKey("FileList"))
                return ResultCommand.Error(new ArgumentException("Paremeter FileList is not found"));
            IFileListViewModel flvm = pm["FileList"] as IFileListViewModel;
            await flvm.ProcessedEntries.EntriesHelper.LoadAsync(UpdateMode.Update, _force);
            return _nextCommand;
        }


    }

    /// <summary>
    /// Run next command when filelist finished loading
    /// </summary>
    internal class WaitFileList : ScriptCommandBase
    {
        /// <summary>
        ///  Run next command when filelist finished loading,
        /// required FileList (IFileListViewModel)
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ifTrueCommand"></param>
        /// <param name="otherwiseCommand"></param>
        internal WaitFileList(IScriptCommand nextCommand)
            : base("Waiting", "FileList")
        {
            _nextCommand = nextCommand;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            if (!pm.ContainsKey("FileList"))
                return ResultCommand.Error(new ArgumentException("Paremeter FileList is not found"));
            IFileListViewModel flvm = pm["FileList"] as IFileListViewModel;
            using (var releaser = await flvm.ProcessedEntries.EntriesHelper.LoadingLock.LockAsync())
                return _nextCommand;
        }


    }

    /// <summary>
    /// Select/Deselect item based on queryselection.
    /// </summary>
    internal class SelectFileList : ScriptCommandBase
    {
        private Func<IEntryModel, bool> _querySelectionFunc;
        internal SelectFileList(Func<IEntryModel, bool> querySelectionFunc, IScriptCommand nextCommand)
            : base("Select", "FileList")
        {
            _querySelectionFunc = querySelectionFunc;
            _nextCommand = nextCommand;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            if (!pm.ContainsKey("FileList"))
                return ResultCommand.Error(new ArgumentException("Paremeter FileList is not found"));
            IFileListViewModel flvm = pm["FileList"] as IFileListViewModel;
            flvm.Selection.Select(evm => evm != null && _querySelectionFunc(evm.EntryModel));

            return _nextCommand;
        }
    }

    /// <summary>
    /// Broadcast change directory to current selected directory
    /// required FileList (IFileListViewModel)
    /// </summary>
    internal class OpenSelectedDirectory : ScriptCommandBase
    {
        public static OpenSelectedDirectory FromFileList = new OpenSelectedDirectory(WPFExtensionMethods.GetFileListSelectionFunc);
        private Func<ParameterDic, IEntryModel[]> _getSelectionFunc;

        /// <summary>
        /// Broadcast change directory to current selected directory, required FileList (IFileListViewModel)
        /// </summary>
        public OpenSelectedDirectory(Func<ParameterDic, IEntryModel[]> getSelectionFunc)
            : base("OpenSelectedDirectory")
        {
            _getSelectionFunc = getSelectionFunc;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            var selectedItems = _getSelectionFunc(pm);
            return selectedItems.Length == 1 && selectedItems[0].IsDirectory;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var selectedItem = _getSelectionFunc(pm).FirstOrDefault();

            IFileListViewModel flvm = pm["FileList"] as IFileListViewModel;
            IEventAggregator events = pm["Events"] as IEventAggregator;

            events.PublishOnUIThread(new DirectoryChangedEvent(flvm,
                   selectedItem, flvm.CurrentDirectory));

            return ResultCommand.OK;
        }
    }

    internal class OpenInNewWindowCommand : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryModel[]> _getSelectionFunc;
        private IExplorerInitializer _initializer;

        /// <summary>
        /// Broadcast change directory to current selected directory, required FileList (IFileListViewModel)
        /// </summary>
        public OpenInNewWindowCommand(IExplorerInitializer initializer, Func<ParameterDic, IEntryModel[]> getSelectionFunc)
            : base("OpenInNewWindowCommand")
        {
            _initializer = initializer;
            _getSelectionFunc = getSelectionFunc;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            var selectedItems = _getSelectionFunc(pm);
            return selectedItems.Length == 1 && selectedItems[0].IsDirectory;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var selectedItem = _getSelectionFunc(pm).FirstOrDefault();

            if (selectedItem != null)
                return Explorer.NewWindow(_initializer, selectedItem);

            return ResultCommand.OK;
        }
    }

    internal class ToggleRenameCommand : ScriptCommandBase
    {
        public static ToggleRenameCommand ForSelectedItem = new ToggleRenameCommand(WPFExtensionMethods.GetFileListSelectionVMFunc);
        private Func<ParameterDic, IEntryViewModel[]> _getSelectionFunc;

        /// <summary>
        /// Broadcast change directory to current selected directory, required FileList (IFileListViewModel)
        /// </summary>
        public ToggleRenameCommand(Func<ParameterDic, IEntryViewModel[]> getSelectionFunc)
            : base("ToggleRenameCommand")
        {
            _getSelectionFunc = getSelectionFunc;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            var selectedItems = _getSelectionFunc(pm);
            return selectedItems.Length == 1 && selectedItems[0] != null && selectedItems[0].IsRenamable;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var selectedItem = _getSelectionFunc(pm).FirstOrDefault();

            if (selectedItem != null)
                selectedItem.IsRenaming = !selectedItem.IsRenaming;

            return ResultCommand.OK;
        }
    }


    internal class RenameFileBasedEntryCommand : ScriptCommandBase
    {
        //public static RenameFileBasedEntryCommand FromParameter = new RenameFileBasedEntryCommand(
        //    FileBasedScriptCommandsHelper.GetFirstEntryModelFromParameter);
        private Func<ParameterDic, IEntryModel> _srcModelFunc;
        private string _newName;

        public RenameFileBasedEntryCommand(Func<ParameterDic, IEntryModel> srcModelFunc, string newName = null)
            : base("Rename")
        {
            _srcModelFunc = srcModelFunc;
            _newName = newName;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            string newName = _newName ?? pm["NewName"] as string;
            if (String.IsNullOrEmpty(newName))
                return ResultCommand.Error(new ArgumentException("NewName"));
            var srcModel = _srcModelFunc(pm);
            if (srcModel == null)
                return ResultCommand.Error(new ArgumentException());

            IDiskProfile profile = srcModel.Profile as IDiskProfile;
            if (profile == null)
                return ResultCommand.Error(new ArgumentException());

            IEntryModel destModel = await profile.DiskIO.RenameAsync(srcModel, newName, pm.CancellationToken);
            return new NotifyChangedCommand(destModel.Profile, destModel.FullPath,
                srcModel.Profile, srcModel.FullPath, ChangeType.Moved);
        }
    }


    internal class LookupEntryCommand : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryModel[]> _getItemsFunc;
        private Func<IEntryModel, bool> _lookupFunc;
        private Func<IEntryModel, IScriptCommand> _foundCommandFunc;
        private IScriptCommand _notFoundCommand;

        public LookupEntryCommand(
            Func<IEntryModel, bool> lookupFunc,
            Func<IEntryModel, IScriptCommand> foundCommandFunc, IScriptCommand notFoundCommand,
            Func<ParameterDic, IEntryModel[]> getItemsFunc = null
            )
            : base("LookupEntry")
        {
            _getItemsFunc = getItemsFunc ?? WPFExtensionMethods.GetFileListItemsFunc;
            _lookupFunc = lookupFunc;
            _foundCommandFunc = foundCommandFunc;
            _notFoundCommand = notFoundCommand;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            var foundItem = _getItemsFunc(pm).FirstOrDefault(_lookupFunc);
            if (foundItem != null)
                return _foundCommandFunc(foundItem);
            else return _notFoundCommand;
        }
    }

    #endregion

    #region Sidebar based.

    public static class Sidebar
    {
        public static IScriptCommand Show(IScriptCommand nextCommand = null)
        { return new ToggleVisibility("Sidebar", true, nextCommand); }
        public static IScriptCommand Hide(IScriptCommand nextCommand = null)
        { return new ToggleVisibility("Sidebar", false, nextCommand); }
        public static IScriptCommand Toggle(IScriptCommand nextCommand = null)
        { return new ToggleVisibility("Sidebar", nextCommand); }
    }

    /// <summary>
    /// Do certain action using the previewer.
    /// required Sidebar (ISidebarViewModel)
    /// </summary>
    internal class DoSidebar : DoCommandBase<ISidebarViewModel>
    {
        internal DoSidebar(Func<ISidebarViewModel, IScriptCommand> commandFunc)
            : base("Sidebar", commandFunc)
        {
        }
    }

    internal class ToggleVisibility : DoCommandBase<IToggleableVisibility>
    {
        internal ToggleVisibility(string viewModelName, bool toValue, IScriptCommand nextCommand)
            : base(viewModelName, pvm => { pvm.IsVisible = toValue; return nextCommand; })
        {
        }

        internal ToggleVisibility(string viewModelName, IScriptCommand nextCommand)
            : base(viewModelName, pvm => { pvm.IsVisible = !pvm.IsVisible; return nextCommand; })
        {
        }
    }

    #endregion

    #region Clipboard

    public static class ClipboardCommands
    {
        public static CopyToClipboardCommand Copy =
         new CopyToClipboardCommand(WPFScriptCommands.GetEntryModelFromParameter, false);
        public static CopyToClipboardCommand Cut =
            new CopyToClipboardCommand(WPFScriptCommands.GetEntryModelFromParameter, true);

        public static PasteFromClipboardCommand Paste(Func<ParameterDic, IEntryModel> currentDirectoryModelFunc,
            Func<DragDropEffectsEx, IEntryModel[], IEntryModel, IScriptCommand> transferCommandFunc)
        {
            return new PasteFromClipboardCommand(currentDirectoryModelFunc, transferCommandFunc);
        }
    }

    public class CopyToClipboardCommand : ScriptCommandBase
    {

        private static byte[] preferCopy = new byte[] { 5, 0, 0, 0 };
        private static byte[] preferCut = new byte[] { 2, 0, 0, 0 };

        private Func<ParameterDic, IEntryModel[]> _srcModelFunc;
        private bool _removeOrginal;

        public CopyToClipboardCommand(Func<ParameterDic, IEntryModel[]> srcModelFunc, bool removeOrginal)
            : base(removeOrginal ? "Cut" : "Copy")
        {
            _removeOrginal = removeOrginal;
            _srcModelFunc = srcModelFunc;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var _srcModels = _srcModelFunc(pm);
            var da = _srcModels.First().Profile.DragDrop.GetDataObject(_srcModels);

            byte[] moveEffect = _removeOrginal ? preferCut : preferCopy;
            MemoryStream dropEffect = new MemoryStream();
            dropEffect.Write(moveEffect, 0, moveEffect.Length);
            da.SetData("Preferred DropEffect", dropEffect);

            System.Windows.Clipboard.Clear(); 
            System.Windows.Clipboard.SetDataObject(da, true);

            return ResultCommand.NoError;
        }
    }

    public class PasteFromClipboardCommand : ScriptCommandBase
    {

        private Func<ParameterDic, IEntryModel> _currentDirectoryModelFunc;
        private Func<DragDropEffectsEx, IEntryModel[], IEntryModel, IScriptCommand> _transferCommandFunc;

        public PasteFromClipboardCommand(Func<ParameterDic, IEntryModel> currentDirectoryModelFunc,
            Func<DragDropEffectsEx, IEntryModel[], IEntryModel, IScriptCommand> transferCommandFunc)
            : base("Paste")
        {
            _currentDirectoryModelFunc = currentDirectoryModelFunc;
            _transferCommandFunc = transferCommandFunc;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            var currentDirectory = _currentDirectoryModelFunc(pm);
            if (currentDirectory != null)
            {
                System.Windows.IDataObject da = System.Windows.Clipboard.GetDataObject();
                if (da != null)
                {
                    var srcModels = currentDirectory.Profile.DragDrop.GetEntryModels(da);
                    if (srcModels != null && srcModels.Count() > 0)
                    {
                        return _transferCommandFunc(DragDropEffectsEx.Copy, srcModels.ToArray(), currentDirectory);
                    }
                }
            }

            return ResultCommand.NoError;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            var currentDirectory = _currentDirectoryModelFunc(pm);
            if (currentDirectory != null)
            {
                System.Windows.IDataObject da = System.Windows.Clipboard.GetDataObject();
                var srcModels = currentDirectory.Profile.DragDrop.GetEntryModels(da);
                return srcModels != null && srcModels.Count() > 0;
            }
            return false;
        }
    }

    #endregion


    #region Transfer Based

    /// <summary>
    /// Transfer Source entries (IEntryModel[]) to Dest directory (IEntryModel)
    /// </summary>
    public class TransferCommand : ScriptCommandBase
    {
        #region Constructor

        public TransferCommand(Func<DragDropEffectsEx, IEntryModel, IEntryModel, IScriptCommand> transferOneFunc)
            : base("Transfer", "Source", "Dest", "DragDropEffects")
        {            
            _transferOneFunc = transferOneFunc;
        }

        #endregion

        #region Methods

        public override bool CanExecute(ParameterDic pm)
        {
            var source = pm["Source"] as IEntryModel[];
            var dest = pm["Dest"] as IEntryModel;
            DragDropEffectsEx effects = pm.ContainsKey("DragDropEffects") && pm["DragDropEffects"] is DragDropEffectsEx ?
                (DragDropEffectsEx)pm["DragDropEffects"] : DragDropEffectsEx.Copy;

            if (source == null || source.Length == 0 || dest == null)
                return false;

            var transferCommands = source.Select(s => _transferOneFunc(effects, s, dest)).ToArray();
            var cannotTransfer = transferCommands.FirstOrDefault(c => c == null || !c.CanExecute(pm));
            return cannotTransfer == null;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {

            var source = pm["Source"] as IEntryModel[];
            var dest = pm["Dest"] as IEntryModel;
            DragDropEffectsEx effects = pm.ContainsKey("DragDropEffects") && pm["DragDropEffects"] is DragDropEffectsEx ?
              (DragDropEffectsEx)pm["DragDropEffects"] : DragDropEffectsEx.Copy;

            var transferCommands = source.Select(s => _transferOneFunc(effects, s, dest)).ToArray();
            if (transferCommands.Any(c => c == null || !c.CanExecute(pm)))
                return ResultCommand.Error(new ArgumentException("Not all items are transferrable."));

            return WPFScriptCommands.ReportProgress(
                TransferProgress.IncrementTotalEntries(source.Length),
                ScriptCommands.RunSequence(null, transferCommands)
                );          
        }

        #endregion

        #region Data

        private Func<DragDropEffectsEx, IEntryModel, IEntryModel, IScriptCommand> _transferOneFunc;

        #endregion

        #region Public Properties

        #endregion
    }

    #endregion
}
