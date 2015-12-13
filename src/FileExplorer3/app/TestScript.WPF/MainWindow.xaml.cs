using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Models;
using FileExplorer.Script;
using FileExplorer.UnitTests;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace TestScript.WPF
{
    





    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           //WPFScriptCommands.ParseOrCreatePath

        }       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AsyncUtils.RunSync(() => ScriptCommandTests.UnitTest());



            AsyncUtils.RunSync(() => ScriptCommandTests.Test_DownloadFile());           

            
        }

        
    }
}
