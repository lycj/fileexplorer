using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_ShellDragDemo
{
    public class FileModel : NotifyPropertyChanged
    {
        public FileModel(string fileName)
        {
            FileName = fileName;            
        }

        public string FileName { get; set; }
        

        public override bool Equals(object obj)
        {
            return obj is FileModel && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }

        public override string ToString()
        {
            return "FVM - " + FileName;
        }
    }
}
