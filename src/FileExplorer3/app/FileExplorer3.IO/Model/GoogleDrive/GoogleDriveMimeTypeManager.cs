using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Models
{
    public interface IMimeTypeManager
    {
        IList<string> GetExportableMimeTypes(string mimeType);
        string GetExtension(string mimeType);
    }


    public class GoogleMimeTypeManager : IMimeTypeManager
    {
        public static string FolderMimeType = "application/vnd.google-apps.folder";


        private static Dictionary<string, string> DefaultMimeTypes = new Dictionary<string, string>()
        {   
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
            { "application/vnd.openxmlformats-officedocument.presentationml.presentation",".pptx" },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document",".docx" },
        };

        #region Constructor

        public GoogleMimeTypeManager(Google.Apis.Drive.v2.Data.About aboutInfo)
        {
            _aboutInfo = aboutInfo;
            _exportComparer = new PriorityStringComparer(DefaultMimeTypes.Keys.ToArray(), StringComparer.CurrentCulture);
        }

        #endregion

        #region Methods

        public IList<string> GetExportableMimeTypes(string mimeType)
        {
            var export = _aboutInfo.ExportFormats.FirstOrDefault(ef => ef.Source.Equals(mimeType, StringComparison.CurrentCultureIgnoreCase));
            if (export != null)
            {
                var outputList = export.Targets.Where(mime => !String.IsNullOrEmpty(GetExtension(mime))).ToList();
                if (outputList.Count > 0)
                {
                    outputList.Sort(_exportComparer);
                    return outputList;
                }
            }

            return new List<string>() { mimeType };
        }

        public string GetExtension(string mimeType)
        {
            if (DefaultMimeTypes.ContainsKey(mimeType))
                return DefaultMimeTypes[mimeType];

            if (!_mimeLookup.ContainsKey(mimeType))
                _mimeLookup[mimeType] = ShellUtils.MIMEType2Extension(mimeType);

            return _mimeLookup[mimeType];
        }

        #endregion

        #region Data

        private ConcurrentDictionary<string, string> _mimeLookup = new ConcurrentDictionary<string, string>();
        private Google.Apis.Drive.v2.Data.About _aboutInfo;
        private PriorityStringComparer _exportComparer;

        #endregion

        #region Public Properties

        #endregion

    }

    class PriorityStringComparer : StringComparer
    {
        private List<string> _priorityStrings;
        private StringComparer _defaultStringComparer;
        public PriorityStringComparer(string[] priorityStrings, StringComparer defaultStringComparer)
        {
            _priorityStrings = priorityStrings.ToList();
            _defaultStringComparer = defaultStringComparer;
        }

        public override bool Equals(string x, string y)
        {
            return _defaultStringComparer.Equals(x, y);
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
        public override int Compare(string x, string y)
        {
            int idxX = _priorityStrings.IndexOf(x);
            int idxY = _priorityStrings.IndexOf(y);
            if (idxX != -1 && idxY != -1)
                return idxX.CompareTo(idxY);
            if (idxX != -1)
                return idxX;
            if (idxY != -1)
                return idxY;
            return _defaultStringComparer.Compare(x, y);
        }
    }

}
