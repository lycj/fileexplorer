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
using QuickZip.IO.PIDL.UserControls.ViewModel;
using QuickZip.IO.PIDL.UserControls.Model;

namespace QuickZip.IO.PIDL.UserControls
{

    [ValueConversion(typeof(ExViewModel), typeof(ImageSource))]
    public class ExModelToIconConverter : ExToIconConverter
    {
        protected override void ValueToKey(object value, out string key, out string fastKey, out bool delayLoading)
        {
            key = ""; fastKey = ""; delayLoading = false;

            ExModel model = null;
            if (value is ExViewModel)
                model = (value as ExViewModel).EmbeddedModel;
            else if (value is ExModel)
                model = value as ExModel;

            FileSystemInfoEx entry = null;
            if (model != null)
            {
                if (model is FileModel)
                {
                    key = fastKey = PathEx.GetExtension(model.Name);
                    if (key == "")
                        key = fastKey = ".AaAaA";
                    if (imageFilter.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) != -1 ||
                        specialExtFilter.Split(',').Contains(key))
                    {
                        entry = model.EmbeddedEntry;
                        delayLoading = true;
                    }
                }
                else
                {

                    if (IsSpecialFolder(model.FullName))
                    {
                        entry = model.EmbeddedEntry;
                        delayLoading = true;
                    }
                    else
                    {
                        key = fastKey = tempPath;
                    }
                }

            }

            if (entry != null)
                base.ValueToKey(entry, out key, out fastKey, out delayLoading);
        }
    }
}
