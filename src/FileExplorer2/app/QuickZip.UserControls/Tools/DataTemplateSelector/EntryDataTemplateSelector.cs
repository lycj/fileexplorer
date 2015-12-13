using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QuickZip.UserControls
{
    public class EntryDataTemplateSelector : DataTemplateSelector
    {        
            private string _templateType = "";
            private int _mode = 2;

            public string TemplateType
            {
                get { return _templateType; }
                set { _templateType = value; }
            }

            public int Mode
            {
                get { return _mode; }
                set { _mode = value; }
            }

            public EntryDataTemplateSelector()
            {
            }

            private DataTemplate mode2(object item, DependencyObject container)
            {
                switch (item.ToString()[0])
                {
                    case 'F':
                        return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
                    case 'R':
                    case 'D':
                        return (container as FrameworkElement).FindResource(TemplateType + "DirectoryTemplate") as DataTemplate;
                    default:
                        return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
                }

            }

            private DataTemplate mode3(object item, DependencyObject container)
            {
                switch (item.ToString()[0])
                {
                    case 'F':
                        return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
                    case 'R':
                        return (container as FrameworkElement).FindResource(TemplateType + "DriveTemplate") as DataTemplate;
                    case 'D':
                        return (container as FrameworkElement).FindResource(TemplateType + "DirectoryTemplate") as DataTemplate;
                    default:
                        return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
                }
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                switch (Mode)
                {
                    case 2: return mode2(item, container);
                    case 3: return mode3(item, container);
                }
                return null;
            }
        }
   
}
