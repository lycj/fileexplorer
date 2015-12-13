using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using FileExplorer.Scripting;
using FileExplorer.Utils;
using FileExplorer;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UnitTest.uiScripting
{
    [TestClass]
    public class Test_ParameterDic
    {
        [TestMethod]
        public void Test_Basic_Get_And_Set_Operation()
        {
            IParameterDic dic = new ParameterDic();
            int totalItems1, totalItems2;
            string property1Value;
            int property2Value;

            dic.Set("{Property1}", "Property1");
            dic.Set("{Property2}", 2);
            totalItems1 = dic.List().Count();

            property1Value = dic.Get("{Property1}") as string;
            property2Value = dic.Get<int>("{Property2}");
            dic.Remove("{Property2}");
            totalItems2 = dic.List().Count();

            Assert.AreEqual(2, totalItems1);
            Assert.AreEqual(1, totalItems2);
            Assert.AreEqual("Property1", property1Value);
            Assert.AreEqual(2, property2Value);
        }

        [TestMethod]
        public void Test_Add_Int_And_String()
        {
            IParameterDic dic = new ParameterDic();
            int property1Value = -1;
            string property2Value;

            dic.Set("{Property1}", 10);
            dic.Set("{Property2}", "Once");

            dic.Add("{Property1}", 1, 2, 3);
            dic.Add("{Property2}", "upon", "a", "time");

            property1Value = dic.Get("{Property1}", -1);
            property2Value = dic.Get("{Property2}", "");

            Assert.AreEqual(16, property1Value);
            Assert.AreEqual("Onceuponatime", property2Value);
        }


        [TestMethod]
        public void Test_MultiLevel_Variable_Names()
        {
            IParameterDic dic = new ParameterDic();
            IParameterDic subDic1, subDic2;
            int dictionaryCount, subDic1Count, subDic2Count, 
                property1Value, property3Value, property5Value;

            dic.Set("{SubDic1.Property1}", 1);
            dic.Set("{SubDic1.SubDic2.Property3}", 3);
            dic.Set("{SubDic1.SubDic2.Property5}", 3);
            dic.Add("{SubDic1.SubDic2.Property5}", 2);

            subDic1 = dic.Get<ParameterDic>("{SubDic1}");
            property1Value = dic.Get("{SubDic1.Property1}", -1);
            subDic2 = dic.Get<ParameterDic>("{SubDic1.SubDic2}");
            property3Value = dic.Get("{SubDic1.SubDic2.Property3}", -1);
            property5Value = dic.Get("{SubDic1.SubDic2.Property5}", -1);

            dictionaryCount = dic.List().Count();
            subDic1Count = subDic1.List().Count();
            subDic2Count = subDic2.List().Count();

            Assert.AreEqual(1, dictionaryCount, "dictionaryCount");
            Assert.IsNotNull(subDic1, "subDic1");
            Assert.IsNotNull(subDic2, "subDic2");
            Assert.AreEqual(2, subDic1Count, "subDic1Count");
            Assert.AreEqual(2, subDic2Count, "subDic2Count");

            Assert.AreEqual(1, property1Value, "property1Value");
            Assert.AreEqual(3, property3Value, "property3Value");
            Assert.AreEqual(5, property5Value, "property5Value");



        }


    }
}