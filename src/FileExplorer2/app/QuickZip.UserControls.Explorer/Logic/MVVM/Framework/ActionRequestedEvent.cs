using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.Logic
{
    /// <summary>
    /// Contain event data related to action requested by an EntryModel
    /// </summary>
    public class ActionRequestedEventArgs : EventArgs
    {
        string _actionName;
        public string ActionName { get { return _actionName; } }
        public ActionRequestedEventArgs(string actionName)
        {
            _actionName = actionName;
        }
        
    }

    /// <summary>
    /// Represent the method that will EntryModel Action request data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ActionRequestedEventHandler(object sender, ActionRequestedEventArgs e);
}
