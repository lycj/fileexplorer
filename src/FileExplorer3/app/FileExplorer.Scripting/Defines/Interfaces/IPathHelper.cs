using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public interface IPathHelper
    {
        string Combine(string path1, params string[] paths);
        string GetDirectoryName(string path);
        string GetFileName(string path);
        string GetExtension(string path);
        char Separator { get; }
    }
}
