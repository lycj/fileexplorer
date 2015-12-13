using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    /// <summary>
    /// CommandModel that allow user to pick from a list of int, betweeen min and max
    /// </summary>
    public class SliderCommandModel : SelectorCommandModel<int>
    {
        #region Constructor

        public SliderCommandModel(int min, int max, int def, SelectorItemInfo<int>[] midPoints)
            : base(midPoints)            
        {            
            SliderMinimum = min;
            SliderMaximum = max;
            Value = def;
        }

        protected SliderCommandModel(int min, int max, int def)
        {            
            SliderMinimum = min;
            SliderMaximum = max;
            Value = def;
        }


        #endregion

        #region Methods       
        
        #endregion

        #region Data        

        private int _sliderMin, _sliderMax;

        #endregion

        #region Public Properties

        public int SliderMinimum { get { return _sliderMin; } protected set { _sliderMin = value; NotifyPropertyChanged("SliderMinimum"); } }
        public int SliderMaximum { get { return _sliderMax; } protected set { _sliderMax = value; NotifyPropertyChanged("SliderMaximum"); } }        

        #endregion
    }

    
}
