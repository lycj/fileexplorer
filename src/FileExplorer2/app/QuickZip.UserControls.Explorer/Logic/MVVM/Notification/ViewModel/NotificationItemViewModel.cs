using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Notification.Model;
using Cinch;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using QuickZip.UserControls.MVVM.Command.ViewModel;

namespace QuickZip.UserControls.MVVM.Notification.ViewModel
{
    public class NotificationItemViewModel : Cinch.ViewModelBase
    {
        #region Constructor

        public NotificationItemViewModel(NotificationItemModel embeddedModel)
        {
            _embeddedModel = embeddedModel;
            _commands = new DispatcherNotifiedObservableCollection<CommandViewModel>();

            setupDoWorkCommand();
            setupBackgroundWorker();

            bgWorker_loadCommands.RunBackgroundTask();
            _embeddedModel.OnCommandsChanged += (o, e) => { bgWorker_loadCommands.RunBackgroundTask(); };
            _embeddedModel.OnRemoveRequested += (o, e) => { if (ParentModel != null) ParentModel.RemovePending(); };
            _embeddedModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "NotificationMode" :
                            if (ProgressBrushDic.ContainsKey(_embeddedModel.NotificationMode))
                                ProgressForeground = ProgressBrushDic[_embeddedModel.NotificationMode];
                            if (_embeddedModel.NotificationMode == NotificationMode.Error &&
                                PercentCompleted < 33)
                                PercentCompleted = 33;
                            break;
                        case "PercentCompleted" :
                            if (_embeddedModel.NotificationMode != NotificationMode.Error)
                                PercentCompleted = _embeddedModel.PercentCompleted;
                            break;
                    }
                };
        }

        #endregion

        #region Methods

        private void setupBackgroundWorker()
        {
            bgWorker_loadCommands = new BackgroundTaskManager<object, CommandViewModel[]>(
                (x) =>
                {
                    var vms = from m in EmbeddedModel.GetCommands() select m.ToViewModel();
                    return vms.ToArray();
                },
                (vms) =>
                {
                    lock (_commands)
                    {
                        _commands.Clear();
                        var vmsList = new List<CommandViewModel>(vms);
                        vmsList.Sort();
                        foreach (var vm in vmsList)
                        {
                            //vm.ToString();
                            _commands.Add(vm);
                        }
                    }
                });
        }

        private void setupDoWorkCommand()
        {
            DoWorkCommand = new SimpleCommand
            {
                CanExecuteDelegate = x =>
                {
                    return EmbeddedModel.CanDoWork &&
                        EmbeddedModel.NotificationMode != NotificationMode.Processing;
                },
                ExecuteDelegate = x =>
                {
                    EmbeddedModel.DoWork();
                }
            };
        }
        

        #endregion

        #region Data

        public NotificationSourceViewModel _parentModel;
        private NotificationItemModel _embeddedModel;
        private ushort _percentCompleted;
        private DispatcherNotifiedObservableCollection<CommandViewModel> _commands;
        private BackgroundTaskManager<object, CommandViewModel[]> bgWorker_loadCommands;
        private ICommand _doWorkCommand;

        public static Dictionary<NotificationMode, Brush> ProgressBrushDic = new Dictionary<NotificationMode, Brush>()
        {
            { NotificationMode.Aborted, Brushes.Yellow },
            { NotificationMode.Completed, Brushes.CadetBlue },
            { NotificationMode.Error, Brushes.Red },
            { NotificationMode.PendingToRemove, Brushes.CadetBlue },
            { NotificationMode.Processing, Brushes.CadetBlue },
            { NotificationMode.Waiting, Brushes.CadetBlue }
        };

        public static Brush ErrorProgressBrush = Brushes.DarkRed;

        private Brush _progressForeground = ProgressBrushDic[NotificationMode.Waiting];

        #endregion

        #region Public Properties
        
        public NotificationSourceViewModel ParentModel { get { return _parentModel; } set { _parentModel = value; NotifyPropertyChanged("ParentModel"); } }
        public NotificationItemModel EmbeddedModel { get { return _embeddedModel; } private set { _embeddedModel = value; NotifyPropertyChanged("EmbeddedModel"); } }

        public DispatcherNotifiedObservableCollection<CommandViewModel> Commands { get { return _commands; } }

        static PropertyChangedEventArgs doWorkCommandChangeArgs =
         ObservableHelper.CreateArgs<NotificationItemViewModel>(x => x.DoWorkCommand);

        public ICommand DoWorkCommand
        {
            get { return _doWorkCommand; }
            set { _doWorkCommand = value; NotifyPropertyChanged(doWorkCommandChangeArgs); }
        }

        static PropertyChangedEventArgs progressForegroundChangeArgs =
       ObservableHelper.CreateArgs<NotificationItemViewModel>(x => x.ProgressForeground);
        public Brush ProgressForeground
        {
            get { return _progressForeground; }
            set { _progressForeground = value; NotifyPropertyChanged(progressForegroundChangeArgs); }
        }

        static PropertyChangedEventArgs percentCompletedChangeArgs =
          ObservableHelper.CreateArgs<NotificationItemViewModel>(x => x.PercentCompleted);
        public ushort PercentCompleted
        {
            get { return _percentCompleted; }
            set { _percentCompleted = value; NotifyPropertyChanged(percentCompletedChangeArgs); }
        }

        

        #endregion


    }
}
