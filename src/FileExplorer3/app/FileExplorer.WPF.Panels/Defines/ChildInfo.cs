using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF
{
    public class ChildInfo
    {
        public static ChildInfo Empty = new ChildInfo();

        public ChildInfo()
        {
        }

        public Rect? ArrangedRect { get; set; }
        public Size? DesiredSize { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, {1}", DesiredSize, ArrangedRect);
        }
    }
}
