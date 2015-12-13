using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public class ConsoleProgress : IProgress<TransferProgress>
    {
        int _totalEntries = 0;
        int _processedEntries = 0;

        public void Report(TransferProgress value)
        {
            if (value.TotalEntriesIncrement.HasValue)
                _totalEntries += value.TotalEntriesIncrement.Value;

            if (value.ProcessedEntriesIncrement.HasValue)
                _processedEntries += value.ProcessedEntriesIncrement.Value;

            string message = value.Message ?? String.Format("{0} -> {1}", value.Source, value.Destination);
            
            Console.WriteLine(String.Format("{0}/{1} {2} ({3}%)", _processedEntries, _totalEntries,  message, value.CurrentProgressPercent));
        }
    }
}
