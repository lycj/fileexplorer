using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using FileExplorer.WPF.Views;
using FileExplorer.WPF.UserControls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MetroLog;
using FileExplorer.WPF.ViewModels;

namespace TestApp
{
    public class AppBootstrapper : BootstrapperBase//<IScreen>
    {
        private CompositionContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, LogLevel.Fatal, new ConsoleTarget());
            LogManagerFactory.DefaultConfiguration.IsEnabled = true;
            DisplayRootViewFor<IScreen>();
        }
        protected override void Configure()
        {
            container = new CompositionContainer(
                new AggregateCatalog(
                    AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                        .Concat(new ComposablePartCatalog[] { new DirectoryCatalog(".") }))
                );
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            container = new CompositionContainer(catalog);


            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new AppWindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());            

            batch.AddExportedValue(container);
            
            //Debug.WriteLine(ConventionManager.GetElementConvention(typeof(ListViewEx)));

            //To-Do: https://caliburnmicro.codeplex.com/wikipage?title=All%20About%20Conventions
            //ConventionManager.AddElementConvention<ListViewEx>(ListViewEx.OuterRightContentProperty, 
            //    "DataContext", "Loaded").GetBindableProperty =
            //    delegate(DependencyObject foundControl)
            //    {
            //        var element = (ListViewEx)foundControl;

            //        return !(element.OuterRightContent is DependencyObject)
            //            ? View.ModelProperty
            //            : ListViewEx.OuterRightContentProperty;
            //    };
            container.Compose(batch);;

        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = base.SelectAssemblies().ToList();
            assemblies.Add(typeof(FileExplorer.WPF.ViewModels.FileListViewModel).GetTypeInfo().Assembly);

            return assemblies;
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }
    }
}
