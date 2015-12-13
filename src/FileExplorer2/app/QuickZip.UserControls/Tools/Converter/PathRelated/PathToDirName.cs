using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;

namespace QuickZip.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class PathToDirNameConverter : IValueConverter
    {
        #region IValueConverter Members
        public static PathToDirNameConverter Instance = new PathToDirNameConverter();

        private string GetPathLastDir(string path)
        {
            string[] inputSplit = path.Split(new char[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
            if (inputSplit.Length == 0)
                return path;
            return inputSplit[inputSplit.Length - 1];
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            if (path == null)
                return null;
            if (path.StartsWith("*")) //Bypass
                return path.Substring(1);


            string ppath = GetPathLastDir(path);
            if (ppath.EndsWith(":")) ppath += "\\";

            if (ppath.EndsWith(":\\") && (ppath.Length==3)) //Root
            {
                DriveInfo di = new DriveInfo(ppath);
                if (di.IsReady)
                    return ppath + " (" + di.VolumeLabel + ")";
            }

            return ppath == "" ? path : ppath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();                
        }

        #endregion
    }
}
