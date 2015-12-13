using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand AssignAdornerLayer(AdornerType assignAdornerType, 
            string destinationVariable = "{AdornerLayer}", bool skipIfExists = false, 
            IScriptCommand foundCommand = null, IScriptCommand notFoundCommand = null)
        {
            return new AssignAdornerLayer()
            {
                AssignAdornerType = assignAdornerType,
                DestinationKey = destinationVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)foundCommand,
                FailCommand = (ScriptCommandBase)notFoundCommand
            };
        }

        public static IScriptCommand AssignAdornerLayer(string adornerName, 
           string destinationVariable = "{AdornerLayer}", bool skipIfExists = false,
           IScriptCommand foundCommand = null, IScriptCommand notFoundCommand = null)
        {
            return new AssignAdornerLayer()
            {
                AssignAdornerType = AdornerType.Named,
                AdornerName = adornerName,
                DestinationKey = destinationVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)foundCommand,
                FailCommand = (ScriptCommandBase)notFoundCommand
            };
        }
    }


    public enum AdornerType
    {
        ItemsControl,
        /// <summary>
        /// Find the first ISelectable item that's ISelectable.IsSelected, and return it's adorner.
        /// </summary>
        SelectedItem, 
        /// <summary>
        /// Find a AdornerDecorator named 
        /// </summary>
        Named
    }

    /// <summary>
    /// Serializable, Get adorner layer from {Sender} and store to AdornerLayerKey.
    /// </summary>
    public class AssignAdornerLayer : UIScriptCommandBase
    {
        /// <summary>
        /// Adorner desired.
        /// </summary>
        public AdornerType AssignAdornerType { get; set; }

        /// <summary>
        /// Point to where to store the found adorner layer. Default = {AdornerLayer}
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// If AssignAdornerType = Named, assign the adorner name.
        /// </summary>
        public string AdornerName { get; set; }

        /// <summary>
        /// Run if adorner layer is not found.
        /// </summary>
        public ScriptCommandBase FailCommand { get; set; }

        public bool SkipIfExists { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<AssignAdornerLayer>();

        public AssignAdornerLayer()
            : base("AssignAdornerLayer")
        {
            AssignAdornerType = AdornerType.ItemsControl;
            DestinationKey = "{AdornerLayer}";
            FailCommand = null;
            SkipIfExists = false;
        }


        public override IScriptCommand Execute(ParameterDic pm)
        {
            AdornerLayer value = null;
            FrameworkElement sender = pm.GetValue<FrameworkElement>(SenderKey);
            if (sender == null)
                return ResultCommand.Error(new KeyNotFoundException(SenderKey));
            switch (AssignAdornerType)
            {
                case AdornerType.Named:
                    Window parentWindow = Window.GetWindow(sender);
                    AdornerDecorator decorator = UITools.FindVisualChildByName<AdornerDecorator>(parentWindow, "PART_DragDropAdorner");
                    value = AdornerLayer.GetAdornerLayer(sender);
                    break;
                case AdornerType.ItemsControl:
                    value = AdornerLayer.GetAdornerLayer(sender);
                    break;
                case AdornerType.SelectedItem:
                    if (!(sender is ItemsControl))
                        return ResultCommand.Error(new ArgumentException(SenderKey));
                    ItemsControl ic = sender as ItemsControl;
                    foreach (var item in ic.ItemsSource)
                        if (item is ISelectable && (item as ISelectable).IsSelected)
                        {
                            UIElement selectedItem = ic.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                            value = UITools.FindVisualChild<AdornerLayer>(selectedItem);

                        }
                    break;
            }

            if (value != null)
                return ScriptCommands.Assign(DestinationKey,
                     value, SkipIfExists, NextCommand);            
            return FailCommand;
        }



    }
}
