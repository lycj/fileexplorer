using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.MVVM.Model
{


    public class GenericMetadataModel : Cinch.ValidatingObject, IComparable<GenericMetadataModel>, IComparable
    {
        #region Constructor

        protected GenericMetadataModel(Type propertyType, string propertyKey, string propertyLabel, bool isReadOnly = true, int priority = 0)
        {
            PropertyType = propertyType;
            _isReadOnly = isReadOnly;
            _propertyKey = propertyKey;
            _propertyLabel = propertyLabel;
            _isValid = true;
        }

        protected GenericMetadataModel(object value, string propertyKey, string propertyLabel, bool isReadOnly = true, int priority = 0)
        {
            changeValue(value);
            _isReadOnly = isReadOnly;
            _propertyKey = propertyKey;
            _propertyLabel = propertyLabel;
            _isValid = true;
        }

        #endregion

        #region Methods
        public int CompareTo(GenericMetadataModel other)
        {
            if (other == null) return 0;
            return -Priority.CompareTo(other.Priority);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as GenericMetadataModel);
        }

        public MetadataViewModel ToViewModel()
        {
            return new MetadataViewModel(this);
        }

        void changeValue(object value)
        {
            PropertyType = value.GetType();
            _value = value;
            NotifyPropertyChanged("Value");
        }

        #endregion

        #region Data

        private string _propertyKey;
        private string _propertyLabel;
        private bool _isReadOnly;
        private bool _isValid;
        private object _value;
        private bool _occupyFullLine = false;

        #endregion

        #region Public Properties

        public int Priority { get; set; }
        public Type PropertyType { get; private set; }

        public bool OccupyFullLine { get { return _occupyFullLine; } }
        public string PropertyKey { get { return _propertyKey; } }
        public string Header { get { return _propertyLabel; } }
        public bool IsReadOnly { get { return _isReadOnly; } }
        public bool IsValid { get { return _isValid; } protected set { _isValid = value; NotifyPropertyChanged("IsValid"); } }
        public object Value { get { return _value; } set { changeValue(value); } }

        #endregion


    }


    public class GenericMetadataModel<T> : GenericMetadataModel
    {

        #region Constructor

        public GenericMetadataModel(T value, string propertyKey, string propertyLabel = "", bool isReadOnly = true, int priority = 0)
            :base(typeof(T), propertyKey, propertyLabel, isReadOnly, priority)
        {
            
            _value = value;
            
            if (typeof(DateTime).Equals(typeof(T)))
                if (value.ToString() == DateTime.MinValue.ToString())
                    IsValid = false;
        }

        #endregion

        #region Methods

        

        public virtual void ChangeValue(T newVaue)
        {
            return;
            //TO-DO:       
            //if (!IsReadOnly)
            //{
            //    //foreach (EntryModel<FI, DI, FSI> entryModel in AppliedEntryModels)
            //    //    entryModel.ChangeValue(PropertyKey, value, newValue);            
            //    //_value = Value;
            //}

        }

        #endregion

        #region Data

        
        
        private T _value;


        #endregion

        #region Public Properties
      
        public new T Value {
            get { return _value; } 
            
            set { ChangeValue(value); NotifyPropertyChanged("Value"); } }


        #endregion
    }


}
