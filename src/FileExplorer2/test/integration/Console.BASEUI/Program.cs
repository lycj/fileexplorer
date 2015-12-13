using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTest.COFE;

namespace BASEUI
{
    class Program
    {
        static void Main(string[] args)
        {
            new ToStringTest().CommandViewModel_Return_ToString_Correctly();
            Console.ReadLine();
        }
    }
}
