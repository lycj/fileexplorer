using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.Translation;
using Cinch;
using QuickZip.UserControls.MVVM.Command.Model;

namespace QuickZip.UserControls.MVVM
{
    public class DirectoryViewerViewModel<FI, DI, FSI> : StatusDirectoryViewerViewModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {
        #region Constructor
        private void init(Profile<FI, DI, FSI> profile, int viewSize)
        {
            _viewModeM = new ViewModeCommandModel(viewSize);
            _viewModeM.PropertyChanged += (o, e) => 
                {
                    if (e.PropertyName == "Value")
                        ViewSize = _viewModeM.Value;
                };
            _organizeM = new OrganizeCommandModel<FI, DI, FSI>(profile, this);
            _togglePrevieM = new TogglePreviewModel(this);
        }

        public DirectoryViewerViewModel(Profile<FI, DI, FSI> profile,
           DirectoryModel<FI, DI, FSI> embedDirectoryModel, int viewSize = (int)ViewMode.vmGrid)
            : base(profile, embedDirectoryModel)
        {
            init(profile, viewSize);
        }

        public DirectoryViewerViewModel(Profile<FI, DI, FSI> profile, int viewSize = (int)ViewMode.vmGrid)
            : base(profile)
        {
            init(profile, viewSize);
        }
        #endregion

        #region Methods

        private void setupCommands()
        {

        }


        protected override IEnumerable<CommandModel> getActionModel()
        {
            yield return _togglePrevieM;
            yield return _viewModeM;
            yield return _organizeM;

            EntryModel<FI, DI, FSI>[] selectedModels = (new List<EntryModel<FI, DI, FSI>>(
                            from vm in SelectedViewModels select vm.EmbeddedModel)).ToArray();

            if (selectedModels.Length > 0)
                foreach (var model in _profile.GetCommands(selectedModels))
                    yield return model;
            else
                foreach (var model in _profile.GetCommands(EmbeddedDirectoryViewModel.EmbeddedDirectoryModel))
                    yield return model;
             
            if (SelectedModels.Count == 1 && SelectedModels[0] is FileModel<FI, DI, FSI>)
                yield return new OpenWithCommandModel<FI, DI, FSI>(_profile, SelectedModels[0] as FileModel<FI, DI, FSI>);

            //yield return new SliderCommandModel(10, 50, 10, new SelectorItemInfo<int>[] 
            //{  

            //    new SelectorItemInfo<int>() { Header = "5", Value =50 },  
            //    new SelectorItemInfo<int>() { Header = "3", Value =30 },                
            //    new SelectorItemInfo<int>() { Header = "2", Value =20 },
            //    new SelectorItemInfo<int>() { Header = "1", Value =10 }

            //}) { Header = "Test2" };
        }


        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            UpdateToolbar();

            updatePreviewerSource();
        }

        private void updatePreviewerSource()
        {
            if (IsPreviewerVisible)
            {
                if (SelectedModels.Count == 1)
                    if (SelectedModels[0] is FileModel<FI, DI, FSI>)
                    {
                        PreviewerSource = _profile.GetDiskPath(SelectedModels[0].EmbeddedEntry);
                    }
            }
            else PreviewerSource = null;
        }

        protected override void OnIsPreviewerVisibleChanged()
        {
            base.OnIsPreviewerVisibleChanged();
            updatePreviewerSource();
        }

        #endregion

        #region Data

        private ViewModeCommandModel _viewModeM;

        private OrganizeCommandModel<FI, DI, FSI> _organizeM;
        private TogglePreviewModel _togglePrevieM;

        #endregion

        #region Public Properties


        #endregion

    }
}
