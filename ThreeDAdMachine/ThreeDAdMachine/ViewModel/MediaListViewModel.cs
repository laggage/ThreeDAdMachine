 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MediaProcess.Model;
using MediaProcess.Service;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using ThreeDAdMachine.Extensions;
using ThreeDAdMachine.Helper;

namespace ThreeDAdMachine.ViewModel
{
    /// <summary>
    /// 视频和图片数据显示视图的试图模型
    /// </summary>
    public class MediaListViewModel : NotificationObject
    {
        #region Constructor

        public MediaListViewModel()
        {
            //initialize command
            LoadDataCommand = new DelegateCommand(LoadData);
            AddMediaCommand = new DelegateCommand<object>(AddMedia);
            BuildMediaSampleDataCommand = new DelegateCommand(BuildMediaSampleData);
            DeleteMediaItemCommand = new DelegateCommand(DeleteMediaItem);
            OpenDataFileInExploreCommand = new DelegateCommand(OpenDataFileInExplore);

            if(Application.Current.MainWindow != null) Application.Current.MainWindow.Closing += MainWindow_Closing; ;
        }

        #endregion

        #region Fields

        private const string MediaSamplingStateDescription = "正在生成数据文件 - ";

        #endregion

        #region NofityProperty

        #region MediaStateDescription

        private string _mediaStateDescription;

        public string MediaStateDescription
        {
            get => _mediaStateDescription;
            set
            {
                if (_mediaStateDescription == value)
                    return;
                _mediaStateDescription = value;
                RaisePropertyChanged(nameof(MediaStateDescription));
            }
        }

        #endregion

        #region ImageModels

        private ObservableCollection<ImageModel> _imageModels = new ObservableCollection<ImageModel>();

        public ObservableCollection<ImageModel> ImageModels
        {
            get => _imageModels;
            set
            {
                if (_imageModels == value)
                    return;
                _imageModels = value;
                RaisePropertyChanged(nameof(ImageModels));
            }
        }

        #endregion

        #region SelectedRegion

        private Rect _selectedRegion;

