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
    /// Given a number of profiles, allow user to select a directory.
    /// </summary>
    public class AddDirectoryViewModel : Screen
    {

        #region Constructors

        public AddDirectoryViewModel(IExplorerInitializer initializer, IProfile[] rootProfiles)
        {
            DisplayName = "Map directory";
            _initializer = initializer.Clone();
            _rootProfiles = rootProfiles;
            _selectedPath = null;

        }

        #endregion

        #region Methods

        public void Add()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            SelectedDirectory = null;
            TryClose(false);
        }

        public async Task BrowsePath()
        {
            if (SelectedRootProfile != null)
            {
                IEntryModel rootDirectory = await SelectedRootProfile.ParseAsync("");
                _initializer.RootModels = new IEntryModel[] { rootDirectory };
                var directoryPicker = new DirectoryPickerViewModel(_initializer);

                if (_initializer.WindowManager.ShowDialog(directoryPicker).Value)
                    SelectedDirectory = directoryPicker.SelectedDirectory;


            }
        }

        #endregion

        #region Data

        private IProfile[] _rootProfiles;
        private IProfile _selectedRootProfile;
        private IEntryModel _selectedPath;
        private IExplorerInitializer _initializer;

        #endregion

        #region Public Properties

        public bool CanAdd { get { return _selectedPath != null; } }

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
            }
        }
        public IEntryModel SelectedDirectory
        {
            get { return _selectedPath; }
            set
            {
                _selectedPath = value; 
                NotifyOfPropertyChange(() => SelectedDirectory);
                NotifyOfPropertyChange(() => CanAdd);
            }
        }

        #endregion
    }
}
