using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public interface INavigationViewModel : ISupportCommandManager
    {
        void GoUp();        
        void GoBack();
        void GoNext();

        bool CanGoUp { get; }
        bool CanGoBack { get; }
        bool CanGoNext { get; }

    }
}
