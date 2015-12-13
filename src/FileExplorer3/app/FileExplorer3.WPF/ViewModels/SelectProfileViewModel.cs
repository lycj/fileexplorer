using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    /// <summary>
    /// Given a number of profiles, allow user to select one, and assign to SelectedRootProfile.
    /// </summary>
    public class SelectProfileViewModel : Screen
    {

        #region Constructors

        public SelectProfileViewModel(IProfile[] rootProfiles)
        {
            DisplayName = "Map directory";         
            _rootProfiles = rootProfiles;            
        }

        #endregion

        #region Methods

        public void Add()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            SelectedRootProfile = null;
            TryClose(false);
        }

        public void Select()
        {
            TryClose(true);
        }

        #endregion

        #region Data

        private IProfile[] _rootProfiles;
        private IProfile _selectedRootProfile;
        private IEntryModel _selectedPath;
        private IExplorerInitializer _initializer;

        #endregion

        #region Public Properties

        public bool CanSelect { get { return SelectedRootProfile != null; } }

        public IProfile[] RootProfiles
        {
            get { return _rootProfiles; }
            set
            {
                _rootProfiles = value;
                NotifyOfPropertyChange(() => RootProfiles);
            }
        }
        public IProfile SelectedRootProfile
        {
            get { return _selectedRootProfile; }
            set
            {
                _selectedRootProfile = value;
                NotifyOfPropertyChange(() => SelectedRootProfile);
                NotifyOfPropertyChange(() => CanSelect);
            }
        }   

        #endregion
    }
}