        public Rect SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                if (_selectedRegion == value)
                    return;
                _selectedRegion = value;
                RaisePropertyChanged(nameof(SelectedRegion));
            }
        }

        #endregion

        #region DragRegion

        private Rect _dragRegion;

        public Rect DragRegion
        {
            get => _dragRegion;
            set
            {
                if (_dragRegion == value)
                    return;
                _dragRegion = value;
                RaisePropertyChanged(nameof(DragRegion));
            }
        }

        #endregion

        #region DeviceAndConnectViewModel

        public DevicesAndConnectViewModel DeviceAndConnectViewModel => AppHelper.AppViewModel.DevicesAndConnectViewModel;

        #endregion

        #region EditingImage
        /// <summary>
        /// 当前正在处理的图片
        /// </summary>
        private ImageModel _editingImage;

        public ImageModel EditingImage
        {
            get
            {
                ApplySettingToMedia(_editingImage);
                return _editingImage;
            }
            set
            {
                if (_editingImage == value)
                    return;

                ApplySettingToMedia(_editingImage);
                LoadSettingFromMedia(value);

                if (value != null) EditingVideo = null;

                _editingImage = value;
                RaisePropertyChanged(nameof(EditingImage));
            }
        }

        #endregion

        #region EditingVideo

        private VideoModel _editingVideo = null;

        public VideoModel EditingVideo
        {
            get
            {
                ApplySettingToMedia(_editingVideo);
                return _editingVideo;
            }
            set
            {
                if (_editingVideo == value)
                    return;

                ApplySettingToMedia(_editingImage);
                LoadSettingFromMedia(value);

                //VideoModel and ImageModel can only exist one at the same time
                if (value != null) EditingImage = null;

                _editingVideo = value;
                RaisePropertyChanged(nameof(EditingVideo));
            }
        }

        #endregion

        #region Radius

        private int _radius;
        /// <summary>
        /// 半径方向的采样点数   
        /// </summary>
        public int Radius
        {
            get => _radius;
            set
            {
                if (_radius == value)
                    return;
                _radius = value;
                RaisePropertyChanged(nameof(Radius));
            }
        }

        #endregion

        #region MaxRadius

        private int _maxRadius;

        public int MaxRadius
        {
            get => _maxRadius;
            set
            {
                if (_maxRadius == value)
                    return;
                _maxRadius = value;
                RaisePropertyChanged(nameof(MaxRadius));
            }
        }


        #endregion

        #region IntervalAngle

        private double _intervalAngle;

        public double IntervalAngle
        {
            get => _intervalAngle;
            set
            {
                if (_intervalAngle.Equals(value))
                    return;
                _intervalAngle = value;
                RaisePropertyChanged(nameof(IntervalAngle));
            }
        }

        #endregion

        #region PlayTime

        private TimeSpan _playTime;

        public TimeSpan PlayTime
        {
            get => _playTime;
            set
            {
                if (_playTime == value)
                    return;
                _playTime = value;
                RaisePropertyChanged(nameof(PlayTime));
            }
        }

        #endregion

        #region CircleTimes 

        private int _circleTimes;

        public int CircleTimes
        {
            get => _circleTimes;
            set
            {
                if (_circleTimes == value)
                    return;
                _circleTimes = value;
                RaisePropertyChanged(nameof(CircleTimes));
            }
        }

        #endregion

        #region MediaSampleViewModel

        private MediaSampleViewModel _mediaSampleViewModel;

        public MediaSampleViewModel MediaSampleViewModel
        {
            get => _mediaSampleViewModel;
            set
            {
                if (_mediaSampleViewModel == value)
                    return;
                _mediaSampleViewModel = value;
                RaisePropertyChanged(nameof(MediaSampleViewModel));
            }
        }

        #endregion

        #region VideoModels

        private ObservableCollection<VideoModel> _videoModels;

        public ObservableCollection<VideoModel> VideoModels
        {
            get => _videoModels;
            set
            {
                if (_videoModels == value)
                    return;
                _videoModels = value;
                RaisePropertyChanged(nameof(VideoModels));
            }
        }

        #endregion

        #endregion

        #region Command

        #region LoadDataCommand

        public DelegateCommand LoadDataCommand { get; set; }

        private void LoadData()
        {
            if (!(MediaService.LoadMediaList(DataFilePath.ImageListInfoPath.GetDescription(), 
                      MediaType.Image, out _imageModels) &&
                  MediaService.LoadMediaList(DataFilePath.VideoListInfoPath.GetDescription(), 
                      MediaType.Video, out _videoModels)))
                ShowWaring("加载历史数据错误...");
            if (ImageModels != null && ImageModels.Count > 0)
                EditingImage = ImageModels[0];
        }

        #endregion

        #region AddMediaCommand

        public DelegateCommand<object> AddMediaCommand { get; set; }

        private void AddMedia(object e)
        {
            if (e == null)
                return;

            if (bool.TryParse(e.ToString(), out var isVideo))
            {
                if (isVideo) AddVideoToList();
                else AddImageToList();
            }
        }

        #endregion

        #region BuildImageSampleDataCommand

        public DelegateCommand BuildMediaSampleDataCommand { get; set; }

        private void BuildMediaSampleData()
        {
            try
            {
                if (EditingImage != null)
                    BuildImageSampleData();
                else if (EditingVideo != null)
                    BuildVideoSampleData();
            }
            catch
            { ShowWaring("采样参数错误,请设置正确的参数"); }
        }

        private void BuildVideoSampleData()
        {
            ApplySettingToMedia();

            //if (EditingVideo == null)
            //    return;
            if (EditingVideo.DataModel.IsSampling)
            {
                ShowWaring("正在生成数据文件,请耐心等待");
                return;
            }

            MediaSampleViewModel = MediaSampleViewModel ?? new MediaSampleViewModel();
            MediaSampleViewModel.IsActive = true;
            MediaSampleViewModel.ReportProgressDetail = true;
            MediaStateDescription = MediaSamplingStateDescription;

            VideoService videoService = new VideoService(EditingVideo);
            videoService.SampleProgressChanged += (sender, e) =>
            {
                MediaSampleViewModel.ProgressValue = e.ProgressPercentage;
            };
            videoService.SampleCompleted += (sender, e) =>
            {
                MediaSampleViewModel.IsActive = false;
                MediaStateDescription = "";
            };
            videoService.BeginSampleAsync();
        }

        private async void BuildImageSampleData()
        {
            ApplySettingToMedia();
            if (EditingImage == null) return;
            if (EditingImage.DataModel.IsSampling)
            {
                ShowWaring("正在生成数据文件,请耐心等待");
                return;
            }
            if (MediaSampleViewModel == null)
                MediaSampleViewModel = new MediaSampleViewModel();
            MediaSampleViewModel.IsActive = true;
            MediaStateDescription = MediaSamplingStateDescription;
            EditingImage.DataModel.IsSampling = true;
            try
            {
                await ImageService.ImageSampleAsync(EditingImage.Path, EditingImage.DataModel);
            }
            catch
            {
                ShowWaring("采样参数错误,请设置正确的参数");
            }
            EditingImage.DataModel.IsSampling = false;
            MediaSampleViewModel.IsActive = false;
            MediaStateDescription = "";
        }

        #endregion

        #region DeleteMediaItemCommand

        public DelegateCommand DeleteMediaItemCommand { get; set; }

        private void DeleteMediaItem()
        {
            if (EditingImage != null)
            {
                int newIndex = ImageModels.IndexOf(EditingImage);
                MediaService.ClearMediaDataCache(EditingImage);
                ImageModels.Remove(EditingImage);
                newIndex = newIndex == ImageModels.Count ? newIndex - 1 : newIndex;
                if (newIndex >= 0)
                    EditingImage = ImageModels[newIndex];
            }
            else if (EditingVideo != null)
            {
                int newIndex = VideoModels.IndexOf(EditingVideo);
                MediaService.ClearMediaDataCache(EditingVideo);
                VideoModels.Remove(EditingVideo);
                newIndex = newIndex == VideoModels.Count ? newIndex - 1 : newIndex;
                if (newIndex >= 0)
                    EditingVideo = VideoModels[newIndex];
            }
            else
                AppHelper.AppViewModel.ShowWarning(MediaWaringType.NoMediaWaring.GetDescription());
        }

        #endregion

        #region OpenDataFileInExploreCommand

        public DelegateCommand OpenDataFileInExploreCommand { get; set; }

        public void OpenDataFileInExplore()
        {
            MediaBaseModel media = null;
            if (EditingImage != null)
                media = EditingImage;
            else if (EditingVideo != null)
                media = EditingVideo;
            if (media == null || media.DataModel == null)
                return;
            if (File.Exists(media.DataModel.DataPath))
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "explorer.exe";
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), media.DataModel.DataPath.Remove(0,2).Replace('/','\\'));
                    p.StartInfo.Arguments = "/select, " + fullPath;
                    p.Start();
                }
            else
                ShowWaring("请先生成数据文件");
        }

        #endregion

        #region AddMediaToDeviceCommand 

        //public DelegateCommand

        #endregion

        #endregion

        #region Method

        private void AddImageToList()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "图片|*.bmp;*.jpg;*.jpeg;*.png;*.gif"
            };
            bool? result = ofd.ShowDialog();

            if (true != result) return;

            AddImagesToListAsync(ofd.FileNames);
        }

        private async Task AddImageToListAsync(string path)
        {
            if (ImageModels == null)
                ImageModels = new ObservableCollection<ImageModel>();
            ImageModel newImageModel = await Task.Run(() => new ImageModel(path)
            {
                PlaySettingModel = new PlaySettingModel(3, TimeSpan.FromSeconds(10))
            });
            ImageModels.Add(newImageModel);
            await Task.Run(() => EditingImage = newImageModel);
        }

        private async void AddImagesToListAsync(IEnumerable<string> urls)
        {
            foreach (string url in urls)
            {
                await AddImageToListAsync(url);
            }
        }

        private void AddVideoToList()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "视频|*.mp4;*.avi;"
            };
            bool? result = ofd.ShowDialog();

            if (true != result) return;

            AddVideosToListAsync(ofd.FileNames);
        }

        private async Task AddVideoToListAsync(string path)
        {
            if (VideoModels == null)
                VideoModels = new ObservableCollection<VideoModel>();
            VideoModel newVideoModel = await Task.Run(() => new VideoModel(path)
            {
                PlaySettingModel = new PlaySettingModel(3, TimeSpan.FromSeconds(10))
            });
            VideoModels.Add(newVideoModel);
            await Task.Run(() => EditingVideo = newVideoModel);
        }

        private async void AddVideosToListAsync(IEnumerable<string> urls)
        {
            foreach (string url in urls)
            {
                await AddVideoToListAsync(url);
            }
        }

        private void LoadSettingFromMedia(MediaBaseModel media)
        {
            if (media == null)
                return;
            Radius = media.DataModel.Radius;
            MaxRadius = media.DataModel.MaxRadius;
            IntervalAngle = media.DataModel.IntervalAngle;
            DragRegion = new Rect(0, 0, media.FrameSize.Width, media.FrameSize.Height);
            SelectedRegion = media.DataModel.SelectedRegion;
            PlayTime = media.PlaySettingModel.PlayTime;
            CircleTimes = media.PlaySettingModel.CircleTimes;

            if (MediaSampleViewModel != null)
            {
                MediaSampleViewModel.IsActive = media.DataModel.IsSampling;
                MediaStateDescription = media.DataModel.IsSampling ? MediaSamplingStateDescription : "";
            }
        }

        private void ApplySettingToMedia()
        {
            if (EditingImage != null)
                ApplySettingToMedia(_editingImage);
            if (EditingVideo != null)
                ApplySettingToMedia(_editingVideo);
        }

        public void ApplySettingToMedia(MediaBaseModel media)
        {
            if (media == null)
                return;
            media.DataModel.Radius = Radius;
            media.DataModel.IntervalAngle = IntervalAngle;
            media.DataModel.SelectedRegion = SelectedRegion;
            media.PlaySettingModel.PlayTime = PlayTime;
            media.PlaySettingModel.CircleTimes = CircleTimes;
        }

        private void ShowWaring(string warningInfo)
        {
            AppHelper.AppViewModel.WarningFlyoutViewModel.ShowWaring(warningInfo);
        }

        #endregion

        #region EventHandler

        /// <summary>
        /// Application exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplySettingToMedia();
            if (!(MediaService.SaveMediaList(ImageModels, DataFilePath.ImageListInfoPath.GetDescription(), MediaType.Image) &&
                  MediaService.SaveMediaList(VideoModels, DataFilePath.VideoListInfoPath.GetDescription(), MediaType.Video)))
            {
                MessageBox.Show("保存数据时发生错误...", "警告", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        #endregion

    }
}
