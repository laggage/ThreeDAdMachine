using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using MediaProcess.Service;
using Newtonsoft.Json;

namespace MediaProcess.Model
{
    public class VideoModel:MediaBaseModel
    {
        #region Constructor

        public VideoModel(string url)
        {
            Path = url;
        }

        #endregion

        #region Property

        private string _path;
        public new string Path
        {
            get => _path;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Init(value);
                _path = value;
                base.Path = value;
            }
        }

        private TimeSpan _playTime;
        public TimeSpan PlayTime => _playTime;

        private Size _frameSize;
        public override Size FrameSize => _frameSize;

        [JsonIgnore]
        public BitmapSource Thumbnail => VideoService.GetRepresentThumbnail(Path);

        public int Frames { get; private set; }

        #endregion

        #region Method

        private void Init(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            using (Capture c = new Capture(url))
            {
                Frames = (int)c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                _playTime = TimeSpan.FromSeconds(Frames*c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
                _frameSize.Width = c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth);
                _frameSize.Height = c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight);
            }
            DataModel = new DataModel(url, FrameSize);
        }

        #endregion

    }
}
