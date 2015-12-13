//using FileExplorer;
//using FileExplorer.Defines;
//using FileExplorer.Script;
//using FileExplorer.WPF.BaseControls;
//using FileExplorer.WPF.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Documents;

//namespace FileExplorer.UIEventHub
//{
////    public static partial class HubScriptCommands
////    {
////        //{        
////        //    public static IScriptCommand AttachDragDropAdorner(
////        //        IScriptCommand nextCommand = null)
////        //    {
////        //        return AssignAdornerLayer("PART_DragDropAdorner", AssignDragDropAdorner.AdornerLayerKey, false,
////        //              new AssignDragDropAdorner()
////        //              {
////        //                    NextCommand = (ScriptCommandBase)
////        //                    HubScriptCommands.AttachAdorner(AssignDragDropAdorner.AdornerLayerKey, AssignDragDropAdorner.AdornerKey)
////        //              });

////        //    }

////        //    public static IScriptCommand DetachDragDropAdorner(
////        //        IScriptCommand nextCommand = null)
////        //    {
////        //        return ScriptCommands.IfAssigned(AssignDragDropAdorner.AdornerKey,
////        //                AssignAdornerLayer("PART_DragDropAdorner", AssignDragDropAdorner.AdornerLayerKey, false,
////        //                HubScriptCommands.DetachAdorner(AssignDragDropAdorner.AdornerLayerKey, AssignDragDropAdorner.AdornerKey)));
////        //    }
////    }

//    //public enum UpdateAdornerMode : int {  Draggables = 0 << 1, Label = 0 << 2, DraggableAndLabel = Draggables | Label };

//    public class UpdateDragDropAdorner : UIScriptCommandBase<UIElement, RoutedEventArgs>
//    {
//        //UpdateAdornerMode Mode { get; set; }

//        /// <summary>
//        /// Point to draggables thats i that support ISupportShellDrop or ISupportDrop, Default={Draggables}
//        /// </summary>
//        public string DraggablesKey { get; set; }

//        /// <summary>
//        /// Point to label of the adorner, Default={Label}
//        /// </summary>
//        public string LabelKey { get; set; }

//        /// <summary>
//        /// Point to where to store the DragDropAdorner, default={DragDropAdorner}, 
//        /// shared with AssignDragDropAdorner command.
//        /// </summary>
//        public string AdornerKey { get; set; }

//        public UpdateDragDropAdorner()
//            : base("UpdateDragDropAdorner")
//        {
//            //Mode = UpdateAdornerMode.DraggableAndLabel;
//            DraggablesKey = "{Draggables}";
//            AdornerKey = "{DragDropAdorner}";
//            LabelKey = "{Label}";
//        }

//        private static int _prevDataObjectId = -1;

//        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
//        {            
//            DragAdorner adorner = pm.GetValue<DragAdorner>(AdornerKey);
//            IEnumerable<IDraggable> draggables = DraggablesKey == null ? null : pm.GetValue<IEnumerable<IDraggable>>(DraggablesKey);
//            string label = LabelKey == null ? null : pm.GetValue<string>(LabelKey);

//            if (adorner != null)
//            {
//                if (draggables != null)
//                adorner.DraggingItems = draggables;
//                if (label != null)
//                    adorner.Text = label;


//            }

//            return NextCommand;
//        }


//    }
//}
