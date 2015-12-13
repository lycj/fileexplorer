//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using QuickZip.UserControls.MVVM.ViewModel;
//using QuickZip.UserControls.MVVM.Model;
//using QuickZip.UserControls.MVVM;
//using System.Windows.Data;
//using Cinch;
//using System.ComponentModel;
//using System.Windows.Input;

//namespace QuickZip.UserControls.MVVM
//{

//    public class BookmarkExplorerViewModel<FI, DI, FSI> : ExplorerViewModel<FI, DI, FSI>
//        where DI : FSI
//        where FI : FSI
//    {
//        #region Constructor

//        public BookmarkExplorerViewModel(Profile<FI, DI, FSI> profile)
//            : base(profile)
//        {
//            setupCommands();
//        }

//        #endregion

//        #region Methods

//        //public override ICommand GetViewerCommand(string commandName)
//        //{
//        //    switch (commandName)
//        //    {
//        //        case "ToggleBookmark": return CurrentBrowserViewModel.CopyCommand;                
//        //    }
//        //    return null;
//        //}

//        private void setupCommands()
//        {
//            _toggleBookmarkCommand = new SimpleCommand()
//            {
//                CanExecuteDelegate = (x) =>
//                    {
//                        return true;
//                    },
//                ExecuteDelegate = (x) =>
//                    {

//                    }
//            };
//        }

//        protected override void ChangeCurrentBrowserViewModel(EntryModel<FI, DI, FSI> newEntryModel)
//        {
//            base.ChangeCurrentBrowserViewModel(newEntryModel);

//            IsBookmarked = Profile.GetIsBookmarked(newEntryModel);

//        }


//        #endregion

//        #region Data

//        private bool _isBookmarked = false;
//        private SimpleCommand _toggleBookmarkCommand;

//        #endregion

//        #region Public Properties


//        #region IsBookmarked

//        public SimpleCommand ToggleBookmarkCommand { get { return _toggleBookmarkCommand; } }

//        static PropertyChangedEventArgs isBookmarkedChangeArgs =
//          ObservableHelper.CreateArgs<BookmarkExplorerViewModel<FI, DI, FSI>>(x => x.IsBookmarked);

//        public bool IsBookmarked
//        {
//            get { return _isBookmarked; }
//            set
//            {
//                _isBookmarked = value;
//                NotifyPropertyChanged(isBookmarkedChangeArgs);
//            }
//        }
//        #endregion

//        #endregion
//    }
//}
