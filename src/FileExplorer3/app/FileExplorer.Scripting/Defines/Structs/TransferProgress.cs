using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Defines
{
    public class TransferProgress
    {
        public ProgressType Type { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
        public Int32? TotalEntriesIncrement { get; set; }
        public Int32? ProcessedEntriesIncrement { get; set; }
        public short? CurrentProgressPercent { get; set; }
        public string Source { get; set; }
        public IPathHelper SourcePathHelper { get; set; }
        public string Destination { get; set; }
        public IPathHelper DestinationPathHelper { get; set; }

        public static TransferProgress SetAction(string action)
        {
            return new TransferProgress()
            {
                Action = action
            };
        }

        public static TransferProgress Error(Exception ex)
        {
            return new TransferProgress()
            {
                Type = ProgressType.Error,
                Message = ex.Message
            };
        }

        public static TransferProgress SetMessage(ProgressType type, string message)
        {
            return new TransferProgress()
            {
                Type = type,
                Message = message
            };
        }


        public static TransferProgress Completed()
        {
            return new TransferProgress()
            {
                Type = ProgressType.Completed
            };
        }

        public static TransferProgress From(string src, IPathHelper srcPathHelper, string dest, IPathHelper destPathHelper)
        {
            return new TransferProgress()
            {
                Type = ProgressType.Running,
                Source = src,
                SourcePathHelper = srcPathHelper,
                Destination = dest,
                DestinationPathHelper = destPathHelper
            };
        }

        public static TransferProgress From(string src, IPathHelper pathHelper = null)
        {
            return From(src, pathHelper, null, null);
        }

        public static TransferProgress From(string src, string dest)
        {
            return From(src, PathHelper.Auto(src), dest, PathHelper.Auto(dest));
        }

        public static TransferProgress To(string dest, IPathHelper pathHelper = null)
        {
            return From(null, null, dest, pathHelper);
        }

        public static TransferProgress IncrementTotalEntries(int count = 1)
        {
            return new TransferProgress() { Type = ProgressType.Running, TotalEntriesIncrement = count };
        }
        public static TransferProgress IncrementProcessedEntries(int count = 1)
        {
            return new TransferProgress() { Type = ProgressType.Running, ProcessedEntriesIncrement = count };
        }
        public static TransferProgress UpdateCurrentProgress(short percent = 1)
        {
            return new TransferProgress() { Type = ProgressType.Running, CurrentProgressPercent = percent };
        }
    }

    public class NullTransferProgress : IProgress<TransferProgress>
    {
        public static NullTransferProgress Instance = new NullTransferProgress();
        public void Report(TransferProgress value)
        {
        }
    }
}
