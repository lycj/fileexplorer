using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.WPF.BaseControls;

namespace FileExplorer.WPF.ViewModels
{
    /// <summary>
    /// Specialized for ICommand related (IScriptCommandContainer, ICommandHelper)
    /// </summary>
    public class VMParameterDic : UIParameterDic
    {
        public IEventAggregator Events
        {
            get { return this.ContainsKey("Events") && this["Events"] is IEventAggregator ? this["Events"] as IEventAggregator : null; }
            set { if (this.ContainsKey("Events")) this["Events"] = value; else this.Add("Events", value); }
        }


        public IExplorerViewModel Explorer
        {
            get { return this.ContainsKey("Explorer") && this["Explorer"] is IExplorerViewModel ? this["Explorer"] as IExplorerViewModel : null; }
            set { if (this.ContainsKey("Explorer")) this["Explorer"] = value; else this.Add("Explorer", value); }
        }

        public IFileListViewModel FileList
        {
            get { return this.ContainsKey("FileList") && this["FileList"] is IFileListViewModel ? this["FileList"] as IFileListViewModel : null; }
            set { if (this.ContainsKey("FileList")) this["FileList"] = value; else this.Add("FileList", value); }
        }

        public IDirectoryTreeViewModel DirectoryTree
        {
            get { return this.ContainsKey("DirectoryTree") && this["DirectoryTree"] is IDirectoryTreeViewModel ? this["DirectoryTree"] as IDirectoryTreeViewModel : null; }
            set { if (this.ContainsKey("DirectoryTree")) this["DirectoryTree"] = value; else this.Add("DirectoryTree", value); }
        }


        public INavigationViewModel Navigation
        {
            get { return this.ContainsKey("Navigation") && this["Navigation"] is INavigationViewModel ? this["Navigation"] as INavigationViewModel : null; }
            set { if (this.ContainsKey("Navigation")) this["Navigation"] = value; else this.Add("Navigation", value); }
        }     

    }    


}
