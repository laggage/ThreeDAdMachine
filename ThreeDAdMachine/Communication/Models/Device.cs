using System.Collections.Generic;
using System.ComponentModel;
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

        public Device()
        {
            Name = null;
            IpAddress = null;
            State = DeviceState.OffLine;
            Port = 10000;
            MediaList = new List<MediaBaseModel>();
        }

        public Device(string deviceName, string deviceIp, int port = 1000) : this()
        {
            Name = deviceName;
            IpAddress = deviceIp;
            Port = port;
        }

        #endregion


        #region Fields

        private CommunicationService _communicationService;

        #endregion


        #region Property

        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DeviceState State { get; set; }
        public List<MediaBaseModel> MediaList { get; set; }

        #endregion


        #region Method

        #region Override

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name)?"Untitled Machine":Name;
        }

        #endregion

        #endregion
    }
}
