using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Tools;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using FileExplorer.WPF.Models;
using QuickZip.Converters;
using QuickZip.UserControls.Logic.Tools.IconExtractor;
using ExifLib;
using System.Windows.Media.Imaging;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.IO.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.IO;

namespace FileExplorer.Models
{    

    public class GetFromSystemImageListUsingExtension
    {
        private static IconExtractor _iconExtractor = new IconExtractor();
        static GetFromSystemImageListUsingExtension dummy = new GetFromSystemImageListUsingExtension();
        public static IModelIconExtractor<IEntryModel> Instance = Create();
        public static IModelIconExtractor<IEntryModel> Create(Func<IEntryModel, string> fnameFunc = null)
        {
            fnameFunc = fnameFunc == null ? e => e.Label : fnameFunc;
            Func<IEntryModel, string> keyFunc = (m) =>
                {
                    if (m.IsDirectory)
                        return "GetFromSystemImageListUsingExtension - Directory";
                    else
                    {
                        string fname = fnameFunc(m);
                        string extension = m.Profile.Path.GetExtension(fname).ToLower();

                        if (String.IsNullOrEmpty(extension))
                        {
                            //Without extension.
                            return "GetFromSystemImageListUsingExtension - File";
                        }
                        else
                        {
                            return "GetFromSystemImageListUsingExtension - " + extension;
                        }
                    }
                };
            Func<IEntryModel, byte[]> getIconFunc = em =>
                {
                    if (em.IsDirectory)
                    {
                        return ResourceUtils.GetResourceAsByteArray(dummy, "/Themes/Resources/FolderIcon.png");
                    }

                    if (em != null)
                    {
                        string fname = fnameFunc(em);
                        using (Bitmap bitmap =
                            _iconExtractor.GetBitmap(IconSize.large, fname, em.IsDirectory, false))
                            if (bitmap != null)
                                return bitmap.ToByteArray();

                    }
                    return null;

                };

            return ModelIconExtractor<IEntryModel>.FromFuncCachable(
            keyFunc,
            (em) => getIconFunc(em)
            );
        }
    }

}
