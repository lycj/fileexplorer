using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace QuickZip.UserControls
{
    public class Explorer2ContentSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string[] itemStr = item.ToString().Split(new char[] { ';' });
            DataTemplate retVal = null;
            if (itemStr.Length > 0)
                retVal = (container as ContentPresenter).TryFindResource(itemStr[0]) as DataTemplate;
            if (retVal != null)
                return retVal;

            return base.SelectTemplate(item, container);
        }
    }
}
