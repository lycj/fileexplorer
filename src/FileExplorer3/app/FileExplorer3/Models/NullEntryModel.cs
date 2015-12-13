using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FileExplorer.UIEventHub;

namespace FileExplorer.Models
{    
    public class NullEntryModel : IEntryModel
    {
        public static NullEntryModel Instance = new NullEntryModel();


        public IProfile Profile { get { return null; } }

        public bool IsDirectory { get { return false; } }
        public IEntryModel Parent { get { return null; } }
        public string Label { get { return null; } }
        public string Name { get { return null; } set { } }
        public string Description { get { return null; } }
        public string FullPath { get { return null; } }
        public bool IsRenamable { get { return false; } }

        public DateTime CreationTimeUtc { get { return DateTime.MinValue; } }
        public DateTime LastUpdateTimeUtc { get { return DateTime.MinValue; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(IEntryModel other)
        {
            return false;
        }

        public bool IsDragging { get; set;}
        public string DisplayName { get { return Label; }}
    }
}
