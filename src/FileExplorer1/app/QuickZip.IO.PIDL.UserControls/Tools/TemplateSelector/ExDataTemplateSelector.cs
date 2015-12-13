using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using QuickZip.IO.PIDL.UserControls.Model;
using QuickZip.IO.PIDL.UserControls.ViewModel;

namespace QuickZip.IO.PIDL.UserControls
{
    //0.2
    public class ExDataTemplateSelector : DataTemplateSelector
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

        public ExDataTemplateSelector()
        {
        }

        private DataTemplate mode2(object item, DependencyObject container)
        {
            if (item is DirectoryInfoEx || item is DirectoryModel ||
                (item is ExViewModel && (item as ExViewModel).EmbeddedModel is DirectoryModel))
                return (container as FrameworkElement).FindResource(TemplateType + "DirectoryTemplate") as DataTemplate;
            else
                return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
        }

        private DataTemplate mode3(object item, DependencyObject container)
        {
            if (item is DriveModel ||
                (item is ExViewModel && (item as ExViewModel).EmbeddedModel is DriveModel))
                return (container as FrameworkElement).FindResource(TemplateType + "DriveTemplate") as DataTemplate;
            else
                if (item is DirectoryInfoEx || item is DirectoryModel ||
                    (item is ExViewModel && (item as ExViewModel).EmbeddedModel is DirectoryModel))
                    return (container as FrameworkElement).FindResource(TemplateType + "DirectoryTemplate") as DataTemplate;
                else
                    return (container as FrameworkElement).FindResource(TemplateType + "FileTemplate") as DataTemplate;
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
