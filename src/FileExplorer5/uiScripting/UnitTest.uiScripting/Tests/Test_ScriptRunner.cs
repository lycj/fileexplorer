using FileExplorer.Scripting;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.uiScripting
{
    [TestClass]
    public class Test_ScriptRunner
    {
        [TestMethod]
        public void Test_ParameterDic_Not_Altered()
        {
            IParameterDic pm = new ParameterDic();
            pm.Set("{var1}", 1);
            ScriptRunner.RunScript(pm, false, //Not Cloned
                ScriptCommands.Assign("{var1}", 2));
            Assert.AreEqual(2, pm.Get<int>("{var1}", -1));

            pm.Set("{var1}", 1);
            ScriptRunner.RunScript(pm, true, //Cloned
                ScriptCommands.Assign("{var1}", 2));
            Assert.AreEqual(1, pm.Get<int>("{var1}", -1));
        }

        [TestMethod]
        public void Test_RunScript_Return_Result()
        {
            int var1 = ScriptRunner.RunScript<int>("{var1}",
                null, ScriptCommands.Assign("{var1}", 1));
            Assert.AreEqual(1, var1);
        }
    }
}
