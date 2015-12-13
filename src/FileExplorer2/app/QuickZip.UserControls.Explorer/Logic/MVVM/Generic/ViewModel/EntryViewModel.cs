using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;
using System.Windows.Media;
using QuickZip.Converters;
using System.ComponentModel;
using Cinch;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using QuickZip.UserControls.Logic.Tools.IconExtractor;

namespace QuickZip.UserControls.MVVM.ViewModel
{
    public abstract class EntryViewModel<FI, DI, FSI> : Cinch.ViewModelBase, IComparable
        where FI : FSI
        where DI : FSI
    {

        #region Defines
        private static string _slowInitialIconKey = ".1234567";
        protected class ThumbnailInfo
        {
            public IconSize iconsize;
            public System.Drawing.Size outputSize;
            public WriteableBitmap bitmap;
            public FSI entry;
            public ThumbnailInfo(WriteableBitmap b, FSI e, IconSize size, System.Drawing.Size outSize)
            {
                bitmap = b;
                entry = e;
                iconsize = size;
                outputSize = outSize;
            }
        }
        #endregion

        #region Constructor
        public EntryViewModel(Profile<FI, DI, FSI> profile, EntryModel<FI, DI, FSI> embeddedModel,
            DirectoryViewModel<FI, DI, FSI> parentViewModel = null)
            : base()
        {
            _profile = profile;
            _embeddedModel = embeddedModel;
            _parentViewModel = parentViewModel; 
            CustomPosition = embeddedModel.CustomPosition;
            setupLazyIconLoading();

            _embeddedModel.PropertyChanged += (PropertyChangedEventHandler)((o, e) =>
            {
                switch (e.PropertyName)
                {
                    //case "CustomPosition": CustomPosition = embeddedModel.CustomPosition; break;
                    //case "IsReadOnly": NotifyPropertyChanged(isEditableChangeArgs); break;
                }
            });
        }
        #endregion

        #region Data

        private DirectoryViewModel<FI, DI, FSI> _parentViewModel = null;
        private Profile<FI, DI, FSI> _profile;
        private EntryModel<FI, DI, FSI> _embeddedModel;
        private IconExtractor<FSI> _iconExtractor;
        private bool _isSelected = false;
        private Tuple<Lazy<ImageSource>, Lazy<ImageSource>> _jumbo, _largeIcon, _icon, _smallIcon;
        private int _customPosition = -1;

        #endregion

        #region Methods

        #region Initialization method - setupLazyIconLoading
        /// <summary>
        /// Setup the Lazy Icon, which loads only when requested.
        /// </summary>
        private void setupLazyIconLoading()
        {
            //int workerThreads, completionPortThreads;
            _iconExtractor = _profile.IconExtractor;
            bool isDir = this is DirectoryViewModel<FI, DI, FSI>;

            Func<IconSize, Lazy<ImageSource>> createFastFunc = (size) =>
            {
                try
                {
                    return new Lazy<ImageSource>(() =>
                    {
                        return ImageTools.loadBitmap(
                        _iconExtractor.GetIcon(EmbeddedModel.EmbeddedEntry, size, isDir, true));
                    }, LazyThreadSafetyMode.PublicationOnly);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("createFastFunc " + ex.Message);
                    return null;
                }
            };

            Func<IconSize, bool, WaitCallback> createSlowFuncCallBack = null;

            createSlowFuncCallBack = (size, fast) =>
            {
                return (state) =>
                {
                    ThumbnailInfo workingInfo = (ThumbnailInfo)state;
                    WriteableBitmap workingBitmap = workingInfo.bitmap;
                    IconSize workingSize = workingInfo.iconsize;

                    try
                    {


                        BitmapSource storedBitmap = ImageTools.loadBitmap(
                            _iconExtractor.GetIcon(EmbeddedModel.EmbeddedEntry, size, isDir, fast));
                        if (storedBitmap != null)
                        {
                            //ImageTools.clearBackground(workingBitmap, true);                            
                            ImageTools.copyBitmap(storedBitmap, workingBitmap, true, 0, false);
                        }

                        if (fast) //Do slow phase
                        {
                            ThreadPool.QueueUserWorkItem(createSlowFuncCallBack(size, false), workingInfo);
                        }
                        else storedBitmap.Freeze();
                    }
                    catch (Exception ex)
                    { Debug.WriteLine("PollThumbnailCallback " + ex.Message + "(" + workingInfo.entry.ToString() + ")"); }

                };
            };

            Func<IconSize, Lazy<ImageSource>> createSlowFunc = (size) =>
            {
                return new Lazy<ImageSource>(() =>
                {
                    try
                    {
                        if (!_iconExtractor.IsDelayLoading(EmbeddedModel.EmbeddedEntry, size))
                            return ImageTools.loadBitmap(
                        _iconExtractor.GetIcon(EmbeddedModel.EmbeddedEntry, size, isDir, true)); ;

                        Size pixelSize = IconExtractor.IconSizeToSize(size);
                        Bitmap defaultBitmap =
                            new Bitmap(_iconExtractor.GetIcon(EmbeddedModel.EmbeddedEntry, _slowInitialIconKey, false, size));
                        //    new Bitmap(32, 32);
                        //using (Graphics g = Graphics.FromImage(defaultBitmap))
                        //g.FillRectangle(System.Drawing.Brushes.Black, new Rectangle(5,5,5,5));

                        if (_iconExtractor.IsDelayLoading(EmbeddedModel.EmbeddedEntry, size))
                        {
                            WriteableBitmap bitmap = new WriteableBitmap(ImageTools.loadBitmap(defaultBitmap));
                            ThumbnailInfo info = new ThumbnailInfo(bitmap, EmbeddedModel.EmbeddedEntry, size, new System.Drawing.Size(bitmap.PixelWidth, bitmap.PixelHeight));

                            ThreadPool.QueueUserWorkItem(createSlowFuncCallBack(size, true), info);

                            return bitmap;
                        }
                        else
                            return ImageTools.loadBitmap(defaultBitmap);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("createSlowFunc " + ex.Message);
                        return null;
                    }

                }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            };


            SmallIcon = new Tuple<Lazy<ImageSource>, Lazy<ImageSource>>(
                createFastFunc(IconSize.small), createSlowFunc(IconSize.small));
            Icon = new Tuple<Lazy<ImageSource>, Lazy<ImageSource>>(
               createFastFunc(IconSize.large), createSlowFunc(IconSize.large));
            LargeIcon = new Tuple<Lazy<ImageSource>, Lazy<ImageSource>>(
               createFastFunc(IconSize.extraLarge), createSlowFunc(IconSize.extraLarge));
            JumboIcon = new Tuple<Lazy<ImageSource>, Lazy<ImageSource>>(
               createFastFunc(IconSize.jumbo), createSlowFunc(IconSize.jumbo));
        }
        #endregion

        #region Comparsion methods - Equals, GetHashCode, ToString

        public override bool Equals(object obj)
        {
            if (_embeddedModel == null)
                return false;

            if (obj is EntryViewModel<FI, DI, FSI>)
                return EmbeddedModel.Equals((obj as EntryViewModel<FI, DI, FSI>).EmbeddedModel);
            return false;
        }

        public override int GetHashCode()
        {
            if (_embeddedModel == null)
                return 0;
            return EmbeddedModel.GetHashCode();
        }

        public override string ToString()
        {
            return "VM:" + EmbeddedModel.ToString();
        }
        #endregion

        /// <summary>
        /// Broadcast filesystem changes, so it can update on the UI, return if the specified parseName applied to current node.
        /// </summary>
        /// <param name="parseName"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        internal virtual bool BroadcastChange(string parseName, WatcherChangeTypesEx changeType)
        {
            if (EmbeddedModel != null)
                switch (changeType)
                {
                    //case WatcherChangeTypesEx.Created:
                    //case WatcherChangeTypesEx.Deleted:                    
                    //    if (!parseName.EndsWith(".7z.tmp"))
                    //        if (EmbeddedModel.ParseName.Equals(PathEx.GetDirectoryName(parseName)))
                    //        {
                    //            Refresh(true);
                    //            return true;
                    //        }
                    //    break;
                    default:
                        if (EmbeddedModel.ParseName.Equals(parseName))
                        {
                            Refresh(true);
                            return true;
                        }
                        break;
                }
            return false;
        }

        public virtual void Refresh(bool full = true)
        {
            if (EmbeddedModel != null)
                EmbeddedModel.Refresh(full);
        }



        /// <summary>
        /// When unloaded (not disposed), unload resource consuming variables.
        /// </summary>
        public virtual void OnUnload()
        {
            setupLazyIconLoading();
            _loaded = false;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _iconExtractor = null;
            _embeddedModel = null;

            JumboIcon = null;
            LargeIcon = null;
            Icon = null;
            SmallIcon = null;
        }

        #endregion

        #region Data

        private bool _loaded = true;
        private bool _isRenaming = false;
        private bool _isEditing = false;

        #endregion

        #region Public Properties

        internal Profile<FI, DI, FSI> Profile { get { return _profile; } }

        /// <summary>
        /// Embedded EntryModel
        /// </summary>
        public EntryModel<FI, DI, FSI> EmbeddedModel { get { return _embeddedModel; } }


        static PropertyChangedEventArgs nameChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.Name);

        public string Name
        {
            get { return _embeddedModel.Name; }
            set
            {
                try
                {
                    IsRenaming = true;
                    EmbeddedModel.Rename(value);
                }
                finally
                {
                    IsRenaming = false;
                    NotifyPropertyChanged(nameChangeArgs);
                }
            }
        }

        /// <summary>
        /// IsRenaming, Notify the UI the Rename operation is working
        /// </summary>
        static PropertyChangedEventArgs isRenamingChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.IsRenaming);

