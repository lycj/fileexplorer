using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class GetDefaultIcon : IModelIconExtractor<IEntryModel>
    {
        private static byte[] FileIcon { get; set; }
        private static byte[] FolderIcon { get; set; }
        public static GetDefaultIcon Instance = new GetDefaultIcon();

        static GetDefaultIcon()
        {
            var assembly = typeof(GetDefaultIcon).GetTypeInfo().Assembly;
            //string libraryName = assembly.GetName().Name;            
            string libraryName = "FileExplorer";
            FileIcon = assembly.GetManifestResourceStream(
                PathUtils.MakeResourcePath(libraryName, "/Themes/Resources/file.ico")).ToByteArray();
            FolderIcon = assembly.GetManifestResourceStream(
                PathUtils.MakeResourcePath(libraryName, "/Themes/Resources/folder.ico")).ToByteArray();


        }

        public Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            if (model.IsDirectory)
                return Task.FromResult(FolderIcon);
            else return Task.FromResult(FileIcon);
        }
    }



   

}
