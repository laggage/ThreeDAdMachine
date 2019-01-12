using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Communication.Services
{
    public class CommunicationService: IDisposable
    {
        #region Fields

        private TcpClient _tcpClient;

        #endregion

        #region Method

        #region Public

        public bool ConnectDevice(string ipAddress,int port)
        {
            if (_tcpClient == null)
                _tcpClient = new TcpClient();
            _tcpClient.Connect(ipAddress, port);
            return true;
        }

        //public async bool ConnectDeviceAsync(string ipAddress, int port)
        //{
        //    await Task.Run(() =>
        //    {
        //        Stopwatch st = new Stopwatch();
        //        st.Start();
        //        ConnectDevice(ipAddress, port);
        //        //st.ElapsedMilliseconds
        //        st.Stop();
        //    });
        //}

        #endregion

        #endregion

        #region Implementation

        public void Dispose()
        {
            _tcpClient?.Close();
        }

        #endregion
    }
}
