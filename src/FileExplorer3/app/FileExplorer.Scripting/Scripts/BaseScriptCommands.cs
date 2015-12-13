using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileExplorer.Script
{
    [XmlInclude(typeof(ArithmeticCommand))]
    [XmlInclude(typeof(Assign))]
    [XmlInclude(typeof(AssignCanExecuteCondition))]
    [XmlInclude(typeof(AssignParameterDic))]
    [XmlInclude(typeof(AssignGlobalParameterDic))]
    [XmlInclude(typeof(AssignValueConverter))]
    [XmlInclude(typeof(Delay))]
    [XmlInclude(typeof(Dump))]
    [XmlInclude(typeof(ForEach))]
    [XmlInclude(typeof(FormatText))]
    [XmlInclude(typeof(IfValue))]
    [XmlInclude(typeof(Print))]
    [XmlInclude(typeof(Reassign))]
    [XmlInclude(typeof(RunCommands))]
    [XmlInclude(typeof(RunICommand))]
    [XmlInclude(typeof(RunScriptCommand))]
    [XmlInclude(typeof(ResultCommand))]
    public class BaseScriptCommands
    {

    }
}
