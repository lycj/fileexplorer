//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using QuickZip.IO.DirectoryLister;
//using System.Windows.Data;

//namespace QuickZip.Logic
//{
//    public static class EntryModelUtils
//    {
//        static EntryModel _desktopModel = new EntryModel(EntryUtils.DesktopEntry);
//        public static EntryModel DesktopModel { get { return _desktopModel; } }
//        public static bool MatchEntryModel(EntryModel item1, EntryModel item2)
//        {
//            if (item1 == null && item2 == null) return true;
//            if (item1 == null) return false;
//            if (item2 == null) return false;
//            return EntryUtils.MatchEntry(item1.Entry, item2.Entry);
//        }

//        public static string ConvertTimeSpanToStr(TimeSpan span)
//        {
//            string outStr = "";

//            if (span.Hours > 0) outStr += span.Hours.ToString() + " Hours ";
//            if (span.Minutes > 0) outStr += span.Minutes.ToString() + " Minutes ";
//            if (span.Seconds > 0) outStr += span.Seconds.ToString() + " Seconds ";

//            if (outStr == "") outStr = "0 Seconds ";

//            return outStr;
//        }
//    }
//}
