using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public interface INetworkServer
    {
        #region Properties

        bool Listening
        {
            get;
        }

        String Host
        {
            get;
            set;
        }

        int Port
        {
            get;
            set;
        }

        #endregion

        #region Methods

        Task Listen();

        void Close();

        int Send(byte[] buffer);

        int Send(String client, byte[] buffer);
       
        #endregion
    }
}
