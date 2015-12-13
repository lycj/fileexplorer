using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public class ProgressConverter<T1, T2> : IProgress<T1>
    {
        private Func<T1, T2> _convertFunc;
        private IProgress<T2> progress;
        public ProgressConverter(IProgress<T2> destProgress, Func<T1, T2> convertFunc)
        {
            _convertFunc = convertFunc;
            progress = destProgress;
        }

        public void Report(T1 value)
        {
            progress.Report(_convertFunc(value));
        }
    }
}
