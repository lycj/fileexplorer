using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Data;
using FileExplorer.WPF.ViewModels;

namespace Test_UIScriptCommands
{
    public class AppWindowManager : WindowManager
    {
        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            Window window = base.CreateWindow(rootModel, isDialog, context, settings);
            if (!isDialog)
                window.Owner = null;
            return window;
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            Window window = base.EnsureWindow(model, view, isDialog);

            switch (model.GetType().ToString())
            {                
                case "FileExplorer.WPF.ViewModels.ExplorerViewModel":
                    window.SizeToContent = SizeToContent.Manual;
                    IExplorerViewModel evm = model as IExplorerViewModel;
                    window.Width = evm.Parameters.Width;
                    window.Height = evm.Parameters.Height;
                    window.SizeToContent = SizeToContent.Manual;

                    window.SetBinding(Window.WidthProperty, new Binding("Parameters.Width") { Mode = BindingMode.TwoWay });
                    window.SetBinding(Window.HeightProperty, new Binding("Parameters.Height") { Mode = BindingMode.TwoWay });
                    window.SetBinding(Window.IconProperty, new Binding("CurrentDirectory.Icon") { Mode = BindingMode.OneWay });
                    break;
               
                default:
                    window.SizeToContent = SizeToContent.Manual;
                    window.Width = 800; window.Height = 500;
                    break;


            }




            return window;
        }
    }


    //public class TabbedAppWindowManager : TabbedWindowManager
    //{
    //    public TabbedAppWindowManager(ITabbedExplorerViewModel tabExplorer)
    //        : base(tabExplorer)
    //    {

    //    }

    //    protected override Window EnsureWindow(object model, object view, bool isDialog)
    //    {
    //        Window window = base.EnsureWindow(model, view, isDialog);

    //        if (model is FileExplorer.ViewModels.ProgressDialogViewModel)
    //        {
    //            window.SizeToContent = SizeToContent.WidthAndHeight;
    //        }
    //        else
    //            if (model is FileExplorer.ViewModels.MessageDialogViewModel)
    //            {
    //                window.SizeToContent = SizeToContent.Height;
    //                window.Width = 300;
    //            }
    //            else if (model is FileExplorer.ViewModels.AddDirectoryViewModel)
    //            {
    //                window.SizeToContent = SizeToContent.Height;
    //                window.Width = 300;
    //            }
    //            else
    //            {
    //                window.SizeToContent = SizeToContent.Manual;

    //                if (model is FileExplorer.ViewModels.ExplorerViewModel || 
    //                    model is FileExplorer.ViewModels.TabbedExplorerViewModel)
    //                {
    //                    window.Width = 800; window.Height = 500;
    //                }
    //                else if (model is FileExplorer.ViewModels.LoginViewModel)
    //                {
    //                    window.Width = 300; window.Height = 350;
    //                }
    //                else
    //                {
    //                    window.Width = 500; window.Height = 500;
    //                }

    //                window.SetBinding(Window.IconProperty, new Binding("CurrentDirectory.Icon") { Mode = BindingMode.OneWay });
    //            }

    //        return window;
    //    }
    //}
}
