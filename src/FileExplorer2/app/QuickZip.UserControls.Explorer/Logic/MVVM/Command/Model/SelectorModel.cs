using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;
using System.Diagnostics;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    /// <summary>
    /// CommandModel that allow user to pick from a list of values
    /// </summary>
    public class SelectorCommandModel<T> : DirectoryCommandModel
    {
        #region Constructor

        public SelectorCommandModel(SelectorItemInfo<T>[] subItems)
        {
            _subItems = subItems;
            
        }

        protected SelectorCommandModel()
        {
            
        }


        #endregion

        #region Methods
        
        private void changeValue(T newValue)
        {
            //Debug.WriteLine(newValue);
            if (_value == null || !_value.Equals(newValue))
            {
                _value = newValue;
                NotifyPropertyChanged("Value");                
                OnValueChanged();
            }
        }

        protected virtual void OnValueChanged()
        {

        }

        public override IEnumerable<CommandModel> GetSubActions()
        {
            foreach (var subItem in _subItems)
                yield return new SelectorItemActionModel<T>(this, subItem);
        }    

        #endregion

        #region Data

        private T _value = default(T);
        private SelectorItemInfo<T>[] _subItems;


        #endregion

        #region Public Properties

        public T Value { get { return _value; } 
            set { changeValue(value); } }

        #endregion
    }

    public class SelectorItemInfo<T>
    {
        public string Header { get; set; }
        public Bitmap HeaderIcon { get; set; }
        public T Value { get; set; }        
    }

    public class SelectorItemActionModel<T> : CommandModel
    {
        #region Constructor

        internal SelectorItemActionModel(SelectorCommandModel<T> parentModel, SelectorItemInfo<T> itemInfo)
        {
            _parentModel = parentModel;
            Header = itemInfo.Header;
            HeaderIcon = itemInfo.HeaderIcon;            
            StoredValue = itemInfo.Value;
        }

        #endregion

        #region Methods

        public override void Execute(object param)
        {
            base.Execute(param);
            _parentModel.Value = _value;            
        }

        #endregion

        #region Data

        private T _value;
        private SelectorCommandModel<T> _parentModel;
        private bool _isStepStop;


        #endregion

        #region Public Properties

        public T StoredValue { get { return _value; } set { _value = value; NotifyPropertyChanged("StoredValue"); } }
        public bool IsStepStop { get { return _isStepStop; } set { _isStepStop = value; NotifyPropertyChanged("IsStepStop"); } }

        #endregion
    }
}
