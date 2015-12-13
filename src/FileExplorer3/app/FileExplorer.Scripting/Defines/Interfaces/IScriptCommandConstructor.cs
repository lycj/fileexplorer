using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    //public interface IScriptCmdCtor<T>
    //{
    //    IScriptCommand Construct(T parameter);
    //}


    ///// <summary>
    ///// Serializable constructor which is used to replace Func[T,IScriptCommand]
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class ScriptCommandConstructor<T> : IScriptCmdCtor<T>
    //{        
    //    public string TypeName { get; set; }

    //    [XmlIgnore()]
    //    public Type Type { get { return Type.GetType(TypeName); } }

    //    public IScriptCommand NextCommand { get; set; }
        
    //    public static ScriptCommandConstructor<T> ReturnCommand(IScriptCommand command)
    //    {
    //        return new ScriptCommandConstructor<T>()
    //        {
    //            TypeName = null,
    //            NextCommand = command
    //        };
    //    }

    //    public static ScriptCommandConstructor<T> Construct<T>(Type type, IScriptCommand nextCommand = null)
    //    {
    //        return new ScriptCommandConstructor<T>()
    //        {
    //            TypeName = type.AssemblyQualifiedName,
    //            NextCommand = nextCommand
    //        };
    //    }

    //    public IScriptCommand Construct(T parameter)
    //    {
    //        if (TypeName == null)
    //            return NextCommand ?? ResultCommand.NoError;

    //        if (NextCommand == null)
    //            return (IScriptCommand)Activator.CreateInstance(Type, parameter);
    //        else return (IScriptCommand)Activator.CreateInstance(Type, parameter, NextCommand);
    //    }
    //}
}
