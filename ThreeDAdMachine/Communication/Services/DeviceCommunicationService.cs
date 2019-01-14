using System;
using System.IO;
using System.Net.Sockets;
using Communication.Models;

namespace Communication.Services
{
    public class ConnectionStateChangedEventArgs:EventArgs
    {
        public ConnectionStateChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
        public bool IsConnected { get; }
    }

    /********************************************************************************

    ** 类名称： DeviceCommunicationService

    ** 描述：负责与设备进行通信的类,实现 连接设备,向设备发送文件 功能

    ** 作者： Laggage

    ** 创建时间：2019 1/12

    ** 最后修改人：Laggage

    ** 最后修改时间：Laggage

    ** 版权所有 (C) :Laggage

    *********************************************************************************/
    public class DeviceCommunicationService : IDisposable
    {
        public DeviceCommunicationService(Device servicedDevice)
        {
            if (servicedDevice == null) throw new ArgumentNullException();
            _servicedDevice = servicedDevice;
        }

        private TcpClient _tcpClient;
        private readonly Device _servicedDevice;

        private bool IsConnected
        {
            set => ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(value));
        }

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        #region Method

        #region private

        /// <summary>
        /// 连接到指定ip和端口号的设备
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns>成功:true 失败:false</returns>
        private bool ConnectDevice(string ipAddress, int port)
        {
            if (_tcpClient == null) _tcpClient = new TcpClient();
            try
            {
                _tcpClient.Connect(ipAddress, port);
                IsConnected = _tcpClient.Connected;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Public

        public bool ConnectDevice()
        {
            if (_servicedDevice == null) throw new NullReferenceException("_servicedDevice 不可以为null");
            return ConnectDevice(_servicedDevice.IpAddress, _servicedDevice.Port);
        }

        public void DisConnectDevice()
        {
            _tcpClient?.Close();
            _tcpClient = null;
            IsConnected = false;
        }

        /// <summary>
        /// 向设备发送指令
        /// </summary>
        /// <param name="command">指令</param>
        public bool SendCommand(int command)
        {
            if (_tcpClient == null) throw new NullReferenceException("_tcpClient");
            try
            {
                NetworkStream ns = _tcpClient.GetStream();
                byte[] buf = BitConverter.GetBytes(command);
                ns.Write(buf, 0, buf.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送文件到设备
        /// </summary>
        /// <param name="fileName"></param>
        public void SendFileToDevice(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException();
            if (!File.Exists(fileName)) throw new FileNotFoundException();

            try
            {
                NetworkStream ns = _tcpClient.GetStream();
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    byte[] buf = new byte[200];
                    while (fs.Read(buf, 0, buf.Length) > 0)
                    {
                        ns.Write(buf, 0, buf.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool TrySendFileToDevice(string fileName)
        {
            try
            {
                SendFileToDevice(fileName);
                return true;
            }
            catch (Exception) { return false; }
        }

        #endregion

        #region Implementation

        public void Dispose()
        {
            _tcpClient?.Close();
        }

        #endregion

        #endregion

    }

   
}
