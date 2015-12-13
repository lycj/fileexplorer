using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using System.Windows.Media;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.UserControls
{
    public class DisplayTemplateSelector : DataTemplateSelector
    {
        //public static Func<DependencyObject, DisplayType> FromStatusbar = ((container) =>
        //    {
        //        var parentItem = UITools.FindAncestor<StatusbarItemEx>(container);
        //        if (parentItem != null)
        //            return (DisplayType)parentItem.GetValue(StatusbarItemEx.TypeProperty);
        //        else return DisplayType.Auto;
        //    });

        public static Func<DependencyObject, DisplayType> FromDisplayContentControl = ((container) =>
        {
            var parentItem = UITools.FindAncestor<DisplayContentControl>(container);
            if (parentItem != null)
                return (DisplayType)parentItem.GetValue(DisplayContentControl.TypeProperty);
            else return DisplayType.Auto;
        });

        private Func<DependencyObject, DisplayType> _displayTypeFunc;
        #region Cosntructor

        public DisplayTemplateSelector(Func<DependencyObject, DisplayType> displayTypeFunc)
        {
            _displayTypeFunc = displayTypeFunc ?? FromDisplayContentControl;
        }

        public DisplayTemplateSelector()
            : this(null)
        {

        }

        #endregion

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DisplayType type = _displayTypeFunc(container);
            DataTemplate retVal = null;

            if (type == DisplayType.Auto)
            {
                if (item is string && (item as string).Any(c => c.Equals('.') || c.Equals('\\'))) type = DisplayType.Filename;
                else if (item is Int16) type = DisplayType.Percent;
                else if (item is Int32 || item is Int64) type = DisplayType.Number;
                else if (item is Boolean) type = DisplayType.Boolean;
                else if (item is ImageSource) type = DisplayType.Image;
                else type = DisplayType.Text;
            }

            switch (type)
            {
                case DisplayType.Text: retVal = TextTemplate; break;
                case DisplayType.Number: retVal = NumberTemplate; break;
                case DisplayType.Link: retVal = LinkTemplate; break;
                case DisplayType.DateTime: retVal = DateTimeTemplate; break;
                case DisplayType.TimeElapsed: retVal = TimeElapsedTemplate; break;
                case DisplayType.Percent: retVal = PercentTemplate; break;
                case DisplayType.Kb: retVal = KbTemplate; break;
                case DisplayType.Filename: retVal = FilenameTemplate; break;
                case DisplayType.Boolean: retVal = BooleanTemplate; break;
                case DisplayType.Image: retVal = ImageTemplate; break;
            }

            return retVal ?? base.SelectTemplate(item, container);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public DataTemplate TextTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        public DataTemplate LinkTemplate { get; set; }
        public DataTemplate DateTimeTemplate { get; set; }
        public DataTemplate TimeElapsedTemplate { get; set; }
        public DataTemplate KbTemplate { get; set; }
        public DataTemplate PercentTemplate { get; set; }
        public DataTemplate FilenameTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

        #endregion
    }
}
