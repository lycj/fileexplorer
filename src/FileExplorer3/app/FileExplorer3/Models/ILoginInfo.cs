using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class BrowserStatus
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }

    public interface ILoginInfo
    {
        bool CheckLogin(BrowserStatus status);
        string StartUrl { get; }
    }

}
