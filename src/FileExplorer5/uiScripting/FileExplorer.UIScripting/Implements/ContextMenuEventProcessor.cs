using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.BaseControls
{
    public class ContextMenuEventProcessor : UIEventProcessorBase
    {
        public ContextMenuEventProcessor()
        {
            _processEvents.AddRange(
                new[] {
                 FrameworkElement.MouseRightButtonUpEvent
                }
             );
        }

        protected override FileExplorer.Script.IScriptCommand onEvent(RoutedEvent eventId)
        {
            if (EnableContextMenu)
                switch (eventId.Name)
                {
                    case "MouseRightButtonUp": return new ShowContextMenu(ContextMenu);
                }

            return base.onEvent(eventId);

        }

        public static DependencyProperty ContextMenuProperty =
         DependencyProperty.Register("ContextMenu", typeof(ContextMenu),
         typeof(ContextMenuEventProcessor), new PropertyMetadata(null));

        public ContextMenu ContextMenu
        {
            get { return (ContextMenu)GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        public static DependencyProperty EnableContextMenuProperty =
      DependencyProperty.Register("EnableContextMenu", typeof(bool),
      typeof(ContextMenuEventProcessor), new PropertyMetadata(true));

        public bool EnableContextMenu
        {
            get { return (bool)GetValue(EnableContextMenuProperty); }
            set { SetValue(EnableContextMenuProperty, value); }
        }
    }


    public class ShowContextMenu : UIScriptCommandBase<Control, RoutedEventArgs>
    {
        private ContextMenu _contextMenu;
        public ShowContextMenu(ContextMenu contextMenu) : base("ShowContextMenu") { _contextMenu = contextMenu; }

        protected override IScriptCommand executeInner(ParameterDic pd, Control sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            if (!evnt.Handled)
            {
                pd.IsHandled = true;

                if (input.InputType == UIInputType.MouseRight)
                {
                    //_contextMenu.DataContext =
                    //    _contextMenu.DataContext ??
                    //    (pm["Sender"] as FrameworkElement).DataContext;
                    _contextMenu.PlacementTarget = sender;
                    _contextMenu.SetValue(ContextMenu.IsOpenProperty, true);
                    evnt.Handled = true;
                }
            }
            return ResultCommand.NoError; //Set Handled to true.
        }


    }
}
