using FileExplorer;
using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand AttachDragDropAdorner(string adornerLayerVariable = "{AdornerLayer}", string adornerVariable = "{DragDropAdorner}",
            IScriptCommand nextCommand = null)
        {
            return AssignAdornerLayer("PART_DragDropAdorner", adornerLayerVariable, false,
                  new AssignDragDropAdorner()
                  {
                      AdornerKey = adornerVariable,
                      AdornerLayerKey = adornerLayerVariable,
                      NextCommand = (ScriptCommandBase)HubScriptCommands.AttachAdorner(adornerLayerVariable, adornerVariable, nextCommand),
                  });

        }

        public static IScriptCommand DetachDragDropAdorner(string adornerLayerVariable = "{AdornerLayer}", string adornerVariable = "{DragDropAdorner}",
            IScriptCommand nextCommand = null)
        {
            return ScriptCommands.IfAssigned(adornerVariable,
                    AssignAdornerLayer("PART_DragDropAdorner", adornerLayerVariable, false,
                    HubScriptCommands.DetachAdorner(adornerLayerVariable, adornerVariable)));
        }
    }

    public class AssignDragDropAdorner : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {
        /// <summary>
        /// Point to where to store the DragDropAdorner, default={DragDropAdorner}.
        /// </summary>
        public string AdornerKey { get; set; }

        /// <summary>
        /// Point to the adorner layer to attach/detach adorner, Default = {AdornerLayer}
        /// </summary>
        public string AdornerLayerKey { get; set; }

        public AssignDragDropAdorner()
            : base("AssignDragDropAdorner")
        {
            AdornerKey = "{DragDropAdorner}";
            AdornerLayerKey = "{AdornerLayer}";
        }


        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var adornerLayer = pm.GetValue<AdornerLayer>(AdornerLayerKey);

            DragAdorner adorner = new DragAdorner(adornerLayer)
            {
                IsHitTestVisible = false,
                DraggingItemTemplate = UIEventHubProperties.GetDragItemTemplate(sender)
            };
            pm.SetValue(AdornerKey, adorner);
            return NextCommand;
        }


    }



}
