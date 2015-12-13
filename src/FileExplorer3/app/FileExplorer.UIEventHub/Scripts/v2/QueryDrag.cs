using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Call DragSource (ISupportDrag).QueryDrag and assign result to destination.
        /// </summary>
        /// <param name="dragSourceVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryDrag(string dragSourceVariable = "{ISupportDrag}",
            string destinationVariable = "{DragResult}",
          IScriptCommand nextCommand = null)
        {
            return new QueryDrag()
            {
                DragSourceKey = dragSourceVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand,

            };
        }

        /// <summary>
        /// Assign destination to DragMethod.Menu if Mouse action is right click, otherwise DragMethod.Normal.
        /// </summary>
        /// <returns></returns>
        public static IScriptCommand AssignDragMethod(string variable, IScriptCommand nextCommand = null)
        {
            return
                ScriptCommands.RunSequence(nextCommand,
                HubScriptCommands.IfMouseGesture(new MouseGesture() { MouseAction = MouseAction.RightClick },
                     ScriptCommands.Assign(variable, DragMethod.Menu),
                     HubScriptCommands.IfTouchGesture(new TouchGesture() { TouchAction =  UITouchGesture.Drag },
                        ScriptCommands.Assign(variable, DragMethod.Menu),
                         ScriptCommands.Assign(variable, DragMethod.Normal))
                         ));
        }
    }

    public enum DragMethod { None, Normal, Menu }

    public class QueryDrag : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {

        /// <summary>
        /// Point to DataContext (ISupportDrag) to initialize the drag, default = "{ISupportDrag}".
        /// </summary>
        public string DragSourceKey { get; set; }

        /// <summary>
        /// Store Drag result (DragDropEffects) to destination variable, default = "{DragResult}".
        /// </summary>
        public string DestinationKey { get; set; }

        //public string DraggablesKey { get; set; }

        //public string DataObjectKey { get; set; }


        public static string DragDropModeKey { get { return DragDropLiteCommand.DragDropModeKey; } }

        /// <summary>
        ///  Specify the DragMethod (DragMethod, Menu/Normal), Default=Normal.
        /// Store DragMethod in ParameterDic for further use, Default={DragDrop.DragDropMode}.
        /// </summary>
        public static string DragMethodKey { get; set; }


        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<QueryDrag>();


        static QueryDrag()
        {
            DragMethodKey = "{DragDrop.DragMethod}";
        }


        public QueryDrag()
            : base("DragDropCommand")
        {
            DragSourceKey = "{ISupportDrag}";
            DestinationKey = "{DragResult}";
        }

        #region Methods

        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            //Debug.WriteLine(String.Format("DoDragDrop"));

            ISupportDrag _isd = pm.GetValue<ISupportDrag>(DragSourceKey);
            var draggables = _isd.GetDraggables();
            
            var effect = _isd.QueryDrag(draggables);

            if (_isd is ISupportShellDrag)
            {
                _dataObj = (_isd as ISupportShellDrag).GetDataObject(draggables);
                if (_dataObj == null)
                    return ResultCommand.NoError; //Nothing to drag.
                _dataObj.SetData(typeof(ISupportDrag), _isd);
            }


            System.Windows.DragDrop.AddQueryContinueDragHandler(sender, new QueryContinueDragEventHandler(OnQueryContinueDrag));

            //Determine and set the desired drag method. (Normal, Menu)            
            _dataObj.SetData(typeof(DragMethod), pm.GetValue(DragMethodKey, DragMethod.Normal));
            //Notify Shell DragDrop mode is activated.
            pm.SetValue(DragDropModeKey, "Shell", false);

            foreach (var d in draggables) d.IsDragging = true;

            DragDropEffectsEx resultEffect;
            try
            {
                //Start the DragDrop.
                resultEffect = (DragDropEffectsEx)System.Windows.DragDrop.DoDragDrop(sender, _dataObj, (DragDropEffects)effect);
            }
            finally
            {
                //Reset DragDropMode.
                pm.SetValue<string>(DragDropModeKey, null, false);
                //Notify draggables not IsDragging.
                foreach (var d in draggables) d.IsDragging = false;
                System.Windows.DragDrop.RemoveQueryContinueDragHandler(sender, new QueryContinueDragEventHandler(OnQueryContinueDrag));
                //Debug.WriteLine(String.Format("NotifyDropCompleted {0}", resultEffect));                
                (inpProcs.First(p => p is DragInputProcessor) as DragInputProcessor).IsDragging = false;
            }

            //_isd.OnDragCompleted(draggables, _dataObj, resultEffect);
            var dataObj = _dataObj;
            _dataObj = null;
            pm.SetValue(DestinationKey, resultEffect, false);

            return NextCommand;
        }

        #endregion

        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //The control, like treeview or listview
            FrameworkElement control = sender as FrameworkElement;

            //ESC pressed
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                control.AllowDrop = true;
            }
            else
                if (e.KeyStates == DragDropKeyStates.None)
                {
                    _dataObj.SetData(ShellClipboardFormats.CFSTR_INDRAGLOOP, 0);
                    e.Action = DragAction.Drop;
                }
                else
                    e.Action = DragAction.Continue;

            _dataObj.SetData(typeof(DragDropKeyStates), e.KeyStates);

            e.Handled = true;
        }

        private void OnPreviewDrop(object sender, QueryContinueDragEventArgs e)
        {

        }



        #region Data

        private IDataObject _dataObj = null;

        #endregion

    }
}
