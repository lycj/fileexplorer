using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Windows;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using vbAccelerator.Components.ImageList;
using System.Diagnostics;
using System.Windows.Resources;
using QuickZip.IO.COFE.UserControls.ViewModel;
using QuickZip.IO.COFE.UserControls.Model;

namespace QuickZip.IO.COFE.UserControls
{

    [ValueConversion(typeof(ExAViewModel), typeof(ImageSource))]
    public class ExAModelToIconConverter : ExAToIconConverter
    {
        protected override void ValueToKey(object value, out string key, out string imageKey, out bool delayLoading)
        {
            key = ""; imageKey = ""; delayLoading = false;
            if (value is ExAViewModel)
                base.ValueToKey((value as ExAViewModel).EmbeddedModel.EmbeddedEntry, out key, out imageKey, out delayLoading);
            else if (value is ExAModel)
                base.ValueToKey((value as ExAModel).EmbeddedEntry, out key, out imageKey, out delayLoading);
        }
    }
}
