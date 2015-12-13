using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Cinch;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.Models;
using FileExplorer.Defines;
using System.Threading;

namespace FileExplorer.WPF.ViewModels
{
    public class CommandViewModel : PropertyChangedBase, ICommandViewModel
    {
        #region Constructor

        public CommandViewModel(ICommandModel commandModel, IParameterDicConverter parameterDicConverter, ICommandViewModel parentCommandViewModel = null)
        {
            CommandModel = commandModel;
            _parentCommandViewModel = parentCommandViewModel;
            _parameterDicConverter = parameterDicConverter;

            if (CommandModel != null)
                if (CommandModel is IRoutedCommandModel && (CommandModel as IRoutedCommandModel).RoutedCommand != null)
                    CommandBinding = ScriptCommandBinding.ForRoutedUICommand((CommandModel as IRoutedCommandModel).RoutedCommand);
                else
                    CommandBinding = ScriptCommandBinding.FromScriptCommand(
                        ApplicationCommands.NotACommand, commandModel, cm => cm.Command,
                        parameterDicConverter, /*ParameterDicConverters.ConvertUIParameter, */ScriptBindingScope.Local);

            CommandModel.PropertyChanged += (o, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "IsChecked":
                        case "Symbol":
                        case "HeaderIconExtractor":
                            RefreshIcon();
                            break;
                        case "SubCommands":
                            SubCommands.LoadAsync(UpdateMode.Replace, true);
                            RefreshIcon();
                            break;
                        case "IsEnabled":
                        case "IsVisibleOnMenu":
                        case "IsVisibleOnToolbar":
                            NotifyOfPropertyChange(() => IsVisibleOnMenu);
                            NotifyOfPropertyChange(() => IsVisibleOnToolbar);
                            break;
                    }
                };

            RefreshIcon();

            if (commandModel is IDirectoryCommandModel)
            {
                IDirectoryCommandModel directoryModel = CommandModel as IDirectoryCommandModel;
                SubCommands = new EntriesHelper<ICommandViewModel>(
                    (cts) => Task.Run<IEnumerable<ICommandViewModel>>(
                        () => directoryModel.SubCommands.Select(c => (ICommandViewModel)new CommandViewModel(c, parameterDicConverter, this))));
                SubCommands.LoadAsync(UpdateMode.Replace, false);
            }
        }

        #endregion

        #region Methods


        public void RefreshIcon()
        {
            if (CommandModel.IsChecked)
                Icon = null;
            if (CommandModel.HeaderIconExtractor != null)
            {
                byte[] bytes = AsyncUtils.RunSync(() =>
                    CommandModel.HeaderIconExtractor.GetIconBytesForModelAsync(CommandModel,
                        CancellationToken.None));
                
                if (bytes != null && bytes.Length > 0)
                    Icon = new System.Windows.Controls.Image()
                    {
                        Source =
                            BitmapSourceUtils.CreateBitmapSourceFromBitmap(bytes),
                        MaxWidth = 16,
                        MaxHeight = 16

                    };
            }
        }

        private ToolbarItemType getCommandType()
        {
            if (CommandModel is ISeparatorModel)
                return ToolbarItemType.Separator;

            if (CommandModel is ISelectorCommandModel)
                return (CommandModel as ISelectorCommandModel).IsComboBox ? ToolbarItemType.Combo : ToolbarItemType.Check;

            if (_parentCommandViewModel != null &&
                (_parentCommandViewModel.CommandType == ToolbarItemType.Combo || _parentCommandViewModel.CommandType == ToolbarItemType.Check)
                )
                return (CommandModel as ISelectorCommandModel).IsComboBox ? ToolbarItemType.Combo : ToolbarItemType.Check;

            if (CommandModel is ISliderCommandModel)
                return ToolbarItemType.MenuButton;

            if (CommandModel is IDirectoryCommandModel)
                return ToolbarItemType.MenuButton;

            return ToolbarItemType.Button;
        }

        public int CompareTo(ICommandViewModel other)
        {
            if (other != null)
                return this.CommandModel.CompareTo(other.CommandModel);
            return 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is ICommandViewModel)
                return CompareTo(obj as ICommandViewModel);
            return 0;
        }


        #endregion

        #region Data

        private object _icon = null;
        private ICommandModel _commandModel;
        private IScriptCommandBinding _commandBinding;
        private bool _subCommandsLoaded = false;
        private EntriesHelper<ICommandViewModel> _subCommands;
        private ICommandViewModel _parentCommandViewModel;
        private IParameterDicConverter _parameterDicConverter;

        #endregion

        #region Public Properties

        public IEntriesHelper<ICommandViewModel> SubCommands { get; private set; }

        public ICommandModel CommandModel { get { return _commandModel; } set { _commandModel = value; NotifyOfPropertyChange(() => CommandModel); } }

        public IScriptCommandBinding CommandBinding { get { return _commandBinding; } set { _commandBinding = value; NotifyOfPropertyChange(() => CommandBinding); } }

        public ToolbarItemType CommandType { get { return getCommandType(); } }

        public Object Icon
        {
            get
            {
                return _icon is System.Windows.Controls.Image ?
                    new System.Windows.Controls.Image()
                    {
                        MaxWidth = 16,
                        MaxHeight = 16,
                        Source = (_icon as System.Windows.Controls.Image).Source
                    } : null;
                ;

            }
            set { _icon = value; NotifyOfPropertyChange(() => Icon); }
        }

        public bool IsVisibleInContextMenu { get { return CommandType != ToolbarItemType.Separator; } }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return CommandModel is ISliderStepCommandModel ?
                    (CommandModel as ISliderStepCommandModel).VerticalAlignment :
                    VerticalAlignment.Center;
            }
        }

        public bool IsSliderEnabled { get { return _commandModel is ISliderCommandModel; } }
        public bool IsSliderStep
        {
            get
            {
                return _parentCommandViewModel != null &&
                    _parentCommandViewModel.CommandModel is ISliderCommandModel &&
                    CommandType != ToolbarItemType.Separator;
            }
        }

        public bool IsVisibleOnMenu { get { return _commandModel.IsEnabled && _commandModel.IsVisibleOnMenu; } }
        public bool IsVisibleOnToolbar { get { return _commandModel.IsEnabled && _commandModel.IsVisibleOnToolbar; } }

        #endregion

    }
}
