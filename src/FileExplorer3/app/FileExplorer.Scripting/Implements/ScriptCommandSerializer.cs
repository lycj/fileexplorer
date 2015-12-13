using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    public class ScriptCommandSerializer : XmlSerializer
    {
        public ScriptCommandSerializer(params Type[] commandTypes)
            : base(typeof(ScriptCommandBase), commandTypes)
        {

        }

        public Stream SerializeScriptCommand(IScriptCommand command)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter myWriter = new StreamWriter(ms);
            Serialize(myWriter, command);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public IScriptCommand DeserializeScriptCommand(Stream stream)
        {
            return Deserialize(stream) as ScriptCommandBase;
        }
    }   
}
