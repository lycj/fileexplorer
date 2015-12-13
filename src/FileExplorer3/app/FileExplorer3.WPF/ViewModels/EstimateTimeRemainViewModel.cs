using FileExplorer.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels
{
    public interface IEstimateTimeRemainViewModel : INotifyPropertyChanged
    {
        void Reset();
        DateTime StartTime { get; set; }
        TimeSpan RequiredTime { get; }
        int TotalItems { get; set; }
        int ProcessedItems { get; set; }
    }

    public class EstimateTimeRemainViewModel : NotifyPropertyChanged, IEstimateTimeRemainViewModel
    {
        #region Constructors

        public EstimateTimeRemainViewModel()
        {
            _startTimeUtc = DateTime.UtcNow;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _startTimeUtc = DateTime.UtcNow;
            _totalItems = 0;
            _processedItems = 0;

            notifyPropertiesChanged();
        }


        private TimeSpan estimateTime()
        {
            if (_totalItems == 0 || _processedItems == 0)
                return TimeSpan.FromSeconds(0);
            TimeSpan elpasedTime = DateTime.UtcNow.Subtract(_startTimeUtc);
            //if (elpasedTime.TotalSeconds < 5)
            //    return TimeSpan.FromSeconds(0);

            return TimeSpan.FromMilliseconds(
                (float)elpasedTime.TotalMilliseconds / (float)ProcessedItems * (TotalItems - ProcessedItems));
        }

        private void notifyPropertiesChanged()
        {
            NotifyOfPropertyChanged(() => RequiredTime);
            NotifyOfPropertyChanged(() => Progress);
            NotifyOfPropertyChanged(() => IsIndeterminate);
        }

        #endregion

        #region Data

        DateTime _startTimeUtc;
        private int _totalItems;
        private int _processedItems;

        #endregion

        #region Public Properties
        public bool IsIndeterminate { get { return !(TotalItems != 0 && ProcessedItems != 0); } }

        public DateTime StartTime
        {
            get { return _startTimeUtc; }
            set
            {
                _startTimeUtc = value;
                NotifyOfPropertyChanged(() => StartTime);
                notifyPropertiesChanged();
            }
        }
        public TimeSpan RequiredTime { get { return estimateTime(); } }
        public short Progress { get { return (short)(IsIndeterminate ? 0 : Math.Truncate((float)ProcessedItems / TotalItems * 100.0)); } }
        public int TotalItems
        {
            get { return _totalItems; }
            set
            {
                _totalItems = value;
                NotifyOfPropertyChanged(() => TotalItems);
                notifyPropertiesChanged();
            }
        }
        public int ProcessedItems
        {
            get { return _processedItems; }
            set
            {
                _processedItems = value;
                NotifyOfPropertyChanged(() => ProcessedItems);
                notifyPropertiesChanged();
            }
        }


        #endregion


    }
}
