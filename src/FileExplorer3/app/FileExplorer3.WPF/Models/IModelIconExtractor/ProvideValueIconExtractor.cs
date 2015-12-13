using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public static partial class EntryModelIconExtractors
    {
        public static IModelIconExtractor<IEntryModel> ProvideValue(byte[] value)
        {
            return new ProvideValueIconExtractor(value);
        }
    }


    public class ProvideValueIconExtractor : IModelIconExtractor<IEntryModel>
    {
        private static byte[] Value { get; set; }

        public ProvideValueIconExtractor(byte[] value)
        {
            Value = value;
        }

        public Task<byte[]> GetIconBytesForModelAsync(IEntryModel model, CancellationToken ct)
        {
            return Task<ImageSource>.FromResult(Value);
        }
    }

  
    
}
