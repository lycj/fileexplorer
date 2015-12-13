using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public class ViewAttached : ViewAware
    {
        private bool _isViewAttached = false;
        public bool IsViewAttached { get { return _isViewAttached; }  private set { _isViewAttached = value; }} 
        protected override void OnViewAttached(object view, object context)
        {
            IsViewAttached = true;
            base.OnViewAttached(view, context);
        }

    }
}
