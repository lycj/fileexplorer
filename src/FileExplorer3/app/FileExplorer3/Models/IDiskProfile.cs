using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.IO
{
    public interface IDiskProfile : IProfile
    {
        IDiskIOHelper DiskIO { get; }
    }
}
