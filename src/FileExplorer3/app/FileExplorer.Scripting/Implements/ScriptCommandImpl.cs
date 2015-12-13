using MetroLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class ResultCommand : ScriptCommandBase
    {        
        /// <summary>
        /// Represent no error and IsHandled.
        /// </summary>
        public static ResultCommand OK = new ResultCommand(null, true);
        /// <summary>
        /// Represent no error and does not mark IsHandled.
        /// </summary>
        public static ResultCommand NoError = new ResultCommand(null, false);        

        /// <summary>
        /// Represent there's an error.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ResultCommand Error(Exception ex) { return new ResultCommand(ex); }

        Action<ParameterDic> _executeFunc = (pm) => { };

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ResultCommand>();

        /// <summary>
        /// Used by serializer only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ResultCommand() { }


        private ResultCommand(Exception ex = null, bool markHandled = true)
            : base( ex == null ? "OK" : "FAIL")
        {

            MarkHandled = markHandled;
            _exception = ex;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (_exception == null)
            {
                if (MarkHandled)
                    pm.IsHandled = true;
                //logger.Debug("OK");
            }
            else
            {
                logger.Error(_exception.Message, _exception);
                pm.Error = _exception;
            }
            return NextCommand;
        }

        public bool MarkHandled { get; set; }
        private Exception _exception = null;
        public string ErrorMessage
        {
            get { return _exception == null ? "" : _exception.Message; }
            set { if (!String.IsNullOrEmpty(value)) _exception = new Exception(value); }
        }
        
    }

    /// <summary>
    /// A script command that cannot execute.
    /// </summary>    
    public class NullScriptCommand : ScriptCommandBase
    {
        public static IScriptCommand Instance = new NullScriptCommand();

       
        public NullScriptCommand()
            : base("Null")
        {

        }
        
        public override IScriptCommand Execute(ParameterDic pm)
        {
            return ResultCommand.Error(new Exception("NullScriptCommand should not be called."));
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            return ResultCommand.Error(new Exception("NullScriptCommand should not be called."));
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return false;
        }

        
    }


}
