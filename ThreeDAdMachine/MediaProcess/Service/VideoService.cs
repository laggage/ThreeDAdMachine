using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Emgu.CV;
using MediaProcess.Model;
using Newtonsoft.Json;
using MediaProcess.ExtensionMethod;

namespace MediaProcess.Service
{
    public class VideoService:IDisposable
    {
        #region Fields

        private readonly VideoModel _videoModel;
        private BackgroundWorker _sampleWorker;

        #endregion

        #region Property

        public ProgressChangedEventHandler SampleProgressChanged { get; set; }
        public RunWorkerCompletedEventHandler SampleCompleted { get; set; }

        #endregion

        #region Constructor

        public VideoService(VideoModel videoModel)
        {
            _videoModel = videoModel;
        }

        #endregion

        #region Method

        public void BeginSampleAsync()
        {
            if (_videoModel == null)
                throw new NullReferenceException("for sampling,_videoModel can not be null");
            _sampleWorker = _sampleWorker ?? new BackgroundWorker();
            _sampleWorker.WorkerReportsProgress = true;
            _sampleWorker.ProgressChanged += SampleProgressChanged;
          
            _sampleWorker.RunWorkerCompleted += SampleCompleted;
            _sampleWorker.RunWorkerCompleted += _sampleWorker_RunWorkerCompleted;
            _sampleWorker.DoWork += VideoSample;
            _sampleWorker.RunWorkerAsync();
        }

      

        public void VideoSample(object sender, DoWorkEventArgs e)
        {
            _videoModel.DataModel.IsSampling = true;
            _videoModel.DataModel.DataPath.CreateDirectoryIfNotExist();
            using (Capture c = new Capture(_videoModel.Path))
            using (FileStream fs = new FileStream(_videoModel.DataModel.DataPath, FileMode.OpenOrCreate))
            {
                int frameCount = (int)c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                for (int i = 0; i < frameCount; i++)
                {
                    c.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount, i);
                    using (Mat m = c.QueryFrame())
                    {
                        ImageService.ImageSample(m, _videoModel.DataModel, fs);
                    }

                    _sampleWorker.ReportProgress((int)(100 * ((double) i / frameCount)));
                }

                fs.Close();
            }
        }

        #endregion

        #region EventHandler

        private void _sampleWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _videoModel.DataModel.IsSampling = false;
            _sampleWorker.Dispose();
            _sampleWorker = null;
        }

        #endregion

        #region Static Method

        public static BitmapSource GetRepresentThumbnail(string path,bool randomFrame = true)
        {
            if (string.IsNullOrEmpty(path))
                return default(BitmapSource);
            using (Capture c = new Capture(path))
            {
                c.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames,
                    randomFrame ? (new Random()).Next((int) c.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount)):0);
                return BitmapSourceConvert.ToBitmapSource(c.QueryFrame());
            }
        }

        public static bool SaveVideoList(IEnumerable<VideoModel> videos, string fullPath)
        {
            bool result = true;
            try
            {
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    });
                    serializer.Serialize(sw, videos);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool LoadVideoList<T>(string fullPath, out T videos)
            where T : IEnumerable<MediaBaseModel>
        {
            bool result = true;
            try
            {
                using (StreamReader sr = File.OpenText(fullPath))
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    videos = (T) serializer.Deserialize(sr, typeof(T));
                }
            }
            catch
            {
                videos = default(T);
                result = false;
            }
            return result;
        }

        #endregion

        #region Implementation

        public void Dispose()
        {
            _sampleWorker?.Dispose();
        }

        #endregion
    }
}
