using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    /// <summary>
    /// Contain a list of other GenericCommandModels
    /// </summary>
    public class DirectoryCommandModel : CommandModel
    {
        #region Constructor

        public DirectoryCommandModel()
        {
            //_subActions = new DispatcherNotifiedObservableCollection<GenericCommandModel>();
        }
        

        #endregion

        #region Methods


        public virtual IEnumerable<CommandModel> GetSubActions()
        {
            yield break;
        }
        

        //protected virtual void AddSubActionModel(GenericCommandModel model)
        //{
        //    _subActions.Add(model);
        //    IsDirectory = true;
        //    NotifyPropertyChanged("SubActions");
        //}

        //protected virtual void ClearSubActionModel()
        //{
        //    _subActions.Clear();
        //    IsDirectory = false;
        //    NotifyPropertyChanged("SubActions");
        //}



        #endregion

        #region Data

        //public DispatcherNotifiedObservableCollection<GenericCommandModel> _subActions;
        

        #endregion

        #region Public Properties

        //public DispatcherNotifiedObservableCollection<GenericCommandModel> SubActions { get { return _subActions; } }        

        #endregion

        
    }


    public class PredefinedDirectoryCommandModel : DirectoryCommandModel
    {
        #region Constructor

        public PredefinedDirectoryCommandModel(string header)
        {
            Header = header;
            IsExecutable = false;
        }

        public PredefinedDirectoryCommandModel(string header, CommandModel[] subCommandModels)
        {
            Header = header;
            _subCommandModels.AddRange(subCommandModels);
            IsExecutable = false;
        }
        #endregion

        #region Methods

        public CommandModel this[string header]
        {
            get { foreach (var cm in _subCommandModels) if (cm.Header.Equals(header)) return cm;                                
                return null; } 
        }

        public void AddSubModel(CommandModel subCommandModel)
        {
            _subCommandModels.Add(subCommandModel);
        }

        public override IEnumerable<CommandModel> GetSubActions()
        {
            foreach (CommandModel cm in _subCommandModels)
                yield return cm;
        }

        #endregion

        #region Data

        private List<CommandModel> _subCommandModels = new List<CommandModel>();



        #endregion
    }
}
