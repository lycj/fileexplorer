using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.IO;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{

    public abstract class DiskProfileBase : ProfileBase, IDiskProfile
    {

        public DiskProfileBase(IEventAggregator events, params IConverterProfile[] converters)
            : base(events, converters)
        {
            MetadataProvider = new MetadataProviderBase(new BasicMetadataProvider(), new FileBasedMetadataProvider());
            CommandProviders.Add(new FileBasedCommandProvider());//Open, Cut, Copy, Paste etc 
            DeleteCommand =
                ScriptCommands.AssignProperty("{DeleteEntries}", "Length", "{DeleteEntries-Length}",  //Assign DeleteEntries Length
                    ScriptCommands.IfValue<int>(ComparsionOperator.GreaterThanOrEqual, "{DeleteEntries.Length}", 1, //If DeleteEntries Length >= 1                        
                        UIScriptCommands.MessageBoxYesNo("FileExplorer", "Delete {DeleteEntries[0]} and {DeleteEntries-Length} Item(s)?", //IfTrue                
                            CoreScriptCommands.DiskDeleteMultiple("{DeleteEntries}", true))));
            CreateFolderCommand =
                 CoreScriptCommands.DiskCreateFolder("{BaseFolder.Profile}", "{BaseFolder.FullPath}\\{FolderName}",
                    "{CreatedFolder}", NameGenerationMode.Rename);
        }

        public IDiskIOHelper DiskIO { get; protected set; }
        //public IDragDropHandler DragDrop { get; protected set; }
        //public new IShellDragDropHandler DragDrop { get; protected set; }        
    }
}
