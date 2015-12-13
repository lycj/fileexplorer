using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls
{
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

    public class ProgressEventArgs : EventArgs
        {
        static uint id = 0;
        public static uint NewID() { return ++id; }
        
        public string Text { get; private set; }
        public uint ID { get; private set; }

        public WorkType Work { get; private set; }
        public WorkStatusType WorkStatus { get; private set; }
        public WorkResultType WorkResult { get; private set; }

        private void init(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)  
        {
            ID = id;
            Text = text;
            Work = work;
            WorkStatus = workStatus;
            WorkResult = workResult;
        }

        public ProgressEventArgs(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)            
        {
            init(id, text, work, workStatus, workResult);
        }
    }
}
