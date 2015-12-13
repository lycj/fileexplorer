using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    [XmlInclude(typeof(DirectoryTreeToggleNode))]
    [XmlInclude(typeof(ExplorerAssignCurrentDirectory))]
    [XmlInclude(typeof(ExplorerAssignScriptParameters))]
    [XmlInclude(typeof(ExplorerCreate))]
    //[XmlInclude(typeof(ExplorerDo))] //NotSerializable!
    [XmlInclude(typeof(ExplorerGoTo))]
    [XmlInclude(typeof(ExplorerParam))]
    [XmlInclude(typeof(ExplorerShow))]
    [XmlInclude(typeof(FileListAssignEntries))]
    [XmlInclude(typeof(FileListRefresh))]
    [XmlInclude(typeof(FileListSelect))]
    [XmlInclude(typeof(MessageBoxShow))]
    [XmlInclude(typeof(NotifyDirectoryChanged))]
    [XmlInclude(typeof(NotifyRootChanged))]
    [XmlInclude(typeof(ProfilePicker))]
    [XmlInclude(typeof(SetScriptCommand))]
    [XmlInclude(typeof(TabbedExplorerShow))]
    [XmlInclude(typeof(TabExplorerNewTab))]
    [XmlInclude(typeof(TabExplorerCloseTab))]
    [XmlInclude(typeof(RunCommadLine))]
    public class FileExplorer3WPFCommands
    {

    }
}
