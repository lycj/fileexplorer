using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using QuickZip.Translation;
using QuickZip.UserControls.Input;
using QuickZip.Converters;
//using QuickZip.IO.COFE.UserControls.ViewModel;
using System.Drawing;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    public class OrganizeCommandModel<FI, DI, FSI> : DirectoryCommandModel
        where FI : FSI
        where DI : FSI
    {
        #region Constructor

        private Bitmap _icon7z, _iconZip, _iconFolder;

        public OrganizeCommandModel(Profile<FI, DI, FSI> profile, DirectoryViewerViewModel<FI, DI, FSI> rootModel)
        {
            Header = Texts.strOrganize;
            _rootModel = rootModel;
            _profile = profile;

            _icon7z = _profile.IconExtractor.GetFileBasedFSBitmap(".7z", IconSize.small);
            _iconZip = _profile.IconExtractor.GetFileBasedFSBitmap(".zip", IconSize.small);
            _iconFolder = _profile.IconExtractor.GetFileBasedFSBitmap(".7z", IconSize.small);
        }

        #endregion

        #region Methods
        

        public override IEnumerable<CommandModel> GetSubActions()
        {
            //IconExtractor.Instance.GetIcon("", IconSize.small, true);

            Func<string, CommandModel> constructNewFolderModel =
            (ext) =>
            {
                switch (ext.ToLower())
                {
                    case ".7z" :
                        return new GenericCommandModel("7z", ExplorerCommands.NewFolder, ext)
                         { HeaderIcon = _icon7z };
                    case ".zip" :
                        return new GenericCommandModel("Zip", ExplorerCommands.NewFolder, ext)
                         { HeaderIcon = _iconZip };
                    default :
                        return new GenericCommandModel(Texts.strFolder, ExplorerCommands.NewFolder, ext)
                         { HeaderIcon = _iconFolder };
                }
            };




            yield return new PredefinedDirectoryCommandModel(Texts.strNew,
                new CommandModel[] 
                {
                    new GenericCommandModel(Texts.strWindow, ApplicationCommands.New, _rootModel.EmbeddedDirectoryViewModel.EmbeddedDirectory),
                    new SeparatorCommandModel(),
                    constructNewFolderModel(""),
                    new SeparatorCommandModel(),
                    constructNewFolderModel(".7z"),
                    constructNewFolderModel(".zip")
                });

            yield return new SeparatorCommandModel();

            yield return new GenericCommandModel(ApplicationCommands.Copy);
            yield return new GenericCommandModel(ApplicationCommands.Paste);

            yield return new SeparatorCommandModel();
            yield return new GenericCommandModel(ApplicationCommands.SelectAll);

            yield return new SeparatorCommandModel();
            yield return new GenericCommandModel(ApplicationCommands.Delete);
            yield return new GenericCommandModel(ApplicationCommands.Properties);

            yield return new SeparatorCommandModel();
            yield return new GenericCommandModel(ExplorerCommands.Settings);

            yield return new SeparatorCommandModel();
            yield return new GenericCommandModel(ApplicationCommands.Close);
        }



        #endregion

        #region Data

        private DirectoryViewerViewModel<FI, DI, FSI> _rootModel;
        private Profile<FI, DI, FSI> _profile;

        #endregion

        #region Public Properties

        #endregion
    }






}
