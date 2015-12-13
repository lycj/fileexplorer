using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand CaptureMouse(CaptureMouseMode mode, IScriptCommand nextCommand = null)
        {
            return new CaptureMouse(mode)
            {
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ReleaseMouse(IScriptCommand nextCommand = null)
        {
            return CaptureMouse(CaptureMouseMode.Release, nextCommand);
        }
    }


    public enum CaptureMouseMode { UIElement, ScrollContentPresenter, Release }

    public class CaptureMouse : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {
        private CaptureMouseMode _mode;


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<CaptureMouse>();

        public CaptureMouse(CaptureMouseMode mode)
            : base("CaptureMouse")
        {
            _mode = mode;
        }

        protected override IScriptCommand executeInner(ParameterDic pm, UIElement ic,
            RoutedEventArgs eventArgs, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            if (Keyboard.Modifiers != ModifierKeys.None)
                return ResultCommand.NoError;

            logger.Debug(String.Format("Capture {0}", _mode));
            switch (_mode)
            {
                case CaptureMouseMode.UIElement:
                    Mouse.Capture(ic, CaptureMode.Element);
                    break;
                case CaptureMouseMode.ScrollContentPresenter:
                    if (ic is ItemsControl)
                    {
                        var scp = ControlUtils.GetScrollContentPresenter(ic as ItemsControl);
                        if (scp == null)
                            return ResultCommand.NoError;

                        Mouse.Capture(scp, CaptureMode.SubTree);
                    }
                    break;
                case CaptureMouseMode.Release:
                    Mouse.Capture(null);
                    break;
                default:
                    return ResultCommand.Error(new NotSupportedException("CaptureMouseMode"));
            }
            return NextCommand;
        }

    }
}
