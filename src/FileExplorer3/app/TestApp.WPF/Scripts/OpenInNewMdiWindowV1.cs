using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.Models;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.MDI;

namespace TestApp.Script
{


    public class OpenInNewMdiWindowV1 : ScriptCommandBase
    {
        private Func<ParameterDic, IEntryModel[]> _getSelectionFunc;
        private IExplorerInitializer _initializer;
        private MdiContainer _container;

        public OpenInNewMdiWindowV1(MdiContainer container,
            IExplorerInitializer initializer,
            Func<ParameterDic, IEntryModel[]> getSelectionFunc = null)
            : base("OpenInNewWindow")
        {
            _container = container;
            _initializer = initializer;
            _getSelectionFunc = getSelectionFunc;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {

            var viewModel = new ExplorerViewModel(_initializer);
            pm["Explorer"] = viewModel;
            var view = new ExplorerView();
            Caliburn.Micro.Bind.SetModel(view, viewModel); //Set the ViewModel using this command.
            var mdiChild = new MdiChild
            {
                DataContext = viewModel,
                ShowIcon = true,
                Content = view,
                Width = 500,
                Height = 334,
                Position = new Point(0, 0)
            };
            mdiChild.SetBinding(MdiChild.TitleProperty, new Binding("DisplayName") { Mode = BindingMode.OneWay });
            mdiChild.SetBinding(MdiChild.IconProperty, new Binding("CurrentDirectory.Icon") { Mode = BindingMode.OneWay });
            _container.Children.Add(mdiChild);

            var selection = _getSelectionFunc == null ? null : _getSelectionFunc(pm);
            if (selection != null && selection.Count() > 0)
                return UIScriptCommands.ExplorerGoToValue(selection.First());

            return ResultCommand.NoError;
        }


        public override bool CanExecute(ParameterDic pm)
        {
            var selection = _getSelectionFunc == null ? null : _getSelectionFunc(pm);
            return selection == null || (selection.Count() == 1 && selection[0].IsDirectory);
        }
    }
}
