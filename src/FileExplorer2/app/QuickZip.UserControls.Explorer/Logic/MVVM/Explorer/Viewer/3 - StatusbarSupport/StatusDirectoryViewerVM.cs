using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.Translation;
using Cinch;

namespace QuickZip.UserControls.MVVM
{
    public class StatusDirectoryViewerViewModel<FI, DI, FSI> : DndDirectoryViewerViewModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {
        #region Constructor
        public StatusDirectoryViewerViewModel(Profile<FI, DI, FSI> profile,
           DirectoryModel<FI, DI, FSI> embedDirectoryModel)
            : base(profile, embedDirectoryModel)
        {
            IsSimpleStatusbar = false;
            setupCommands();
        }

        public StatusDirectoryViewerViewModel(Profile<FI, DI, FSI> profile)
            : base(profile)
        {
            IsSimpleStatusbar = false;
            setupCommands();
        }
        #endregion

        #region Methods


        private new void setupCommands()
        {
            RenameCommand = new SimpleCommand()
            {
                CanExecuteDelegate = (x) =>
                    {
                        return SelectedViewModels.Count == 1;
                    },
                ExecuteDelegate = (x) =>
                    {
                        if (SelectedViewModels.Count == 1)
                            SelectedViewModels[0].IsEditing = true;
                    }
            };
        }

        private static void countModels(EntryViewModel<FI, DI, FSI>[] entryModels, out int fold, out int file, out long size)
        {
            fold = 0;
            file = 0;
            size = 0;

            foreach (var model in entryModels)
                if (model is FileViewModel<FI, DI, FSI>)
                {
                    file++;
                    size += (model as FileViewModel<FI, DI, FSI>).EmbeddedModel.Length;
                }
                else fold++;

        }

        protected override IEnumerable<GenericMetadataModel> getMetadataModel()
        {
            if (_uIHighlightCount != 0)
            {
                yield return new GenericMetadataModel<string>(String.Format(Texts.strSelectingHeader, _uIHighlightCount),
                    "Key_HighlightCount");
                yield break;
            }

            else
            {
                int fold, file; long size;

                if (SelectedViewModels != null && SelectedViewModels.Count != 0)
                {
                    if (SelectedViewModels.Count >= 1)
                    {
                        //More than one selected

                        EntryModel<FI, DI, FSI>[] selectedModels = (new List<EntryModel<FI, DI, FSI>>(
                            from vm in SelectedViewModels select vm.EmbeddedModel)).ToArray();

                        countModels(SelectedViewModels.ToArray(), out fold, out file, out size);
                        
                        if (SelectedViewModels.Count > 1)
                            yield return new EntryMetadataModel<String, FI, DI, FSI>(selectedModels,
                                String.Format(Texts.strSelectedHeader, fold + file, fold, file), "Key_Selected");

                        foreach (var model in _profile.GetMetadata(selectedModels))
                            yield return model;

                        yield break;
                    }

                }

                EntryModel<FI, DI, FSI>[] childModels = (new List<EntryModel<FI, DI, FSI>>(
                    from vm in EmbeddedDirectoryViewModel.SubEntries select vm.EmbeddedModel)).ToArray();

                //Nothing selected

                countModels(EmbeddedDirectoryViewModel.SubEntries.ToArray(), out fold, out file, out size);
                yield return new EntryMetadataModel<String, FI, DI, FSI>(childModels,
                    String.Format(Texts.strTotalHeader, fold + file, fold, file), "Key_Total");



                yield break;
            }
        }

        #endregion

        #region Data

        private int _uIHighlightCount = 0;

        #endregion

        #region Public Properties

        public int UIHighlightCount
        {
            get { return _uIHighlightCount; }
            set { _uIHighlightCount = value; NotifyPropertyChanged("UIHighlightCount");

            //if (StatusItemList != null && StatusItemList.Count == 1 &&                 
            //    StatusItemList[0].EmbeddedModel.PropertyKey == "Key_HighlightCount")
            //    (StatusItemList[0].EmbeddedModel as GenericMetadataModel<string>).Value =
            //        String.Format(Texts.strSelectingHeader, _uIHighlightCount);
            //else                
                UpdateStatusbar(); }
        }

        #endregion

    }
}
