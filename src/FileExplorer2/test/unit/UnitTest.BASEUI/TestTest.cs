using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Test.COFE
{
    [TestFixture]
    public class TestTest
    {
       
        [Test]
        public void One_Plus_One_Equals_Two()
        {
            Console.WriteLine("One_Plus_One_Equals_Two");
            Assert.AreEqual(1 + 1, 2);
        }

        [Test]
        public void One_Concat_One_Equals_Eleven()
        {
            Console.WriteLine("One_Concat_One_Equals_Eleven");
            Assert.AreEqual("1" + "1", "11");
        }


    }
}
