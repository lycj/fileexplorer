using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{    
    [XmlInclude(typeof(ParsePath))]
    [XmlInclude(typeof(DiskCreatePath))]
    [XmlInclude(typeof(DiskOpenStream))]
    [XmlInclude(typeof(CopyStream))]
    [XmlInclude(typeof(Download))]    
    [XmlInclude(typeof(NotifyEntryChanged))]
    [XmlInclude(typeof(BaseScriptCommands))]
    public class FileExplorer3Commands
    {

    }
}
