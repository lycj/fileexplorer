using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{

    public static partial class DragDropScriptCommands
    {
        /// <summary>
        /// Find ISelectable under cursor over element's datacontext, if it's ISelectable.IsSelected, 
        /// assign canDragKey to true, otherwise false.
        /// </summary>
        /// <param name="canDragKey"></param>
        /// <returns></returns>
        public static IScriptCommand IfItemUnderMouseIsSelected(IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return
                //Calculate a number of positions.
                HubScriptCommands.ObtainPointerPosition(
                //Assign the datacontext item of the UIelement that's undermouse to {ItemUnderMouse}
                HubScriptCommands.AssignItemUnderMouse("{ItemUnderMouse}", false,
                //And If it's exists and selected,                                                    
                ScriptCommands.IfAssigned("{ItemUnderMouse}",
                ScriptCommands.IfNotAssigned("{ItemUnderMouse.IsSelected}", trueCommand,
                    ScriptCommands.IfTrue("{ItemUnderMouse.IsSelected}", trueCommand,
                otherwiseCommand)), otherwiseCommand)));
        }

        public static IScriptCommand StartShellDrag(string iShellDragVariable = "{DragDrop.SupportDrag}", IScriptCommand nextCommand = null)
        {
            return
                //If CanDrag (ItemUnderMouse is selected), Check and set IsDraggingProperty to true.
                ScriptCommands.SetPropertyValue(iShellDragVariable, (ISupportDrag d) => d.IsDraggingFrom, true, 
                HubScriptCommands.AssignDragMethod(QueryDrag.DragMethodKey,
                
                //Initialize shell based drag drop
                HubScriptCommands.QueryDrag(iShellDragVariable, "{DragResult}",                
                //Reset IsDragging property.
                ScriptCommands.SetPropertyValue(iShellDragVariable, (ISupportDrag d) => d.IsDraggingFrom, false,
                //Reset IShellDrag variable.
                ScriptCommands.Assign(iShellDragVariable, null, false,
                nextCommand)))));
        }

        /// <summary>
        /// Update the DragAdorner's PointerPosition so selected Items will be shown at the spot.
        /// </summary>
        public static IScriptCommand UpdateAdornerPointerPosition(string adornerVariable = "{DragDrop.Adorner}",  IScriptCommand nextCommand = null)
        {
            string cursorPositionVariable = "{DragDrop.CursorPosition}";
            return HubScriptCommands.AssignCursorPosition(PositionRelativeToType.Window, cursorPositionVariable, false,
                    ScriptCommands.RunSequence(null, 
                        ScriptCommands.IfEquals(DragDropLiteCommand.DragDropDeviceKey, UIInputType.Touch,  
                            HubScriptCommands.OffsetPositionValue("{DragDrop.CursorPosition}", new Point(-100,-100), "{DragDrop.CursorPosition}")),
                        ScriptCommands.SetProperty(adornerVariable, (DragAdorner a) => a.PointerPosition, cursorPositionVariable, nextCommand)));
        }
        /// <summary>
        /// Update DragAdorner's Text to Move xyz.txt to Dest, syntax =
        /// [queryDropResult.supported/PreferredEffects] ([draggables.length] Items/draggables[0].DisplayName) to [isd.DisplayName]
        /// </summary>
        public static IScriptCommand UpdateAdornerText(string adornerVariable = "{DragDrop.Adorner}",
            string dragMethodVariable = "{DragDrop.DragMethod}",
            string draggablesVariable = "{DragDrop.Draggables}",
            string queryDropResultVariable = "{DragDrop.QueryDropResult}",
            string ISupportDropVariable = "{ISupportDrop}", IScriptCommand nextCommand = null)
        {
            string adornerTextVariable = ParameterDic.CombineVariable(adornerVariable, ".Text", false);
            string queryDropSupportedEffectsVariable = ParameterDic.CombineVariable(queryDropResultVariable, ".SupportedEffects", false);
            string queryDropPreferredEffectVariable = ParameterDic.CombineVariable(queryDropResultVariable, ".PreferredEffect", false);
            string draggablesLengthVariable = ParameterDic.CombineVariable(draggablesVariable, ".Count()", false);
            string draggablesFirstLabelVariable = ParameterDic.CombineVariable(draggablesVariable, ".First().DisplayName", false);
            string iSupportDropLabelVariable = ParameterDic.CombineVariable(ISupportDropVariable, ".DropTargetLabel", false);
            string iSupportDropDisplayNameVariable = ParameterDic.CombineVariable(ISupportDropVariable, ".DisplayName", false);

            return
                ScriptCommands.RunSequence(nextCommand,
                   ScriptCommands.IfEquals(draggablesLengthVariable, 1,
                       ScriptCommands.FormatText("{ItemLabel}", draggablesFirstLabelVariable),
                       ScriptCommands.FormatText("{ItemLabel}", draggablesLengthVariable + " items")),
                   ScriptCommands.IfEquals(dragMethodVariable, DragMethod.Menu,
                       ScriptCommands.Assign("{MethodLabel}", queryDropSupportedEffectsVariable),
                       ScriptCommands.Assign("{MethodLabel}", queryDropPreferredEffectVariable)),

                   ScriptCommands.IfEquals(queryDropResultVariable, QueryDropEffects.None,                                        
                       ScriptCommands.FormatText(adornerTextVariable, "{ItemLabel}"), 
                       ScriptCommands.IfAssigned(iSupportDropLabelVariable,
                        ScriptCommands.FormatText(adornerTextVariable, iSupportDropLabelVariable),
                        ScriptCommands.SetProperty(adornerTextVariable, iSupportDropDisplayNameVariable)
                        ))
                   );
        }

        /// <summary>
        /// Update DragAdorner's draggable to {DragDrop.Draggables}.
        /// </summary>
        public static IScriptCommand UpdateAdornerDraggables(string adornerVariable = "{DragDrop.Adorner}",
            string draggablesVariable = "{DragDrop.Draggables}", IScriptCommand nextCommand = null)
        {
            return ScriptCommands.SetProperty(adornerVariable, (DragAdorner a) => a.DraggingItems, draggablesVariable, nextCommand);
        }

        public static IScriptCommand UpdateAdorner(string adornerVariable = "{DragDrop.Adorner}",
            string iSupportDropVariable = "{ISupportDrop}",
            string dragMethodVariable = "{DragDrop.DragMethod}",
            string draggablesVariable = "{DragDrop.Draggables}",
            IScriptCommand nextCommand = null)
        {
            string queryDropResultVariable = "{DragDrop.QueryDropResult}";
            return DragDropScriptCommands.UpdateAdornerPointerPosition(adornerVariable,
              DragDropScriptCommands.UpdateAdornerDraggables(adornerVariable, draggablesVariable,
              HubScriptCommands.QueryDropEffects(iSupportDropVariable, draggablesVariable, null, null, queryDropResultVariable, false,
              DragDropScriptCommands.UpdateAdornerText(adornerVariable, dragMethodVariable, draggablesVariable,
                queryDropResultVariable, iSupportDropVariable, nextCommand))));
        }

        public static IScriptCommand DetachAdorner(string adornerLayerVariable = "{DragDrop.AdornerLayer}",
            string adornerVariable = "{DragDrop.Adorner}", IScriptCommand nextCommand = null)
        {
            return HubScriptCommands.DetachAdorner(adornerLayerVariable, adornerVariable,
                ScriptCommands.Reset(nextCommand, adornerVariable, adornerLayerVariable));
        }

        public static IScriptCommand AttachAdorner(string adornerLayerVariable = "{DragDrop.AdornerLayer}",
             string adornerVariable = "{DragDrop.Adorner}",
            string iSupportDropVariable = "{ISupportDrop}",
            string dragMethodVariable = "{DragDrop.DragMethod}",
            string draggablesVariable = "{DragDrop.Draggables}",

            IScriptCommand nextCommand = null)
        {
            return DragDropScriptCommands.DetachAdorner(adornerLayerVariable, adornerVariable,
                    HubScriptCommands.AttachDragDropAdorner(adornerLayerVariable, adornerVariable,
                        HubScriptCommands.SetDependencyPropertyValue(adornerVariable, DragAdorner.IsDraggingProperty, true,
                            DragDropScriptCommands.UpdateAdorner(adornerVariable, iSupportDropVariable,
                            dragMethodVariable, draggablesVariable, nextCommand))));
        }

     
    }

}
