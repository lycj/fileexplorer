using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;

namespace QuickZip.UserControls.MVVM.Model
{
    public abstract class FileModel<FI, DI, FSI> : EntryModel<FI, DI, FSI> 
        where FI : FSI
        where DI : FSI
    {

        #region Constructor

        public FileModel(FI embeddedFile)
            : base(embeddedFile)
        {

        }

        #endregion

        #region Methods

        public override ViewModel.EntryViewModel<FI, DI, FSI> ToViewModel(Profile<FI, DI, FSI> profile)
        {
            return new ViewModel.FileViewModel<FI, DI, FSI>(profile, this);
        }

        #endregion

        #region Data
        

        #endregion

        #region Public Properties

        public FI EmbeddedFile { get { return (FI)EmbeddedEntry; } }
        


        #endregion

    }

}
