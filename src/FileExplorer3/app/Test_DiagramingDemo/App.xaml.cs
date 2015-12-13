using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramingDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, new ConsoleTarget());
            //LogManagerFactory.DefaultConfiguration.IsEnabled = true;
    //        int[] array = new int[] { 1, 3, 5 };	
    //        IScriptCommand filterCommand = ScriptCommands.FilterArray("{Array}", null, ComparsionOperator.GreaterThan,
    //"{Value}", "{Array}",
    //  ScriptCommands.PrintDebug("{Array}"));

    //        int[] value = AsyncUtils.RunSync(() => ScriptRunner.RunScriptAsync<int[]>("{Array}", new ParameterDic() { 
    //            { "Array", array }, 
    //            { "Value", 2 }
    //        }, filterCommand));


            base.OnStartup(e);
        }
    }
}
