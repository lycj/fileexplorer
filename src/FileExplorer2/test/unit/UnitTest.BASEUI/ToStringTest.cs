using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Diagnostics;
using QuickZip.UserControls.MVVM.Command.Model;
using QuickZip.UserControls.MVVM.Command.ViewModel;
using Cinch;

namespace UnitTest.COFE
{
    [TestFixture]
    public class ToStringTest
    {
        [Test]
        public void CommandViewModel_Return_ToString_Correctly()
        {
            CommandModel cm = new GenericCommandModel("Test", new SimpleCommand()  );
            CommandViewModel cvm = new CommandViewModel(cm);
            Assert.AreNotEqual(cvm.ToString(), typeof(CommandViewModel).ToString());
            Console.WriteLine(cvm.ToString());
        }

       

    }
}

