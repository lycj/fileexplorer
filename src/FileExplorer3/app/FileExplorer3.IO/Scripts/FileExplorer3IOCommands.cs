using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    [XmlInclude(typeof(DiskClipboard))]
    [XmlInclude(typeof(DiskRun))]
    [XmlInclude(typeof(DiskTransfer))]
    [XmlInclude(typeof(IOExplorerDefault))]
    [XmlInclude(typeof(IOExplorerDefaultToolbarCommands))]
    [XmlInclude(typeof(SzsDiskTransfer))]      
    public class FileExplorer3IOCommands
    {

    }
}
