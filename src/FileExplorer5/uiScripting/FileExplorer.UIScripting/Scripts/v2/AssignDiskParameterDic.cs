using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Serializable, Assign a new Disk based ParameterDic to currrent ParameterDic, which 
        /// load properties from disk and save them to disk when disposed.
        /// Settings file are stored in %appdata%\{assemblyName}\{filename}
        /// </summary>
        /// <example>
        ///   ScriptRunner.RunScript(
        ///    HubScriptCommands.AssignDiskParameterDic("{DiskPD}", null, "Settings.xaml",
        ///        false,
        ///        ScriptCommands.IfAssigned("{DiskPD.LastEditTime}",
        ///            ScriptCommands.PrintDebug("{DiskPD.LastEditTime}", ScriptCommands.Reset(null, "{DiskPD.LastEditTime}")),
        ///            ScriptCommands.Assign("{DiskPD.LastEditTime}", DateTime.Now))));          
        /// </example>
        /// <param name="variable"></param>
        /// <param name="assemblyName"></param>
        /// <param name="fileName"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignDiskParameterDic(string variable = "{Variable}",
            string assemblyName = null, string fileName = "Settings.xml",
            bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new AssignDiskParameterDic()
            {
                VariableKey = variable,
                AssemblyName = assemblyName,
                FileName = fileName,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class AssignDiskParameterDic : Assign
    {
        /// <summary>
        /// Settings file are stored in %appdata%\{assemblyName}\{filename}, default = null = %Current assembly name%.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Settings file are stored in %appdata%\{assemblyName}\{filename}, default = Settings.xml
        /// </summary>
        public string FileName { get; set; }

        public AssignDiskParameterDic()
        {
            VariableKey = "Dic#" + new Random().Next().ToString();
            AssemblyName = null;
            FileName = "Settings.xml";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            AssemblyName = AssemblyName ?? Application.Current.MainWindow.GetType().Assembly.GetName().Name;
            Value = new ParameterDic(new DiskParameterDicStore(AssemblyName + "\\" + FileName));
            return base.Execute(pm);
        }
    }
}
