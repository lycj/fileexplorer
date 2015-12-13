using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.Windows;
using QuickZip.IO.PIDL.Tools;
using System.IO.Tools;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public class ExViewModel : Cinch.ViewModelBase, IComparable 
    {

        #region Constructor
        internal ExViewModel()
        {

        }

        public ExViewModel(RootModelBase rootModel, Model.ExModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null.");
            EmbeddedModel = model;
            RootModel = rootModel;
            IsDirectory = model.EmbeddedEntry is DirectoryInfoEx;
        }

        public ExViewModel(Model.ExModel model)
        {
            if (model == null)
                throw new ArgumentException("Model cannot be null.");
            EmbeddedModel = model;
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            return this.FullName;
        }

        protected virtual void FetchData(bool refresh)
        {

        }

        protected virtual void OnProgress(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
        {
            if (_rootModel != null)
                _rootModel.RaiseProgressEvent(id, text, work, workStatus, workResult);
        }

        public override bool Equals(object obj)
        {
            if (EmbeddedModel == null)
                return obj is ExViewModel ? (obj as ExViewModel).EmbeddedModel == null : false;

            if (obj is ExViewModel)
                return EmbeddedModel.Equals((obj as ExViewModel).EmbeddedModel);
            return EmbeddedModel.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected void RaiseProgressEvent(uint id, string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
        {
            OnProgress(id, text, work, workStatus, workResult);
        }

        protected uint RaiseProgressEvent(string text, WorkType work, WorkStatusType workStatus, WorkResultType workResult)
        {
            uint newID = ProgressEventArgs.NewID();
            OnProgress(newID, text, work, workStatus, workResult);
            return newID;
        }

        #endregion

        #region Data

        private bool _isDirectory;
        protected RootModelBase _rootModel = null;
        private Model.ExModel _embeddedModel;
        //private string _fullName = null;        

        #endregion

        #region Public Properties
        /// <summary>
        /// EmbeddedModel
        /// </summary>
        static PropertyChangedEventArgs embeddedModelChangeArgs =
            ObservableHelper.CreateArgs<ExViewModel>(x => x.EmbeddedModel);

        public Model.ExModel EmbeddedModel
        {
            get { return _embeddedModel; }
            protected set
            {
                _embeddedModel = value;
                NotifyPropertyChanged(embeddedModelChangeArgs);
                FetchData(true);
            }
        }

        public RootModelBase RootModel
        {
            get { return _rootModel; }
            private set { _rootModel = value; }
        }

        public bool IsDirectory
        {
            get { return _isDirectory; }
            private set { _isDirectory = value; }
        }

        public string Name
        {
            get { return PathEx.GetFileName(EmbeddedModel.FullName); }
        }

        public string FullName
        {
            get { return EmbeddedModel.FullName; }
        }

       
        #endregion


        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is ExViewModel)
            {
                ExViewModel objA = obj as ExViewModel;
                if (this.IsDirectory != objA.IsDirectory)
                    return IsDirectory.CompareTo(objA.IsDirectory);
                else return this.Name.CompareTo(objA.Name);
            }
            return 0;
        }

        #endregion
    }
}
