using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.Script;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public class SeparatorCommandModel : CommandModel, ISeparatorModel
    {
        public SeparatorCommandModel() : base() { IsEnabled = true; IsVisibleOnMenu = true; IsVisibleOnToolbar = true; }


    }

    public class DirectoryCommandModel : CommandModel, IDirectoryCommandModel
    {
        #region Constructor

        public DirectoryCommandModel(IScriptCommand command, params ICommandModel[] commandModels)
            : base(command)
        {
            SubCommands = new List<ICommandModel>(commandModels);
        }

        public DirectoryCommandModel(RoutedUICommand routedCommand, params ICommandModel[] commandModels)
            : base(routedCommand)
        {
            SubCommands = new List<ICommandModel>(commandModels);
        }

        public DirectoryCommandModel(params ICommandModel[] commandModels)
            : base()
        {
            SubCommands = new List<ICommandModel>(commandModels);
        }

        #endregion

        #region Methods


        #endregion

        #region Data

        List<ICommandModel> _subCommands;

        #endregion

        #region Public Properties

        public List<ICommandModel> SubCommands { get { return _subCommands; } set { _subCommands = value; NotifyOfPropertyChange(() => SubCommands); } }

        #endregion
    }

    public class SliderCommandModel : DirectoryCommandModel, ISliderCommandModel
    {

        #region Constructor

        private void init(ICommandModel[] commandModels)
        {
            SliderMinimum = Int32.MaxValue;
            SliderMaximum = Int32.MinValue;
            var sliderCommandModels = commandModels.Where(cm => cm is SliderStepCommandModel).Cast<SliderStepCommandModel>();
            if (sliderCommandModels.Count() > 2)
            {
                sliderCommandModels.First().VerticalAlignment = VerticalAlignment.Top;
                sliderCommandModels.Last().VerticalAlignment = VerticalAlignment.Bottom;
            }

            foreach (ISliderStepCommandModel scm in sliderCommandModels)
            {
                if (scm.Command == null)
                    scm.Command = new SimpleScriptCommand("",
                        pd =>
                        {
                            SliderValue = scm.SliderStep; return ResultCommand.NoError;
                        });

                if (scm.SliderStep < SliderMinimum) SliderMinimum = scm.SliderStep;
                if (scm.SliderStep > SliderMaximum) SliderMaximum = scm.SliderStep;
            }
        }

        public SliderCommandModel(IScriptCommand command, params ICommandModel[] commandModels)
            : base(command, commandModels)
        {
            init(commandModels);
        }


        public SliderCommandModel(RoutedUICommand routedCommand, params ICommandModel[] commandModels)
            : base(routedCommand, commandModels)
        {
            init(commandModels);
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        private int _sliderMaximum;
        private int _sliderMinimum;
        private int _sliderValue;

        #endregion

        #region Public Properties

        public int SliderMaximum { get { return _sliderMaximum; } set { _sliderMaximum = value; NotifyOfPropertyChange(() => SliderMaximum); } }
        public int SliderMinimum { get { return _sliderMinimum; } set { _sliderMinimum = value; NotifyOfPropertyChange(() => SliderMinimum); } }
        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (value > SliderMaximum)
                    value = SliderMaximum;
                if (value < SliderMinimum)
                    value = SliderMinimum;
                _sliderValue = value; NotifyOfPropertyChange(() => SliderValue);
            }
        }

        #endregion
    }

    public class SliderStepCommandModel : CommandModel, ISliderStepCommandModel
    {

        #region Constructor

        public SliderStepCommandModel(IScriptCommand command = null)
            : base(command)
        {
            IsEnabled = true;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return String.Format("SliderStepCommandModel {0} Step:{1} Height:{2}", Header, _sliderStep, _itemHeight);
        }

        #endregion

        #region Data

        private int _sliderStep;
        private double? _itemHeight;
        private VerticalAlignment _verticalAlignment = VerticalAlignment.Center;

        #endregion

        #region Public Properties

        public SliderCommandModel SliderCommandModel { get; set; }

        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } set { _verticalAlignment = value; NotifyOfPropertyChange(() => VerticalAlignment); } }

        public int SliderStep { get { return _sliderStep; } set { _sliderStep = value; NotifyOfPropertyChange(() => SliderStep); } }

        public double? ItemHeight
        {
            get { return _itemHeight; }
            set { _itemHeight = value; NotifyOfPropertyChange(() => ItemHeight); }
        }

        #endregion
    }
}
