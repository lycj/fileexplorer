using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FileExplorer.UIEventHub;

namespace FileExplorer.Models
{
    public interface IEntryModel : INotifyPropertyChanged, IEquatable<IEntryModel>, IDraggable
    {
        IProfile Profile { get; }

        bool IsDirectory { get;  }
        IEntryModel Parent { get; }
        string Label { get; }
        string Name { get; set; }
        string Description { get; }
        string FullPath { get; }
        bool IsRenamable { get; }

        DateTime CreationTimeUtc { get; }
        DateTime LastUpdateTimeUtc { get; }
    }

    public interface IConvertedEntryModel : IEntryModel
    {
        IEntryModel OriginalEntryModel { get; }
    }

    public interface IEntryLinkModel : IEntryModel
    {
        string LinkPath { get; }
    }
}
