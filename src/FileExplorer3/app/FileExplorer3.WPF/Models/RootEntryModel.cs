using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileExplorer.WPF.Models
{
    public class RootEntryModel : EntryModelBase
    {       
        #region Constructor

        public RootEntryModel(params IEntryModel[] subEntryModels)
            : base(subEntryModels[0].Profile)
        {
            Label = "Root";
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
