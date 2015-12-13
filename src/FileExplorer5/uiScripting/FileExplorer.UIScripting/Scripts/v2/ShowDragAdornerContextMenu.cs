using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
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
        public static IScriptCommand ShowDragAdornerContextMenu(string adornerVariable = "{DragDropAdorner}",
            string supportedEffectsVariable = "{SupportedEffects}",
            string defaultEffectVariable = "{DefaultEffect}",
            string resultEffectVariable = "{ResultEffect}",
            IScriptCommand onCloseCommand = null, IScriptCommand nextCommand = null)
        {
            return new ShowDragAdornerContextMenu()
            {
                AdornerKey = adornerVariable,
                SupportedEffectsKey = supportedEffectsVariable,
                DefaultEffectKey = defaultEffectVariable,
                ResultEffectKey = resultEffectVariable,
                OnCloseCommand = (ScriptCommandBase)onCloseCommand,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    public class ShowDragAdornerContextMenu : ScriptCommandBase
    {
        /// <summary>
        /// Point to where to store the DragAdorner, default={DragDropAdorner}.
        /// </summary>
        public string AdornerKey { get; set; }

        public ScriptCommandBase OnCloseCommand { get; set; }

        /// <summary>
        /// Point to options to show (Copy Link etc).
        /// </summary>
        public string SupportedEffectsKey { get; set; }

        /// <summary>
        /// Point to the default option (bold).
        /// </summary>
        public string DefaultEffectKey { get; set; }

        /// <summary>
        /// Store selected option here.
        /// </summary>
        public string ResultEffectKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ShowDragAdornerContextMenu>();

        public ShowDragAdornerContextMenu()
            : base("ShowDragAdornerContextMenu")
        {
            AdornerKey = "{DragDropAdorner}";
            SupportedEffectsKey = "{SupportedEffects}";
            DefaultEffectKey = "{DefaultEffect}";
            ResultEffectKey = "{ResultEffect}";
            OnCloseCommand = ResultCommand.NoError;
        }



        public override IScriptCommand Execute(ParameterDic pm)
        {
            DragAdorner dragAdorner = pm.GetValue<DragAdorner>(AdornerKey);
            DragDropEffectsEx supportedEffects = pm.GetValue<DragDropEffectsEx>(SupportedEffectsKey, DragDropEffectsEx.All);
            DragDropEffectsEx defaultEffect = pm.GetValue<DragDropEffectsEx>(DefaultEffectKey, DragDropEffectsEx.Copy);

            RoutedEventHandler ContextMenu_Closed = null;
            ContextMenu_Closed = (o, e) =>
                {
                    dragAdorner.ContextMenu.RemoveHandler(ContextMenu.ClosedEvent, ContextMenu_Closed);
                    pm.SetValue(ResultEffectKey, dragAdorner.DragDropEffect);
                    ScriptRunner.RunScriptAsync(pm, OnCloseCommand);
                };


            dragAdorner.SetSupportedDragDropEffects(supportedEffects, defaultEffect);
            dragAdorner.ContextMenu.AddHandler(ContextMenu.ClosedEvent, ContextMenu_Closed);
            dragAdorner.ContextMenu.IsOpen = true;

            return NextCommand;
        }
    }
}
