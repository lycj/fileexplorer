using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    //public class ScriptCommandSerializer : XmlSerializer
    //{

    //    public ScriptCommandSerializer()
    //        : base(typeof(ScriptCommandBase),
    //            new Type[] {                     
    //                typeof(ResultCommand),

    //                typeof(ParsePath),
    //                typeof(DiskCreatePath),
    //                typeof(OpenStream),
    //                typeof(CopyStream),
    //                typeof(Download),

    //                // FileExplorer.WPF.BaseControls.CommonCommands
    //                typeof(MarkEventHandled),
    //                typeof(SetEventIsHandled),
    //                typeof(SetIsHandled),

    //                //FileExplorer.Script (in FileExplorer.WPF)
    //                typeof(ShowMessageBox)
    //            })
    //    {

    //    }

    //    public static ScriptCommandSerializer Instance = new ScriptCommandSerializer();

    //    public static Stream SerializeScriptCommand(IScriptCommand command)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        StreamWriter myWriter = new StreamWriter(ms);
    //        Instance.Serialize(myWriter, command);
    //        ms.Seek(0, SeekOrigin.Begin);
    //        return ms;
    //    }

    //    public static IScriptCommand DeserializeScriptCommand(Stream stream)
    //    {
    //        return Instance.Deserialize(stream) as ScriptCommandBase;
    //    }
    //}
}
