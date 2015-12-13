using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class ExtensionMethods 
    {
        public static IProgress<TransferProgress> GetProgress(this ParameterDic pd)
        {
            return pd.GetValue<IProgress<TransferProgress>>("{Progress}", NullTransferProgress.Instance); }

        public static void SetProgress(this ParameterDic pd, IProgress<TransferProgress> progress)
        {
            pd.SetValue("{Progress}", progress);
        }
    }
}
