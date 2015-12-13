using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand AttachResizeItemAdorner(
            string adornerLayerVariable = "{AdornerLayer}",
            string resizeItemAdornerVariable = "{ResizeItemAdorner}",
            IScriptCommand nextCommand = null)
        {
            return new ResizeItemAdornerCommand()
            {
                AdornerLayerKey = adornerLayerVariable,
                ResizeItemAdornerKey = resizeItemAdornerVariable,
                AdornerMode = UIEventHub.AdornerMode.Attach,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand DettachResizeItemAdorner(
            string adornerLayerVariable = "{AdornerLayer}",
            string resizeItemAdornerVariable = "{ResizeItemAdorner}",
            IScriptCommand nextCommand = null)
        {
            return new ResizeItemAdornerCommand()
            {
                AdornerLayerKey = adornerLayerVariable,
                ResizeItemAdornerKey = resizeItemAdornerVariable,
                AdornerMode = UIEventHub.AdornerMode.Detach,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand UpdateResizeItemAdorner(string resizeItemAdornerVariable = "{CanvasResize.ResizeItemAdorner}",
            string resizeModeVariable = "{CanvasResize.ResizeMode}",
            string startPositionVariable = "{CanvasResize.StartPosition}",
            string currentPositionVariable = "{CanvasResize.CurrentPosition}",
            IScriptCommand nextCommand = null)
        {
            return new ResizeItemAdornerCommand()
            {
                AdornerMode = UIEventHub.AdornerMode.Update,
                ResizeItemAdornerKey = resizeItemAdornerVariable,
                ResizeModeKey = resizeModeVariable,
                StartPositionKey = startPositionVariable,
                CurrentPosittionKey = currentPositionVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    public class ResizeItemAdornerCommand : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        public AdornerMode AdornerMode { get; set; }

        /// <summary>
        /// Point to the adorner layer to attach/detach adorner, Default = {CanvasResize.AdornerLayer}
        /// </summary>
        public string AdornerLayerKey { get; set; }

        /// <summary>
        /// If attach, the selection adorner (of type ResizeItemAdorner) will be set to the key, 
        /// If update, 
        /// If detach, the ResizeItemAdorner will be point to null.
        /// Default = "{CanvasResize.ResizeItemAdorner}".
        /// </summary>
        public string ResizeItemAdornerKey { get; set; }

        /// <summary>
        /// If set, the selected item (IResizable) will be used for adorner's datacontext, 
        /// otherwise lookup from ItemsControl, Default = null.
        /// </summary>
        public string TargetItemKey { get; set; }

        public string ResizeModeKey { get; set; }

        public string StartPositionKey { get; set; }
        public string CurrentPosittionKey { get; set; }


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ResizeItemAdornerCommand>();


        public ResizeItemAdornerCommand()
            : base("ResizeItemAdornerCommand")
        {
            AdornerLayerKey = "{CanvasResize.AdornerLayer}";
            ResizeItemAdornerKey = "{CanvasResize.ResizeItemAdorner}";
            TargetItemKey = null;
            ResizeModeKey = "{CanvasResize.ResizeMode}";
            StartPositionKey = "{CanvasResize.StartPosition}";
            CurrentPosittionKey = "{CanvasResize.CurrentPosition}";
            //StartPositionAdjustedKey = "{StartPositionAdjusted}";
            //CurrentPositionKey = "{CurrentPosition}";
        }

        protected override Script.IScriptCommand executeInner(ParameterDic pm, ItemsControl ic,
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            if (AdornerMode == UIEventHub.AdornerMode.Update)
            {

                var updateAdorner = pm.GetValue<ResizeItemAdorner>(ResizeItemAdornerKey);
                string mode = pm.GetValue<string>(ResizeModeKey);
                Point startPosition = pm.GetValue<Point>(StartPositionKey);
                Point currentPosition = pm.GetValue<Point>(CurrentPosittionKey);
                Vector offset = currentPosition - startPosition;
                //                   private static IScriptCommand previewNorthCommand =
                //    ScriptCommands.Multiply<double>("{DiffY}", -1, "{DiffY1}",
                //    ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetY}", "{DiffY1}", false,
                //    ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetTop}", "{DiffY1}")));
                //private static IScriptCommand previewSouthCommand = ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetY}", "{DiffY}");
                //private static IScriptCommand previewWestCommand = ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetX}", "{DiffX}", false,
                //                                                    ScriptCommands.Multiply<double>("{DiffX}", 1, "{DiffX1}",
                //                                                    ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetLeft}", "{DiffX1}")));
                //private static IScriptCommand previewEastCommand = ScriptCommands.Assign("{CanvasResize.ResizeItemAdorner.OffsetX}", "{DiffX}");

                if (mode.Contains("N"))
                {
                    updateAdorner.OffsetTop = offset.Y;
                    updateAdorner.OffsetHeight = -1 * offset.Y;
                }

                if (mode.Contains("E"))
                {                    
                    updateAdorner.OffsetWidth = offset.X;
                }

                if (mode.Contains("S"))
                {                    
                    updateAdorner.OffsetHeight = offset.Y;
                }

                if (mode.Contains("W"))
                {
                    updateAdorner.OffsetLeft = offset.X;
                    updateAdorner.OffsetWidth = -1 * offset.X;
                }



                //adornerLayer = UITools.FindVisualChild<AdornerLayer>(selectedItem);
                //ResizeItemAdorner updateAdorner = new ResizeItemAdorner(adornerLayer);
                //adornerLayer.Add(updateAdorner);

                ////if (updateAdorner == null)
                ////    return ResultCommand.Error(new Exception("Adorner not found."));


                //updateAdorner.SelectedItem = targetItem;

            }
            else
            {
                var scp = ControlUtils.GetScrollContentPresenter(ic);
                if (scp != null)
                {
                    AdornerLayer adornerLayer = pm.GetValue<AdornerLayer>(AdornerLayerKey);
                    if (adornerLayer != null)

                        switch (AdornerMode)
                        {
                            case UIEventHub.AdornerMode.Attach:

                                IResizable targetItem = TargetItemKey == null ?
                                   (ic.ItemsSource as IEnumerable).OfType<ISelectable>()
                                       .FirstOrDefault(s => s is IResizable && s.IsSelected) as IResizable :
                                   pm.GetValue<IResizable>(TargetItemKey);

                                UIElement selectedItem = ic.ItemContainerGenerator.ContainerFromItem(targetItem) as UIElement;
                                ResizeItemAdorner resizeAdorner = new ResizeItemAdorner(adornerLayer) { SelectedItem = targetItem };
                                pm.SetValue(ResizeItemAdornerKey, resizeAdorner, false);
                                adornerLayer.Add(resizeAdorner);

                                break;

                            case UIEventHub.AdornerMode.Detach:
                                ResizeItemAdorner detachAdorner = pm.GetValue<ResizeItemAdorner>(ResizeItemAdornerKey);
                                if (detachAdorner != null)
                                    adornerLayer.Remove(detachAdorner);
                                pm.SetValue<object>(ResizeItemAdornerKey, null);
                                break;



                        }



                }
            }
            return NextCommand;
        }

    }
}
