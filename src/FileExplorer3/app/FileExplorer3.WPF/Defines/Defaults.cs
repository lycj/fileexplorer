using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.Defines
{
    public static class Defaults
    {
        public static int MaximumFlickTime = 350; //milliseconds.
        public static int MinimumFlickThreshold = 150; //pixels
        //public static int MinimumDragDistance = 20;
        //Touch click within the interval and distance is considered clicking in same spot (e.g. ClickCount++)
        public static int MaximumClickInterval = 1000;
        public static Point MaximumTouchClickDragDistance = new Point(20, 20);

        //To init drag, user must first hold touch inside the Threshold
        public static Point MaximumTouchDragThreshold = new Point(10, 10);
        public static int MaximumTouchHoldInterval = 500;
    }
}
