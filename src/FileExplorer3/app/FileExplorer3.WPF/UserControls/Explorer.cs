using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using FileExplorer.WPF.Views;
using FileExplorer.WPF.Views.Explorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.WPF.ViewModels;
using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.Models;
using System.Windows.Data;

namespace FileExplorer.WPF.UserControls
{

    public class Explorer : ContentControl
    {
        public enum ExplorerMode { Unknown, Normal, ToolWindow, ToolWindow2, DirectoryPicker, DirectoryPickerCombo }
        #region Constructors

        static Explorer()
        {
            new FileExplorerBootStrapper();//.Initialize();
        }

        public Explorer()
        {
            _wm = new WindowManager();
            _events = new EventAggregator();
            _evm = new ExplorerViewModel(_wm, _events);

            this.SetBinding(CurrentDirectoryProperty, new Binding("FileList.CurrentDirectory") { Source = _evm, Mode = BindingMode.TwoWay });
            this.SetBinding(SelectedEntriesProperty, new Binding("FileList.Selection.SelectedModels") { Source = _evm, Mode = BindingMode.TwoWay });
        }

        #endregion

        #region Methods

        public static void OnPropertiesChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Explorer exp = sender as Explorer;


            switch (args.Property.Name)
            {
                case "RootDirectories":
                    IEntryModel[] newRootDirs = args.NewValue as IEntryModel[];
                    if (newRootDirs != null && newRootDirs.Length > 0)
                        exp._evm.Commands.Execute(
                            UIScriptCommands.NotifyRootChanged(newRootDirs)
                            );
                    break;
                case "Mode":
                    switch ((ExplorerMode)args.NewValue)
                    {
                        case ExplorerMode.Normal: exp.Content = new ExplorerView(); break;
                        case ExplorerMode.ToolWindow: exp.Content = new ToolWindow(); break;
                        case ExplorerMode.ToolWindow2: exp.Content = new ToolWindow2(); break;
                        case ExplorerMode.DirectoryPicker: exp.Content = new DirectoryPicker(); break;
                        case ExplorerMode.DirectoryPickerCombo: exp.Content = new DirectoryPickerCombo(); break;
                        default: exp.Content = null; break;
                    }

                    if (exp.Content != null)
                        Caliburn.Micro.Bind.SetModel(exp.Content as DependencyObject, exp._evm);
                    break;
                case "EnableContextMenu":
                case "EnableDrag":
                case "EnableDrop":
                case "EnableBookmark":
                case "EnableMultiSelect":
                    ExplorerParameterType prop;
                    if (Enum.TryParse<ExplorerParameterType>(args.Property.Name, true, out prop))
                        exp.setVMProperty(prop, (bool)args.NewValue);
                    break;

            }


        }

        private T getVMProperty<T>(ExplorerParameterType property)
        {
            ParameterDic pd = new ParameterDic();
            _evm.Commands.Execute(UIScriptCommands.ExplorerGetParameter(property, "{OutputValue}"), pd);
            return pd.GetValue<T>("{OutputValue}");
        }

        private void setVMProperty<T>(ExplorerParameterType property, T value)
        {
            ParameterDic pd = new ParameterDic();
            pd.SetValue("{PropertyValue}", value, false);
            _evm.Commands.Execute(UIScriptCommands.ExplorerSetParameter(property, "{PropertyValue}"), pd);
        }

        #endregion

        #region Data

        protected IWindowManager _wm;
        protected IEventAggregator _events;
        protected IExplorerViewModel _evm;

        #endregion

        #region Public Properties

        public IExplorerViewModel ViewModel { get { return _evm; } }

        public IWindowManager WindowManager { get { return _wm; } set { _wm = value; } }
        public IEventAggregator Events { get { return _events; } set { _events = value; } }

        public static DependencyProperty EnableContextMenuProperty = DependencyProperty.Register("EnableContextMenu",
             typeof(bool), typeof(Explorer), new PropertyMetadata(true, OnPropertiesChanged));

        public bool EnableContextMenu
        {
            get { return (bool)GetValue(EnableContextMenuProperty); }
            set { SetValue(EnableContextMenuProperty, value); }
        }

        public static DependencyProperty EnableDragProperty = DependencyProperty.Register("EnableDrag",
            typeof(bool), typeof(Explorer), new PropertyMetadata(true, OnPropertiesChanged));

        public bool EnableDrag
        {
            get { return (bool)GetValue(EnableDragProperty); }
            set { SetValue(EnableDragProperty, value); }
        }

        public static DependencyProperty EnableDropProperty = DependencyProperty.Register("EnableDrop",
            typeof(bool), typeof(Explorer), new PropertyMetadata(true, OnPropertiesChanged));

        public bool EnableDrop
        {
            get { return (bool)GetValue(EnableDropProperty); }
            set { SetValue(EnableDropProperty, value); }
        }

        public static DependencyProperty EnableMultiSelectProperty = DependencyProperty.Register("EnableMultiSelect",
            typeof(bool), typeof(Explorer), new PropertyMetadata(true, OnPropertiesChanged));

        public bool EnableMultiSelect
        {
            get { return (bool)GetValue(EnableMultiSelectProperty); }
            set { SetValue(EnableMultiSelectProperty, value); }
        }

        public static DependencyProperty EnableBookmarkProperty = DependencyProperty.Register("EnableBookmark",
            typeof(bool), typeof(Explorer), new PropertyMetadata(false, OnPropertiesChanged));

        public bool EnableBookmark
        {
            get { return (bool)GetValue(EnableBookmarkProperty); }
            set { SetValue(EnableBookmarkProperty, value); }
        }

        public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode",
            typeof(ExplorerMode), typeof(Explorer), new PropertyMetadata(ExplorerMode.Unknown, OnPropertiesChanged));

        public ExplorerMode Mode
        {
            get { return (ExplorerMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }


        public static DependencyProperty CurrentDirectoryProperty = DependencyProperty.Register("CurrentDirectory",
            typeof(IEntryModel), typeof(Explorer));

        public IEntryModel CurrentDirectory
        {
            get { return (IEntryModel)GetValue(CurrentDirectoryProperty); }
            set { SetValue(CurrentDirectoryProperty, value); }
        }

        public static DependencyProperty SelectedEntriesProperty = DependencyProperty.Register("SelectedEntries",
           typeof(IEntryModel[]), typeof(Explorer));


        public IEntryModel[] SelectedEntries
        {
            get { return (IEntryModel[])GetValue(SelectedEntriesProperty); }
            set { SetValue(SelectedEntriesProperty, value); }
        }

        public static DependencyProperty RootDirectoriesProperty = DependencyProperty.Register("RootDirectories",
            typeof(IEntryModel[]), typeof(Explorer), new PropertyMetadata(null, OnPropertiesChanged));

        public IEntryModel[] RootDirectories
        {
            get { return (IEntryModel[])GetValue(RootDirectoriesProperty); }
            set { SetValue(RootDirectoriesProperty, value); }
        }



        #endregion

    }
}
