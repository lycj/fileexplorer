using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public class MetadataViewModel : PropertyChangedBase, IMetadataViewModel
    {
        #region Cosntructor

        public static MetadataViewModel FromMetadata(IMetadata metadata)
        {
            return new MetadataViewModel() { MetadataModel = metadata };
        }

        public static MetadataViewModel FromText(string header, string category, string value, bool isHeader = false)
        {
            return FromMetadata(
                new Metadata(FileExplorer.Defines.DisplayType.Text, category, header, value) { IsHeader = isHeader });
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj is IMetadataViewModel)
            {
                var objViewModel = obj as IMetadataViewModel;
                if (objViewModel.MetadataModel.Category != MetadataModel.Category)
                    return false;
                if (!String.IsNullOrEmpty(objViewModel.MetadataModel.HeaderText))
                    return objViewModel.MetadataModel.HeaderText == MetadataModel.HeaderText;

                return objViewModel.MetadataModel.Content == null ||
                    objViewModel.MetadataModel.Content.Equals(MetadataModel.Content);
            }
            return false;
        }

        #endregion

        #region Data


        #endregion

        #region Public Properties

        public IMetadata MetadataModel { get; private set; }

        #endregion
    }
}
