///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.IO.Tools
{
    [MarkupExtensionReturnType(typeof(FileSystemInfoEx))]
    public class ExExtension : MarkupExtension
    {
        [ConstructorArgument("FullPath")]
        public string FullPath { get; set; }

        public ExExtension() { }
        public ExExtension(string fullPath) { FullPath = fullPath; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!String.IsNullOrEmpty(FullPath))
                if (DirectoryEx.Exists(FullPath))
                    return new DirectoryInfoEx(FullPath);
                else return new FileInfoEx(FullPath);
                
            return DirectoryInfoEx.DesktopDirectory;
        }
    }
}
