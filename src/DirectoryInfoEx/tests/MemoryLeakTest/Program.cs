using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShellDll;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = DirectoryInfoEx.FromString(@"C:\Users\lycj\Downloads") as DirectoryInfoEx;
                        
            var runCount = 0;
            int iteration = 1;
            string line = Console.ReadLine();
            while (line == "" || Int32.TryParse(line, out runCount))            
            {
                if (line == "") runCount = 1;
                for (int i = 0; i < runCount; i++)
                {
                    var count = d.EnumerateFileSystemInfos().Count();
                    Console.WriteLine(String.Format("{0} - {1} listed", iteration++, count));
                }
                GC.Collect(0, GCCollectionMode.Forced);
                Console.WriteLine(
                    String.Format("AllocatedCount - PIDL={0} DirEx={1}", PIDL.Counter, FileSystemInfoEx.counter));
                line = Console.ReadLine();
            }


        }
    }
}
