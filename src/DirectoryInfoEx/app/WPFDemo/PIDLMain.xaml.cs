using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Tools;
using System.Diagnostics;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool firstOption = true;
        bool secondOption = false;

        public void handleCommand(string cmd)
        {
            if (cmd != null)
                switch (cmd)
                {
                    case @"Tools\Add": firstOption = !firstOption; break;
                    case @"Tools\Remove": secondOption = !secondOption; break;
                    default: MessageBox.Show(cmd); break;
                }
        }

        public Window1()
        {            
            InitializeComponent();           
            Debug.Write("tv.DataContext=");
            Debug.WriteLine(tv.DataContext);
            ContextMenuWrapper cmw = new ContextMenuWrapper();
            cmw.OnQueryMenuItems += (QueryMenuItemsEventHandler)delegate(object s, QueryMenuItemsEventArgs args)
            {
                string firstCmd = @"Tools\&Add" + (firstOption ? "[*]" : "");
                string secondCmd = @"Tools\Remove" + (secondOption ? "[*]" : "");

                args.ExtraMenuItems = new string[] { firstCmd, @"Tools\---", secondCmd, "Again", "---" };
                args.GrayedItems = new string[] { "delete", "rename", "cut", "copy" };
                args.HiddenItems = new string[] { "link" };
                args.DefaultItem = 1;
            };
            cmw.OnMouseHover += (MouseHoverEventHandler)delegate(object s, MouseHoverEventArgs args)
            {
                sbar1.Text = args.Info == "" ? args.Command : args.Info;
            };
            

            lv.PreviewMouseDown += (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
            {
                //Double Click
                if (args.LeftButton == MouseButtonState.Pressed && args.ClickCount == 2)
                {
                    FileInfoEx selected = lv.SelectedValue as FileInfoEx;
                    if (selected != null)
                        Process.Start(selected.FullName);
                    args.Handled = true;
                }
            };

            lv.MouseUp += (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
            {
                if (args.ChangedButton == MouseButton.Right)
                {
                    List<FileSystemInfoEx> files = new List<FileSystemInfoEx>();
                    foreach (object item in lv.SelectedItems)
                        files.Add((FileSystemInfoEx)item);

                    //FileSystemInfoEx file = lv.SelectedItem as FileSystemInfoEx;
                    if (files.Count > 0)
                    {
                        Point pt = this.PointToScreen(args.GetPosition(null));
                        handleCommand(cmw.Popup( files.ToArray() , new System.Drawing.Point((int)pt.X, (int)pt.Y)));
                        
                    }
                }
            };

            tv.MouseUp += (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs args)
            {
                if (args.ChangedButton == MouseButton.Right)
                {
                    DirectoryInfoEx dir = tv.SelectedItem as DirectoryInfoEx;
                    
                    if (dir != null)
                    {
                        Point pt = this.PointToScreen(args.GetPosition(null));
                        handleCommand(cmw.Popup(new FileSystemInfoEx[] { dir }, new System.Drawing.Point((int)pt.X, (int)pt.Y)));                        
                    }
                }
            };
        }
    }
}
