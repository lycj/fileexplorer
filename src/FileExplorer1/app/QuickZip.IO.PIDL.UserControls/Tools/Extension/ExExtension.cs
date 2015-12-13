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
using QuickZip.IO.PIDL;
using System.IO;

namespace QuickZip.IO.PIDL.Tools
{    
    [MarkupExtensionReturnType(typeof(FileSystemInfoEx), typeof(DirectoryInfoEx))]
    public class ExExtension : MarkupExtension
    {
        [ConstructorArgument("FullName")]
        public string FullName { get; set; }

        public ExExtension() { }
        public ExExtension(string fullName) { FullName = fullName; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!String.IsNullOrEmpty(FullName))
                return FileSystemInfoEx.FromString(FullName);
            return DirectoryInfoEx.DesktopDirectory;
        }
    }
}
