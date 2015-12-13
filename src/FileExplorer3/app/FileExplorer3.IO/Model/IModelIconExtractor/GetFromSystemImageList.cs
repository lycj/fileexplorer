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
    public class GetFromSystemImageList //: IModelIconExtractor<IEntryModel>
    {
        private static IconExtractor _iconExtractor = new ExIconExtractor();
        public static IModelIconExtractor<IEntryModel> Instance = Create();
        private static string[] excludedExtensions = ".lnk,.exe".Split(',');
        public static IModelIconExtractor<IEntryModel> Create()
        {
            Func<IEntryModel, string> keyFunc = (m) =>
            {
                if (m.IsDirectory)
                    //if (model.FullPath.StartsWith("::"))
                    return "GetFromSystemImageList - " + m.FullPath;
                //else return "GetFromSystemImageList - Directory";
                else
                {
                    string extension = m.GetExtension().ToLower();

                    if (String.IsNullOrEmpty(extension))
                    {
                        //Without extension.
                        if (m.FullPath.StartsWith("::"))
                            return "GetFromSystemImageList - " + m.FullPath;
                        else return "GetFromSystemImageList - File";
                    }
                    else
                    {
                        if (excludedExtensions.Contains(extension))
                            return "GetFromSystemImageList - " + m.FullPath;
                        return "GetFromSystemImageList - " + extension;
                    }
                }
            };
            Func<IEntryModel, byte[]> getIconFunc = em =>
            {
                if (em != null && !String.IsNullOrEmpty(em.FullPath))
                    using (FileSystemInfoEx fsi = FileSystemInfoEx.FromString(em.FullPath))
                    {
                        Bitmap bitmap = null;
                        if (fsi != null && fsi.Exists)
                            return fsi.RequestPIDL(pidl =>
                            {
                                if (pidl != null)
                                {
                                    bitmap = _iconExtractor.GetBitmap(QuickZip.Converters.IconSize.extraLarge,
                                        pidl.Ptr, em.IsDirectory, false);

                                    if (bitmap != null)
                                        return
                                            bitmap.ToByteArray();
                                }
                                return new byte[] { };
                            });
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
