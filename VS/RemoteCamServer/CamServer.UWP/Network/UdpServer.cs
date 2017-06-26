using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public class UdpServer
        : INetworkServer
    {
        #region Fields

        #endregion

        #region .ctor

        #endregion
        public bool Listening { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Task Listen()
        {
            throw new NotImplementedException();
        }
      
        public int Send(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int Send(String client, byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
