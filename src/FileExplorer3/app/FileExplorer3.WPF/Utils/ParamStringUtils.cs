using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public static class ParamStringUtils
    {
        private static string ParseParamStringPattern = @"([&]?(?<key>[^&^=]*)=(?<value>[^&^=]*))";
        public static Dictionary<string, string> ParseParamString(string url)
        {
            url = url.Substring(url.IndexOf('?') + 1);

            int startPos = 0;
            Regex regex = new Regex(ParseParamStringPattern);
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            while (startPos < url.Length)
            {
                var match = regex.Match(url, startPos);
                if (!match.Success)
                    throw new ArgumentException();
                startPos = match.Index + match.Length;
                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value.Replace("AmPAmP", "&").Replace("eQuAleQual", "=");
                retVal.Add(key, value);
            }
            return retVal;
        }
    }
}
