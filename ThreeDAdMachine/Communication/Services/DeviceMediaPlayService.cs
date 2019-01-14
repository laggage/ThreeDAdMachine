using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using Communication.Models;
using MediaProcess.Model;

namespace Communication.Services
{
    public enum MediaPlayMode
    {
        [Description("顺序播放")]
        Order,
        [Description("随机播放")]
        Random,
        [Description("列表循环")]
        Circulation,
        [Description("单曲循环")]
        SingleCircle
    }

    public enum MediaPlayState
    {
        [Description("正在播放")]
        Playing,
        [Description("暂停")]
        Pause,
        [Description("停止")]
        Stop,
        [Description("正在向设备发送数据")]
        SendingMedia
    }

    public class PlayPositionChangedEventArgs : EventArgs
    {
        public PlayPositionChangedEventArgs(double finishedProgress)
        {
            FinishedProgress = finishedProgress;
        }
        public double FinishedProgress;
    }

    public class DeviceMediaPlayService
    {

        #region Constructor

        private DeviceMediaPlayService()
        {
            PlayMode = MediaPlayMode.Circulation;  //默认播放模式为 列表循环
            PlayState = MediaPlayState.Stop;    //默认播放状态为 停止
            //初始化计时器
            _mediaPlayTimer = new Timer(1000);
        }

        public DeviceMediaPlayService(Device servicedDevice):this()
        {
            _servicedDevice = servicedDevice ?? throw new ArgumentNullException();
        }

        #endregion


        #region Fields

        private readonly Device _servicedDevice;
        private readonly Timer _mediaPlayTimer;
        private int _currentMediaPos;

        #endregion


        #region Property

        public MediaPlayMode PlayMode { get; set; }
        public MediaPlayState PlayState { get; set; }
        private List<MediaBaseModel> MediaPlayList => _servicedDevice.MediaList;
        public int TotalPlaySeconds => MediaPlayList[_currentMediaPos].PlaySettingModel.PlayTime.Seconds;
        public int PlayedSeconds { get; private set; }

        public event EventHandler<PlayPositionChangedEventArgs> PlayPositionChanged;

        #endregion


        #region Method

        #region private

        private void CalculateNextMediaPos()
        {
            switch (PlayMode)
            {
                case MediaPlayMode.Circulation:
                    {
                        if (_currentMediaPos >= MediaPlayList.Count) _currentMediaPos = 0;
                        else _currentMediaPos++;
                    }
                    break;
                case MediaPlayMode.Order:
                {
                    if (_currentMediaPos >= MediaPlayList.Count) _currentMediaPos = -1;
                    else if(_currentMediaPos != -1) _currentMediaPos++;
                }
                    break;
                case MediaPlayMode.Random:
                {
                    Random random = new Random();
                    _currentMediaPos = random.Next(MediaPlayList.Count);
                }
                    break;
                case MediaPlayMode.SingleCircle:
                    break;
            }
        }

        private void SendMediaToDevice(MediaBaseModel media)
        {
            _servicedDevice.DeviceCommunicationService.SendFileToDevice(media.DataModel.DataPath);
        }

        private void PlayMedia()
        {
            PlayState = MediaPlayState.SendingMedia;
            SendMediaToDevice(MediaPlayList[_currentMediaPos]);
            PlayState = MediaPlayState.Playing;
            
            _mediaPlayTimer.Elapsed += (sender, e) =>
            {
                PlayedSeconds++;
                PlayPositionChanged?.Invoke(this,
                    new PlayPositionChangedEventArgs((double) PlayedSeconds / TotalPlaySeconds));

                if (PlayedSeconds <= TotalPlaySeconds) return; 
                //一次播放结束
                _mediaPlayTimer.Stop();
                PlayedSeconds = 0;
                Play();
            };
            _mediaPlayTimer.Start();
        }

        #endregion

        #region public

        public void Play(MediaBaseModel mediaToPlay = null)
        {
            if (mediaToPlay == null) CalculateNextMediaPos();
            else
            {
                if (MediaPlayList.Exists((m) => m == mediaToPlay))
                    _currentMediaPos = MediaPlayList.FindIndex((m) => m == mediaToPlay);
                else
                    _currentMediaPos = -1;
            }

            if (_currentMediaPos == -1)
                return;

            PlayMedia();
        }

        public void Pause()
        {

        }

        #endregion

        #endregion

    }
}
