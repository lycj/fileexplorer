using MetroLog;
using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroLog
{
    public class ConsoleTarget : SyncTarget
    {
        public ConsoleTarget(Layouts.Layout layout)
            : base(layout)
        {

        }

        public ConsoleTarget()
            : this(new Layouts.SingleLineLayout())
        {

        }

        

        protected override void Write(LogWriteContext context, LogEventInfo entry)
        {            
            Console.WriteLine(Layout.GetFormattedString(context, entry));            
        }
    }
}
