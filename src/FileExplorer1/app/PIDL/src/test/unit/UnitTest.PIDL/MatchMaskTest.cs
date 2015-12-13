using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

namespace Test.COFE
{
    [TestFixture]
    public class MatchMaskTest
    {

        [Test]
        public void Test_Slash_Check()
        {


            Action<string, string, bool> checkMask = (p, m, b) =>
                {
                    if (b)
                        Assert.IsTrue(IOTools.MatchFileMask(p, m, true), string.Format("error : {0} does not match {1}", p, m));
                    else Assert.IsFalse(IOTools.MatchFileMask(p, m, true), string.Format("error : {0} does match {1}", p, m));
                };

            checkMask("abc\\def", "abc\\*", true);
            checkMask("abc\\def\\ghi", "abc\\*", false);
            checkMask("abc\\def\\ghi\\jkl", "abc\\(#<folder>**)\\jkl", true);
            checkMask("abc\\jkl", "abc\\**\\jkl", false);

        }

        [Test]
        public void Test_Variable_Check()
        {

            Action<string, string> checkNotMatch = (p, m) =>
            {
                Match match;
                Assert.IsFalse(IOTools.MatchFileMask(p, m, true, out match), string.Format("error : {0} does not match {1}", p, m));
                Assert.IsFalse(match.Success);
            };

            Action<string, string, string, string> checkMatch = (p, m, n, v) =>
            {
                Match match;
                Assert.IsTrue(IOTools.MatchFileMask(p, m, true, out match), string.Format("error : {0} does not match {1}", p, m));
                Assert.AreEqual(v, match.Groups[n].Value, String.Format("error : {0} does not equal to {1}", n, v));

            };

            checkNotMatch("abc\\def\\ghi", "abc\\*");
            checkMatch("abc\\def\\jkl", "abc\\(#<folder>*)\\jkl", "folder", "def");
            checkMatch("abc\\def\\ghi\\jkl", "abc\\(#<folder>**)\\jkl", "folder", "def\\ghi");


        }



    }
}
