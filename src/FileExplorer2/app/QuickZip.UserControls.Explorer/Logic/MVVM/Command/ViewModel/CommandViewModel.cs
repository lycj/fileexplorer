using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Command.Model;
using System.Windows.Input;
using System.ComponentModel;
using Cinch;

namespace QuickZip.UserControls.MVVM.Command.ViewModel
{
    public class CommandViewModel : Cinch.ViewModelBase, IComparable, IComparable<CommandViewModel>
    {
        #region Constructor

        public CommandViewModel(CommandModel embeddedModel)
        {
            EmbeddedModel = embeddedModel;
            EmbeddedModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "Value":
                            NotifyPropertyChanged("SliderValue");
                            break;

                    }
                };

            _showHeaderText = !String.IsNullOrEmpty(embeddedModel.Header);

            if (embeddedModel.IsExecutable)
                setupCommands();

            setupBackgroundWorker();
            bgworker_getSubActions.WorkerArgument = (EmbeddedModel as DirectoryCommandModel);
            bgworker_getSubActions.RunBackgroundTask();

        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (EmbeddedModel is GenericCommandModel)
            {
                GenericCommandModel gcm = EmbeddedModel as GenericCommandModel;
                return String.Format("{0} - {1}", gcm.Header, gcm.Command.GetHashCode());    
            }
            else return base.ToString();
        }

        private void setupCommands()
        {
            if (EmbeddedModel is GenericCommandModel && (EmbeddedModel as GenericCommandModel).Command != null)
            {
                DoWorkCommand = (EmbeddedModel as GenericCommandModel).Command;
                DoWorkCommandParameter = (EmbeddedModel as GenericCommandModel).CommandParameter;
            }
            else
                DoWorkCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => EmbeddedModel.CanExecute(x),
                    ExecuteDelegate = x => EmbeddedModel.Execute(x)
                };
        }

        private void setupBackgroundWorker()
        {
            bgworker_getSubActions = new BackgroundTaskManager<DirectoryCommandModel, CommandViewModel[]>()
            {
                TaskFunc = (dcm) =>
                    {
                        if (dcm == null)
                            return new CommandViewModel[] { };

                        List<CommandViewModel> retList = new List<CommandViewModel>();
                        foreach (var m in dcm.GetSubActions())
                            retList.Add(m.ToViewModel());
                        return retList.ToArray();
                    },
                CompletionAction = (list) =>
                    {
                        _subActions.Clear();
                        foreach (CommandViewModel vm in list)
                            _subActions.Add(vm);
                    }

            };
        }

        public int CompareTo(CommandViewModel other)
        {
            if (other != null)
                return this.EmbeddedModel.CompareTo(other.EmbeddedModel);
            return 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is CommandViewModel)
                return CompareTo(obj as CommandViewModel);
            return 0;
        }

        #endregion

        #region Data

        private ICommand _doWorkCommand = null;
        private object _doWorkCommandParameter = null;
        private bool _showHeaderText = true;
        private BackgroundTaskManager<DirectoryCommandModel, CommandViewModel[]>
            bgworker_getSubActions = null;

        private DispatcherNotifiedObservableCollection<CommandViewModel> _subActions =
            new DispatcherNotifiedObservableCollection<CommandViewModel>();

        #endregion

        #region Public Properties

        public CommandModel EmbeddedModel { get; private set; }


        #region Slider
        public bool ShowSlider { get { return EmbeddedModel is SliderCommandModel; } }
        public int SliderMaximum { get { return ShowSlider ? (EmbeddedModel as SliderCommandModel).SliderMaximum : 100; } }
        public int SliderMinimum { get { return ShowSlider ? (EmbeddedModel as SliderCommandModel).SliderMinimum : 0; } }
        public int SliderValue
        {
            get { return ShowSlider ? (EmbeddedModel as SliderCommandModel).Value : 0; }
            set { if (ShowSlider) (EmbeddedModel as SliderCommandModel).Value = value; NotifyPropertyChanged("SliderValue"); }
        }
        public int SliderStep
        {
            get
            {
                return (EmbeddedModel is SelectorItemActionModel<int>) ?
                    (EmbeddedModel as SelectorItemActionModel<int>).StoredValue : 10;
            }
        }
        public int ItemHeight { get { return 0; } }
        public bool IsStepStop
        {
            get
            {
                return (EmbeddedModel is SelectorItemActionModel<int>) ?
                    (EmbeddedModel as SelectorItemActionModel<int>).IsStepStop : false;
            }
        }

        #endregion

        public bool IsSeparator { get { return EmbeddedModel is SeparatorCommandModel; } }
        public int ExtraItemHeight { get { return EmbeddedModel.ExtraItemHeight; } }

        public System.Windows.HorizontalAlignment Alignment
        {
            get { return (EmbeddedModel.IsRightAligned) ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Stretch; }
        }

        public DispatcherNotifiedObservableCollection<CommandViewModel> SubActions { get { return _subActions; } }

        public bool ShowHeaderText { get { return _showHeaderText; } }

        static PropertyChangedEventArgs doWorkCommandChangeArgs =
         ObservableHelper.CreateArgs<CommandViewModel>(x => x.DoWorkCommand);

        public ICommand DoWorkCommand
        {
            get { return _doWorkCommand; }
            set
            {
                _doWorkCommand = value;
                NotifyPropertyChanged(doWorkCommandChangeArgs);
                NotifyPropertyChanged(isCommandEnabledChangeArgs);
            }
        }

        static PropertyChangedEventArgs doWorkCommandParameterChangeArgs =
         ObservableHelper.CreateArgs<CommandViewModel>(x => x.DoWorkCommandParameter);

        public object DoWorkCommandParameter
        {
            get { return _doWorkCommandParameter; }
            set
            {
                _doWorkCommandParameter = value; NotifyPropertyChanged(doWorkCommandParameterChangeArgs);
                NotifyPropertyChanged(isCommandEnabledChangeArgs);
            }
        }

        static PropertyChangedEventArgs isCommandEnabledChangeArgs =
         ObservableHelper.CreateArgs<CommandViewModel>(x => x.IsCommandEnabled);

        public bool IsCommandEnabled
        {
            get { return

                !(EmbeddedModel is GenericCommandModel) ||
                (_doWorkCommand != null ? _doWorkCommand.CanExecute(_doWorkCommandParameter) : false); }
        }


        #endregion


    }
}
