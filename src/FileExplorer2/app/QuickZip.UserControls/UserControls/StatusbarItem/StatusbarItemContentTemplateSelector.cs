using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QuickZip.UserControls
{
    public class StatusbarItemContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            StatusbarItem element = UITools.FindAncestor<StatusbarItem>(container);

            if (element != null)
            {
                switch (element.ItemType)
                {
                    case ItemType.itString : return element.FindResource("stringTemplate") as DataTemplate;

                }               
            }

            return null;
        }
    }
}

