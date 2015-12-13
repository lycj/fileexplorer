using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.Windows;
using QuickZip.UserControls.Logic.Tools.DragnDrop;

namespace QuickZip.UserControls.MVVM.Model
{
    [Flags]
    public enum AddActions : int
    {
        None = 1 << 0,
        Copy = 1 << 1,
        Link = 1 << 2,
        Move = 1 << 3, 
        All = AddActions.Copy | AddActions.Link | AddActions.Move
    }

    public abstract class DirectoryModel<FI, DI, FSI> : EntryModel<FI, DI, FSI>
        where DI : FSI
        where FI : FSI
    {        
        protected enum DirPropertyEnum { isChildReadOnly }

        #region Constructor

        public DirectoryModel(DI embeddedDir)
            : base(embeddedDir)
        {

        }

        #endregion
        

        #region Methods

        protected abstract IEnumerable<EntryModel<FI, DI, FSI>> getSubEntries(string filter = "*", bool directory = true, bool file = true);        
        public abstract void Delete(FSI[] entries);        
        
        public IEnumerable<EntryModel<FI, DI, FSI>> GetSubEntries(bool forceReload = false, string filter = "*", bool directory = true, bool file = true)
        {
            if (!forceReload && _subEntriesList != null)
            {
                List<EntryModel<FI, DI, FSI>> _subEntriesListClone = new List<EntryModel<FI, DI, FSI>>();
                lock (_subEntriesList)
                    _subEntriesListClone.AddRange(_subEntriesList);

                foreach (EntryModel<FI, DI, FSI> subEntry in _subEntriesListClone)
                       if ((subEntry is FileModel<FI, DI, FSI> && file) ||
                        subEntry is DirectoryModel<FI, DI, FSI> && directory)
                            yield return subEntry;
                yield break;
            }
            else
            {
                List<EntryModel<FI, DI, FSI>> retList = new List<EntryModel<FI, DI, FSI>>();
                IEnumerator<EntryModel<FI, DI, FSI>> enumerator = getSubEntries(filter, true, true).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Current is FileModel<FI, DI, FSI> && file) || 
                        enumerator.Current is DirectoryModel<FI, DI, FSI> && directory)
                        yield return enumerator.Current;
                    retList.Add(enumerator.Current);
                }

                _subEntriesList = retList;
                yield break;
            }
        }

        //protected void InvalidateChild()
        //{
        //    OnChildRefreshNeeded(this, new EventArgs());
        //}

        public abstract bool HasChild(FSI entry);

        public bool EqualsOrHasChild(EntryModel<FI, DI, FSI> entryModel)
        {
            if (entryModel == null) return false;
            return this.Equals(entryModel) || HasChild(entryModel.EmbeddedEntry);
        }

        public override ViewModel.EntryViewModel<FI, DI, FSI> ToViewModel(Profile<FI, DI, FSI> profile)
        {
            return new ViewModel.DirectoryViewModel<FI, DI, FSI>(profile, this) { };
        }

        public ViewModel.DirectoryViewModel<FI, DI, FSI> ToDirectoryViewModel(Profile<FI, DI, FSI> profile, ViewModel.DirectoryViewModel<FI, DI, FSI> parentViewModel = null)
        {
            return new ViewModel.DirectoryViewModel<FI, DI, FSI>(profile, this) { ParentViewModel = parentViewModel };
        }      

        #endregion

        #region Data

        private List<EntryModel<FI, DI, FSI>> _subEntriesList;
        private bool _hasSubDirectories = true;
        private bool _isSupportAdd = false;
        private AddActions _supportedAddAction = AddActions.Copy;
        private bool _isLink = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// If IsLink, DirectoryTree will not list it's subdirectory.
        /// </summary>
        public bool IsLink { get { return _isLink; } protected set { _isLink = value; NotifyPropertyChanged("IsLink"); } }
        public DI EmbeddedDirectory { get { return (DI)EmbeddedEntry; } }
        public bool IsSupportAdd { get { return _isSupportAdd; } protected set { _isSupportAdd = value; } }
        
        internal bool IsCached { get { return _subEntriesList != null; } }

        #region HasSubDirectories
        static PropertyChangedEventArgs hasSubDirectoriesChangeArgs =
            ObservableHelper.CreateArgs<DirectoryModel<FI, DI, FSI>>(x => x.HasSubDirectories);
        /// <summary>
        /// If the current directory has sub directories. (default true)
        /// </summary>
        public bool HasSubDirectories
        {
            get { return _hasSubDirectories; }
            protected set { _hasSubDirectories = value; NotifyPropertyChanged(hasSubDirectoriesChangeArgs); }
        }
        #endregion

        #endregion
    }

}
