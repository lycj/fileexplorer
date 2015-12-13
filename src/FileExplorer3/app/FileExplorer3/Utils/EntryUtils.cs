using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public static class EntryUtils
    {
        public static async Task<IList<IEntryModel>> ListAsync(IEntryModel[] entries,
            CancellationToken ct, Func<IEntryModel, bool> filter, bool refresh = false)
        {
            List<IEntryModel> retList = new List<IEntryModel>();

            foreach (var entry in entries)
                retList.AddRange(await entry.Profile.ListAsync(entry, ct, filter, refresh));

            return retList;
        }
    }
}
