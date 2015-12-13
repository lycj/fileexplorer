using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = new System.IO.DirectoryInfoEx(@"C:\Temp\COFE2");
            foreach (var item in dir.EnumerateFileSystemInfos("*", System.IO.SearchOption.TopDirectoryOnly))
                using (item)
                    Console.WriteLine(item.Label);
            //Console.WriteLine(dir.EnumerateFileSystemInfos("*", System.IO.SearchOption.AllDirectories).Count());
            Console.WriteLine(dir.RefreshMode);
            Console.WriteLine(dir.LastAccessTime);
            Console.WriteLine(dir.RefreshMode);
            Console.ReadLine();
        }
    }
}
