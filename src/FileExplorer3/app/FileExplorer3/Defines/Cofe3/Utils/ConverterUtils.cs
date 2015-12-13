using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FileExplorer.WPF.Utils
{
    public static class ConverterUtils
    {
        public static Int32 ToInt32(string str, Int32 def = 0)
        {
            int retVal = 0;
            if (Int32.TryParse(str, out retVal))
                return retVal;
            return def;
        }

        public static float ToFloat(string str, float def = 0.0f)
        {
            float retVal = 0.0f;
            if (Single.TryParse(str, out retVal))
                return retVal;
            return def;
        }

       
    }
}
