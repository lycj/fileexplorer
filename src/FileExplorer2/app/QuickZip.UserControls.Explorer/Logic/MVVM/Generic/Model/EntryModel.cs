using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using QuickZip.IO.COFE;
using System.Windows.Media;
using QuickZip.Converters;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.MVVM.Model
{    

    public abstract class EntryModel : Cinch.ValidatingObject
    {
        #region Data

        private string _label, _name = null, _parseName = null;
        private string _hint = null;     
        private string _entryTypeName;
        private bool _isRenaming = false;
        private bool _isReadOnly = false;
        private bool _isVirtual = false;
        private DateTime _creationTime, _lastWriteTime, _lastAccessTime;
        private long _length = -1;
        private int _customPosition = -1;

        #endregion

        #region Methods

        /// <summary>
        /// Refresh all properties
        /// </summary>
        /// <param name="fullRename">Whether refresh all properties (or filename only)</param>
        public abstract void Refresh(bool full = true);
        public abstract void Delete();
        public abstract void Rename(string newName);

        public override bool Equals(object obj)
        {
            if (obj is EntryModel)
                return ParseName.Equals((obj as EntryModel).ParseName);
            return false;
        }

        public override int GetHashCode()
        {
            return ParseName.GetHashCode();
        }

        #endregion


        #region Public Properties

        //public EventHandler OnRefreshNeeded = (o, e) => { };
        //public EventHandler OnParentRefreshNeeded = (o, e) => { };
        //public EventHandler OnChildRefreshNeeded = (o, e) => { };

        #region EntryTypeName
        static PropertyChangedEventArgs entryTypeNameChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.EntryTypeName);
        /// <summary>
        /// Type of embedded entry.
        /// </summary>
        public string EntryTypeName
        {
            get { return _entryTypeName; }
            protected set { _entryTypeName = value; NotifyPropertyChanged(entryTypeNameChangeArgs); }
        }

        #endregion

        public int CustomPosition { get { return _customPosition; } set { _customPosition = value; NotifyPropertyChanged("CustomPosition"); } }

        #region Name, IsRenaming

        static PropertyChangedEventArgs nameChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.Name);
        /// <summary>
        /// Entry Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(nameChangeArgs);
                //if (_name == null)
                //{
                //    _name = value;
                //}

                //else
                //    try
                //    {
                //        IsRenaming = true;
                //        Rename(value);
                //    }
                //    finally
                //    {
                //        IsRenaming = false;
                //        NotifyPropertyChanged(nameChangeArgs);
                //    }
            }
        }

        /// <summary>
        /// IsRenaming
        /// </summary>
        static PropertyChangedEventArgs isRenamingChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.IsRenaming);

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set
            {
                _isRenaming = value;
                NotifyPropertyChanged(isRenamingChangeArgs);
            }
        }
        #endregion

        #region Label, ParseName, FullName

        static PropertyChangedEventArgs labekChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.Label);
        /// <summary>
        /// Displayable text for the entry.
        /// </summary>
        public string Label
        {
            get { return _label; }
            protected set { _label = value; NotifyPropertyChanged(labekChangeArgs); }
        }

        static PropertyChangedEventArgs parseNameChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.ParseName);
        /// <summary>
        /// Parsable name of the entry.
        /// </summary>
        public string ParseName
        {
            get { return _parseName; }
            protected set { _parseName = value; NotifyPropertyChanged(parseNameChangeArgs); }
        }        

        #endregion

        #region IsReadOnly
        static PropertyChangedEventArgs isReadOnlyChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.IsReadonly);
        /// <summary>
        /// If the current item (not it's childs) renamable.
        /// </summary>
        public bool IsReadonly
        {
            get { return _isReadOnly; }
            protected set { _isReadOnly = value; NotifyPropertyChanged(isReadOnlyChangeArgs); }
        }

        public bool IsEditable { get { return !IsReadonly; } }
        
        #endregion

        #region IsVirtual
        static PropertyChangedEventArgs isVirtualChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.IsVirtual);
        /// <summary>
        /// If the current item (not it's childs) renamable.
        /// </summary>
        public bool IsVirtual
        {
            get { return _isVirtual; }
            protected set { _isVirtual = value; NotifyPropertyChanged(isVirtualChangeArgs); }
        }

        #endregion

        #region Time related
        /// <summary>
        /// CreationTime
        /// </summary>
        static PropertyChangedEventArgs CreationTimeChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.CreationTime);

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set
            {
                _creationTime = value;
                NotifyPropertyChanged(CreationTimeChangeArgs);
            }
        }

        /// <summary>
        /// LastWriteTime
        /// </summary>
        static PropertyChangedEventArgs lastWriteTimeChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.LastWriteTime);

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set
            {
                _lastWriteTime = value;
                NotifyPropertyChanged(lastWriteTimeChangeArgs);
            }
        }

        /// <summary>
        /// LastAccessTime
        /// </summary>
        static PropertyChangedEventArgs lastAccessTimeChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.LastAccessTime);

        public DateTime LastAccessTime
        {
            get { return _lastAccessTime; }
            set
            {
                _lastAccessTime = value;
                NotifyPropertyChanged(lastAccessTimeChangeArgs);
            }
        }
        #endregion

        #region Length

        static PropertyChangedEventArgs lengthChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.Length);
        /// <summary>
        /// File size
        /// </summary>
        public long Length
        {
            get { return _length; }
            set
            {
                _length = value;
                NotifyPropertyChanged(lengthChangeArgs);
            }
        }
        #endregion

        #region ToolTip

        static PropertyChangedEventArgs hintChangeArgs =
            ObservableHelper.CreateArgs<EntryModel>(x => x.Hint);
        /// <summary>
        /// ToolTip for the entry, if available.
        /// </summary>
        public string Hint
        {
            get { return _hint; }
            set
            {
                _hint = value;
                NotifyPropertyChanged(hintChangeArgs);
            }
        }
        #endregion

        #endregion


    }


    #region Defined

    public enum OpenResult { none, failed, openFolder }
    #endregion
    
    public abstract class EntryModel<FI, DI, FSI> : EntryModel, IEquatable<EntryModel<FI, DI, FSI>>
        where FI : FSI
        where DI : FSI
    {

        #region Constructor
        public EntryModel(FSI embeddedEntry)
            : base()
        {            
            _embeddedEntry = embeddedEntry;
            EntryTypeName = embeddedEntry.GetType().ToString();
            Refresh(true);

        }

        #endregion

        #region Methods

        public abstract EntryViewModel<FI, DI, FSI> ToViewModel(Profile<FI, DI, FSI> profile);
        public abstract bool HasParent(DI directory);
        public abstract bool Equals(EntryModel<FI, DI, FSI> other);
        public abstract bool Open();
        protected abstract DI getParent();

        //protected void Invalidate(bool smallChanges = true)
        //{
        //    Refresh(!smallChanges);

        //    if (smallChanges)
        //        OnRefreshNeeded(this, new EventArgs());
        //    else OnParentRefreshNeeded(this, new EventArgs());
        //}
        

        #endregion

        #region Data

        //private Profile<FI, DI, FSI> _profile;
        private FSI _embeddedEntry;
        private DI _parent;
        private bool _canDropOverItself = false;
        private bool _isEncrypted = false;

        #endregion

        #region Public Properties
        
        public FSI EmbeddedEntry { get { return _embeddedEntry; } }
        public bool IsEncrypted { get { return _isEncrypted; } protected set { _isEncrypted = value; NotifyPropertyChanged("IsEncrypted"); } }
        public bool IsDroppableOverItself { get { return _canDropOverItself; }
            protected set { _canDropOverItself = value; NotifyPropertyChanged("IsDroppableOverItself"); }
        }
        public DI Parent
        {
            get { if (_parent == null) _parent = getParent(); return _parent; }            
        }


        #endregion


    }
}
