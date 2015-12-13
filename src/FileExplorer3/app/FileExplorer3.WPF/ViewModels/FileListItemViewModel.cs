using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    public class FileListItemViewModel : EntryViewModel, IFileListItemViewModel, ISupportDropHelper
    {
        #region FileListItemDropHelper

        #region MyRegion

        internal class FileListItemDropHelper : LambdaShellDropHelper<IEntryModel>
        {
            
            public FileListItemDropHelper(FileListItemViewModel flvm)
                : base(
                new LambdaValueConverter<IEntryViewModel, IEntryModel>(
                    (evm) => evm.EntryModel, 
                    (em) => EntryViewModel.FromEntryModel(em)),

                new LambdaValueConverter<IEnumerable<IEntryModel>, IDataObject>(
                        ems => flvm.EntryModel.Profile.DragDrop.GetDataObject(ems), 
                        da => flvm.EntryModel.Profile.DragDrop.GetEntryModels(da)), 

                (ems, eff) => flvm.EntryModel.Profile.DragDrop.QueryDrop(ems, flvm.EntryModel, eff),
                (ems, da, eff) => flvm.EntryModel.Profile.DragDrop.OnDropCompleted(ems, da, flvm.EntryModel, eff))

            {   
                
            }
        }

        #endregion

        public FileListItemViewModel(IEntryModel model, IReportSelected<IEntryViewModel> reportSelected)
            : base(model)
        {
            _reportSelected = reportSelected;
            var dragDropHandler = model.Profile.DragDrop;
            DropHelper = dragDropHandler == null ? NoDropHelper.Instance : 
                dragDropHandler.QueryCanDrop(model) ? (ISupportDrop)new FileListItemDropHelper(this) {  DisplayName = model.Label }
                : NoDropHelper.Instance;
            
        }

        #endregion

        #region Methods

        public override void NotifyOfPropertyChange(string propertyName = "")
        {
            base.NotifyOfPropertyChange(propertyName);

            switch (propertyName)
            {
                case "IsSelected" :
                    if (this.IsSelected)
                        _reportSelected.ReportChildSelected(this);
                    else _reportSelected.ReportChildUnSelected(this);
                    break;
            }
        }

        #endregion

        #region Data

        IReportSelected<IEntryViewModel> _reportSelected;


        #endregion

        #region Public Properties

        public ISupportDrop DropHelper { get; private set; }

        #endregion
    }
}
