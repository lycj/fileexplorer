using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls
{
    public delegate void ProgressRoutedEventHandler(object sender, ProgressRoutedEventArgs e);

    public class ProgressRoutedEventArgs : RoutedEventArgs
    {
        public static uint NewID() { return ProgressEventArgs.NewID(); }

        public static readonly RoutedEvent ProgressEvent = EventManager.RegisterRoutedEvent("ProgressEvent",
            RoutingStrategy.Bubble, typeof(ProgressRoutedEventHandler), typeof(ProgressRoutedEventArgs));

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

        public ProgressRoutedEventArgs(RoutedEvent routedEvent, object source,
            uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
            : base(routedEvent, source)
        {
            init(id, text, work, workStatus, workResult);
        }


        public ProgressRoutedEventArgs(RoutedEvent routedEvent, ProgressEventArgs args)
            : base(routedEvent)
        {
            init(args.ID, args.Text, args.Work, args.WorkStatus, args.WorkResult);
        }
    }
}
