using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.ObjectModel;
using FileExplorer.WPF.Utils;
using MetroLog;
using FileExplorer.UnitTests;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.Models;

namespace TestScript.WPF
{
    [Export(typeof(IScreen))]
    public class AppViewModel : Screen
    {
        #region Constructor

        public AppViewModel()
        {

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, new ConsoleTarget());
            LogManagerFactory.DefaultConfiguration.IsEnabled = true;
            
            LogManagerFactory.DefaultLogManager.GetLogger<AppViewModel>().Log(LogLevel.Debug, "Test");
            //AsyncUtils.RunSync(() => ScriptCommandTests.Test_DownloadFile());      


    //        IScriptCommand diskTransferCommand =
    //ScriptCommands.ParsePath("{SourceFile}", "{Source}",
    //ScriptCommands.DiskParseOrCreateFolder("{DestinationDirectory}", "{Destination}",
    //IOScriptCommands.DiskTransfer("{Source}", "{Destination}", false, false)));

            //await ScriptRunner.RunScriptAsync(new ParameterDic() { 
            //                { "Profile", FileSystemInfoExProfile.CreateNew() },
            //                { "SourceFile", srcFile },
            //                { "DestinationFile", destFile }
            //            }, copyCommand);


            //string tempDirectory = "C:\\Temp";
            //string destDirectory = "C:\\Temp\\Destination1";
            //string srcFile = System.IO.Path.Combine(tempDirectory, "file1.txt");
            //string destFile = System.IO.Path.Combine(destDirectory, "file2.txt");

            //AsyncUtils.RunSync(() => ScriptRunner.RunScriptAsync(new ParameterDic() { 
            //    { "Profile", FileExplorer.Models.FileSystemInfoExProfile.CreateNew() },
            //    { "SourceFile", srcFile },
            //    { "DestinationDirectory", destDirectory }
            //}, diskTransferCommand));			

            string tempDirectory = "C:\\Temp";
            string destDirectory = "C:\\Temp\\Debug2";
            string srcDirectory = "C:\\Temp\\aaaaabc";


            IScriptCommand diskTransferCommand =
                CoreScriptCommands.ParsePath("{Profile}", srcDirectory, "{Source}",
                CoreScriptCommands.DiskParseOrCreateFolder("{Profile}", destDirectory, "{Destination}",
                IOScriptCommands.DiskTransfer("{Source}", "{Destination}", null, false, false)));

            AsyncUtils.RunSync(() => ScriptRunner.RunScriptAsync(new ParameterDic() { 
                { "Profile", FileSystemInfoExProfile.CreateNew() }
            }, diskTransferCommand));

            
        }

        #endregion

        #region Methods

        public void Serialize()
        {
            //CSharpCodeProvider.cre
            CSharpCodeProvider prov = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });            

            CompilerParameters cp = new CompilerParameters(new [] 
            {
                 "mscorlib.dll", "System.dll", "System.IO.dll", "System.xml.dll", "System.Xml.Serializer.dll", "System.Runtime.dll", 
                 "System.ObjectModel.dll", "System.Collections.dll",
                 "FileExplorer3.dll", "FileExplorer3.IO.dll", "FileExplorer3.WPF.dll", 
                 "Caliburn.Micro.dll", "Caliburn.Micro.Platform.dll"
            });
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
          
            Progress.Clear();
            
            CompilerResults cr = prov.CompileAssemblyFromSource(cp, Script);

            if (cr.Errors.HasErrors)
            {
                Progress.Add("Error when serializing:");
                cr.Errors.Cast<CompilerError>().ToList().ForEach(err => 
                    Progress.Add(String.Format("{0} at line {1} column {2} ", err.ErrorText, err.Line, err.Column)));
            }
        }

        #endregion

        #region Data

        private string _script;
        private string _xml;
        private ObservableCollection<string> _progress = new ObservableCollection<string>();

        #endregion

        #region Public Properties

        public string Script { get { return _script; } set { _script = value; NotifyOfPropertyChange(() => Script); } }
        public string Xml { get { return _xml; } set { _xml = value; NotifyOfPropertyChange(() => Xml); } }

        public ObservableCollection<string> Progress { get { return _progress; } set { _progress = value; NotifyOfPropertyChange(() => Progress); } }

        #endregion

    }
}
