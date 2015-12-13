using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF;
using FileExplorer.Script;


using System.Windows.Media.Animation;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.UnitTests;
using FileExplorer.WPF.Models;
using FileExplorer.Models;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;

namespace TestTemplate.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        //private void test(System.Collections.IEnumerable itemSource, HierarchicalDataTemplate dt)
        //{


        //}

        public class DummySuggestSource : ISuggestSource
        {

            public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
            {
                return Task.FromResult<IList<object>>(new List<object>()
                {
                     new { Header = input + "-add xyz", Value = input + "xyz" },
                     new { Header = input + "-add abc", Value = input + "abc" }
                });
            }

            public bool RunInDispatcher { get { return false; } }

        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            setupDropDownList();
            setupBreadcrumb();
            setupBreadcrumbTree();
            setupDragAndDrop();
            setupToolbarItem();
            setupInputProcessor();
            setupTabControl();
        }


        private static IUIInputManager _inputProcessors;
        private void setupInputProcessor()
        {
            #region Moved to Test_InputProcessor, See FileExplorer3 - UIEventHub solution.

            //_inputProcessors =
            //    new UIInputManager(
            //    new DragInputProcessor()
            //    {
            //        DragStartedFunc = inp => inputProcessorOutput.Items.Add("*DragStarted* " + inp.ToString()),
            //        DragStoppedFunc = inp => inputProcessorOutput.Items.Add("*DragStopped* " + inp.ToString())
            //    });


            //string _lastItem = null;
            //RoutedEventHandler handler = (o, e) =>
            //    {
            //        IUIInput input = UIInputBase.FromEventArgs(o, e as InputEventArgs);
            //        // e.Handled = true;
            //        _inputProcessors.Update(ref input);
            //        if (input.InputState != UIInputState.NotApplied)
            //        {
            //            string currentItem = input.ToString();
            //            if (currentItem != _lastItem)
            //            {
            //                _lastItem = currentItem;
            //                inputProcessorOutput.Items.Add(_lastItem);
            //            }
            //        }
            //        while (inputProcessorOutput.Items.Count > 15)
            //            inputProcessorOutput.Items.RemoveAt(0);
            //    };

            //inputProcessorOutput.MouseDoubleClick += (o, e) =>
            //    inputProcessorOutput.Items.Clear();
            //inputProcessorCanvas.AddHandler(UIElement.MouseDownEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.MouseMoveEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.MouseUpEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.TouchDownEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.TouchMoveEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.TouchUpEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.StylusDownEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.StylusMoveEvent, handler);
            //inputProcessorCanvas.AddHandler(UIElement.StylusUpEvent, handler);

            #endregion

        }

        private void setupDropDownList()
        {
            ddl.ItemsSource = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        private void setupDragAndDrop()
        {
            #region moved to Test_NonShellDragDemo project, see FileExplorer - UIEventHub solution
            //ScriptRunner runner = new ScriptRunner();

            //DragDropItemViewModel vm0, vm1, vm2, vm3, vm4;

            ////StartIdx, child count, droppable, child droppable
            //tvDnd1.DataContext = vm0 = new DragDropItemViewModel(1, 10, true, true);
            //lvDnd1.DataContext = vm1 = new DragDropItemViewModel(20, 20, true, false);
            //lvDnd2.DataContext = vm2 = new DragDropItemViewModel(41, 20, true, false);
            //lvDnd3.DataContext = vm3 = new DragDropItemViewModel(61, 20, true, false);
            //lvDnd4.DataContext = vm4 = new DragDropItemViewModel(81, 20, true, false);

            //#region To register manually
            ////tvDnd1.RegisterEventProcessors(new DragDropEventProcessor());            
            ////lvDnd1.RegisterEventProcessors(new DragDropEventProcessor(), new MultiSelectEventProcessor(vm1.UnselectAllCommand));
            ////lvDnd2.RegisterEventProcessors(new DragDropEventProcessor(), new MultiSelectEventProcessor(vm2.UnselectAllCommand));
            ////lvDnd3.RegisterEventProcessors(new DragDropEventProcessor(), new MultiSelectEventProcessor(vm3.UnselectAllCommand));
            ////lvDnd4.RegisterEventProcessors(new DragDropEventProcessor(), new MultiSelectEventProcessor(vm4.UnselectAllCommand));        

            ////lvDnd1.Loaded += (o, e) =>
            ////    {
            ////        var pd = new UIParameterDic() { Sender = lvDnd1 };
            ////        pd["IsSelecting"] = true;
            ////        pd["StartPosition"] = new Point(30, 30);
            ////        pd["EndPosition"] = new Point(100, 100);

            ////        MultiSelectScriptCommands.AttachAdorner.Execute(pd);

            ////        if (pd["SelectionAdorner"] == null)
            ////            throw new Exception();

            ////        lvDnd1.Dispatcher.BeginInvoke(new System.Action(() =>
            ////            {
            ////                MultiSelectScriptCommands.UpdateAdorner.Execute(pd);
            ////            }));

            ////        //MultiSelectScriptCommands.DetachAdorner.Execute(pd);
            ////    };
            //#endregion
            #endregion
        }

        private void setupBreadcrumbTree()
        {
            var tvModel = new TreeViewModel();
            btreeTab.DataContext = tvModel;
            bexp.AddValueChanged(ComboBox.SelectedValueProperty, (o, e) =>
                {
                    string path = bexp.SelectedValue as string;
                    if (path != null)
                        tvModel.Selection.AsRoot().SelectAsync(path);
                });
            selectBTreeItem.Click += (RoutedEventHandler)((o, e) =>
                {
                    tvModel.Selection.AsRoot().SelectAsync(selectBTreeCombo.Text);
                });
            //var items = TreeViewModel.GenerateFakeTreeViewModels().RootItems;
            //items[0].Subitems[1].IsExpanded = true;
            //items[0].Subitems[1].Subitems[2].IsExpanded = true;
            //items[0].Subitems[1].Subitems[2].Subitems[3].IsExpanded = true;
            //items[0].Subitems[1].Subitems[2].Subitems[3].Subitems[4].SelectionHelper.IsSelected = true;
            //btree.ItemsSource = items;

        }


        private void setupBreadcrumb()
        {
            FakeViewModel fvm = new FakeViewModel("Root");
            for (int i = 1; i < 10; i++)
                fvm.SubDirectories.Add(new FakeViewModel("Sub" + i.ToString(), "Sub" + i.ToString() + "1", "Sub" + i.ToString() + "2"));
            breadcrumbCore.ItemsSource = fvm.SubDirectories;
            breadcrumbCore.RootItemsSource = fvm.SubDirectories;

            //SuggestBoxes            
            suggestBoxDummy.SuggestSources = new List<ISuggestSource>(new[] { new DummySuggestSource() });
            suggestBoxAuto.RootItem = fvm;

            suggestBoxAuto2.HierarchyHelper = suggestBoxAuto.HierarchyHelper =
                new PathHierarchyHelper("Parent", "Value", "SubDirectories");

            //suggestBoxAuto2
            suggestBoxAuto2.RootItem = FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0.5));
            suggestBoxAuto2.SuggestSources = new List<ISuggestSource>(new[] { new AutoSuggestSource() }); //This is default value, suggest based on HierarchyLister.List()


            //breadcrumb
            breadcrumb1.RootItem = FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0));
            breadcrumb2.RootItem = FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0));

            bool UseGenericHierarchyHelper = true;

            if (UseGenericHierarchyHelper)
            {
                //Generic version is faster than Nongeneric PathHierarchyHelper.
                //This replaced the ParentPath, ValuePath and SubEntriesPath in markup.
                IHierarchyHelper hierarchyHelper = new PathHierarchyHelper<FakeViewModel>("Parent", "Value", "SubDirectories");
                //suggestBoxAuto.HierarchyHelper = hierarchyHelper;
                //suggestBoxAuto2.HierarchyHelper = hierarchyHelper;
                breadcrumb1.HierarchyHelper = hierarchyHelper;
                breadcrumb2.HierarchyHelper = hierarchyHelper;
            }


        }

        private void setupToolbarItem()
        {
            tbiMenu1.SetValue(ItemsControl.ItemsSourceProperty, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            tbiMenu2.SetValue(ItemsControl.ItemsSourceProperty,
                 (new int[] { 200, 100, 50, 20, 0 }).Select(num => new ToolbarSubItemEx()
                 {
                     Value = num,
                     Height = num + 20,
                     VerticalContentAlignment = num == 200 ? VerticalAlignment.Top : num == 0 ?
                         VerticalAlignment.Bottom : VerticalAlignment.Top,
                     IsStepStop = true,
                     Header = num.ToString()
                 }));
        }

        private void setupTabControl()
        {
            
            tabControl1.DataContext = new TabControlViewModel();
            //for (int i = 1; i < 20; i++)
            //    tabControl1.Items.Add(new TabItemEx() { Header = "Tab "  + i.ToString(), 
            //        Content = String.Format("This is page {0}", i) });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //sbar.Items.Add(new StatusbarItemEx() { Content = "Add", Type = FileExplorer.Defines.DisplayType.Text, Header = "New" });
        }

        private void ToolbarClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }


    }
}
