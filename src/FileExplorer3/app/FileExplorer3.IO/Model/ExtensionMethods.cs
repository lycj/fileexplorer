using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public static partial class ExtensionMethods
    {
        public static bool IsFileWithExtension(this IEntryModel model, string extensions)
        {
            string extension = model.Profile.Path.GetExtension(model.Name);
            return !model.IsDirectory && extensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Contains(extension, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}