        public bool IsRenaming
        {
            get { return _isRenaming; }
            protected set
            {
                _isRenaming = value;
                NotifyPropertyChanged(isRenamingChangeArgs);
            }
        }

        /// <summary>
        /// IsEditing, Toggle edit mode (if available)
        /// </summary>
        static PropertyChangedEventArgs isEditingChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.IsEditing);

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (!value || !EmbeddedModel.IsReadonly)
                {
                    Debug.WriteLine(value);
                    _isEditing = value;
                    NotifyPropertyChanged(isEditingChangeArgs);
                }
            }
        }

        /// <summary>
        /// Owner of the current EntryViewModel, not necessary to have value.
        /// </summary>
        public DirectoryViewModel<FI, DI, FSI> ParentViewModel { get { return _parentViewModel; } set { _parentViewModel = value; } }

        /// <summary>
        /// On Demand Icon in Jumbo size (80x80), invoked by Item1.Value (faster, non-threaded) or Item2.Value (slower, threaded)        
        /// </summary>
        public Tuple<Lazy<ImageSource>, Lazy<ImageSource>> JumboIcon { get { return _jumbo; } protected set { _jumbo = value; } }
        /// <summary>
        /// On Demand Icon in Large size (64x64), invoked by Item1.Value (faster, non-threaded) or Item2.Value (slower, threaded)        
        /// </summary>
        public Tuple<Lazy<ImageSource>, Lazy<ImageSource>> LargeIcon { get { return _largeIcon; } protected set { _largeIcon = value; } }
        /// <summary>
        /// On Demand Icon in Regular size (32x32), invoked by Item1.Value (faster, non-threaded) or Item2.Value (slower, threaded)        
        /// </summary>
        public Tuple<Lazy<ImageSource>, Lazy<ImageSource>> Icon { get { return _icon; } protected set { _icon = value; } }
        /// <summary>
        /// On Demand Icon in Small size (16x16), invoked by Item1.Value (faster, non-threaded) or Item2.Value (slower, threaded)        
        /// </summary>
        public Tuple<Lazy<ImageSource>, Lazy<ImageSource>> SmallIcon { get { return _smallIcon; } protected set { _smallIcon = value; } }


        #region ToolTip
        static PropertyChangedEventArgs toolTipChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.ToolTip);

        /// <summary>
        /// Hint of the embeddedmodel.
        /// </summary>
        public string ToolTip
        {
            get { return EmbeddedModel.Hint; }
            //set { NotifyPropertyChanged(toolTipChangeArgs); }
        }
        #endregion


        #region IsSelected
        static PropertyChangedEventArgs isSelectedChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.IsSelected);
        /// <summary>
        /// If the current item (not it's childs) editable.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(isSelectedChangeArgs); }
        }
        #endregion


        #region CustomPosition
        static PropertyChangedEventArgs CustomPositionChangeArgs =
            ObservableHelper.CreateArgs<EntryViewModel<FI, DI, FSI>>(x => x.CustomPosition);
        /// <summary>
        /// Custom postion in view model level.
        /// </summary>
        public int CustomPosition
        {
            get { return _customPosition; }
            set { _customPosition = value; NotifyPropertyChanged(CustomPositionChangeArgs); }
        }
        #endregion

        #endregion


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return 0;
        }

        #endregion


    }






}
