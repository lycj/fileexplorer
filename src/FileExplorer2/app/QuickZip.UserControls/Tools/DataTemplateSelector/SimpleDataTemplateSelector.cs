using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QuickZip.UserControls
{
    /// <summary>
    /// DataTemplate is returned based on the following syntax
    /// PREFIX_ITEM_Template
    /// PREFIX is defined by the template selector.
    /// ITEM is 
    /// </summary>
    public class SimpleDataTemplateSelector : DataTemplateSelector
    {
        private string _templateType = "";
        private int _mode = 2;

        /// <summary>
        /// Prefix of the template key, e.g. XYZ in XYZ_ABCTemplate, 
        /// </summary>
        public string Prefix
        {
            get { return _templateType; }
            set { _templateType = value; }
        }

        public SimpleDataTemplateSelector()
        {
        }
      
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return (container as FrameworkElement).FindResource(Prefix + "_" + item.ToString() + "_Template") as DataTemplate;           
        }
    }

}
