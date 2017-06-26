using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public interface INetworkClient
    {
        #region Events

        event EventHandler<DataReceivedEventArgs> DataReceived;

        #endregion

        #region Properties

        bool Connected
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

        Task Connect();

        void Close();

        int Send(byte[] buffer);

        #endregion
    }
}
