using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System;
namespace QuickZip.Logic
{
    public static class ModelCommands
    {
        //private static RoutedUICommand _goPrevCommand = new RoutedUICommand("GoPrev", "goPrev", typeof(ModelCommands));
        //private static RoutedUICommand _goNextCommand = new RoutedUICommand("GoNext", "goNext", typeof(ModelCommands));
        //private static RoutedUICommand _closeTabCommand = new RoutedUICommand("Close Tab", "closeTab", typeof(ModelCommands));
        private static RoutedUICommand _newTabCommand = new RoutedUICommand("Tab", "newTab", typeof(ModelCommands));
        private static RoutedUICommand _switchViewsCommand = new RoutedUICommand("Views", "switchviews", typeof(ModelCommands));
        private static RoutedUICommand _togglePreviewCommand = new RoutedUICommand("Preview Frame", "togglePreview", typeof(ModelCommands));
        private static RoutedUICommand _toggleDetailsCommand = new RoutedUICommand("Details Frame", "toggleDetails", typeof(ModelCommands));
        private static RoutedUICommand _toggleGuideCommand = new RoutedUICommand("Guide Frame", "toggleGuide", typeof(ModelCommands));
        private static RoutedUICommand _clearWorkListCommand = new RoutedUICommand("Clear", "clearWorkList", typeof(ModelCommands));

        public static RoutedUICommand NewTab { get { return _newTabCommand; } }
        public static RoutedUICommand SwitchViews { get { return _newTabCommand; } }
        public static RoutedUICommand TogglePreview { get { return _togglePreviewCommand; } }
        public static RoutedUICommand ToggleDetails { get { return _toggleDetailsCommand; } }
        public static RoutedUICommand ToggleGuide { get { return _toggleGuideCommand; } }
        public static RoutedUICommand ClearWorkList { get { return _clearWorkListCommand; } }

        private static Dispatcher _dispatcher;

        static ModelCommands()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            //CommandManager.RegisterClassCommandBinding(typeof(Window),
            //    new CommandBinding(_goPrevCommand, ExecuteGoPrevCommand, CanExecuteGoPrevCommand));
            //CommandManager.RegisterClassCommandBinding(typeof(Window),
            //    new CommandBinding(_goNextCommand, ExecuteGoNextCommand, CanExecuteGoNextCommand));                
        }


        public static void RegisterGesture(Type hostType, RoutedUICommand command, InputGesture inputGesture)
        {
            CommandManager.RegisterClassInputBinding(hostType, new InputBinding(command, inputGesture));
        }



        #region GoPrevCommand

        //public static RoutedUICommand GoPrev
        //{
        //    get { return _goPrevCommand; }
        //}

        //public static RoutedUICommand GoNext
        //{
        //    get { return _goNextCommand; }
        //}

        //public static RoutedUICommand CloseTab
        //{
        //    get { return _closeTabCommand; }
        //}

        //public static RoutedUICommand NewTab
        //{
        //    get { return _newTabCommand; }
        //}



        #endregion

        //public static RoutedUICommand GoPrev
        //{
        //    get { return _goPrevCommand.Command; }
        //}

        //private class GoPrevCommand : CommandModel
        //{
        //    public GoPrevCommand()
        //    {                    
        //    }

        //    public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        //    {
        //        e.CanExecute = false;
        //        if (e.Parameter is ExplorerViewModel)
        //            e.CanExecute = (e.Parameter as ExplorerViewModel).CanGoPrev;
        //        else
        //            if (e.Parameter is TabbedExplorerModel)
        //                e.CanExecute = (e.Parameter as TabbedExplorerModel).CanGoPrev;                    
        //        e.Handled = true;
        //    }

        //    public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        //    {
        //        if (e.Parameter is ExplorerViewModel)
        //            (e.Parameter as ExplorerViewModel).GoPrev();
        //        else
        //            if (e.Parameter is TabbedExplorerModel)
        //                (e.Parameter as TabbedExplorerModel).GoPrev();
        //    }
        //}

        //private class GoNextCommand : CommandModel
        //{
        //    public GoNextCommand()
        //    {
        //    }

        //    public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
        //    {
        //        e.CanExecute = false;
        //        if (e.Parameter is ExplorerViewModel)
        //            e.CanExecute = (e.Parameter as ExplorerViewModel).CanGoNext;
        //        else
        //            if (e.Parameter is TabbedExplorerModel)
        //                e.CanExecute = (e.Parameter as TabbedExplorerModel).CanGoNext;
        //        e.Handled = true;
        //    }

        //    public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
        //    {
        //        if (e.Parameter is ExplorerViewModel)
        //            (e.Parameter as ExplorerViewModel).GoNext();
        //        else
        //            if (e.Parameter is TabbedExplorerModel)
        //                (e.Parameter as TabbedExplorerModel).GoNext();
        //    }
        //}

        ////http://blogs.conchango.com/richardgriffin/archive/2007/02/23/WPF-Commands-a-scenic-tour-part-I.aspx
        //private static Dispatcher _dispatcher;
        //private static RoutedUICommand _addCommand = new RoutedUICommand("Add", "add", typeof(ModelCommands));
        //private static RoutedUICommand _removeCommand = new RoutedUICommand("Remove", "remove", typeof(ModelCommands));

        //public static RoutedUICommand Add
        //{
        //    get { return _addCommand; }
        //}


        //public static RoutedUICommand Remove
        //{
        //    get { return _removeCommand; }
        //}


        //static ModelCommands()
        //{
        //    _dispatcher = Dispatcher.CurrentDispatcher;

        //    //Application.Current.MainWindow.CommandBindings.Add(
        //    //    new CommandBinding(_addCommand, ExecuteAddCommand, CanExecuteAddCommand));

        //    //Application.Current.MainWindow.CommandBindings.Add(
        //    //    new CommandBinding(_removeCommand, ExecuteRemoveCommand, CanExecuteRemoveCommand));

        //    //CommandManager.RegisterClassCommandBinding(typeof(ListView), new CommandBinding(_addCommand, ExecuteNavgiateForwardCommand, CanExecuteNavigateForwardCommand));
        //    //CommandManager.RegisterClassCommandBinding(typeof(ListView), new CommandBinding(_backwardCommand, ExecuteNavgiateBackwardCommand, CanExecuteNavigateBackwardCommand));
        //}

        //public static void CanExecuteAddCommand(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    // Handler to provide mechanism for determining when the add command can be executed.
        //    e.Handled = true;
        //}

        //public static void CanExecuteRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    // Handler to provide mechanism for determining when the remove command can be executed.
        //    e.Handled = true;
        //}



        //public static void ExecuteAddCommand(object sender, ExecutedRoutedEventArgs e)
        //{
        //    // Handler to actually execute the code to perform the add command
        //    //add your code here to handle
        //}


        //public static void ExecuteRemoveCommand(object sender, ExecutedRoutedEventArgs e)
        //{
        //    // Handler to actually execute the code to perform the remove command
        //    //add your code here to handle
        //} 
    }
}