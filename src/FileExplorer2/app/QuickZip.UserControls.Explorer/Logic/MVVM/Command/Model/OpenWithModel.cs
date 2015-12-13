using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.Translation;
using Cinch;
using System.IO;
using System.IO.Tools;
using System.Drawing;
using System.Diagnostics;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM.Command.Model
{

    public class QueryOpenWithInfoEventArgs<FI, DI, FSI> : EventArgs
        where FI : FSI
        where DI : FSI
    {
        public EntryModel<FI, DI, FSI> FileInfo { get; private set; }
        public List<OpenWithInfo> ReturnList { get; private set; }

        public QueryOpenWithInfoEventArgs(EntryModel<FI, DI, FSI> entryModel)
        {
            FileInfo = entryModel;
            ReturnList = new List<OpenWithInfo>();
        }
    }


    public class OpenWithCommandModel<FI, DI, FSI> : DirectoryCommandModel
        where FI : FSI
        where DI : FSI
    {


        #region Constructor

        public OpenWithCommandModel(Profile<FI, DI, FSI> profile, FileModel<FI, DI, FSI> appliedEntryModel)
        {
            Header = Texts.strOpen;
            HeaderIcon = profile.IconExtractor.GetIcon(appliedEntryModel.EmbeddedEntry, Converters.IconSize.small, false, true);
            IsExecutable = true;

            _profile = profile;
            _appliedEntryModel = appliedEntryModel;

        }

        #endregion

        #region Methods

        public override IEnumerable<CommandModel> GetSubActions()
        {
            string ext = PathEx.GetExtension(_appliedEntryModel.Name);

            foreach (OpenWithInfo info in FileTypeInfoProvider.GetFileTypeInfo(ext).OpenWithList)
                if (info.OpenCommand != null && File.Exists(OpenWithInfo.GetExecutablePath(info.OpenCommand)))
                    yield return new OpenWithItemCommandModel<FI, DI, FSI>(_profile, _appliedEntryModel, info);

            QueryOpenWithInfoEventArgs<FI, DI, FSI> openInfoArgs = new QueryOpenWithInfoEventArgs<FI, DI, FSI>(_appliedEntryModel);
            OnQueryOpenWithInfo(this, openInfoArgs);
            foreach (OpenWithInfo info in openInfoArgs.ReturnList)
                yield return new OpenWithItemCommandModel<FI, DI, FSI>(_profile, _appliedEntryModel, info);

            yield return new OpenWithItemCommandModel<FI, DI, FSI>(_profile, _appliedEntryModel, OpenWithInfo.OpenAs);
        }

        public override void Execute(object param)
        {
            _profile.Open(_appliedEntryModel);
        }


        #endregion

        #region Data

        private Profile<FI, DI, FSI> _profile;
        private EntryModel<FI, DI, FSI> _appliedEntryModel;

        #endregion

        #region Public Properties

        public static EventHandler<QueryOpenWithInfoEventArgs<FI, DI, FSI>> OnQueryOpenWithInfo = (o, e) =>
        {
#if DEBUG
            e.ReturnList.Add(new OpenWithInfo() { Description = "Test", OpenCommand = "bbb", KeyName = "DEBUG" });
#endif
        };


        #endregion
    }


    public class OpenWithItemCommandModel<FI, DI, FSI> : CommandModel
        where FI : FSI
        where DI : FSI
    {
        #region Constructor
        //public OpenWithItemCommandModel(Profile<FI, DI, FSI> profile,
        //   FileModel<FI, DI, FSI> appliedEntryModel)
        //{
        //    _profile = profile;
        //    _appliedEntryModel = appliedEntryModel;
        //    IsExecutable = true;

        //    Header = Texts.strChooseDefaultProgram;
        //}


        public OpenWithItemCommandModel(Profile<FI, DI, FSI> profile,
            EntryModel<FI, DI, FSI> appliedEntryModel, OpenWithInfo info)
        {
            _profile = profile;
            _appliedEntryModel = appliedEntryModel;
            _info = info;
            IsExecutable = true;

            if (info != null && !info.Equals(OpenWithInfo.OpenAs))
            {
                string exePath = OpenWithInfo.GetExecutablePath(info.OpenCommand);
                if (File.Exists(exePath))
                    HeaderIcon = _profile.IconExtractor.GetIcon(exePath, Converters.IconSize.small, false);

                if (File.Exists(exePath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(exePath);
                    if (fvi.ProductName.EndsWith("Operating System")) //WordPad / NotePad
                        Header = fvi.FileDescription;
                    Header = fvi.ProductName;
                }
            }
            else Header = Texts.strChooseDefaultProgram;
        }
        #endregion

        #region Methods

        public override void Execute(object param)
        {
            _profile.Open(_appliedEntryModel, _info);
        }

        #endregion

        #region Data

        private Profile<FI, DI, FSI> _profile;
        private OpenWithInfo _info = null;
        private EntryModel<FI, DI, FSI> _appliedEntryModel;
        private BackgroundTaskManager<OpenWithInfo, Bitmap> bgWorker_loadInfo = null;
        private BackgroundTaskManager<OpenWithInfo, string> bgWorker_loadHeaderInfo = null;

        #endregion

        #region Public Properties

        #endregion
    }


}
