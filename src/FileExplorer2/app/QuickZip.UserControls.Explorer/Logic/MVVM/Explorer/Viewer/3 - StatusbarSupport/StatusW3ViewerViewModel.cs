using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace QuickZip.UserControls.MVVM
{
    public class StatusW3ViewerViewModel : BaseW3ViewerViewModel
    {
        #region Constructor

        public StatusW3ViewerViewModel(Uri initialUri)
            : base(initialUri)
        {
            IsSimpleStatusbar = true;
            StatusText = initialUri.AbsoluteUri;
        }

        #endregion


        #region Methods

        public override string ToString()
        {
            return "W3ViewerVM;" + WebAddress.ToString();
        }

        public override void ChangeAddress(Uri value)
        {
            base.ChangeAddress(value);
           
        }
               


        #endregion

        #region Data

        

        #endregion

        #region Public Properties

        

        #endregion
    }
}
