using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{

    public class UIEventAdapter : DependencyObject
    {
        //http://siderite.blogspot.com/2010/12/collection-attached-properties-with.html
        private static DependencyProperty ProcessorsProperty =
            DependencyProperty.RegisterAttached("ProcessorsInternal", typeof(FreezableCollection<UIEventProcessorBase>),
            typeof(UIEventAdapter),
            new PropertyMetadata(null));

        public static FreezableCollection<UIEventProcessorBase> GetProcessors(DependencyObject obj)
        {
            var retVal = (FreezableCollection<UIEventProcessorBase>)obj.GetValue(ProcessorsProperty);
            if (retVal == null)
            {
                retVal = new FreezableCollection<UIEventProcessorBase>();
                
                var ehub = new UIEventHub(new ScriptRunner(), obj as UIElement, true) 
                { EventProcessors = retVal };               
                SetUIEventHub(obj, ehub);
                SetProcessors(obj, retVal);                
            }
            return retVal;
        }

        public static void SetProcessors(DependencyObject obj, FreezableCollection<UIEventProcessorBase> value)
        {
            if (obj != null)
            {
                obj.SetValue(ProcessorsProperty, value);                
            }
        }


        public static DependencyProperty UIEventHubProperty =
            DependencyProperty.RegisterAttached("UIEventHub", typeof(IUIEventHub),
            typeof(UIEventAdapter),
            new PropertyMetadata(null));

        public static IUIEventHub GetUIEventHub(DependencyObject obj)
        {
            return (IUIEventHub)obj.GetValue(UIEventHubProperty);
        }

        public static void SetUIEventHub(DependencyObject obj, IUIEventHub value)
        {
            obj.SetValue(UIEventHubProperty, value);
        }
    }


}
