using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.Models.Bookmark;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FileExplorer.WPF.ViewModels
{
    public class AddBookmarksViewModel : Screen, IHandle<DirectoryChangedEvent>
    {
        #region fields

        public enum AddBookmarkState
        {
            Inactive, //IsVisible is false
            Added,    //IsVisible is true, just added bookmark.
            Found,    //IsVisible is true, just found bookmark link to that directory.
            Changed   //IsVisible is true, after Added or Found, user changed label or directory.
        }

        private IEntryModel _currentDirectory;
        private IEntryModel _currentBookmarkDirectory;
        private bool _isVisible;
        private string _bookmarkLabel;
        private bool _isBookmarkEnabled;
        private BookmarkModel _lastAddedLink;
        private AddBookmarkState _state = AddBookmarkState.Inactive;
        private bool _isBookmarked;
        private IEventAggregator _events;

        #endregion

        #region constructors

        public AddBookmarksViewModel(BookmarkProfile bProfile, IWindowManager windowManager, IEventAggregator events)
        {
            Profile = bProfile;
            _events = events;
            events.Subscribe(this);


            AddBookmarkCommand = new RelayCommand(e => AddBookmark());
            GotoBookmarkDirectoryCommand = new RelayCommand(e => GotoBookmarkDirectory());
            //CurrentBookmarkDirectory = Profile.RootModel;
        }

        #endregion

        #region events

        #endregion

        #region properties

        public RelayCommand AddBookmarkCommand { get; private set; }
        public RelayCommand GotoBookmarkDirectoryCommand { get; private set; }
        public BookmarkProfile Profile { get; set; }
        public bool IsVisible { get { return _isVisible; } set { setIsVisible(value); } }

        public AddBookmarkState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => State);
                //NotifyOfPropertyChange(() => Header);
                NotifyOfPropertyChange(() => IsUpdateBookmarkEnabled);
            }
        }

        public bool IsUpdateBookmarkEnabled { get { return State == AddBookmarkState.Changed; } }
        public bool IsBookmarked { get { return _isBookmarked; } set { _isBookmarked = value; 
            NotifyOfPropertyChange(() => IsBookmarked);
            NotifyOfPropertyChange(() => BookmarkFillBrush);
        }
        }
        public bool IsBookmarkEnabled { get { return _isBookmarkEnabled; } set { _isBookmarkEnabled = value; NotifyOfPropertyChange(() => IsBookmarkEnabled); } }
        public Brush BookmarkFillBrush { get { return IsBookmarked ? SystemColors.ActiveCaptionBrush : Brushes.Transparent; } } 

        public ICommandManager Commands { get; private set; }

        public IEntryModel CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                NotifyOfPropertyChange(() => CurrentDirectory);
            }
        }

        public IEntryModel CurrentBookmarkDirectory
        {
            get { return _currentBookmarkDirectory; }
            set
            {
                if (_currentBookmarkDirectory != value)
                {
                    _currentBookmarkDirectory = value;
                    NotifyOfPropertyChange(() => CurrentBookmarkDirectory);

                    if (State == AddBookmarkState.Added || State == AddBookmarkState.Found)
                        State = AddBookmarkState.Changed;
                }
            }
        }

        //public string Header
        //{
        //    get
        //    {
        //        switch (State)
        //        {
        //            case AddBookmarkState.Inactive: return "";
        //            case AddBookmarkState.Added: return "Bookmark added.";
        //            case AddBookmarkState.Found: return "Bookmark already added.";
        //            case AddBookmarkState.Changed: return "Press Update to update bookmark.";
        //            default: return "";
        //        }

        //    }
        //}

        public string BookmarkLabel
        {
            get { return _bookmarkLabel; }
            set
            {
                _bookmarkLabel = value;
                NotifyOfPropertyChange(() => BookmarkLabel);

                if (State == AddBookmarkState.Added || State == AddBookmarkState.Found)
                    State = AddBookmarkState.Changed;
            }
        }


        #endregion

        #region methods

        public void Handle(DirectoryChangedEvent message)
        {
            _lastAddedLink = null;
            CurrentDirectory = message.NewModel;
            if (CurrentDirectory != null)
            {
                BookmarkLabel = CurrentDirectory.Label;
                IsBookmarkEnabled = !(CurrentDirectory is BookmarkModel);
                IsBookmarked = Profile.AllBookmarks.Any(bm => CurrentDirectory.FullPath == bm.LinkPath);
            }
            else IsBookmarkEnabled = false;
        }

        private void setIsVisible(bool value)
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);

                if (value)
                    AddBookmark();
                else
                {
                    if (_lastAddedLink != null) //Not removed.
                        UpdateBookmark();
                    State = AddBookmarkState.Inactive;
                }
            }
        }

        public async Task AddBookmark(string label = null)
        {


            if (CurrentDirectory != null)
            {
                var foundBookmark = Profile.AllBookmarks.FirstOrDefault(em => em.LinkPath == CurrentDirectory.FullPath);

                if (foundBookmark != null)
                {
                    _lastAddedLink = foundBookmark;
                    CurrentBookmarkDirectory = _lastAddedLink.Parent;
                    BookmarkLabel = _lastAddedLink.Label;
                    State = AddBookmarkState.Found;
                }
                else
                {
                    _lastAddedLink = (CurrentBookmarkDirectory as BookmarkModel)
                        .AddLink(label ?? CurrentDirectory.Label, CurrentDirectory.FullPath);
                    State = AddBookmarkState.Added;
                    IsBookmarked = true;
                }

            }
        }

        public async Task GotoBookmarkDirectory()
        {
            if (State == AddBookmarkState.Changed)
                UpdateBookmark();
            IsVisible = false;
            await _events.PublishOnUIThreadAsync(new DirectoryChangedEvent(this, CurrentBookmarkDirectory, CurrentDirectory));
        }

        public void UpdateBookmark()
        {
            if (_lastAddedLink != null)
            {
                (_lastAddedLink.Parent as BookmarkModel).Remove(_lastAddedLink.Label);
                AddBookmark(BookmarkLabel);
                IsVisible = false;
            }
        }

        public void RemoveBookmark()
        {
            if (_lastAddedLink != null)
            {
                (_lastAddedLink.Parent as BookmarkModel).Remove(_lastAddedLink.Label);
                _lastAddedLink = null;
                IsVisible = false;
                IsBookmarked = false; 
            }
        }


        protected override void OnActivate()
        {
            base.OnActivate();
        }

        #endregion


    }
}
