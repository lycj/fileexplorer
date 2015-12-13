using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using FileExplorer.Scripting;
using FileExplorer.Utils;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UnitTest.uiScripting
{
    [TestClass]
    public class Test_Script_ScriptCommand
    {
        private bool runCommand(IScriptCommand command, 
            IParameterDic pm = null)
        {
            pm = pm ?? new ParameterDic();
            while (command != null && !(command is ResultCommand))
                command = command.Execute(pm);
            return (command == null) && (pm.Error() == null);
        }

        [TestMethod]
        public void Test_Script_Assign()
        {
            IParameterDic pm = new ParameterDic();
            int var1; bool resultValue;

            bool success = runCommand(ScriptCommands.Assign("{var1}", 1, 
                ScriptCommands.IfEquals("{var1}", 1,
                ScriptCommands.Assign("{resultValue}", true),
                ScriptCommands.Assign("{resultValue}", false))), pm);
            Assert.IsTrue(success);
            var1 = pm.Get("{var1}", -1);
            resultValue = pm.Get("{resultValue}", false);

            Assert.AreEqual(var1, 1);
            Assert.IsTrue(resultValue);
        }

        [TestMethod]
        public void Test_AssignValueFunc()
        {
            IParameterDic pm = new ParameterDic();
            DateTime utcNow; 

            bool success = runCommand(ScriptCommands.AssignValueFunc("{utcNow}",
                () => DateTime.UtcNow), pm);
            Assert.IsTrue(success);
            utcNow = pm.Get("{utcNow}", DateTime.MinValue);

            Assert.AreNotEqual(DateTime.MinValue, utcNow);
        }

        [TestMethod]
        public void Test_ForEach()
        {
            int[] array = new int[] { 1, 3, 5 };
            IParameterDic pm = new ParameterDic();
            int sum;

            bool success = runCommand(
                ScriptCommands.Assign("{array}", array,  
                    ScriptCommands.ForEach("{array}", "{i}", 
                        ScriptCommands.Add("{i}", "{sum}", "{sum}"))), pm);
            Assert.IsTrue(success);
            sum = pm.Get<int>("{sum}");

            Assert.AreEqual(9, sum);
        }

        [TestMethod]
        public void Test_FilterArray()
        {
            int[] array = new int[] { 1, 3, 5 };
            IParameterDic pm = new ParameterDic();
            int[] outputArray; int arraySize;

            bool success = runCommand(
                ScriptCommands.Assign("{array}", array,  
                    ScriptCommands.FilterArray("{array}", null, 
                    ComparsionOperator.GreaterThan, 2, "{outputArray}",
                 ScriptCommands.PrintDebug("{outputArray.Length}"))), pm);
            Assert.IsTrue(success);
            outputArray = pm.Get<int[]>("{outputArray}");
            arraySize = outputArray.Length;

            Assert.IsNotNull(outputArray);
            Assert.AreEqual(2, arraySize);
        }

        [TestMethod]
        public void Test_FormatText()
        {
            string fmtString = "{day}-{month}-{year}";
            int day = 5; int month = 12; int year = 2015;
            string outputStr;
            IParameterDic pm = new ParameterDic();

            bool success = runCommand(
                ScriptCommands.Assign("{day}", day, 
                 ScriptCommands.Assign("{month}", month, 
                  ScriptCommands.Assign("{year}", year,
                   ScriptCommands.FormatText("{output}", fmtString)))), pm);
            Assert.IsTrue(success);
            outputStr = pm.Get<string>("{output}");

            Assert.AreEqual("5-12-2015", outputStr);
        }


        private class testClass
        {
            public int val1 { get; set; }
            public int val2 { get; set; }
        }

        [TestMethod]
        public void Test_GetProperty()
        {
            IParameterDic pm = new ParameterDic();
            Assert.IsTrue(runCommand(
                ScriptCommands.Assign("{utcNow}", DateTime.UtcNow,
                ScriptCommands.GetProperty("{utcNow}", "Ticks", "{ticks1}",
                ScriptCommands.Reassign("{utcNow.Ticks}", null, "{ticks2}"))), pm));
            Assert.AreNotEqual(0, pm.Get("{ticks1}"));
            Assert.AreEqual(pm.Get<long>("{ticks1}"),
                pm.Get<long>("{ticks2}"));
        }

        [TestMethod]
        public void Test_SetProperty()
        {
            IParameterDic pm = new ParameterDic();
            testClass obj = new testClass() { val1 = 1, val2 = 2 };
            Assert.IsTrue(runCommand(
                ScriptCommands.Assign("{obj}", obj,
                ScriptCommands.SetProperty("{obj}", "val1", "{obj.val2}",
                ScriptCommands.Assign("{obj.val2}", 1))), pm));
            Assert.AreEqual(2, pm.Get<int>("{obj.val1}"));
            Assert.AreEqual(1, pm.Get<int>("{obj.val2}"));
        }

        [TestMethod]
        public void Test_GetArrayItem()
        {
            IParameterDic pm = new ParameterDic();
            int[] array = new[] { 1, 3, 5 };
            Assert.IsTrue(runCommand(
                ScriptCommands.Assign("{array}", array,
                ScriptCommands.GetArrayItem("{array}", 1, "{item1}",
                ScriptCommands.Reassign("{array[1]}", null, "{item2}"))), pm));
            Assert.AreEqual(3, pm.Get("{item1}"));
            Assert.AreEqual(3, pm.Get("{item2}"));
        }

        [TestMethod]
        public void Test_ExecuteFunc()
        {
            IParameterDic pm = new ParameterDic();
            Assert.IsTrue(runCommand(
                      ScriptCommands.Assign("{utcNow}", DateTime.UtcNow,
                      ScriptCommands.ExecuteFunc("{utcNow}", "AddDays",
                      new object[] { 10 }, "{utcNow10}")), pm));
            Assert.AreNotEqual(pm.Get("{utcNow.Day}"), pm.Get("{utcNow10.Day}"));
        }

        [TestMethod]
        public void Test_SubString()
        {
            IParameterDic pm = new ParameterDic();
            string str = "onceuponatime";
            Assert.IsTrue(runCommand(
               ScriptCommands.Assign("{str}", str,
               ScriptCommands.Substring("{str}", 4, "{outputStr}",
               ScriptCommands.Substring("{str}", 4, 4, "{outputStr2}"))), pm));
            Assert.AreEqual("uponatime", pm.Get("{outputStr}"));
            Assert.AreEqual("upon", pm.Get("{outputStr2}"));
        }

        [TestMethod]
        public void Test_ArithmeticCommands()
        {
            IParameterDic pm = new ParameterDic();
            int val1 = 1; int val2 = 2;
            //Assert.IsTrue(runCommand(
            //    ScriptCommands.Assign(val1,
            //        ScriptCommands.Assign(val2,
            //          ScriptCommands.Add("{val1}", "{val2}", "{outputVal}"))), pm));
            Assert.IsTrue(runCommand(
              ScriptCommands.AssignMulti(
                    ScriptCommands.Add("{val1}", "{val2}", "{outputVal}"),
                    () => val1, 
                    () => val2), pm));
            Assert.AreEqual(3, pm.Get("{outputVal}"));
        }

    }
}
