using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF
{
    public struct Range<T>
    {
        public T StartIndex { get; set; }
        public T EndIndex { get; set; }
    }
}
