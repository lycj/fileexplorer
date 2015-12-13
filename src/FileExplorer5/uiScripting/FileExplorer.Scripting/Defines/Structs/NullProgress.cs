using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Defines
{
    public class NullProgress<T> : IProgress<T>
    {
        public static NullProgress<T> Instance = new NullProgress<T>();
        public void Report(T value)
        {
        }
    }
}
