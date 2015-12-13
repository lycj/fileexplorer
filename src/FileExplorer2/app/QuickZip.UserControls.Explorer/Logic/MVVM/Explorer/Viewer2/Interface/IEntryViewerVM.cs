using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.ComponentModel;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Collections;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM
{
    public interface IEntryViewerVM<FI, DI, FSI> : IViewerVM
        where FI : FSI
        where DI : FSI
    {

        #region Methods

        /// <summary>
        /// Invoked when ParentModel (ExplorerModel) broadcast changes.
        /// </summary>
        /// <param name="parseName"></param>
        /// <param name="changeType"></param>
        void BroadcastChange(string parseName, WatcherChangeTypesEx changeType);

        #endregion


        #region Public Properties

        
        /// <summary>
        /// Specify the EntryViewModel it respresenting.
        /// </summary>
        EntryViewModel<FI, DI, FSI> EmbeddedEntryViewModel { get; }
            

        #endregion
    }
}
