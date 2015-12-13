using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    public class NullProgresViewModel : IProgress<TransferProgress>
    {
        public static NullProgresViewModel Instance = new NullProgresViewModel();
        public void Report(TransferProgress value)
        {
        }
    }

    public class ProgressDialogViewModel : Screen, IProgress<TransferProgress>
    {
        #region Constructor

        public ProgressDialogViewModel(ParameterDic pd, CancellationTokenSource cts = null)
        {
            _pd = pd;
            if (_pd.ContainsKey("ProgressHeader"))
                DisplayName = Header = _pd["ProgressHeader"] as string;
            _cts = cts ?? new CancellationTokenSource();
            _timeRemain = new EstimateTimeRemainViewModel();
            _cancelCommand = new RelayCommand((o) => _cts.Cancel());
            _closeCommand = new RelayCommand((o) => TryClose());
        }

        #endregion

        #region Methods

        public void Report(TransferProgress value)
        {
            _lastProgress = value;
            switch (value.Type)
            {
                case ProgressType.Running:
                    if (value.Source != null) Source = value.Source;
                    if (value.Destination != null) Destination = value.Destination;
                    if (value.SourcePathHelper != null) SourcePathHelper = value.SourcePathHelper;
                    if (value.DestinationPathHelper != null) DestinationPathHelper = value.DestinationPathHelper;

                    if (value.TotalEntriesIncrement.HasValue)
                    {
                        TotalEntries += value.TotalEntriesIncrement.Value;
                        TimeRemain.TotalItems = TotalEntries;
                    }
                    if (value.ProcessedEntriesIncrement.HasValue)
                    {
                        ProcessedEntries += value.ProcessedEntriesIncrement.Value;
                        TimeRemain.ProcessedItems = ProcessedEntries;
                    }
                    if (value.CurrentProgressPercent.HasValue)
                        CurrentEntryProgress = value.CurrentProgressPercent.Value;

                    string src = Source == null ? null : SourcePathHelper == null ? Source : SourcePathHelper.GetFileName(Source);
                    string dest = Destination == null ? null : DestinationPathHelper == null ? Destination : DestinationPathHelper.GetDirectoryName(Destination);
                    Message = value.Message ?? String.Format("From [b]{0}[/b] To [b]{1}[/b]", src, dest);
                    IsCancelEnabled = true;
                    break;
                case ProgressType.Error :
                    if (value.Message != null)
                        Message = value.Message;
                    IsCancelEnabled = false;
                    break;
            }
            IsCompleted = value.Type == ProgressType.Completed || value.Type == ProgressType.Error;
        }

        private short getOverallProgress()
        {
            return (short)(Math.Truncate((_processedEntries * 100.0) / (_totalEntries * 100.0) * 100) + CurrentEntryProgress);
        }

        //public string getMessage()
        //{
        //    if (_lastProgress == null)
        //        return "";


        //}

        #endregion

        #region Data

        private TransferProgress _lastProgress = null;
        private RelayCommand _cancelCommand, _closeCommand;
        private Int32 _totalEntries = 0, _processedEntries = 0;
        private short _currentEntryProgress = 0;
        private CancellationTokenSource _cts;
        private ParameterDic _pd;
        private string _header;
        private string _source;
        private string _destination;
        private string _message;
        private bool _isCompleted = false, _isCancelEnabled = false;
        private IPathHelper _sourcePathHelper;
        private IPathHelper _destinationPathHelper;
        private IEstimateTimeRemainViewModel _timeRemain;

        #endregion

        #region Public Properties

        public RelayCommand CancelCommand { get { return _cancelCommand; } }
        public RelayCommand CloseCommand { get { return _closeCommand; } }
        public IEstimateTimeRemainViewModel TimeRemain { get { return _timeRemain; } set { _timeRemain = value; NotifyOfPropertyChange(() => TimeRemain); } }
        public string Header { get { return _header; } set { _header = value; NotifyOfPropertyChange(() => Header); } }
        public string Message { get { return _message; } set { _message = value; NotifyOfPropertyChange(() => Message); } }

        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                NotifyOfPropertyChange(() => Source);
                NotifyOfPropertyChange(() => Message);
            }
        }
        public IPathHelper SourcePathHelper
        {
            get { return _sourcePathHelper; }
            set
            {
                _sourcePathHelper = value; NotifyOfPropertyChange(() => SourcePathHelper);
                NotifyOfPropertyChange(() => Message);
            }
        }

        public string Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                NotifyOfPropertyChange(() => Destination);
                NotifyOfPropertyChange(() => Message);
            }
        }

        public IPathHelper DestinationPathHelper
        {
            get { return _destinationPathHelper; }
            set
            {
                _destinationPathHelper = value; NotifyOfPropertyChange(() => DestinationPathHelper);
                NotifyOfPropertyChange(() => Message);
            }
        }

        public Int32 TotalEntries
        {
            get { return _totalEntries; }
            set
            {
                _totalEntries = value;
                NotifyOfPropertyChange(() => TotalEntries);
                NotifyOfPropertyChange(() => Progress);
                NotifyOfPropertyChange(() => UnprocessedEntries);
            }
        }
        public Int32 ProcessedEntries
        {
            get { return _processedEntries; }
            set
            {
                _processedEntries = value;
                NotifyOfPropertyChange(() => ProcessedEntries);
                NotifyOfPropertyChange(() => Progress);
                NotifyOfPropertyChange(() => UnprocessedEntries);
            }
        }
        public Int32 UnprocessedEntries { get { return Math.Abs(_totalEntries - _processedEntries); } }

        public short CurrentEntryProgress { get { return _currentEntryProgress; } set { _currentEntryProgress = value; NotifyOfPropertyChange(() => CurrentEntryProgress); NotifyOfPropertyChange(() => Progress); } }
        public short Progress { get { return getOverallProgress(); } }

        public CancellationToken CancellationToken { get { return _cts.Token; } }
        public CancellationTokenSource CancellationTokenSource { get { return _cts; } }

        public bool IsCompleted { get { return _isCompleted; } set { if (_isCompleted != value) { _isCompleted = value; NotifyOfPropertyChange(() => IsCompleted); } } }
        public bool IsCancelEnabled { get { return _isCancelEnabled; } set { if (_isCancelEnabled != value) { _isCancelEnabled = value; NotifyOfPropertyChange(() => IsCancelEnabled); } } }

        #endregion

    }
}
