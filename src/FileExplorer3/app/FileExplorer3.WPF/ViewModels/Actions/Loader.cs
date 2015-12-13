//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using Caliburn.Micro;
//#if WINRT
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//#else
//using System.Windows.Controls;
//#endif

//namespace FileExplorer.WPF.ViewModels
//{
//    public class Loader : IResult
//    {
//        readonly string message;
//        readonly bool hide;

//        public Loader(string message)
//        {
//            this.message = message;
//        }

//        public Loader(bool hide)
//        {
//            this.hide = hide;
//        }

//        public void Execute(ActionExecutionContext context)
//        {
//            var view = context.View as FrameworkElement;
//            while (view != null)
//            {
//                var busyIndicator = view.FindName("BusyIndicator") as ContentControl;
//                if (busyIndicator != null)
//                {
//                    busyIndicator.SetValue(FrameworkElement.VisibilityProperty, 
//                        hide ? Visibility.Collapsed : Visibility.Visible);
//                    busyIndicator.SetValue(ContentControl.ContentProperty, message);
//                    break;
//                }
//                view = view.Parent as FrameworkElement;
//            }

//            Completed(this, new ResultCompletionEventArgs());
//        }

//        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

//        public static IResult Show(string message = null)
//        {
//            return new Loader(message);
//        }

//        public static IResult Hide()
//        {
//            return new Loader(true);
//        }
//    }
//}
