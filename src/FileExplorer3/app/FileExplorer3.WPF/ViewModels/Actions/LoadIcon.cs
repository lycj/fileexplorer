using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FileExplorer.WPF.ViewModels
{
    public class LoadIcon : IResult
    { 
        #region Cosntructor

        public LoadIcon(
        
        #endregion

        #region Methods

        public void Execute(ActionExecutionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        public event EventHandler<ResultCompletionEventArgs> Completed;
        
        #endregion
     

      
    }
}
