using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.MVVM.Model
{

    public class EntryMetadataModel<FI, DI, FSI> : GenericMetadataModel
        where FI : FSI
        where DI : FSI
    {

        #region Constructor

        public EntryMetadataModel(EntryModel<FI, DI, FSI>[] appliedEntryModels, object value, string propertyKey, string propertyLabel = "",
           bool isReadOnly = true, int priority = 0)
            : base(value, propertyKey, propertyLabel, isReadOnly, priority)
        {            
        }

        public EntryMetadataModel(EntryModel<FI, DI, FSI> appliedEntryModel, object value, string propertyKey, string propertyLabel = "",
            bool isReadOnly = true, int priority = 0)
            : this(new EntryModel<FI, DI, FSI>[] { appliedEntryModel }, value, propertyKey, propertyLabel, isReadOnly, priority)
        {

        }

        #endregion

    }

    public class EntryMetadataModel<T, FI, DI, FSI> : GenericMetadataModel<T>
        where FI : FSI
        where DI : FSI
    {
        #region Constructor

        public EntryMetadataModel(EntryModel<FI, DI, FSI>[] appliedEntryModels, T value, string propertyKey, string propertyLabel = "",
            bool isReadOnly = true, int priority = 0)
            : base(value, propertyKey, propertyLabel, isReadOnly, priority)
        {
            
        }

        public EntryMetadataModel(EntryModel<FI, DI, FSI> appliedEntryModel, T value, string propertyKey, string propertyLabel = "", 
            bool isReadOnly = true, int priority = 0)
            : this(new EntryModel<FI, DI, FSI>[] { appliedEntryModel }, value, propertyKey, propertyLabel, isReadOnly, priority)
        {

        }

        #endregion

        #region Methods




        #endregion

        #region Data

       

        #endregion

        #region Public Properties

       
        #endregion
    }
}
