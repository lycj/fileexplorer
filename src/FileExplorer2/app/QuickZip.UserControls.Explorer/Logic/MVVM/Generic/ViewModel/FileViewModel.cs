using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM.ViewModel
{
    public class FileViewModel<FI, DI, FSI> : EntryViewModel<FI, DI, FSI>        
        where FI : FSI
        where DI : FSI
    {
        #region Constructor

        public FileViewModel(Profile<FI, DI, FSI> profile, FileModel<FI, DI, FSI> embeddedFileModel)
            : base(profile, embeddedFileModel)
        {

        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "F" + base.ToString();
            
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public FileModel<FI, DI, FSI> EmbeddedFileModel { get { return EmbeddedModel as FileModel<FI, DI, FSI>; } }        
        public FI EmbeddedFile { get { return EmbeddedFileModel.EmbeddedFile; } }

        #endregion
    }
}
