using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF.BaseControls
{
    public class OverflowItem : ListViewItem
    {
        #region Constructor

        static OverflowItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverflowItem),
                new FrameworkPropertyMetadata(typeof(OverflowItem)));
        }

        #endregion

        #region Methods

        //protected override DependencyObject GetContainerForItemOverride()
        //{
        //    return new BreadcrumbTreeMenuItem();
        //}
        
        
        #endregion

        #region Data
        
        #endregion

        #region Public Properties
        
        #endregion
    }
}
