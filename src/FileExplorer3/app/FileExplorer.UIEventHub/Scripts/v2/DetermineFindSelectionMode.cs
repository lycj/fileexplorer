using FileExplorer.Defines;
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

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand DetermineFindSelectionMode(string findSelectionModeVariable = "{FindSelectionMode}", 
            IScriptCommand nextCommand = null)
        {
            return new DetermineFindSelectionMode()
            {
                FindSelectionModeKey = findSelectionModeVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

    }


    /// <summary>
    /// Check sender (which is ItemsControl), and find the best way to FindSelection.
    /// </summary>
    public class DetermineFindSelectionMode : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        /// <summary>
        /// Point to output of find selection mode (FindSelectionMode), Default = "{FindSelectionMode}"
        /// </summary>
        public string FindSelectionModeKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<DetermineFindSelectionMode>();

        public DetermineFindSelectionMode() : base("DetermineFindSelectionMode")
        {
            FindSelectionModeKey = "{FindSelectionMode}";
        }

        protected override Script.IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {

            var scp = ControlUtils.GetScrollContentPresenter(ic);
            bool isSelecting = UIEventHubProperties.GetIsSelecting(ic);
            
            FindSelectionMode fsMode = FindSelectionMode.HitTest;

            IChildInfo icInfo = UITools.FindVisualChild<Panel>(scp) as IChildInfo;
            if (icInfo != null)
                fsMode = FindSelectionMode.IChildInfo;
            else
                if (ic is ListView && (ic as ListView).View is GridView)
                    fsMode = FindSelectionMode.GridView;

            logger.Debug(String.Format("SelectionMode = {0}", fsMode));
            pm.SetValue(FindSelectionModeKey, fsMode, false);

            return NextCommand;
        }
    }
}
