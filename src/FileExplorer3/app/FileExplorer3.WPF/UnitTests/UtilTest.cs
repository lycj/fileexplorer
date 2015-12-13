using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using NUnit.Framework;
using FileExplorer.WPF.ViewModels.Helpers;

namespace FileExplorer.WPF.UnitTests
{
    [TestFixture]
    public class UtilTest
    {
        public class C
        {
            public string Value { get { return "C"; } }
        }

        public class B
        {
            public B() { C = new C(); }
            public C C { get; set; }
            public string Value { get { return "B"; } }
        }

        public class A
        {
            public A() { B = new B(); }
            public B B { get; set; }
            public string Value { get { return "A"; } }
        }


        [Test]
        public static void PropertyPathHelper_Test()
        {
            var a = new A();

            object valueA = PropertyPathHelper.GetValueFromPropertyInfo(a, "Value");
            object valueB = PropertyPathHelper.GetValueFromPropertyInfo(a, "B.Value");
            object valueC = PropertyPathHelper.GetValueFromPropertyInfo(a, "B.C.Value");
            bool cachedA =
                PropertyPathHelper._cacheDic.ContainsKey(new Tuple<Type, string>(typeof(A), "Value"));
            bool cachedC =
                PropertyPathHelper._cacheDic.ContainsKey(new Tuple<Type, string>(typeof(C), "Value"));


            Assert.AreEqual("A", valueA);
            Assert.AreEqual("B", valueB);
            Assert.AreEqual("C", valueC);
            Assert.AreEqual("C", valueC);

            Assert.IsTrue(cachedA);
            Assert.IsTrue(cachedC);
        }

        public static void Properties_HelperTest()
        {
            dynamic ph = new PropertiesHelper<string>();
            dynamic ch = new CategoryHelper<string>();
            ph.abc = "cde";
            ch.abc.def = "ghi";
            ch.abc.jkl = "jkl";

            var cde = ph.abc;
            var ghi = ch.abc.def;
            var jkl = ch.abc.jkl;


            Assert.AreEqual("cde", cde);
            Assert.AreEqual("ghi", ghi);
        }

        public static void Test()
        {
            PropertyPathHelper_Test();
            Properties_HelperTest();
        }
    }
}
