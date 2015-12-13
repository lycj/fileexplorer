using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls.Model
{

    public class DirectoryModel : ExModel
    {
        #region Data  
              

        #endregion

        #region Ctor        
        public DirectoryModel(DirectoryInfoEx dir)
            : base(dir)
        {            
        }

        #endregion


        #region Public Properties

        public DirectoryInfoEx EmbeddedDirectoryEntry
        {
            get { return EmbeddedEntry as DirectoryInfoEx; } 
        }

        #endregion

    }



}
