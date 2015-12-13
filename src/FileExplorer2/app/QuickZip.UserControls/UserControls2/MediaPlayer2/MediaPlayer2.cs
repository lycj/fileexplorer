using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public enum MediaPlayerMode { MMView, MetaView }


    [TemplatePart(Name = "PART_MediaPlayer", Type = typeof(BasicMediaPlayer))]
    public class MediaPlayer2 : ContentControl
    {


        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mediaPlayer = (BasicMediaPlayer)this.Template.FindName("PART_MediaPlayer", this);
            Debug.Assert(_mediaPlayer != null);
            OnContentChanged(null, this.Content);            
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (_mediaPlayer != null)
                if (newContent is string)
                {
                    string pathName = newContent as string;
                    switch (BasicMediaPlayer.GetMediaType(pathName))
                    {
                        case BasicMediaPlayer.MediaType.mtUnknown:
                            IsMedia = false;
                            break;
                        default:
                            IsMedia = true;
                            _mediaPlayer.Source = new Uri(pathName);
                            break;
                    }
                }
                else
                {
                    _mediaPlayer.Source = null;
                }

        }

        #endregion

        #region Data

        BasicMediaPlayer _mediaPlayer = null;
        private MediaPlayerMode _mediaPlayerMode = QuickZip.UserControls.MediaPlayerMode.MMView;

        #endregion

        #region Public Properties



        public bool IsMedia
        {
            get { return (bool)GetValue(IsMediaProperty); }
            set { SetValue(IsMediaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMedia.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaProperty =
            DependencyProperty.Register("IsMedia", typeof(bool), typeof(MediaPlayer2), new UIPropertyMetadata(false));



        #endregion

    }
}
