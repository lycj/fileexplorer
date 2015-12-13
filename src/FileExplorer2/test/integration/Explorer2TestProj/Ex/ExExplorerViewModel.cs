using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuickZip.UserControls.MVVM.ViewModel
{
    public class ExExplorerViewModel
        : HistoryExplorerViewModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
    {

        public ExExplorerViewModel(ExProfile profile)
            : base(profile)
        {

        }
    }
}
