using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{
    public static partial class ExtensionMethods 
    {
        public static IProgress<TransferProgress> GetProgress(this ParameterDic pd)
        {
            return pd.Get<IProgress<TransferProgress>>("{Progress}", NullTransferProgress.Instance); }

        public static void SetProgress(this ParameterDic pd, IProgress<TransferProgress> progress)
        {
            pd.Set("{Progress}", progress);
        }
    }
}
