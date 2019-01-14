using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Communication.Services;
using MediaProcess.Model;

namespace Communication.Models
{
    public enum DeviceState
    {
        [Description("在线")]
        OnLine,
        [Description("离线")]
        OffLine
    }

    public class Device
    {
        #region Constructor

        private Device()
        {
            State = DeviceState.OffLine;
            Port = 10000;
            MediaList = new List<MediaBaseModel>();
            DeviceCommunicationService = new DeviceCommunicationService(this);
            _deviceMediaPlayService = new DeviceMediaPlayService(this);

            DeviceCommunicationService.ConnectionStateChanged += (sender, e) => State = e.IsConnected ? DeviceState.OnLine : DeviceState.OffLine;
        }

        public Device(string deviceName, string deviceIp, int port = 1000) : this()
        {
            Name = deviceName;
            IpAddress = deviceIp;
            Port = port;

            _hashCode = IpAddress.GetHashCode() ^ port.GetHashCode();
        }

        #endregion


        #region Fields

        private readonly int _hashCode;
        private readonly DeviceMediaPlayService _deviceMediaPlayService;
        private static string DefaultThumbnailPath => Path.Combine(Directory.GetCurrentDirectory(), @"Cache\DeviceData\DeviceThumb\default.jpg");
        //private DeviceCommunicationService _deviceCommunication;

        #endregion


        #region Property

        public string Name { get; set; }
        public string IpAddress { get; set; }
        /// <summary>
        /// 设备缩略图路径 在程序所在目录的 Cache/DeviceData/DeviceThumb/...
        /// </summary>
        public string ThumbnailPath
        {
            get
            {
                string thumbDir = Path.Combine(Directory.GetCurrentDirectory(), @"Cache\DeviceData\DeviceThumb\", Name);
                if (!Directory.Exists(thumbDir)) return DefaultThumbnailPath;
                string[] thumb = Directory.GetFiles(thumbDir);
                return thumb.Length < 1 ? DefaultThumbnailPath : Path.Combine(thumbDir, thumb[0]);
            }
        }
        public int Port { get; set; }
        public DeviceState State { get; private set; }
        public List<MediaBaseModel> MediaList { get; set; }
        public DeviceCommunicationService DeviceCommunicationService { get; private set; }

        #endregion


        #region Method

        #region public

        /// <summary>
        /// 连接到本设备
        /// </summary>
        /// <returns>成功:true 失败:false</returns>
        public bool ConnectToDevice()
        {
            DeviceCommunicationService = DeviceCommunicationService ?? new DeviceCommunicationService(this);
            return DeviceCommunicationService.ConnectDevice();
        }

        /// <summary>
        /// 断开与本设备的连接
        /// </summary>
        public void DisConnectWithDevice()
        {
            DeviceCommunicationService?.DisConnectDevice();
        }

        public bool SendCommand(DeviceCommand command)
        {
            if (State == DeviceState.OffLine) return false;
            return DeviceCommunicationService.SendCommand(command.CommandCode);
        }

        public bool Equals(Device obj)
        {
            if (obj == null) return false;
            return obj.IpAddress == IpAddress && obj.Port == Port;
        }

        #endregion

        #region Override

        public override string ToString() => string.IsNullOrEmpty(Name) ? "Untitled Machine" : Name;

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return GetType() == obj.GetType() && Equals((Device)obj);
        }

        public static bool operator ==(Device left, Device right)
        {
            return left?.Equals(right) ?? ReferenceEquals(right, null);
        }

        public static bool operator !=(Device left, Device right) => !(left == right);

        #endregion

        #endregion
    }
}
