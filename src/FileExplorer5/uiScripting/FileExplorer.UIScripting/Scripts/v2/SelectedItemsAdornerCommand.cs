using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
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
        public static IScriptCommand AttachSelectedItemsAdorner(string SelectedItemsAdornerVariable = "{SelectedItemsAdorner}",
            IScriptCommand nextCommand = null)
        {
            return new SelectedItemsAdornerCommand()
            {
                AdornerMode = UIEventHub.AdornerMode.Attach,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DettachSelectedItemsAdorner(string SelectedItemsAdornerVariable = "{SelectedItemsAdorner}",
            IScriptCommand nextCommand = null)
        {
            return new SelectedItemsAdornerCommand()
            {
                AdornerMode = UIEventHub.AdornerMode.Detach,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand UpdateSelectedItemsAdorner(string SelectedItemsAdornerVariable = "{SelectedItemsAdorner}",
            IScriptCommand nextCommand = null)
        {
            return new SelectedItemsAdornerCommand()
            {
                AdornerMode = UIEventHub.AdornerMode.Update,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class SelectedItemsAdornerCommand : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        public AdornerMode AdornerMode { get; set; }

        /// <summary>
        /// If attach, the selection adorner (of type SelectionAdorner) will be set to the key, 
        /// If update, 
        /// If detach, the SelectionAdorner will be point to null.
        /// Default = "{SelectionAdorner}".
        /// </summary>
        public string SelectedItemsAdornerKey { get; set; }



        /// <summary>
        /// Current position relative to sender, adjusted with scrollbar position.
        /// </summary>
        public string CurrentPositionAdjustedKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SelectedItemsAdornerCommand>();

        public SelectedItemsAdornerCommand()
            : base("SelectedItemsAdornerCommand")
        {
            SelectedItemsAdornerKey = "{SelectedItemsAdorner}";
            CurrentPositionAdjustedKey = "{CurrentPositionAdjusted}";            
        }

        protected override Script.IScriptCommand executeInner(ParameterDic pm, ItemsControl ic,
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(ic);
            if (scp != null)
            {
                AdornerLayer adornerLayer = ControlUtils.GetAdornerLayer(ic);

                switch (AdornerMode)
                {
                    case UIEventHub.AdornerMode.Attach:
                        if (adornerLayer == null)
                            return ResultCommand.Error(new Exception("Adorner layer not found."));
                        if (UIEventHubProperties.GetSelectedItemsAdorner(scp) == null)
                        {

                            //Create and register adorner.
                            SelectedItemsAdorner adorner = new SelectedItemsAdorner(scp);
                            pm.SetValue(SelectedItemsAdornerKey, adorner);
                            UIEventHubProperties.SetSelectedItemsAdorner(scp, adorner);
                            UIEventHubProperties.SetLastScrollContentPresenter(ic, scp); //For used when detach.

                            adornerLayer.Add(adorner);
                        }
                        break;

                    case UIEventHub.AdornerMode.Detach:

                        var lastScp = UIEventHubProperties.GetLastScrollContentPresenter(ic); //For used when detach.
                        var lastAdorner = UIEventHubProperties.GetSelectedItemsAdorner(scp);
                        if (lastAdorner != null)
                            adornerLayer.Remove(lastAdorner);

                        UIEventHubProperties.SetLastScrollContentPresenter(ic, null);
                        UIEventHubProperties.SetSelectedItemsAdorner(scp, null);
                        pm.SetValue<Object>(SelectedItemsAdornerKey, null);
                        break;

                    case UIEventHub.AdornerMode.Update:
                        var updateAdorner = pm.GetValue<SelectedItemsAdorner>(SelectedItemsAdornerKey) ??
                            UIEventHubProperties.GetSelectedItemsAdorner(scp);

                        if (updateAdorner == null)
                            return ResultCommand.Error(new Exception("Adorner not found."));

                        Point current = pm.GetValue<Point>(CurrentPositionAdjustedKey);

                        if (updateAdorner.CurrentPosition.X == -1 && updateAdorner.CurrentPosition.Y == -1)
                        {                            
                            //If the adorner is not initialized.
                            updateAdorner.SetValue(SelectedItemsAdorner.CurrentPositionProperty, current);
                            updateAdorner.SetValue(SelectedItemsAdorner.ItemsProperty, ic.ItemsSource);
                        }
                        else updateAdorner.SetValue(SelectedItemsAdorner.CurrentPositionProperty, current);
                        break;

                }



            }
            return NextCommand;
        }


    }
}
