using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.Models
{
    public class ResourceIconExtractor<T> : ModelIconExtractor<T>
    {
        private static ResourceIconExtractor<T> dummy = new ResourceIconExtractor<T>();

        public static IModelIconExtractor<T> FromEmbeddedResource(object sender, string path2Resource)
        {
            return ModelIconExtractor<T>.FromTaskFunc(
                () => Task.FromResult(
                    ResourceUtils.GetEmbeddedResourceAsByteArray(sender, path2Resource)));
        }

        public static IModelIconExtractor<T> FromEmbeddedResource(string path2Resource)
        {
            return FromEmbeddedResource(dummy, path2Resource);
        }

        public static IModelIconExtractor<T> ForSymbol(int symbol)
        {
            return ForSymbol(Convert.ToChar(symbol));
        } 

        public static IModelIconExtractor<T> ForSymbol(char symbol)
        {  
            string symbolCode = String.Format("{0:x2}", (UInt32)symbol).ToUpper();
            string resourcePath = String.Format("/Themes/Resources/SegoeUISymbols/{0}.png", symbolCode);

            var retVal = ModelIconExtractor<T>.FromFuncCachable("SegoeUI-" + symbol, 
                () =>
                {
                    byte[] resource = ResourceUtils.GetResourceAsByteArray(dummy, resourcePath);
                    if (resource.Length != 0)
                        return resource;
                    else return BitmapUtils.DrawTextToBitmapStream("Segoe UI Symbol", 24, symbol + "").ToByteArray();
                });

            return retVal;
        }
        
    }
}
