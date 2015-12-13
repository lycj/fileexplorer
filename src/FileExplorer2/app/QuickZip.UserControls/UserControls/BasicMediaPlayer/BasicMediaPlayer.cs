using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;

namespace QuickZip.UserControls
{
    [TemplatePart(Name = "PART_Slider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_PauseNotifier", Type = typeof(UIElement))]
    public class BasicMediaPlayer : ContentControl
    {
        public static Uri NotSupported = new Uri("file://xyz.NotSupported");

        public enum MediaType { mtUnknown, mtImage, mtText, mtWeb, mtAudio, mtVideo, mtPrevHandler, mtNone }

        public static string strSelectFileToPreview = "Select a file to preview.";
        public static string strNoPreviewAvail = "No preview available.";        
        
        public static string imageFilter = ".bmp,.jpg,.jpeg,.png,.gif,.tiff,.tib,.pcx";
        public static string textFilter = ".txt,.pas,.cs,.as,.php,.xml,.css,.idz,.ini";
        public static string webFilter = ".xml,.xaml,.htm,.html,.svg";
        public static string audioFilter = ".wma,.mp3";
        public static string videoFilter = ".wmv,.mp4,.ts";        

        static BasicMediaPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicMediaPlayer),
                new FrameworkPropertyMetadata(typeof(BasicMediaPlayer)));
        }

        public BasicMediaPlayer()
            : base()
        {
            this.CommandBindings.Add(new CommandBinding(MediaCommands.TogglePlayPause, TogglePlayPause, CanExecuteMedia));
            this.CommandBindings.Add(new CommandBinding(MediaCommands.Play, Play, CanExecuteMedia));
            this.CommandBindings.Add(new CommandBinding(MediaCommands.Pause, Pause, CanExecuteMedia));
            if (ShowNullMessage)
                Content = strSelectFileToPreview;
        }

        #region static Methods
        public static MediaType GetMediaType(string path)
        {
            if (path == null)
                return MediaType.mtNone;
            if (path.Equals(NotSupported.AbsolutePath))
                return MediaType.mtUnknown;

            string imageFilter = BasicMediaPlayer.imageFilter.ToLower() + ",";
            string textFilter = BasicMediaPlayer.textFilter.ToLower() + ",";
            string webFilter = BasicMediaPlayer.webFilter.ToLower() + ",";
            string videoFilter = BasicMediaPlayer.videoFilter.ToLower() + ",";
            string audioFilter = BasicMediaPlayer.audioFilter.ToLower() + ",";
            
            string ext = Path.GetExtension(path).ToLower();
            if (!String.IsNullOrEmpty(ext))
            {
                if (imageFilter.IndexOf(ext) != -1)
                    return MediaType.mtImage;
                if (textFilter.IndexOf(ext) != -1)
                    return MediaType.mtText;
                if (webFilter.IndexOf(ext) != -1)
                    return MediaType.mtWeb;
                if (videoFilter.IndexOf(ext) != -1)
                    return MediaType.mtVideo;
                if (audioFilter.IndexOf(ext) != -1)
                    return MediaType.mtAudio;
            }

            if (GetPreviewerSupportFunc(path))
                return MediaType.mtPrevHandler;

            return MediaType.mtUnknown;
        }

        public static MediaType GetMediaType(Uri uri)
        {
            if (uri == null)
                return MediaType.mtNone;
            if (uri.Equals(NotSupported))
                return MediaType.mtUnknown;
            return GetMediaType(uri.AbsolutePath);
        }

        public static bool Support(Uri uri)
        {
            return GetMediaType(uri) != MediaType.mtUnknown;
        }

        public static bool Support(string path)
        {
            return GetMediaType(path) != MediaType.mtUnknown;
        }


        #endregion

        #region Methods
        public void CanExecuteMedia(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaElement != null;
        }
        public void Play()
        {
            if (CurrentMediaType == MediaType.mtAudio || CurrentMediaType == MediaType.mtVideo)
                if (mediaElement != null)
                    mediaElement.Play();
            IsMediaPlaying = true;
        }
        public void Play(object sender, ExecutedRoutedEventArgs e)
        {
            Play();
        }
        public void Pause()
        {
            if (CurrentMediaType == MediaType.mtAudio || CurrentMediaType == MediaType.mtVideo)
                if (mediaElement != null)
                    mediaElement.Pause();
            IsMediaPlaying = false;
        }
        public void Pause(object sender, ExecutedRoutedEventArgs e)
        {
            Pause();
        }

        public void TogglePlayPause()
        {
            if (IsMediaPlaying)
                Pause();
            else Play();
        }

        public void TogglePlayPause(object sender, ExecutedRoutedEventArgs e)
        {
            TogglePlayPause();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mmSlider = (Slider)this.Template.FindName("PART_Slider", this);
            mmSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SeekToMediaPosition);

            pauseNotifier = (UIElement)this.Template.FindName("PART_PauseNotifier", this);
            pauseNotifier.MouseDown += new MouseButtonEventHandler((o, e) => { Play(); });

            innerContainer = (Panel)this.Template.FindName("PART_InnerContainer", this);

            mmTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.1) };
            mmTimer.Tick += new EventHandler((o, e) =>
            {
                MediaElement mele = mediaElement;
                if (mele != null && ShowTimeLine)
                    MediaPosition = mediaElement.Position;
            });
            mmTimer.Start();

            if (Source != null)
                Load(Source);
        }

        #region Load related methods
        private string readText(Uri uri)
        {
            WebRequest req = WebRequest.Create(uri);
            WebResponse resp = req.GetResponse();
            using (Stream s = resp.GetResponseStream())
            using (StreamReader sr = new StreamReader(s))
                return sr.ReadToEnd();
        }
        //private BitmapFrame getVideoCache(Uri uri)
        //{
        //    VideoDrawing vd = new VideoDrawing();
        //    vd.
        //}
        private MediaElement createMediaElement(Uri uri)
        {
            MediaElement retMediaElement = null;
            RoutedEventHandler mediaElementOpened = null;
            mediaElementOpened = (o, e) =>
            {
                retMediaElement.RemoveHandler(MediaElement.MediaOpenedEvent, mediaElementOpened);
                MediaInterval = retMediaElement.NaturalDuration.TimeSpan;
            };

            RoutedEventHandler mediaElementEnded = null;
            mediaElementEnded = (o, e) =>
            {
                Pause();
                if (mediaElement != null)
                    mediaElement.Position = TimeSpan.FromSeconds(1);
            };

            retMediaElement = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
                ScrubbingEnabled = true,
                Source = uri,
                Cursor = Cursors.Hand
            };

            retMediaElement.AddHandler(MediaElement.MediaOpenedEvent, mediaElementOpened);
            retMediaElement.AddHandler(MediaElement.MediaEndedEvent, mediaElementEnded);
            retMediaElement.MouseDown += new MouseButtonEventHandler((o, e) => { Pause(); });

            return retMediaElement;
        }

        private void LoadImage(Uri uri, bool Async)
        {
            if (Async)
            {
                BackgroundWorker bw = new BackgroundWorker();
                BitmapImage bi = null;
                bw.DoWork += (o, e) =>
                {
                    bi = new BitmapImage(uri)
                    {
                        DecodePixelHeight = (int)ActualHeight,
                        DecodePixelWidth = (int)ActualWidth
                    };
                    bi.Freeze();
                };
                bw.RunWorkerCompleted += (o, e) =>
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                    {
                        ImageViewer viewer = new ImageViewer() { Source = bi, IsZoomEnabled = this.IsZoomEnabled };
                        Content = viewer;
                    }));
                };
                bw.RunWorkerAsync();
            }
            else
            {
                Content = new ImageViewer()
                {
                    Source = new BitmapImage(uri)
                        {
                            DecodePixelHeight = (int)ActualHeight,
                            DecodePixelWidth = (int)ActualWidth
                        }
                };
            }
        }
        #endregion



        public void Load(Uri uri)
        {
            if (innerContainer == null)
                return;

            CurrentMediaType = GetMediaType(uri);

            #region Free up resources
            mediaElement = null;            
            #endregion

            IsMediaPlaying = true;


            switch (CurrentMediaType)
            {
                case MediaType.mtImage:
                    LoadImage(uri, true);
                    break;
                case MediaType.mtText:
                    Content = new TextBox() { Text = readText(uri), Margin= new Thickness(5,5,5,0),
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Focusable = false,
                        //Background = this.Background, Foreground = this.Foreground, 
                        IsReadOnly = true };
                    break;
                case MediaType.mtWeb:
                    WebBrowser wb;
                    Content = wb = new WebBrowser() { Margin = new Thickness(5, 5, 5, 0) };
                    wb.Navigate(uri);
                    break;
                case MediaType.mtVideo:
                    Content = mediaElement = createMediaElement(uri);
                    Play();
                    break;
                case MediaType.mtAudio:
                    Content = mediaElement = createMediaElement(uri);
                    Play();
                    break;
                case MediaType.mtPrevHandler:
                    object retVal = CreatePreviewerElementFunc(uri);
                    Content = retVal == null ? strNoPreviewAvail : retVal;
                    break;
                case MediaType.mtNone:
                    if (ShowNullMessage)
                        Content = strSelectFileToPreview;
                    break;
                default:
                    Content = strNoPreviewAvail;
                    break;
            }

            bool showInMiddle = CurrentMediaType == MediaType.mtAudio || CurrentMediaType == MediaType.mtVideo || Content is string;
            innerContainer.HorizontalAlignment = showInMiddle ? HorizontalAlignment.Center : HorizontalAlignment.Stretch;
            innerContainer.VerticalAlignment = showInMiddle ? VerticalAlignment.Center : VerticalAlignment.Stretch;
        }

        public static Func<Uri, object> CreatePreviewerElementFunc = (uri) => { return null; };
        public static Func<string, bool> GetPreviewerSupportFunc = (path) => { return false; };


        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)this.mmSlider.Value);
            if (Math.Abs(ts.Subtract(this.mediaElement.Position).TotalMilliseconds) > 1000)
                this.mediaElement.Position = ts;
        }

        public static void OnSourceChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            BasicMediaPlayer prev = (BasicMediaPlayer)sender;
            prev.Load(args.NewValue as Uri);
        }

        public static void OnCurrentMediaTypeChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            BasicMediaPlayer prev = (BasicMediaPlayer)sender;
            if (prev.mmSlider != null)
            {
                MediaType mtype = (MediaType)args.NewValue;
                if (mtype == MediaType.mtVideo || mtype == MediaType.mtAudio)
                    prev.mmSlider.Visibility = Visibility.Visible;
                else prev.mmSlider.Visibility = Visibility.Collapsed;
            }
        }

        public static void OnMediaPositionChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            BasicMediaPlayer prev = (BasicMediaPlayer)sender;
            TimeSpan ts = (TimeSpan)args.NewValue;

            if (Math.Abs(ts.Subtract(prev.mediaElement.Position).TotalMilliseconds) > 1000)
                prev.mediaElement.Position = ts;
        }

        #endregion

        #region variables

        private MediaElement mediaElement = null;
        private DispatcherTimer mmTimer = null;
        private Slider mmSlider = null;
        private UIElement pauseNotifier = null;
        private Panel innerContainer = null;

        #endregion

        #region Dependency Properties



        public bool ShowNullMessage
        {
            get { return (bool)GetValue(ShowNullMessageProperty); }
            set { SetValue(ShowNullMessageProperty, value); }
        }

        public static readonly DependencyProperty ShowNullMessageProperty =
            DependencyProperty.Register("ShowNullMessage", typeof(bool), typeof(BasicMediaPlayer), new UIPropertyMetadata(true));



        public TimeSpan MediaInterval
        {
            get { return (TimeSpan)GetValue(MediaIntervalProperty); }
            set { SetValue(MediaIntervalProperty, value); }
        }

        public static readonly DependencyProperty MediaIntervalProperty =
            DependencyProperty.Register("MediaInterval", typeof(TimeSpan), typeof(BasicMediaPlayer),
            new PropertyMetadata(TimeSpan.MinValue));

        public TimeSpan MediaPosition
        {
            get { return (TimeSpan)GetValue(MediaPositionProperty); }
            set { SetValue(MediaPositionProperty, value); }
        }

        public static readonly DependencyProperty MediaPositionProperty =
            DependencyProperty.Register("MediaPosition", typeof(TimeSpan), typeof(BasicMediaPlayer),
            new PropertyMetadata(OnMediaPositionChanged));

        public MediaType CurrentMediaType
        {
            get { return (MediaType)GetValue(CurrentMediaTypeProperty); }
            set { SetValue(CurrentMediaTypeProperty, value); }
        }

        public static readonly DependencyProperty CurrentMediaTypeProperty =
            DependencyProperty.Register("CurrentMediaType", typeof(MediaType),
            typeof(BasicMediaPlayer), new UIPropertyMetadata(MediaType.mtUnknown, new PropertyChangedCallback(OnCurrentMediaTypeChanged)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(Uri), typeof(BasicMediaPlayer),
            new PropertyMetadata(new PropertyChangedCallback(OnSourceChanged)));

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty ShowTimeLineProperty = DependencyProperty.Register("ShowTimeLine", typeof(bool),
            typeof(BasicMediaPlayer),
            new PropertyMetadata(true));

        public bool ShowTimeLine
        {
            get { return (bool)GetValue(ShowTimeLineProperty); }
            private set { SetValue(ShowTimeLineProperty, value); }
        }

        public bool IsMediaPlaying
        {
            get { return (bool)GetValue(IsMediaPlayingProperty); }
            set { SetValue(IsMediaPlayingProperty, value); }
        }

        public static readonly DependencyProperty IsMediaPlayingProperty =
            DependencyProperty.Register("IsMediaPlaying", typeof(bool), typeof(BasicMediaPlayer),
            new PropertyMetadata(true));



        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoomEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomEnabledProperty =
            ImageViewer.IsZoomEnabledProperty.AddOwner(typeof(BasicMediaPlayer));

        

        #endregion
    }
}
