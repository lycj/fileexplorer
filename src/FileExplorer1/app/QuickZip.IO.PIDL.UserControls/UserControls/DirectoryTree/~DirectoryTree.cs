using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QuickZip.IO.COFE.UserControls
{
    public class DirectoryTree : TreeView
    {
        static DirectoryTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryTree),
                new FrameworkPropertyMetadata(typeof(DirectoryTree)));
        }

        public static readonly DependencyProperty RootDirectoryProperty =
            DependencyProperty.Register("RootDirectory", typeof(IDirectoryInfoExA), typeof(DirectoryTree),
            new FrameworkPropertyMetadata(DirectoryInfoExA.DesktopDirectory));

        public IDirectoryInfoExA RootDirectory
        {
            get { return (IDirectoryInfoExA)GetValue(RootDirectoryProperty); }
            set { SetValue(RootDirectoryProperty, value); }
        }
    }
}
